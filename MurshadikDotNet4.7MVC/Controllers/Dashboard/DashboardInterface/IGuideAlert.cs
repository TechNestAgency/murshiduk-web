using MurshadikCP.Controllers.Dashboard.DashboardViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.Dashboard.DashboardInterface
{
	interface IGuideAlert
	{
		IPagedList<GuideAlertViewModel> GetGuideAlert(int pagesize, int No_Of_Page);

	}
}
