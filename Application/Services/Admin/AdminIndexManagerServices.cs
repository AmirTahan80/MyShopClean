using Application.InterFaces.Admin;
using Application.Utilities.TagHelper;
using Application.ViewModels.Admin;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class AdminIndexManagerServices : IAdminIndexManagerServices
    {
        #region Injections
        private readonly IProductRepository _productRepository;
        private readonly ICommentRepository _comentRepository;
        private readonly IPayRepository _payRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminIndexManagerServices(IProductRepository productRepository,
            ICommentRepository comentRepository,
            IPayRepository payRepository,
            IQuestionRepository questionRepository,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _comentRepository = comentRepository;
            _payRepository = payRepository;
            _questionRepository = questionRepository;
            _userManager = userManager;
        }

        #endregion
        public async Task<int> GetProductCountAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var productsCount = products.Count();
            return productsCount;
        }

        public async Task<int> GetTodayCommentsAsync()
        {
            var comments = await _comentRepository.GetCommentsAsync();
            var commentsCount = comments
                .Where(p => p.CommentInsertTime == ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now))
                .ToList()
                .Count();
            return commentsCount;
        }

        public async Task<int> GetTodayFactorsAsync()
        {
            var factors = await _payRepository.GetFactors();
            var facotrsCount = factors
                .Where(p => p.CreateTime == ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now))
                .ToList()
                .Count();
            return facotrsCount;
        }

        public async Task<int> GetTodayProductCreateAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var productsCount = products
                .Where(p => p.InsertTime == ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now))
                .ToList()
                .Count();
            return productsCount;
        }

        public async Task<int> GetTodayQuestionsAsync()
        {
            var questions = await _questionRepository.GetQuestionsAsync();
            var questionsCount = questions
                .Where(p => p.InserTime == ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now))
                .ToList()
                .Count();
            return questionsCount;
        }

        public async Task<int> GetTodayUserRegisterAsync()
        {
            var todayDate = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);
            var users = await _userManager.Users
                .Where(p => p.RegisterTime == todayDate)
                .ToListAsync();
            var usersCount = users.Count();
            return usersCount;
        }

        public async Task<int> GetUserCountAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersCount = users.Count();
            return usersCount;
        }
        public async Task<ICollection<FactorViewModel>> GetFactorsAsync()
        {
            var today = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now);
            var factors = await _payRepository.GetFactors();
            var todayFactors = factors.Where(p => p.CreateTime == today).ToList();

            var returnFactor = todayFactors.Select(p => new FactorViewModel()
            {
                Id = p.Id,
                DisCountNames = p.Discounts!=null?p.Discounts.Select(c => c.CodeName).ToList():null,
                DisCountPrices = p.Discounts != null ?p.Discounts.Select(c => c.DiscountPrice).ToList():null,
                UserName=p.UserName,
                TotalPrice=p.TotalPrice
            }).ToList();

            return returnFactor;
        }
    }
}
