using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurshadikCP.Controllers.Dashboard.DashboardInterface
{
	public interface IFarmVisitOrderService
	{
		 object GetFarmOrders(long UserRegionId,long UserRoleId);
		 object UpdateFarmOrder(FarmVisitOrder farmVisitOrder);
		object GetFarmOrder(long OrderId);
		object AddReport(long OrderId);

	}
}
