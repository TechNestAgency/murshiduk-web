using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IProductCategoryRepository : IRepository<product_categories>
    {
        product_categories GetProductCategoryByID(long id);
    }
}