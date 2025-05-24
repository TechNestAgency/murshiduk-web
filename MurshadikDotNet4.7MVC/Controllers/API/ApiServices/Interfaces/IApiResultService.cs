using MurshadikCP.Controllers.API.ApiViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
    public interface IApiResultService
    {
        Result Result(Boolean status,object data, string message,object Info);
    }
}
