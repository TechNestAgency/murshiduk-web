using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ProductCategoryRepository : GenericRepository<product_categories>, IProductCategoryRepository
    {
        public product_categories GetProductCategoryByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}