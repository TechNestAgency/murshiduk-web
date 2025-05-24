using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiServices.Interfaces
{
   public interface IVideoService
    {
        object GetVideo(Guid VideoId);
        object GetVideoByCategoryId(long CategoryId);
        object PostVideo(Video Video,HttpPostedFile file);
        object PutVideo(Guid VideoId,string Title, string Description,string Tage,long DepartmentId, long CategoryId, bool IsHidden);
        object GetGallery(long UserId);
    }
}
