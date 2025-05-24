using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.API.ApiServices
{
   public interface IVideoCommentService
    {

         Object AddComment(Guid VideoId,long UserId ,string Comment);
         Object AddCommentReply(Guid VideoId, long UserId ,string Comment,long CommentId);
         Object GetVideoComment(Guid VideoId);
        
    }
}
