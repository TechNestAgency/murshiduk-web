using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using MurshadikCP.Models.DB;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MurshadikCP.Controllers
{
    public class SummaryClass
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index(string start_date = null, string end_date = null)
        {
            start_date = start_date == string.Empty ? null : start_date;
            end_date = end_date == string.Empty ? null : end_date;

            DateTime sdt = DateTime.Now.AddYears(-2); DateTime edt = DateTime.Now;
            TimeSpan daysToSub = new System.TimeSpan(start_date == null ? 6 : 0, 0, 0, 0);
            var startDate = DateTime.Today.Subtract(daysToSub).StartOfDay();
            var endDate = DateTime.Today.EndOfDay();

            if (!string.IsNullOrEmpty(start_date))
            {
                ViewBag.Start_Date = start_date;
                sdt = DateTime.Parse(start_date, null, System.Globalization.DateTimeStyles.RoundtripKind).StartOfDay();
                startDate = sdt;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                ViewBag.End_Date = end_date;
                edt = DateTime.Parse(end_date, null, System.Globalization.DateTimeStyles.RoundtripKind).EndOfDay();
                endDate = edt;
            }

            int farmersCount = db.users.Where(x => x.role_id == 5 && x.created_at >= startDate && x.created_at <= endDate).Count();
            int consultantCount = db.users.Where(x => x.role_id == 6 && x.created_at >= startDate && x.created_at <= endDate).Count();
            int message = db.chatmessages.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            int calls = db.calldetails.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            int questions = db.qa_questions.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            int answers = db.qa_answers.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            var market = db.GeneralReport(startDate, endDate).ToList();

            var markets = db.markets.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            var products = db.products.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            var labs = db.labs.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            var bookedappointments = db.appointments.Where(x => x.is_booked == true && x.created_at >= startDate && x.created_at <= endDate).Count();
            var reports = db.lab_reports.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();
            var news = db.articles.Where(x => x.created_at >= startDate && x.created_at <= endDate).Count();


            ViewBag.farmersCount = farmersCount;
            ViewBag.consultantCount = consultantCount;
            ViewBag.message = message;
            ViewBag.calls = calls;
            ViewBag.questions = questions;
            ViewBag.answers = answers;
            ViewBag.market = market;

            ViewBag.MarketsCount = markets;
            ViewBag.ProductsCount = products;
            ViewBag.LabCount = labs;
            ViewBag.bookedappointmentsCount = bookedappointments;
            ViewBag.TotalReports = reports;
            ViewBag.NewsCount = news;

            ViewBag.Region = new SelectList(db.regions, "id", "name_ar");
            ViewBag.skill = new SelectList(db.skills, "name", "name");

            if (TempData["message"] != null)
                ViewBag.Notificationmessage = 1;

            return View();
        }

        public DataTable GenerateExcelFileForConsultantByFilters(DateTime startDate, DateTime endDate, long[] Region, string[] skill, int Status, int approved, int apptype)
        {
			DataTable dt = new DataTable();
			IQueryable<reportUser> users = (IQueryable<reportUser>)(from user in db.users
						where user.role_id == (int)Role.Consultants && user.created_at >= startDate && user.created_at <= endDate
						orderby user.id descending
						select new reportUser
						{
							qaCount = user.qa_answers.Count(),
							region_id = user.region_id,
							name = user.name,
							last_name = user.last_name,
							phone = user.phone,
							region = user.region,
							active = user.active,
							is_approved = user.is_approved,
							app_type = user.app_type,
							created_at = user.created_at,
							consultant_rating = user.consultant_rating,
							callNo = db.calldetails.Where(x => x.user_to == user.id && (x.status == 3 || x.status == 5 || x.status == 6)).Count(),
							msgNo = db.chatmessages.Where(x => x.message_from == user.id).Count(),
                            skills = user.skills
                            

						});

			//var a = users.consultant_rating.Count() > 0 ? String.Format("{0:0.00}", users.consultant_rating.Average(x => x.rating)) : "0.0";

			if (Region!=null && Region[0]!=0)
                users = users.Where(x => Region.Any(r=> r == x.region_id));

            if (skill!=null && skill[0]!="")
               

                users = users.Where(x => skill.Any(y => y == x.skills));
                //users = users.Where(x => x.skill_user.Where(y => y.is_approved == true && y.user_id == x.id && y.skill_id == skill).Any());

			if (Status == 1)
                users = users.Where(x => x.active == true);
            else if (Status == 2)
                users = users.Where(x => x.active == false);

            if (approved == 1)
                users = users.Where(x => x.is_approved == true);
            else if (approved == 2)
                users = users.Where(x => x.is_approved == false);

            if (apptype > 0)
                users = users.Where(x => x.app_type == apptype);

            List<reportUser> u = users.ToList();

            if (u.Count() > 0)
            {

                dt.Columns.Add("Name");
                dt.Columns.Add("Phone");
                dt.Columns.Add("Skills");
                dt.Columns.Add("Region");
                dt.Columns.Add("Status");
                dt.Columns.Add("Approved");
                dt.Columns.Add("AppType");
                dt.Columns.Add("answersCount");
                dt.Columns.Add("JoiningDate");
                dt.Columns.Add("msgNo");
                dt.Columns.Add("callNo");
                dt.Columns.Add("RateVote");
                dt.Columns.Add("RateCount");
                foreach (var item in u)
                {
                    DataRow dr = dt.NewRow();
                    dr["Name"] = item.name + ' ' + item.last_name;
                    dr["Phone"] = item.phone;
                    dr["Skills"] = item.skills;
                    dr["Region"] = item.region;
                    dr["Status"] = item.active == true ? "مُفعل" : "غير مُفعل";
                    dr["Approved"] = item.is_approved == true ? "وافق" : "إنتظار الموافقة";
                    dr["AppType"] = item.app_type == 1 ? "آي او اس" : "أندرويد";
                    dr["answersCount"] = item.qaCount;
                    dr["JoiningDate"] = item.created_at.ToString("d");
                    dr["msgNo"] = item.msgNo;
                    dr["callNo"] = item.callNo;
                    dr["RateVote"] = item.consultant_rating.Count > 0 ? String.Format("{0:0.00}", item.consultant_rating.Average(x => x.rating)) : "0.0";
                    dr["RateCount"] = item.consultant_rating.Count();

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public DataTable GenerateExcelFileForLab(DateTime startDate, DateTime endDate, long[] Region)
        {
            // Below code is create datatable and add one row into datatable.  
            DataTable dt = new DataTable();
            var result = db.GETLabREPORT(startDate, endDate).ToList();
            if (Region!=null &&Region[0] != 0)
                result = result.Where(x => Region.Any(r => r == x.region_id)).ToList();

            if (result != null)
            {
                dt.Columns.Add("Name");
                dt.Columns.Add("no_of_Samples");
                dt.Columns.Add("no_of_Samples_collected_sent");
                dt.Columns.Add("no_of_reserved_appointment_sample_didnt_recieve");
                foreach (var item in result)
                {
                    DataRow dr = dt.NewRow();
                    dr["Name"] = item.Name;
                    dr["no_of_Samples"] = item.no_of_Samples;
                    dr["no_of_Samples_collected_sent"] = item.no_of_Samples_collected_sent;
                    dr["no_of_reserved_appointment_sample_didnt_recieve"] = item.no_of_reserved_appointment_sample_didnt_recieve;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public DataTable GenerateExcelFileForMarket(DateTime startDate, DateTime endDate, long[] Region, int Status)
        {
            // Below code is create datatable and add one row into datatable.  
            DataTable dt = new DataTable();


          

			
            var  result =(from mb in db.market_products
                        join m in db.markets on mb.market_id equals m.id
                        join re in db.regions on m.region_id equals re.id
                        where mb.created_at >= startDate && mb.created_at <= endDate
                                             
                        select new reportMarket()
                        {
                            Region = re.name_ar ,                
                            Market = m.marketname,
                            is_active =  m.is_active,
                            region_id = m.region_id,  
                            number_of_updated_products = m.market_products.Where(w=>w.created_at >=   startDate && w.created_at <= endDate).Count(),
                            productCount   = m.market_products.Where(w => w.created_at >= startDate && w.created_at <= endDate).Select(s => s.product_id).Distinct().Count(),
                        }).Distinct().OrderBy(o=>o.Market).ToList();



            //var result = db.GETMARKETREPORT(startDate, endDate).OrderBy(o=>o.Market).ToList();


           
                     

            if (Region!=null&& Region[0] != 0)
                result = result.Where(x => Region.Any(r => r == x.region_id)).ToList();

            if (Status > 0)
                result = result.Where(x => x.is_active == Convert.ToBoolean(Status)).ToList();

            if (result != null)
            {
                dt.Columns.Add("number_of_updated_products");
                dt.Columns.Add("number_of_products");
                dt.Columns.Add("Market");
                dt.Columns.Add("Region");
                foreach (var item in result)
                {
                    DataRow dr = dt.NewRow();
                    dr["number_of_updated_products"] = item.number_of_updated_products;
                    dr["number_of_products"] = item.productCount;
                    dr["Market"] = item.Market;
                    dr["Region"] = item.Region;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        [HttpPost]
        public ActionResult GenerateExcel(string modelData, DateTime startDate, DateTime endDate,long[] Region, int Status, string Type, string[] Skill, int Approved, int AppType)
        {
            IWorkbook workbook = new XSSFWorkbook();

            DataTable dataTable = new DataTable();
            if (Type == "market")
                dataTable = GenerateExcelFileForMarket(startDate, endDate, Region, Status);
            else if (Type == "lab")
                dataTable = GenerateExcelFileForLab(startDate, endDate, Region);
            else if (Type == "consultant")
                dataTable = GenerateExcelFileForConsultantByFilters(startDate, endDate, Region, Skill, Status, Approved, AppType);

            string tempfileName = "";

            if (dataTable.Rows.Count > 0)
            {
                ISheet excelSheet = workbook.CreateSheet(Type);
                List<string> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (System.Data.DataColumn column in dataTable.Columns)
                {
                    columns.Add(column.ColumnName);
                    row.CreateCell(columnIndex).SetCellValue(column.ColumnName);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (DataRow dsrow in dataTable.Rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (String col in columns)
                    {
                        row.CreateCell(cellIndex).SetCellValue(dsrow[col].ToString());
                        cellIndex++;
                    }

                    rowIndex++;
                }

                string temppath = Server.MapPath("~/TempFile");
                if (!Directory.Exists(temppath))
                {
                    Directory.CreateDirectory(temppath);
                }
                tempfileName = Type + ".xlsx";

                using (FileStream stream = new FileStream(Path.Combine(temppath, tempfileName), FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(stream);
                }
            }

            return Json(tempfileName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult download(string tempfileName)
        {
            string temppath = Server.MapPath("~/TempFile");
            string filePath = Path.Combine(temppath, tempfileName);
            var downloadfile = File(filePath, "application/vnd.ms-excel", tempfileName);
            return downloadfile;
        }
    }

    public  class reportUser:user
	{
      
        public int qaCount { get; set; }
        public int callNo { get; set; }
        public int msgNo { get; set; }  
        public string Skills { get; set; }
	}
    public  class reportMarket
	{
        public Nullable<int> number_of_updated_products { get; set; }
        public string Market { get; set; }
        public string Region { get; set; }
        public Nullable<long> region_id { get; set; }
        public bool is_active { get; set; }
        public int productCount { get; set; }
       

	} 
}