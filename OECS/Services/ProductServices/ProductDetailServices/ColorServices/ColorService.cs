using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.ColorModels;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.ColorRepository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices.ColorServices
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IProductDetailRepository _productDetailRepository;

        public ColorService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
        }

        public List<ColorModel> ViewListingColor(int categoryID, int subCategoryID)
        {
            IQueryable<ProductDetail> productDetail = _productDetailRepository.ProductDetail();
            return productDetail.Select(s => new ColorModel
                                {
                                    ColorID = (int)s.ColorID,
                                    Color = s.Color.color1,
                                    CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                    SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                }).Distinct()
                                  .ToList();
        }

        public IEnumerable<SelectListItem> ListingColor()
        {
            List<SelectListItem> ColorListTempStorage = new List<SelectListItem>();
            List<Color> colors = _colorRepository.ListColor();

            foreach (var item in colors)
            {
                ColorListTempStorage.Add(new SelectListItem
                {
                    Value = item.ColorID.ToString(),
                    Text = item.color1
                });
            }

            return ColorListTempStorage;
        }
    }
}