using OECS.Models;
using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.BrandRepository;
using OECS.Repository.CategoryRepository;
using OECS.Repository.ProductRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.ColorRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository;
using OECS.Services.BrandServices;
using OECS.Services.CategoryServices;
using OECS.Services.ProductServices.ProductDetailServices.ColorServices;
using OECS.Services.ProductServices.ProductDetailServices.SizeServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            _colorService = new ColorService(new ColorRepository(new oecsEntities()));
            _sizeService = new SizeService(new SizeRepository(new oecsEntities()));
            _brandService = new BrandService(new BrandRepository(new oecsEntities()));
            _categoryService = new CategoryService(new CategoryRepository(new oecsEntities()));
        }

        public List<ViewProductDetailModel> ViewListingNewlyAddedProduct(int? columnToSort, string sortDir)
        {
            List<ViewProductDetailModel> product = _productRepository.ViewListingNewlyAddedProduct(columnToSort, sortDir);

            switch (columnToSort)  //column sorting
            {
                case 0:
                    product = product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 1:
                    product = sortDir == "asc" ? product.OrderBy(c => c.ProductName).ToList() : product.OrderByDescending(c => c.ProductName).ToList();
                    break;
                case 2:
                    product = sortDir == "asc" ? product.OrderBy(c => c.BrandName).ToList() : product.OrderByDescending(c => c.BrandName).ToList();
                    break;
                case 3:
                    product = sortDir == "asc" ? product.OrderBy(c => c.CategoryName).ToList() : product.OrderByDescending(c => c.CategoryName).ToList();
                    break;
                case 5:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Date).ToList() : product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 6:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Price).ToList() : product.OrderByDescending(c => c.Price).ToList();
                    break;
            }
            return product;
        }

        public List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir)
        {
            List<ViewProductDetailModel> product = _productRepository.ViewListingProduct(columnToSort, sortDir);

            switch (columnToSort)  //column sorting
            {
                case 0:
                    product = product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 4:
                    product = sortDir == "asc" ? product.OrderBy(c => c.ProductName).ToList() : product.OrderByDescending(c => c.ProductName).ToList();
                    break;
                case 5:
                    product = sortDir == "asc" ? product.OrderBy(c => c.BrandName).ToList() : product.OrderByDescending(c => c.BrandName).ToList();
                    break;
                case 6:
                    product = sortDir == "asc" ? product.OrderBy(c => c.CategoryName).ToList() : product.OrderByDescending(c => c.CategoryName).ToList();
                    break;
                case 8:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Date).ToList() : product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 9:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Price).ToList() : product.OrderByDescending(c => c.Price).ToList();
                    break;
            }
            return product;
        }

        public List<ViewProductModel> ShowProductList(int filter)
        {
            return _productRepository.ShowProductList(filter);
        }

        public ProductModel AssignProductDetail(ProductModel productModel)
        {
            productModel.BrandSelectList = _brandService.ListBrands();
            productModel.CategorySelectList = _categoryService.ListCategory();
            productModel.Date = DateTime.Now.ToString("yyyy-MM-dd");

            return productModel;
        }

        public ProductDetailModel AssignProductDetail(ProductDetailModel productDetailModel)
        {
            productDetailModel.SizeList = _sizeService.SizeList();
            productDetailModel.ColorList = _colorService.ListingColor();
            return productDetailModel;
        }
        public List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID)
        {
            if (categoryID != 0 && subCategoryID == 0)
            {
                product = product.Where(p => p.Product.SubCategory.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && subCategoryID != 0)
            {
                product = product.Where(p => p.Product.SubCategoryID == subCategoryID).ToList();
            }
            else if (categoryID != 0 && subCategoryID != 0)
            {
                product = product.Where(p => p.Product.SubCategory.CategoryID == categoryID && p.Product.SubCategoryID == subCategoryID).ToList();
            }

            return OnFilter(product, brandID, colorID, sizeID);
        }

        private List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? brandID, int? colorID, int? sizeID)
        {
            if (colorID != 0 && sizeID == 0)
            {
                product = product.Where(p => p.ProductDetail.ColorID == colorID && p.DisplayColor.isDisplay == true).ToList();
            }
            else if (colorID == 0 && sizeID != 0)
            {
                product = product.Where(p => p.ProductDetail.SizeID == sizeID && p.DisplaySize.isDisplay == true).ToList();
            }

            if (brandID != 0)
            {
                product = product.Where(p => p.Product.BrandID == brandID).ToList();
            }
            return product;
        }

        public bool CreateProduct(ProductModel productModel)
        {
            Product product = new Product();
            product.productName = productModel.ProductName;
            product.SubCategoryID = productModel.SubCategoryID;
            product.BrandID = productModel.BrandID;
            product.date = Convert.ToDateTime(productModel.Date);
            product.price = productModel.Price;
            product.description = productModel.Description;

            return _productRepository.CreateProduct(product);
        }

        public Product FindByID(int productID)
        {
            return _productRepository.FindByID(productID);
        }

        public ProductModel AssignProduct(Product product)
        {
            ProductModel productModel = new ProductModel();

            productModel.ProductID = product.ProductID;
            productModel.ProductName = product.productName;
            productModel.SubCategoryID = product.SubCategoryID;
            productModel.BrandID = product.BrandID;
            productModel.Date = Convert.ToDateTime(product.date.ToString()).ToString("yyyy-MM-dd");
            productModel.Price = product.price;
            productModel.Description = product.description;
            productModel.BrandSelectList = _brandService.ListBrands();
            productModel.CategorySelectList = _categoryService.ListCategory();

            return productModel;
        }
    }
}