using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories.AdminRepositories
{
    public class CartRepository : ICartRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public CartRepository(AppWebContext context)
        {
            _context = context;
        }

        #endregion
        public async Task<IEnumerable<Cart>> GetCartsAsync()
        {
            var carts = await _context.Carts.Include(p => p.CartDetails).Include(p=>p.Discounts).ToListAsync();
            return carts;
        }
        public async Task<Cart> GetCartAsync(string userId)
        {
            var cart = await _context.Carts.Include(p => p.CartDetails).ThenInclude(p => p.Product).ThenInclude(p => p.ProductImages)
                .Include(p => p.CartDetails).ThenInclude(p => p.Product).ThenInclude(p => p.ProductAttributes)
                .Include(p => p.CartDetails).ThenInclude(p => p.Product).ThenInclude(p => p.Properties)
                .Include(p => p.CartDetails).ThenInclude(p => p.Templates)
                .Include(p=>p.Discounts)
                .SingleOrDefaultAsync(p => p.UserId == userId && !p.IsFinally);
            return cart;
        }
        public async Task<CartDetail> GetCartDetailAsync(int cartDetailId)
        {
            var cartDetail = await _context.CartDetails
                .Include(p => p.Product).ThenInclude(p => p.AttributeTemplates)
                .Include(p => p.Cart)
                .SingleOrDefaultAsync(p => p.CartDetailId == cartDetailId);

            return cartDetail;
        }
        public async Task<IEnumerable<Discount>> GetDiscountsAsync()
        {
            var discounts = await _context.Discounts.ToListAsync();

            return discounts;
        }

        public async Task AddCart(Cart t)
        {
            await _context.Carts.AddAsync(t);
        }
        public async Task AddCartDetail(CartDetail t)
        {
            await _context.CartDetails.AddAsync(t);
        }
        public async Task AddDiscountAsync(Discount t)
        {
            await _context.Discounts.AddAsync(t);
        }

        public void RemoveCartDetail(CartDetail t)
        {
            _context.CartDetails.Remove(t);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task AddFavorite(UserFavorite t)
        {
            await _context.UserFavorites.AddAsync(t);
        }
        public async Task AddFavoriteDetail(UserFavoritesDetail t)
        {
            await _context.UserFavoritesDetails.AddAsync(t);
        }

        public async Task<UserFavorite> GetFavoriteAsync(string userId)
        {
            var favorite = await _context.UserFavorites.Include(p => p.UserFavoritesDetails).ThenInclude(p=>p.Product).ThenInclude(p=>p.ProductImages)
                .SingleOrDefaultAsync(p => p.UserId == userId);

            return favorite;
        }
        public async Task<UserFavoritesDetail> GetFavoriteDetailAsync(int favoriteDetailId)
        {
            var favoriteDetail = await _context.UserFavoritesDetails.SingleOrDefaultAsync(p=>p.UserFavoritesDetailId== favoriteDetailId);

            return favoriteDetail;
        }

        public void RemoveFavoriteDetail(UserFavoritesDetail t)
        {
            _context.UserFavoritesDetails.Remove(t);
        }

    }
}
