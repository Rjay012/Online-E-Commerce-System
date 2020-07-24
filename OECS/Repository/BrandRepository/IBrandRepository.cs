using OECS.Models;
using System.Collections.Generic;

namespace OECS.Repository.BrandRepository
{
    public interface IBrandRepository
    {
        List<Brand> ListBrands();
    }
}
