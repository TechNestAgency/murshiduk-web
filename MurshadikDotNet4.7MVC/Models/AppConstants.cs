using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class AppConstants
    {
        public enum Role
        {
            Super_Admin = 1,
            Manager = 2,
            Employee = 3,
            Lab_Manager = 4,
            Farmers = 5,
            Consultants = 6,
            Technician = 7,
            Market = 8
        }

        public enum UserStatus
        {
            online = 1,
            offline = 0
        }
        public enum NotificationType
		{
            guide_Alert=1,
            //miss_call=2,


        }

        public enum Type
        {
            Weather = 1,
            Market = 2,
            Lab = 3,
            Account = 4,
            Question = 5,
            Answer = 6,
            Chat = 7,
            Rating = 8,
            GuidedGuide = 9,
            Clinic = 10,
            Consultation = 11,
        }

        public enum Days
        {
            الأحد = 0,
            الاثنين = 1,
            الثلاثاء = 2,
            الأربعاء = 3,
            الخميس = 4,
            الجمعة = 5,
            السبت = 6
        }
        
    }
}