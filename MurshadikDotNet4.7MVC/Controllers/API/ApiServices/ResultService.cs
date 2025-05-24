using MurshadikCP.Controllers.API.ApiServices.Interfaces;
using MurshadikCP.Controllers.API.ApiViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices
{
    public class ResultService : IApiResultService
    {
        public Result Result(Boolean status, object data, string message, object Info)
        {
            Result result;
            return result = new Result
            {
                status = status,
                data = data,
                message = message,
                info =Info
            };
            
        }
    }
}