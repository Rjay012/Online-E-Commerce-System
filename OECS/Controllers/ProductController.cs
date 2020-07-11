using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OECS.Models.ProductModels;
using System.Data.Entity;
using System.Net;
using System.Web.UI.WebControls;
using System.Linq.Dynamic;
using OECS.Repository.ProductRepository;
using OECS.Repository.ProductRepository.ProductGalleryRepository;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.CategoryRepository;
using OECS.Repository.BrandRepository;

namespace OECS.Controllers
{
    public class ProductController : Controller
    {
        readonly oecsEntities dbContext = new oecsEntities();
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IProductGalleryRepository _productGalleryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController()
        {
            _productRepository = new ProductRepository(new oecsEntities());
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
            _productGalleryRepository = new ProductGalleryRepository(new oecsEntities());
            _brandRepository = new BrandRepository(new oecsEntities());
            _categoryRepository = new CategoryRepository(new oecsEntities());
        }

        // GET: Product
        [Authorize(Roles = "1,2,3")]
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Product";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Show(int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID, string searchString)
        {
            List<ViewProductModel> product = _productRepository.ShowProductList(1);
            if (colorID != 0) product = _productRepository.ShowProductList(2);
            if (sizeID != 0) product = _productRepository.ShowProductList(3);

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Product.SubCategory.subCategory1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            product = _productRepository.OnFilter(product, categoryID, subCategoryID, brandID, colorID, sizeID);
            if (colorID != 0 || sizeID != 0) return PartialView("Partials/_ProductList", product);
           
            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductImage.isMainDisplay == true).ToList());
        }

        #region ("START MODAL FORMS")
        public ActionResult ReadyProductListTable()
        {
            return PartialView("Partials/Tables/_ReadyProductList");
        }

        public ActionResult ShowNewlyAddedProductListTable()
        {
            return PartialView("Partials/Tables/_NewlyAddedProduct");
        }
        public ActionResult NewProductModalForm(ProductModel productModel)
        {
            productModel.BrandSelectList = _brandRepository.ListBrands();
            productModel.CategorySelectList = _categoryRepository.ListCategory();
            productModel.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return PartialView("Partials/Modals/_NewProduct", productModel);
        }

        public ActionResult EditProductModalForm(int? productID, ProductModel productModel)
        {
            if(productID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = dbContext.Product.Find(productID);

            if(product == null)
            {
                return HttpNotFound();
            }

            productModel.ProductID = product.ProductID;
            productModel.ProductName = product.productName;
            productModel.SubCategoryID = product.SubCategoryID;
            productModel.BrandID = product.BrandID;
            productModel.Date = Convert.ToDateTime(product.date.ToString()).ToString("yyyy-MM-dd");
            productModel.Price = product.price;
            productModel.Description = product.description;
            productModel.BrandSelectList = _brandRepository.ListBrands();
            productModel.CategorySelectList = _categoryRepository.ListCategory();

            return PartialView("Partials/Modals/_EditProduct", productModel);
        }

        public ActionResult NewProductDetailModalForm(ProductDetailModel productDetailModel)
        {
            return PartialView("Partials/Modals/_NewProductDetail", productDetailModel);
        }

        public ActionResult EditProductDetailModalForm(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null && iconID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ProductImage> productImage = _productGalleryRepository.ViewListingProductImage((int)productID, (int)colorID, (int)iconID);

            ProductDetailModel productDetailModel = new ProductDetailModel
            {
                ProductImage = productImage.ToList(),
                ProductID = (int)productID,
                ColorID = (int)colorID,
                IconID = (int)iconID,
                IconPath = productImage.Select(pi => new { pi.Image.Icon.icon1 }).FirstOrDefault().icon1,
                ProductDetailID = (int)productImage.Select(c => new { c.ProductDetailID }).FirstOrDefault().ProductDetailID
            };
            return PartialView("Partials/Modals/_EditProductDetail", productDetailModel);
        }
        #endregion ("END MODAL FORMS")

        #region ("START CREATE PRODUCT AND ITS DETAILS")
        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "CategorySelectList, BrandSelectList")]ProductModel productModel)
        {
            if(ModelState.IsValid)
            {
                if(_productRepository.CreateProduct(productModel) == true)
                {
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return View(productModel);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProductDetail(ProductDetailModel productDetailModel)
        {
            if (productDetailModel.IconFile == null || productDetailModel.Files == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                _productDetailRepository.CreateProductDetail(productDetailModel);
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        #endregion ("END CREATE PRODUCT AND ITS DETAILS")

        #region ("START UPDATE PRODUCT AND ITS DETAILS")
        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductDetail(ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                _productDetailRepository.EditProductDetail(productDetailModel);
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END UPDATE PRODUCT AND ITS DETAILS")

        #region ("START VIEWING AND MANAGING GALLERY")
        public ActionResult ViewPhotoGallery(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ProductImage> productImages = _productGalleryRepository.ViewListingProductImage((int)productID, (int)colorID, (int)iconID);
            ViewBag.ProductID = productID;
            return PartialView("Partials/Modals/_ProductImageGallery", productImages);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageDisplay(int? defaultDisplayID, int? selectedDisplayID)
        {
            if (defaultDisplayID == null || selectedDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _productGalleryRepository.SetImageDisplay(defaultDisplayID, selectedDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageMainDisplay(int? productID, int? selectedMainDisplayID)
        {
            if (productID == null || selectedMainDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _productGalleryRepository.UpdatePreviousMainDisplay(productID);
            _productGalleryRepository.SetNewMainDisplay(selectedMainDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END VIEWING AND MANAGING GALLERY")

        [Authorize(Roles = "1")]
        public ActionResult ShowProduct(DataTableParam param)
        {
            if (ModelState.IsValid)
            {
                var product = param.isNewlyAdded == false ? _productRepository.ViewListingProduct(param.iSortCol_0, param.sSortDir_0) : _productRepository.ViewListingNewlyAddedProduct(param.iSortCol_0, param.sSortDir_0);

                if (!String.IsNullOrEmpty(param.sSearch))
                {
                    product = product.Where(p => p.ProductName.ToLower().Contains(param.sSearch) ||
                                                 p.CategoryName.ToLower().Contains(param.sSearch)).ToList();
                }

                //pagination
                var displayResult = product.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
   
                return Json(new
                {
                    aaData = displayResult,
                    param.sEcho,
                    iTotalRecords = product.Count(),
                    iTotalDisplayRecords = product.Count()
                }, JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }
    }
}