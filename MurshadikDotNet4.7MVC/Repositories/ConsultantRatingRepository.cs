using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class ConsultantRatingRepository : GenericRepository<consultant_rating>, IConsultantRatingRepository
    {
        public Data AddConsultantRating(int consultant_id, string comment, decimal rating, user user)
        {
            Data d = new Data();
            // check first if userid, consultant_id is exists then update else insert
            consultant_rating cr = _db.consultant_rating.Where(x => x.user_id == user.id && x.consultant_id == consultant_id).FirstOrDefault();
            if (cr != null)
            {
                cr.comment = comment;
                cr.rating = rating;
                cr.created_at = DateTime.Now;
                Update(cr);
            }
            else
            {
                cr = new consultant_rating();
                cr.comment = comment;
                cr.rating = rating;
                cr.user_id = user.id;
                cr.consultant_id = consultant_id;
                cr.created_at = DateTime.Now;
                Insert(cr);
            }

            Save();

            Generic g = new Generic();
            
            user consultant = _db.users.Find(consultant_id);
            g.sendNotificationOnPriceUpdate("تقييم", "تم تقييمك بواسطة  : " + user.name, consultant.phone);

            g.InsertNotificationData(consultant_id, "تم تقييمك بواسطة  : " + g.GetFullName(user.id), (Int16)AppConstants.Type.Rating, cr.id, "");

            d.status = true;
            d.message = "successful";
            d.data = null;
            return d;
        }
    }
}