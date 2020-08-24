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
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Repository.ProductRepository.ProductGalleryRepository;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.ProductRepository;
using OECS.Services.ProductServices;
using OECS.Services.ProductServices.ProductDetailServices;
using OECS.Services.ProductServices.ProductGalleryServices;
using OECS.Services.ProductServices.ProductDetailServices.SizeServices;
using OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository;

namespace OECS.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductDetailService _productDetailService;
        private readonly IProductGalleryService _productGalleryService;
        private readonly ISizeService _sizeService;

        public ProductController()
        {
            _productService = new ProductService(new ProductRepository(new oecsEntities()));
            _productDetailService = new ProductDetailService(new ProductDetailRepository(new oecsEntities()));
            _productGalleryService = new ProductGalleryService(new ProductGalleryRepository(new oecsEntities()));
            _sizeService = new SizeService(new SizeRepository(new oecsEntities()));
        }

        // GET: Product
        [Authorize(Roles = "1,2,3")]
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Product";
            return View(nameof(Index));
        }

        public ActionResult ViewFullDetail(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(nameof(ViewFullDetail));
        }

        public ActionResult PreviewProductDetails(int? productID, int? colorID, int? iconID)
        {
            if(productID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductDetailModel productDetailModel = new ProductDetailModel();
            if (colorID == null && iconID == null)
            {
                productDetailModel = _productGalleryService.GetColorAndIcon((int)productID).SingleOrDefault();

                if(productDetailModel == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                productDetailModel.ColorID = (int)colorID;
                productDetailModel.IconID = (int)iconID;
            }

            ViewProductDetailModel viewProductDetailModel = _productGalleryService.ViewListingProductImage((int)productID, productDetailModel.ColorID, productDetailModel.IconID);
            ViewBag.SizeList = _sizeService.SizeList((int)productID, productDetailModel.ColorID, productDetailModel.IconID);

            if (viewProductDetailModel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Partials/_PreviewProductDetails", viewProductDetailModel);
        }

        public ActionResult ViewTab()
        {
            return PartialView("Partials/Tabs/_TabList");
        }

        [AllowAnonymous]
        public ActionResult Show(int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID, string searchString)
        {
            List<ViewProductModel> product = _productService.ShowProductList(1);
            if (colorID != 0) product = _productService.ShowProductList(2);
            if (sizeID != 0) product = _productService.ShowProductList(3);

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Product.SubCategory.subCategory1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            product = _productService.OnFilter(product, categoryID, subCategoryID, brandID, colorID, sizeID);

            if (colorID != 0 || sizeID != 0)
            {
                return PartialView("Partials/_ProductList", product);
            }
           
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
            return PartialView("Partials/Modals/_NewProduct", _productService.AssignProductDetail(productModel));
        }

        public ActionResult EditProductModalForm(int? productID)
        {
            if(productID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = _productService.FindByID((int)productID);

            if(product == null)
            {
                return HttpNotFound();
            }

            return PartialView("Partials/Modals/_EditProduct", _productService.AssignProduct(product));
        }

        public ActionResult NewProductDetailModalForm(ProductDetailModel productDetailModel)
        {
            return PartialView("Partials/Modals/_NewProductDetail", _productService.AssignProductDetail(productDetailModel));
        }

        public ActionResult EditProductDetailModalForm(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null && iconID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return PartialView("Partials/Modals/_EditProductDetail", _productDetailService.GetProductDetail((int)productID, (int)colorID, (int)iconID));
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
                if(_productService.CreateProduct(productModel) == true)
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
                _productDetailService.CreateProductDetail(productDetailModel);
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
                _productDetailService.EditProductDetail(productDetailModel);
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

            ViewProductDetailModel productDetailModel = _productGalleryService.ViewListingProductImage((int)productID, (int)colorID, (int)iconID);

            if(productDetailModel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Partials/Modals/_ProductImageGallery", productDetailModel);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageDisplay(int? defaultDisplayID, int? selectedDisplayID)
        {
            if (defaultDisplayID == null || selectedDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _productGalleryService.SetImageDisplay((int)defaultDisplayID, (int)selectedDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageMainDisplay(int? productID, int? selectedMainDisplayID)
        {
            if (productID == null || selectedMainDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _productGalleryService.UpdatePreviousMainDisplay((int)productID);
            _productGalleryService.SetNewMainDisplay((int)selectedMainDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END VIEWING AND MANAGING GALLERY")

        [Authorize(Roles = "1")]
        public ActionResult ShowProduct(DataTableParam param)
        {
            if (ModelState.IsValid)
            {
                var product = param.isNewlyAdded == false ? _productService.ViewListingProduct(param.iSortCol_0, param.sSortDir_0) : _productService.ViewListingNewlyAddedProduct(param.iSortCol_0, param.sSortDir_0);

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