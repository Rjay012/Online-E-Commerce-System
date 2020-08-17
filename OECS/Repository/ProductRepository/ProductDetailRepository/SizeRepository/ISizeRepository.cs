using System.Collections.Generic;
using System.Linq;
using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.SizeModels;

namespace OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository
{
    public interface ISizeRepository
    {
        List<Size> SizeList();
        IQueryable<SizeModel> DisplayProductSize(int productID, int colorID, int iconID);
    }
}
