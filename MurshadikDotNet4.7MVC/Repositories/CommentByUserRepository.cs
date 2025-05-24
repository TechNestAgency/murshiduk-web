using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class CommentByUserRepository : GenericRepository<commentsByUser>, ICommentByUserRepository
    {
        public void InsertCommentByUser(long consultantid, float rating, string message, user u)
        {
            commentsByUser cbu = new commentsByUser();
            cbu.consultantId = consultantid;
            cbu.created_at = DateTime.Now;
            cbu.farmerId = u.id;
            cbu.is_active = true;
            cbu.message = message;
            cbu.Rating = rating;
            cbu.updated_at = DateTime.Now;
            Insert(cbu);
            Save();

            Generic g = new Generic();
            
            g.sendNotificationOnPriceUpdate("تقييم", "تم تقييمك من قبل :" + u.name, u.phone);

            g.InsertNotificationData(consultantid, "تم تقييمك من قبل :" + u.name + " " + u.last_name, (Int16)AppConstants.Type.Rating, cbu.id, "");
        }
    }
}