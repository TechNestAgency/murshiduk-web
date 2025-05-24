using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ProductRepository : GenericRepository<product>, IProductRepository
    {
        public product GetProductByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }
    }
}