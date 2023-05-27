using Application.InterFaces.User;
using Application.ViewModels.User;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MyShop.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyShop.XUnitTest
{
    public class ProductControllerTest
    {
        private readonly Mock<IConfiguration> _configuration = new();
        private readonly Mock<IProductUserServices> _productServices = new();
        private readonly new Mock<IAccountUserServices> _accountUserServices = new();
        #region Index Action
        [Fact]
        public async Task Index_GetProductsListWithCategoriesId_ReturnsAListOfProducts()
        {
            // Arranger
            var expectedData = CreateFakeProductIndexData();
            _productServices.Setup(service => service.GetProductsListAsync(new List<int> { 0 }))
                .ReturnsAsync(expectedData);

            var controller = new ProductController(_productServices.Object,
                _accountUserServices.Object,
                _configuration.Object);
            // Act

            var result = await controller.Index(new int[1], 1, "", "");

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<ProductIndexViewModel>(viewResult.Model);
        }
        [Fact]
        public async Task Index_GetProductsListWithCategoriesId_ReturnsNull()
        {
            // Arranger
            var expectedData = CreateFakeProductIndexData();
            _productServices.Setup(service => service.GetProductsListAsync(new List<int> { 1, 2, 3 }))
                .ReturnsAsync(expectedData);

            var controller = new ProductController(_productServices.Object,
                _accountUserServices.Object,
                _configuration.Object);
            // Act

            var result = await controller.Index(new int[1], 1, "", "");

            // Assert
            var res = result as ViewResult;
            Assert.Null(res);
        }
        [Fact]
        public async Task Index_GetProductsListWithSearchProduct_ReturnsNull()
        {
            // Arranger
            var expectedData = CreateFakeProductIndexData();
            _productServices.Setup(service => service.GetProductsListAsync(new List<int> { 0 }))
                .ReturnsAsync(expectedData);

            var controller = new ProductController(_productServices.Object,
                _accountUserServices.Object,
                _configuration.Object);
            // Act

            var result = await controller.Index(new int[1], 1, "7", "");

            // Assert
            var res = result as ViewResult;
            var dto = res.Model as ProductIndexViewModel;
            Assert.Equal(dto.Products.Count(), 0);
        }
        #endregion

        #region Get Product Describtion
        [Fact]
        public async Task Descibtion_GetProductDescribtionWithOurUserId_ReturnsAProduct()
        {
            // Arrange
            var expectedProduct = CreateFakeProductDesciptionData();
            _productServices.Setup(service => service.GetProductDescriptionAsync(1, ""))
                .ReturnsAsync(expectedProduct);

            var controller = new ProductController(_productServices.Object,
                _accountUserServices.Object, _configuration.Object);
            // Act
            var result =await controller.Description(1);

            // Assert
            var resAsViewResult = result as ViewResult;
            Assert.Same(expectedProduct, resAsViewResult?.Model);
        }
        #endregion

        private ProductIndexViewModel CreateFakeProductIndexData()
        {
            return new ProductIndexViewModel()
            {
                Products = new List<GetListOfProductViewModel>()
                {
                    new GetListOfProductViewModel(){Id=1,Count=10,ImageSrc="",InstagramPost=false,Name="Product1",Price=5000},
                    new GetListOfProductViewModel(){Id=2,Count=22,ImageSrc="",InstagramPost=false,Name="Product2",Price=5000},
                    new GetListOfProductViewModel(){Id=3,Count=33,ImageSrc="",InstagramPost=false,Name="Product3",Price=5000},
                    new GetListOfProductViewModel(){Id=4,Count=44,ImageSrc="",InstagramPost=false,Name="Product4",Price=5000},
                    new GetListOfProductViewModel(){Id=5,Count=55,ImageSrc="",InstagramPost=false,Name="Product5",Price=5000}
                },
                CategoriesId = new Collection<int>(),
                CategoriesTreeView = new List<GetCategoriesTreeViewViewModel>()
            };
        }

        private GetProductDescriptionViewModel CreateFakeProductDesciptionData()
        {
            var attributesValue = new List<AttributeValue>()
            {
                new AttributeValue(){ValueId=1,ValueName="Red",ProductAttributeId=1},
                new AttributeValue(){ValueId=2,ValueName="Big",ProductAttributeId=2},
                new AttributeValue(){ValueId=3,ValueName="Men",ProductAttributeId=3},
                new AttributeValue(){ValueId=4,ValueName="Cloth",ProductAttributeId=4},
            };
            var attributes = new List<ProductAttribute>()
            {
                new ProductAttribute(){AttributeId=1,AttributeName="Color",AttributeValues=attributesValue,ProductId=1},
                new ProductAttribute(){AttributeId=2,AttributeName="Size",AttributeValues=attributesValue,ProductId=1},
                new ProductAttribute(){AttributeId=3,AttributeName="Gender",AttributeValues=attributesValue,ProductId=1},
                new ProductAttribute(){AttributeId=4,AttributeName="Type",AttributeValues=attributesValue,ProductId=1}
            };
            return new()
            {
                Count = 10,
                Comments = null,
                Id = 1,
                IsProductHaveAttributes = false,
                Name = "Product 1",
                Properties = null,
                Questions = null,
                Attributes = null,
                CategoryId = 1,
                CategoryName = "Test",
                Comment = null,
                Description = "Test",
                Images = null,
                InstagramPost = false,
                IsInUserFavorite = false,
                Price = "10000",
                Question = null,
                TemplateId = 0,
                Templates = null
            };
        }
    }
}