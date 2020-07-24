using OECS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.ProductRepository.ProductDetailRepository.ColorRepository
{
    public class ColorRepository : IColorRepository
    {
        private readonly oecsEntities _dbContext;
        public ColorRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Color> ListColor()
        {
            return _dbContext.Color.ToList();
        }
    }
}