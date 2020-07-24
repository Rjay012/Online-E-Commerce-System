using OECS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Repository.BrandRepository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly oecsEntities _dbContext;
        public BrandRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Brand> ListBrands()
        {
            return _dbContext.Brand.ToList();
        }
    }
}