using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Data;
using System.Configuration;
using System.Collections;
using MurshadikCP.Models.DB;

namespace MurshadikCP.Models
{
    public class SendSMS
    {
        private mlaraEntities db = new mlaraEntities();
        public void SMSSend(string mobileno, string msg)
        {
            WebClient client = new WebClient();
            List<app_settings> listSetting = db.app_settings.ToList();
            string baseURL = listSetting.Where(x => x.ap_key == "AP_SMSGatewayURL").FirstOrDefault().ap_value;
            string ID = "";
            string Pwd = "";
            string cCode = "966";
            string SenderID = listSetting.Where(x => x.ap_key == "AP_SMSGatewaySenderID").FirstOrDefault().ap_value;
            if (mobileno.isKSANumber())
            {
                ID = listSetting.Where(x => x.ap_key == "AP_SMSSaudiID").FirstOrDefault().ap_value;
                Pwd = listSetting.Where(x => x.ap_key == "AP_SMSSaudiPwd").FirstOrDefault().ap_value;
            }
            else
            {
                ID = listSetting.Where(x => x.ap_key == "AP_SMSWorldID").FirstOrDefault().ap_value;
                Pwd = listSetting.Where(x => x.ap_key == "AP_SMSWorldPwd").FirstOrDefault().ap_value;
                cCode = "All";
            }

            baseURL = baseURL + "?user=" + ID + "&pwd=" + Pwd + "&senderid=" + SenderID + "&CountryCode=" + cCode + "&mobileno=" + mobileno + "&msgtext=" + msg;

            Stream data = client.OpenRead(baseURL);
            StreamReader reader = new StreamReader(data);
            reader.ReadToEnd();
            data.Close();
            reader.Close();
        }
    }
}