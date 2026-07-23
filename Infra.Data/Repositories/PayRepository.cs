using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class PayRepository : IPayRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public PayRepository(AppWebContext context)
        {
            _context = context;
        }

        #endregion
        public async Task<IEnumerable<RequestPay>> GetRequestPaiesAsync()
        {
            var requests = await _context.RequestPays.Include(p => p.ApplicationUser).ThenInclude(p=>p.UserDetail)
                .Include(p => p.Cart).ToListAsync();

            return requests;
        }

        public async Task AddRequestPay(RequestPay t)
        {
            await _context.RequestPays.AddAsync(t);
        }

        public async Task<IEnumerable<Factor>> GetFactors()
        {
            var factors = await _context.Factors.Include(p => p.FactorDetails).ToListAsync();

            return factors;
        }

        public Task<RequestPay> GetRequestPayAsync(string requestPayId)
        {
            return _context.RequestPays
                .Include(request => request.ApplicationUser)
                    .ThenInclude(user => user.UserDetail)
                .Include(request => request.Cart)
                    .ThenInclude(cart => cart.Discounts)
                .Include(request => request.Cart)
                    .ThenInclude(cart => cart.CartDetails)
                        .ThenInclude(detail => detail.Product)
                            .ThenInclude(product => product.ProductImages)
                .Include(request => request.Cart)
                    .ThenInclude(cart => cart.CartDetails)
                        .ThenInclude(detail => detail.Product)
                            .ThenInclude(product => product.ProductAttributes)
                .Include(request => request.Cart)
                    .ThenInclude(cart => cart.CartDetails)
                        .ThenInclude(detail => detail.Templates)
                .SingleOrDefaultAsync(request => request.Id == requestPayId);
        }

        public Task<Factor> GetFactorAsync(int factorId, string userId)
        {
            return _context.Factors
                .AsNoTracking()
                .Include(factor => factor.FactorDetails)
                .Include(factor => factor.Discounts)
                .SingleOrDefaultAsync(factor => factor.Id == factorId && factor.UserId == userId);
        }

        public Task<Factor> GetFactorByCartAsync(int cartId, string userId)
        {
            return _context.Factors
                .AsNoTracking()
                .Include(factor => factor.FactorDetails)
                .Include(factor => factor.Discounts)
                .SingleOrDefaultAsync(factor => factor.CartId == cartId && factor.UserId == userId);
        }

        public async Task<Factor> FinalizePaymentAsync(string requestPayId, int referenceId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var request = await GetRequestPayAsync(requestPayId);
            if (request?.Cart == null || request.ApplicationUser == null)
            {
                return null;
            }

            var existingFactor = await _context.Factors
                .Include(factor => factor.FactorDetails)
                .Include(factor => factor.Discounts)
                .SingleOrDefaultAsync(factor => factor.CartId == request.Cart.CartId);

            if (existingFactor != null)
            {
                await transaction.CommitAsync();
                return existingFactor;
            }

            if (request.IsPay || request.Cart.IsFinally)
            {
                return null;
            }

            foreach (var detail in request.Cart.CartDetails)
            {
                if (detail.ProductCount <= 0)
                {
                    throw new InvalidOperationException("Cart contains an invalid product quantity.");
                }

                if (detail.Templates == null)
                {
                    if (detail.Product.Count < detail.ProductCount)
                    {
                        throw new InvalidOperationException("Insufficient product stock.");
                    }

                    detail.Product.Count -= detail.ProductCount;
                }
                else
                {
                    if (detail.Templates.AttrinbuteTemplateCount < detail.ProductCount)
                    {
                        throw new InvalidOperationException("Insufficient product variant stock.");
                    }

                    detail.Templates.AttrinbuteTemplateCount -= detail.ProductCount;
                }
            }

            request.IsPay = true;
            request.DatePay = DateTime.UtcNow;
            request.RefId = referenceId;
            request.Cart.IsFinally = true;

            var userDetail = request.ApplicationUser.UserDetail;
            var factor = new Factor
            {
                CartId = request.Cart.CartId,
                RefId = referenceId,
                TotalPrice = request.Amount,
                UserAddress = userDetail?.Address ?? string.Empty,
                UserEmail = request.ApplicationUser.Email ?? string.Empty,
                UserName = userDetail?.FirstName ?? request.ApplicationUser.UserName,
                UserFamilly = userDetail?.LastName ?? string.Empty,
                UserPhone = request.ApplicationUser.PhoneNumber,
                UserId = request.ApplicationUser.Id,
                User = request.ApplicationUser,
                Status = FactorStatus.Progssess,
                CreateTime = DateTime.UtcNow,
                Discounts = request.Cart.Discounts?.ToList(),
                FactorDetails = request.Cart.CartDetails.Select(detail => new FactorDetail
                {
                    ImageSrc = detail.Product.ProductImages
                        .Select(image => image.ImgFile + "/" + image.ImgSrc)
                        .FirstOrDefault() ?? string.Empty,
                    ProductCount = detail.ProductCount,
                    ProductName = detail.Product.Name,
                    ProductPrice = detail.ProductPrice,
                    TotalPrice = detail.TotalPrice,
                    AttributesName = detail.Product.ProductAttributes == null
                        ? string.Empty
                        : string.Join(",", detail.Product.ProductAttributes.Select(attribute => attribute.AttributeName)),
                    AttributesValue = detail.Templates?.Template ?? string.Empty
                }).ToList()
            };

            await _context.Factors.AddAsync(factor);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return factor;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddFactor(Factor t)
        {
            await _context.Factors.AddAsync(t);
        }

        public async Task AddFactorDetails(IEnumerable<FactorDetail> t)
        {
            await _context.FactorDetails.AddRangeAsync(t);
        }

    }
}
