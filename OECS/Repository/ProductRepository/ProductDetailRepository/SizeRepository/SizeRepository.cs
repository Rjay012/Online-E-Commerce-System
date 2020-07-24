using OECS.Models;
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
    }
}