using Application.InterFaces.Admin;
using Application.Utilities.TagHelper;
using Application.ViewModels.Admin;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Admin
{
    public class ProductServices : IProudctServices
    {
        #region Ingection
        private readonly IProductRepository _productRepository;
        private readonly ICategoryServices _categoryServices;
        public ProductServices(IProductRepository productRepository,
            ICategoryServices categoryServices)
        {
            _productRepository = productRepository;
            _categoryServices = categoryServices;
        }

        #endregion
        public async Task<IEnumerable<GetProductsAndImageSrcViewModel>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var productsReturn = products.Select(p => new GetProductsAndImageSrcViewModel()
            {
                Id = p.Id,
                Count = p.Count,
                Name = p.Name,
                Price = p.Price,
                ImageSrc = p.ProductImages.OrderBy(c => c.Id).Select(c => c.ImgFile + "/" + c.ImgSrc).Take(2),
                CategoryId = p.CategoryId,
                CategoryName = _categoryServices.GetCategoryAsync(p.CategoryId).GetAwaiter().GetResult().Name,
                ImagesCount = p.ProductImages.Count - 2
            }).ToList();
            return productsReturn;
        }
        public async Task<AddProductViewModel> GetCategoriesTreeViewForAdd()
        {
            var categoriesTreeView = await _categoryServices.GetCategoriesTreeForAdd();
            var returnCategories = new AddProductViewModel()
            {
                CategoriesTreeView = categoriesTreeView.CategoriesTreeView.Skip(1),
            };
            return returnCategories;
        }
        public async Task<GetProductViewModel> GetProductAsync(int productId)
        {
            var product = await _productRepository.GetProductAsync(productId);
            var categories = await _categoryServices.GetCategoriesTreeForAdd();
            var returnProduct = new GetProductViewModel()
            {
                Id = product.Id,
                Count = product.Count,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Detail = product.Detail,
                Price = product.Price,
                CategoryName = product.Category.Name,
                Images = product.ProductImages.Select(p => new GetImagesViewModel()
                {
                    ImgSrc = p.ImgFile + "/" + p.ImgSrc,
                    ImgId = p.Id

                }).ToList(),
                Categories = categories.CategoriesTreeView,
                Properties = product.Properties.Select(p => new GetProductProperties()
                {
                    ValueName = p.ValueName,
                    ValueType = p.ValueType,
                    ValueId = p.PropertyId
                }).ToList()
            };
            return returnProduct;
        }
        public async Task<bool> DeleteListOfProducts(IEnumerable<GetProductsAndImageSrcViewModel> deleteListOfProducts)
        {
            try
            {
                var getProductIdsFordelete = deleteListOfProducts.Where(p => p.IsSelected).Select(p => p.Id);
                var productsList = new List<Product>();
                var productImagesList = new List<ProductImages>();
                var productPropertiesList = new List<ProductProperty>();
                foreach (var productId in getProductIdsFordelete)
                {
                    var product = await _productRepository.GetProductAsync(productId);
                    productsList.Add(product);
                    productImagesList = product.ProductImages.ToList();
                    productPropertiesList = product.Properties.ToList();
                }
                if (productImagesList != null)
                {
                    foreach (var image in productImagesList)
                    {
                        var result = DeletePhoto(image.ImgSrc, image.ImgFile);
                        if (!result)
                            break;
                    }
                    _productRepository.DeletePhoto(productImagesList);
                }
                if (productPropertiesList != null)
                {
                    _productRepository.DeleteProductProperties(productPropertiesList);
                }
                _productRepository.DeleteProduct(productsList);

                await _productRepository.SaveAsync();
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }



        public async Task<bool> AddProductAsync(AddProductViewModel addProduct)
        {
            try
            {
                var category = await FindCategory(addProduct.CatId);
                if (category == null)
                {
                    return false;
                }

                var productForAdd = new Product()
                {
                    Name = addProduct.Name,
                    Count = addProduct.Count,
                    Price = addProduct.Price,
                    Detail = addProduct.Detail,
                    CategoryId = addProduct.CatId,
                    InsertTime = ConverToShamsi.GetDate(DateTime.Now),
                };
                await _productRepository.AddProductAsync(productForAdd);

                if (addProduct.ValueName != null && addProduct.ValueType != null)
                {
                    var productproperties = new List<ProductProperty>();
                    for (int i = 0; i < addProduct.ValueName.Count(); i++)
                    {
                        productproperties.Add(new ProductProperty()
                        {
                            ValueName = addProduct.ValueName[i],
                            ValueType = addProduct.ValueType[i],
                            ProductId = productForAdd.Id,
                            Product = productForAdd
                        });
                    }
                    await _productRepository.AddProductPropertiesAsync(productproperties);
                }

                var images = new List<(string src, string fileName)>();
                var placeFile = "ProductImages";

                foreach (var img in addProduct.Images)
                {
                    var resultUploadImage = uploadImage(img, placeFile);
                    images.Add(new(resultUploadImage.src, resultUploadImage.fileName));
                }
                var uploadProductImages = images.Select(p => new ProductImages()
                {
                    ImgSrc = p.src,
                    ImgFile = p.fileName,
                    Product = productForAdd,
                    ProductId = productForAdd.Id
                });
                await _productRepository.AddProductImagesAsync(uploadProductImages);

                if (addProduct.Adjectives != null)
                {
                    await AddAdjective(addProduct.Adjectives);
                    async Task AddAdjective(IEnumerable<AdjectiveViewModel> t)
                    {
                        var adjective = new List<Adjective>();
                        foreach (var item in t)
                        {
                            var singleAdjective = new Adjective()
                            {
                                AdjectiveName = item.AdjectiveName,
                                ProductId = productForAdd.Id,
                            };
                            await _productRepository.AddProductAdjectivesAsync(singleAdjective);
                            if(item.Values!=null)
                            {
                                foreach (var value in item.Values)
                                {
                                    var valueReturn = await GetAdjectiveValue(value,item.AdjectiveId);
                                    singleAdjective.Values.Add(valueReturn);
                                }
                            }
                            adjective.Add(singleAdjective);
                        }
                    }
                    async Task<Value> GetAdjectiveValue(ValueViewModel t,int adjectiveId)
                    {
                        var value = new Value()
                        {
                            ValueName = t.ValueName,
                            AdjectiveId = adjectiveId,
                            ValueCount = t.Count,
                            ValuePrice = t.Price
                        };
                        await _productRepository.AddProductAdjectiveValuesAsync(value);
                        if(t.SubValue!=null)
                        {
                            var subValueReturn =await GetSubValue(t.SubValue,null);
                            value.SubValue = new SubValue()
                            {
                                SubValueName = subValueReturn.SubValueName,
                                ValueId = value.ValueId,
                                Valuetype = subValueReturn.Valuetype
                            };
                        }
                        return value;
                    }
                    async Task<SubValue> GetSubValue(SubValueViewModel t,int? parentId)
                    {
                        var subvalue = new SubValue()
                        {
                            SubValueName = t.SubValueName,
                            Valuetype = t.Valuetype,
                            ParentId=parentId
                        };
                        await _productRepository.AddProductAdjectiveValueSubValueAsync(subvalue);
                        if(t.SubValue!=null)
                        {
                            var subValueReturn = await GetSubValue(t.SubValue,subvalue.SubValueId);
                            subvalue.GetSubValue = new SubValue()
                            {
                                SubValueName = subValueReturn.SubValueName,
                                ParentId = subValueReturn.ParentId,
                                Valuetype = subValueReturn.Valuetype
                            };
                        }
                        return subvalue;
                    }
                }

                await _productRepository.SaveAsync();
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }
        public async Task<bool> EditProductAsync(GetProductViewModel editProduct)
        {
            try
            {
                if (editProduct.Id == 0) return false;
                var findProductById = await _productRepository.GetProductAsync(editProduct.Id);
                findProductById.Name = editProduct.Name;
                findProductById.Detail = editProduct.Detail;
                findProductById.Count = editProduct.Count;
                findProductById.Price = editProduct.Price;
                findProductById.CategoryId = editProduct.CategoryId;
                _productRepository.EditProduct(findProductById);

                if (editProduct.DeletedProductImagesIds != null)
                {
                    var productsFindForDelete = new List<ProductImages>();
                    foreach (var image in editProduct.DeletedProductImagesIds)
                    {
                        var findImageById = await _productRepository.FindImageByIdAsync(image);
                        productsFindForDelete.Add(findImageById);
                        var result = DeletePhoto(findImageById.ImgSrc, findImageById.ImgFile);
                        if (!result)
                            return false;
                        else
                            continue;
                    }
                    _productRepository.DeletePhoto(productsFindForDelete);
                }

                if (editProduct.UploadImages != null)
                {
                    var productImages = new List<ProductImages>();
                    foreach (var imageForUpload in editProduct.UploadImages)
                    {
                        var imageUploadResult = uploadImage(imageForUpload, "ProductImages");
                        var productUploaded = new ProductImages()
                        {
                            ImgSrc = imageUploadResult.src,
                            ImgFile = imageUploadResult.fileName,
                            Product = findProductById,
                            ProductId = findProductById.Id,
                        };
                        productImages.Add(productUploaded);
                    }
                    await _productRepository.AddProductImagesAsync(productImages);
                }

                if (editProduct.PropertiesDeletedIds != null)
                {
                    var properties = new List<ProductProperty>();
                    foreach (var property in editProduct.PropertiesDeletedIds)
                    {
                        var propertyReturn = await _productRepository.FindPropertyByIdAsync(property);
                        properties.Add(propertyReturn);
                    }
                    _productRepository.DeleteProductProperties(properties);
                }
                if (editProduct.Valuetype != null)
                {
                    var productProperties = new List<ProductProperty>();
                    for (int i = 0; i < editProduct.Valuetype.Count(); i++)
                    {
                        var productProperty = new ProductProperty()
                        {
                            Product = findProductById,
                            ProductId = findProductById.Id,
                            ValueName = editProduct.ValueName[i],
                            ValueType = editProduct.Valuetype[i]
                        };
                        productProperties.Add(productProperty);
                    }
                    await _productRepository.AddProductProperties(productProperties);
                }

                await _productRepository.SaveAsync();
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }

        public async Task<bool> DeletePhotoAsync(int imageId)
        {
            try
            {
                var photo = await _productRepository.FindImageByIdAsync(imageId);
                if (photo == null)
                    return false;
                var result = DeletePhoto(photo.ImgSrc, photo.ImgFile);
                if (!result)
                    return false;

                _productRepository.DeletePhoto(photo);
                await _productRepository.SaveAsync();
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return false;
            }
        }
        public JsonResult UploadFileEditor(IFormFile file)
        {
            try
            {
                if (file.Length <= 0) return null;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Editor");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName).ToLower();
                using (var stream = new FileStream(filePath + "/" + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var url = $"/Images/Editor/{fileName}";
                var result = new UploadFileForCkEditorResultViewModel()
                {
                    FileName = fileName,
                    Uploaded = 1,
                    Url = url
                };
                var successResultJson = new JsonResult(result);
                return successResultJson;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        #region Private Methode Helper
        private (string src, string fileName) uploadImage(IFormFile file, string placeFile)
        {
            if (file == null)
                return ("", "");



            var todayDate = ConverToShamsi.GetMonthAndYear(DateTime.Now);
            string folder = $@"wwwroot\Images\{placeFile}\{todayDate}\";
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }
            if (file.Length != 0)
            {

                string fileName = DateTime.Now.Ticks.ToString() + "-" + file.FileName;
                string filePath = Path.Combine(uploadsRootFolder, fileName);
                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(FileStream);
                }
                return (fileName, todayDate);
            }
            else
                return ("", "");
        }
        private bool DeletePhoto(string imageName, string fileName)
        {
            if (imageName == "" || fileName == "")
                return false;

            string folder = $@"wwwroot\Images\ProductImages\{fileName}\{imageName}";
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (File.Exists(uploadsRootFolder))
            {
                File.Delete(uploadsRootFolder);
                return true;
            }
            else
                return false;
        }
        private async Task<Category> FindCategory(int categoryId)
        {
            try
            {
                var category = await _categoryServices.GetCategoryAsync(categoryId);
                var returnCategory = new Category()
                {
                    Id = category.Id,
                    Name = category.Name
                };
                return returnCategory;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        #endregion
    }
}
