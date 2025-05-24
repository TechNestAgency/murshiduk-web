using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Controllers.API.ApiViewModel
{
	public class FarmVisitViewModel
	{
		public long id { get; set; }
		public string FarmName { get; set; }
		public string FarmerName { get; set; }
		public string Description { get; set; }
		public string PurposeOfVisit { get; set; }
		public string Location { get; set; }
		public string Region { get; set; }
		public string City { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public System.DateTime OrderDate { get; set; }
		public System.DateTime VisitDate { get; set; }
		public List<Media> Medias { get; set; }
		public List<Report>	Report { get; set; }

	}

	public class Media 
	{
		public string FileName { get; set; }
		public string FileCategory { get; set; }
		public string Path { get; set; }
	}
	
	public class Report
	{
		public string Description { get; set; }
		public string FileName { get; set; }

		public string Path { get; set; }
	}
	public class OrderStatus
	{
		public int stat { get; set; }
		public string description { get; set; }
	}

}