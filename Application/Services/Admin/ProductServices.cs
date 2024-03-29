﻿using Application.InterFaces.Admin;
using Application.Utilities.TagHelper;
using Application.ViewModels;
using Application.ViewModels.Admin;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using InstagramApiSharp.Classes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly ICartRepository _cartRepository;
        public ProductServices(IProductRepository productRepository,
            ICategoryServices categoryServices,
            ICartRepository cartRepository)
        {
            _productRepository = productRepository;
            _categoryServices = categoryServices;
            _cartRepository = cartRepository;
        }

        #endregion
        public async Task<IEnumerable<GetProductsAndImageSrcViewModel>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            products = products.OrderByDescending(p => p.Id);
            var productsReturn = products.Select(p => new GetProductsAndImageSrcViewModel()
            {
                Id = p.Id,
                Count = p.Count,
                Name = p.Name,
                Price = p.Price,
                InstagramPost=p.ProductImages.FirstOrDefault().ImgFile==""?true:false,
                ImageSrc = p.ProductImages
                .OrderBy(c => c.Id)
                .Select(c => (c.ImgFile == ""?"": c.ImgFile + "/") + c.ImgSrc)
                .Take(2),
                CategoryId = p.Categories
                .Select(c => c.CategoryId)
                .LastOrDefault(),
                CategoryName = _categoryServices.GetCategoryAsync(p.Categories.LastOrDefault().CategoryId)
                .GetAwaiter().GetResult().Name,
                ImagesCount = p.ProductImages.Count - 2
            }).ToList();
            return productsReturn;
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
                CategoriesId = product.Categories.Select(p => p.CategoryId).ToList(),
                Detail = product.Detail,
                Price = product.Price,
                CategoryName = product.Categories.Select(p => p.Category.Name).ToList(),
                InstagramPost = product.ProductImages.FirstOrDefault().ImgFile == "" ? true : false,
                Images = product.ProductImages.Select(p => new GetImagesViewModel()
                {
                    ImgSrc = (p.ImgFile == "" ? "" : p.ImgFile + "/") + p.ImgSrc,
                    ImgId = p.Id

                }).ToList(),
                Categories = categories.CategoriesTreeView,
                Properties = product.Properties.Select(p => new GetProductProperties()
                {
                    ValueName = p.ValueName,
                    ValueType = p.ValueType,
                    ValueId = p.PropertyId
                }).ToList(),
                ProductAttributesTemplates = product.AttributeTemplates.Select(p => new ProductAttributesTemplatesViewModel()
                {
                    Template = p.Template,
                    PriceOfTemplate = p.AttrinbuteTemplatePrice,
                    CountOfTemplate = p.AttrinbuteTemplateCount
                }).ToList(),
                ProductAttributeNameAndValues = product.ProductAttributes.Select(p => new ProductAttributeNameAndValuesViewModel()
                {
                    Name = p.AttributeName,
                    Values = p.AttributeValues.Select(c => new ProductAttributeValuesViewModel()
                    {
                        ValueName = c.ValueName
                    }).ToList(),
                }).ToList(),
                IsProductHaveAttributes = product.IsProductHaveAttributes,
            };
            return returnProduct;
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


        public async Task<bool> AddProductAsync(AddProductViewModel addProduct)
        {
            try
            {
                var categories = new List<Category>();
                foreach (var item in addProduct.CategoriesId)
                {
                    var findCategory = await FindCategory(item);
                    categories.Add(findCategory);
                }
                if (categories.Count() == 0)
                {
                    return false;
                }

                if (addProduct.IsProductHaveAttributes)
                {
                    var LowerPriceOfAttributes = 0;
                    var CountOfLowerPrice = 0;
                    for (int i = 0; i < addProduct.AttributePrice.Count; i++)
                    {
                        if (LowerPriceOfAttributes == 0)
                        {
                            if (addProduct.AttributeCount[i] != null && addProduct.AttributeCount[i] != null)
                            {
                                var intPrice = Convert.ToInt32(addProduct.AttributePrice[i]);
                                var intCount = Convert.ToInt32(addProduct.AttributeCount[i]);
                                LowerPriceOfAttributes = intPrice;
                                CountOfLowerPrice = intCount;
                            }
                        }
                        else
                        {
                            if (addProduct.AttributeCount[i] != null && addProduct.AttributeCount[i] != null)
                            {
                                var intPrice = Convert.ToInt32(addProduct.AttributePrice[i]);
                                var intCount = Convert.ToInt32(addProduct.AttributeCount[i]);
                                if (intPrice < LowerPriceOfAttributes)
                                {
                                    LowerPriceOfAttributes = intPrice;
                                    CountOfLowerPrice = intCount;
                                }
                            }
                        }
                    }
                    addProduct.Count = CountOfLowerPrice;
                    addProduct.Price = LowerPriceOfAttributes;
                }

                var productForAdd = new Product()
                {
                    Name = addProduct.Name == null? "نام ندارد": addProduct.Name,
                    Count = addProduct.Count,
                    Price = addProduct.Price,
                    Detail = addProduct.Detail,
                    InsertTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now),
                    IsProductHaveAttributes = addProduct.IsProductHaveAttributes == true ? true : false,
                    Categories = categories.Select(p => new CategoryToProduct()
                    {
                        CategoryId = p.Id
                    }).ToList()
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

                if(addProduct.Images != null && addProduct.Images.Count() > 0)
                {
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
                }
                else if(addProduct.ImagesUri !=  null && addProduct.ImagesUri.Count > 0)
                {
                    var uploadProductImages = addProduct.ImagesUri.Select(p => new ProductImages()
                    {
                        ImgSrc = p,
                        ImgFile = "",
                        Product = productForAdd,
                        ProductId = productForAdd.Id
                    });
                    await _productRepository.AddProductImagesAsync(uploadProductImages);

                }

                if (addProduct.IsProductHaveAttributes)
                {
                    //AttributesName
                    var productAttributes = new List<ProductAttribute>();
                    for (int i = 0; i < addProduct.AttributeNames.Count; i++)
                    {
                        if (addProduct.AttributeValues[i] != "" || addProduct.AttributeValues[i] != null || addProduct.AttributeValues[i].Length! < 0)
                        {
                            productAttributes.Add(new ProductAttribute()
                            {
                                AttributeName = addProduct.AttributeNames[i],
                                ProductId = productForAdd.Id,
                                Product = productForAdd
                            });
                        }
                    }
                    await _productRepository.AddProductAttributes(productAttributes);
                    await _productRepository.SaveAsync();

                    //Values
                    var productAttributesValue = new List<AttributeValue>();
                    for (int i = 0; i < addProduct.AttributeNames.Count; i++)
                    {
                        if (addProduct.AttributeValues[i] != null || addProduct.AttributeValues[i].Length! < 0)
                        {
                            //List<string> arrayOfValues = new List<string>();
                            var counter = addProduct.AttributeValues[i].Split(",").Length;
                            var getValue = addProduct.AttributeValues[i].Split(",");
                            //for (int b = 0; b < counter; b++)
                            //{
                            //    arrayOfValues.Add(new(getValue[b]));
                            //}
                            for (int b = 0; b < getValue.Length; b++)
                            {
                                productAttributesValue.Add(new AttributeValue()
                                {
                                    ValueName = getValue[b].TrimEnd().TrimStart(),
                                    ProductAttributeId = productAttributes[i].AttributeId,
                                    ProductAttribute = productAttributes[i]
                                });
                            }
                        }
                    }
                    await _productRepository.AddAttributeValues(productAttributesValue);
                    await _productRepository.SaveAsync();

                    //Template
                    var productAttributesTemplates = new List<AttributeTemplate>();
                    for (int i = 0; i < addProduct.AttributeTemplates.Count; i++)
                    {
                        if (addProduct.AttributePrice[i] != null && addProduct.AttributeCount[i] != null)
                        {
                            if (addProduct.AttributePrice[i] == null)
                            {
                                addProduct.AttributePrice[i] = "0";
                            }
                            else if (addProduct.AttributeCount[i] == null)
                            {
                                addProduct.AttributeCount[i] = "0";
                            }
                            productAttributesTemplates.Add(new AttributeTemplate()
                            {
                                Template = addProduct.AttributeTemplates[i],
                                AttrinbuteTemplateCount = Convert.ToInt32(addProduct.AttributeCount[i]),
                                AttrinbuteTemplatePrice = Convert.ToInt32(addProduct.AttributePrice[i]),
                                ProductId = productForAdd.Id,
                                Product = productForAdd
                            });
                        }
                    }
                    await _productRepository.AddAttributeTemplates(productAttributesTemplates);
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

                if (editProduct.IsProductHaveAttributes)
                {
                    var LowerPriceOfAttributes = 0;
                    var CountOfLowerPrice = 0;
                    for (int i = 0; i < editProduct.AttributePrice.Count; i++)
                    {
                        if (LowerPriceOfAttributes == 0)
                        {
                            if (editProduct.AttributeCount[i] != null && editProduct.AttributeCount[i] != null)
                            {
                                var intPrice = Convert.ToInt32(editProduct.AttributePrice[i]);
                                var intCount = Convert.ToInt32(editProduct.AttributeCount[i]);
                                LowerPriceOfAttributes = intPrice;
                                CountOfLowerPrice = intCount;
                            }
                        }
                        else
                        {
                            if (editProduct.AttributeCount[i] != null && editProduct.AttributeCount[i] != null)
                            {
                                var intPrice = Convert.ToInt32(editProduct.AttributePrice[i]);
                                var intCount = Convert.ToInt32(editProduct.AttributeCount[i]);
                                if (intPrice < LowerPriceOfAttributes)
                                {
                                    LowerPriceOfAttributes = intPrice;
                                    CountOfLowerPrice = intCount;
                                }
                            }
                        }
                    }
                    editProduct.Count = CountOfLowerPrice;
                    editProduct.Price = LowerPriceOfAttributes;

                }

                var findProductById = await _productRepository.GetProductAsync(editProduct.Id);

                if (editProduct.CategoriesId.Count() > 0)
                {
                    var categories = new List<Category>();
                    foreach (var item in editProduct.CategoriesId)
                    {
                        var findCategory = await FindCategory(item);
                        categories.Add(findCategory);
                    }
                    findProductById.Categories = categories.Select(p => new CategoryToProduct()
                    {
                        CategoryId = p.Id
                    }).ToList();
                }

                findProductById.Name = editProduct.Name;
                findProductById.Detail = editProduct.Detail;
                findProductById.Count = editProduct.Count;
                findProductById.Price = editProduct.Price;
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
                    await _productRepository.AddProductPropertiesAsync(productProperties);
                }

                if (editProduct.IsProductHaveAttributes)
                {
                    //Delete Attributes /Name/Values/Template From Product
                    var productFindAttributes = findProductById.ProductAttributes;
                    var productFindAttributesTemplate = findProductById.AttributeTemplates;

                    var productFindAttributeValues = new List<AttributeValue>();

                    foreach (var item in productFindAttributes)
                    {
                        _productRepository.DeleteProductAttributeValues(item.AttributeValues);
                    }

                    _productRepository.DeleteAttributesNamesAndtemplates(productFindAttributes,
                        productFindAttributesTemplate);

                    //Add Attributes /Names/Values/Templates
                    if (editProduct.AttributeNames.Count > 0)
                    {
                        //Add AttributesName
                        var productAttributesName = new List<ProductAttribute>();
                        for (int i = 0; i < editProduct.AttributeNames.Count; i++)
                        {
                            if (editProduct.AttributeValues[i] != "" || editProduct.AttributeValues[i] != null || editProduct.AttributeValues[i].Length! < 0)
                            {
                                productAttributesName.Add(new ProductAttribute()
                                {
                                    AttributeName = editProduct.AttributeNames[i],
                                    Product = findProductById,
                                    ProductId = findProductById.Id
                                });
                            }
                        }
                        await _productRepository.AddProductAttributes(productAttributesName);

                        //Add AtributesValues
                        var productAttributeValues = new List<AttributeValue>();
                        for (int i = 0; i < editProduct.AttributeNames.Count; i++)
                        {
                            if (editProduct.AttributeValues[i] != null || editProduct.AttributeValues[i].Length! < 0)
                            {
                                //var getValuesList = new List<string>();
                                var counterOfValues = editProduct.AttributeValues[i].Split(",").Length;
                                var splitIndexOfValues = editProduct.AttributeValues[i].Split(",");
                                for (int b = 0; b < splitIndexOfValues.Length; b++)
                                {
                                    productAttributeValues.Add(new AttributeValue()
                                    {
                                        ValueName = splitIndexOfValues[b],
                                        ProductAttribute = productAttributesName[i],
                                        ProductAttributeId = productAttributesName[i].AttributeId
                                    });
                                }
                            }
                        }
                        await _productRepository.AddAttributeValues(productAttributeValues);

                        //AddAttributesTemplates
                        var attributesTemplates = new List<AttributeTemplate>();
                        for (int i = 0; i < editProduct.AttributeTemplates.Count; i++)
                        {
                            if (editProduct.AttributePrice[i] != null && editProduct.AttributeCount[i] != null)
                            {
                                if (editProduct.AttributePrice[i] == null)
                                {
                                    editProduct.AttributePrice[i] = "0";
                                }
                                else if (editProduct.AttributeCount[i] == null)
                                {
                                    editProduct.AttributeCount[i] = "0";
                                }
                                attributesTemplates.Add(new AttributeTemplate()
                                {
                                    Template = editProduct.AttributeTemplates[i],
                                    AttrinbuteTemplateCount = Convert.ToInt32(editProduct.AttributeCount[i]),
                                    AttrinbuteTemplatePrice = Convert.ToInt32(editProduct.AttributePrice[i]),
                                    Product = findProductById,
                                    ProductId = findProductById.Id
                                });
                            }
                        }
                        await _productRepository.AddAttributeTemplates(attributesTemplates);

                    }
                }
                else
                {
                    var productFindAttributes = findProductById.ProductAttributes;
                    var productFindAttributesTemplate = findProductById.AttributeTemplates;

                    var productFindAttributeValues = new List<AttributeValue>();

                    foreach (var item in productFindAttributes)
                    {
                        _productRepository.DeleteProductAttributeValues(item.AttributeValues);
                    }

                    _productRepository.DeleteAttributesNamesAndtemplates(productFindAttributes,
                        productFindAttributesTemplate);
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
        public async Task<ResultDto> EditDiscountAsync(DiscountViewMode discountEdit)
        {
            try
            {
                var discounts = await _cartRepository.GetDiscountsAsync();
                var discount = discounts.Where(p => p.Id == discountEdit.Id).SingleOrDefault();

                discount.CodeName = discountEdit.CodeName;
                discount.DiscountPrice = discount.DiscountPrice;

                await _cartRepository.SaveAsync();


                var returnResut = new ResultDto()
                {
                    SuccesMessage = "ویرایش کد تخفیف با موفقیت انجام شد ..",
                    Status = true
                };
                return returnResut;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResut = new ResultDto()
                {
                    ErrorMessage = "در ویرایش کد تخفیف مشکلی به وجود امده است لطفا دقایقی دیگر دوباره امتحان کنید !!!",
                    Status = false
                };
                return returnResut;
            }
        }


        public async Task<bool> DeleteListOfProducts(IEnumerable<GetProductsAndImageSrcViewModel> deleteListOfProducts)
        {
            try
            {
                var getProductIdsFordelete = deleteListOfProducts.Where(p => p.IsSelected).Select(p => p.Id);
                var carts = await _cartRepository.GetCartsAsync();

                var productsList = new List<Product>();

                foreach (var productId in getProductIdsFordelete)
                {
                    var product = await _productRepository.GetProductAsync(productId);
                    if (carts != null)
                    {
                        foreach (var cart in carts)
                        {
                            if (cart.CartDetails.Any())
                            {
                                var cartDetails = cart.CartDetails.Where(p => p.ProductId == product.Id).ToList();
                                if (cartDetails != null)
                                {
                                    foreach (var cartDetail in cartDetails)
                                    {
                                        _cartRepository.RemoveCartDetail(cartDetail);
                                    }
                                }
                            }
                        }
                    }
                    productsList.Add(product);
                    var productImagesList = product.ProductImages.ToList();
                    var productPropertiesList = product.Properties.ToList();
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

                    if (product.IsProductHaveAttributes)
                    {
                        foreach (var item in product.ProductAttributes)
                        {
                            _productRepository.DeleteProductAttributeValues(item.AttributeValues);
                        }
                        _productRepository.DeleteAttributesNamesAndtemplates(product.ProductAttributes,
                            product.AttributeTemplates);
                    }
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

        public async Task<ResultDto> CreateDiscountAsync(DiscountViewMode discountAdd)
        {
            try
            {
                var returnResult = new ResultDto();

                var createDiscount = new Discount()
                {
                    InsertTime = ConverToShamsi.GetDateYeadAndMonthAndDay(DateTime.Now),
                    DiscountPrice = discountAdd.CodePrice,
                    CodeName = discountAdd.CodeName
                };

                await _cartRepository.AddDiscountAsync(createDiscount);

                await _cartRepository.SaveAsync();

                returnResult.SuccesMessage = "کد تخفیف با موفقیت ثبت شد ...";
                returnResult.Status = true;

                return returnResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var returnResult = new ResultDto()
                {
                    ErrorMessage = "کد تخفیف ساخته نشد ... لطفا دوباره امتحان کنید!!",
                    Status = false
                };
                return returnResult;
            }
        }
        public async Task<DiscountViewMode> GetDiscountAsync(int id)
        {
            var discounts = await _cartRepository.GetDiscountsAsync();
            var discount = discounts.Where(p => p.Id == id).SingleOrDefault();

            var discountreturn = new DiscountViewMode()
            {
                Id = discount.Id,
                CodeName = discount.CodeName,
                CodePrice = discount.DiscountPrice
            };

            return discountreturn;
        }
        public async Task<IList<DiscountViewMode>> GetDisCountsAsync()
        {
            try
            {
                var discounts = await _cartRepository.GetDiscountsAsync();

                var returnDiscount = discounts.Select(p => new DiscountViewMode()
                {
                    Id = p.Id,
                    CodeName = p.CodeName,
                    CodePrice = p.DiscountPrice
                }).ToList();

                return returnDiscount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            if (fileName == "")
                return true;
            if (imageName == "")
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
