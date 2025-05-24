using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
    public interface IVideoLikeService
    {
        object AddLike(Guid videoId, long userId, int Status);
        object GetVideoLike(Guid videoId, long userId);
    }
}