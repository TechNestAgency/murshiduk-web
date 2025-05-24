using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.Dashboard.DashboardViewModel
{
	public class GuideAlertViewModel
	{
		public string GuideName { get; set; }	
		public string GuidePhonNumber { get; set;}
		public string Alert { get; set; }	
		public string Cities { get; set; }	
		public string Regon { get; set; }	
		public string Skills { get; set; }	
		public int? RecipientsCount { get; set; }
		public DateTime Created_At { get; set; }
	
	
	}
}