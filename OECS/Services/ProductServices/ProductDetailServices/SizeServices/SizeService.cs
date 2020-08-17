using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.SizeModels;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices.SizeServices
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductDetailRepository _productDetailRepository;

        public SizeService(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
        }

        public List<SizeModel> ViewListingSize(int categoryID, int subCategoryID)
        {
            IQueryable<ProductDetail> productDetail = _productDetailRepository.ProductDetail();
            return productDetail.Select(s => new SizeModel
                                {
                                    SideID = (int)s.SizeID,
                                    Size = s.Size.size1,
                                    CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                    SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                }).Distinct()
                                  .ToList();
        }

        public IEnumerable<SelectListItem> SizeList()
        {
            List<Size> sizes = _sizeRepository.SizeList();
            List<SelectListItem> sizeList = new List<SelectListItem>();

            foreach (var item in sizes)
            {
                sizeList.Add(new SelectListItem
                {
                    Value = item.SideID.ToString(),
                    Text = item.size1
                });
            }
            return sizeList;
        }

        public IEnumerable<SelectListItem> SizeList(int productID, int colorID, int iconID)
        {
            IQueryable<SizeModel> size = _sizeRepository.DisplayProductSize(productID, colorID, iconID);
            List<SelectListItem> sizeList = new List<SelectListItem>();

            foreach (var item in size.Distinct())
            {
                int sizeCount = size.Where(s => s.SideID == item.SideID).Count();
                string sizeAvailability = sizeCount > 1 ? " --- " + sizeCount + " sizes are available" : "";

                sizeList.Add(new SelectListItem
                {
                    Value = item.SideID.ToString(),
                    Text = item.Size + sizeAvailability
                });
            }
            return sizeList;
        }
    }
}