using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Extensions
{
    public static class MyExtensions
    {
        public static string FormattedDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

        public static string FormattedTime(this DateTime dt)
        {
            return dt.ToString("hh:mm tt");
        }

        public static string CheckUser(this string Username)
        {
            string username = "";
            if (Username.Length == 12)
            {
                if (Username.Substring(0, 3) == "966")
                {
                    return username = Username;
                }
            }
            else
            {
                if (Username.Length == 10)
                {
                    if (Username.Substring(0, 2) == "05")
                    {
                        return username = "966" + Username.Substring(1, 9);
                    }
                }
                else if (Username.Length == 9)
                {
                    if (Username.Substring(0, 1) == "5")
                    {
                        return username = "966" + Username;
                    }
                }
            }

            return username;
        }
    }
}