using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ProductTypeRepository : GenericRepository<product_type>, IProductTypeRepository
    {
        public product_type GetProductTypeByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}