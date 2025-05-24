using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public interface IProductRepository : IRepository<product>
    {
        product GetProductByID(long id);
    }
}