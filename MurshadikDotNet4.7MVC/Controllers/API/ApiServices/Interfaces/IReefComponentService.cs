using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
    public interface IReefComponentService
    {
        object GetReefComponentList();

        object GetReefComponentSuggestionsById(int id);


    }
}
