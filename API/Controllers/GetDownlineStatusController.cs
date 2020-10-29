using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetDownlineStatusController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        [HttpGet]
        [Route("api/GetDownlineStatus/DownlinestatusUser")]

        public object DownlinestatusUser(int UserLoginId)
        {
            object obj = new object();

            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();
            try
            {
                //long userID = Convert.ToInt64(Session["ID"]);
                //objLeaderDashboard.Distribute_Designation(Convert.ToInt32(userID));

                Param.LevelId = 0;
                // Param.LevelList = new SelectList(new[]
                //{
                //      new { ID = "0", Name = "All Levels" },
                //      new { ID = "1", Name = "Level: 1" },
                //      new { ID = "2", Name = "Level: 2" },
                //      new { ID = "3", Name = "Level: 3" },
                //      new { ID = "4", Name = "Level: 4" },
                //      new { ID = "5", Name = "Level: 5" },
                //      new { ID = "6", Name = "Level: 6" }
                // },
                // "ID", "Name", 1);
                Param.InActive = true;
                Param.Active = true;
                Param.Name = true;
                Param.EMail = true;
                Param.ParaenttName = true;
                Param.RefId = true;
                Param.JoinDate = true;
                Param.pageNo = 1;

                List<Downline> list = GetDataDownline(UserLoginId);
                foreach (var item in list)
                Param.listDownline = list;
                LeadersMobileDisplay_Downline displayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();
                obj = new { Success = 1, Message = "Successfull", data = Param };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        public List<Downline> GetDataDownline(int UserLoginId)
        {
            List<Downline> list = new List<Downline>();
            long LoginUserId = 0;

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = UserLoginId
            };

            list = db.Database.SqlQuery<Downline>("EXEC spmlmdownlinestatus @UserID", idParam).ToList<Downline>();
            return list;
        }

    }
}
