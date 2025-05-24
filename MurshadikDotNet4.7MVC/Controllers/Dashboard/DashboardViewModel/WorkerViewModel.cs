using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MurshadikCP.Controllers.Dashboard.DashboardViewModel
{
	public class WorkerViewModel
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public bool IsBusy { get; set; }
		public string Rejon  { get; set; }
		public string Job  { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}