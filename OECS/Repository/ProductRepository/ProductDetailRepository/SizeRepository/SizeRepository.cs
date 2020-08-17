using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.SizeModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository
{
    public class SizeRepository : ISizeRepository
    {
        private readonly oecsEntities _dbContext;
        public SizeRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Size> SizeList()
        {
            return _dbContext.Size.ToList();
        }

        public IQueryable<SizeModel> DisplayProductSize(int productID, int colorID, int iconID)
        {
            return _dbContext.DisplaySize
                             .Where(ds => ds.ProductImage.ProductDetail.ProductID == productID && ds.ProductImage.ProductDetail.ColorID == colorID && ds.ProductImage.Image.IconID == iconID)
                             .Select(s => new SizeModel
                             {
                                 SideID = (int)s.ProductImage.ProductDetail.SizeID,
                                 Size = s.ProductImage.ProductDetail.Size.size1
                             });
        }
    }
}