using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Controllers
{
    public class GetLeaderDashboardController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (UserLoginId == null || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Please provoid valid data", data = string.Empty };
                }

                bool result = db.MLMUsers.Where(x => x.UserID == UserLoginId).Any();
                if (!result)
                    return obj = new { Success = 1, Message = "This is not MLM User.", data = string.Empty };
                InviteUser objInviteUser = new InviteUser();
                objInviteUser.Message = "Hi!, i would like to invite you to this awesome scheme launched by ezeelo.com check the link in email for more info";

                HighPerformer objHighPerformer = new HighPerformer();
                objHighPerformer.searchId = 1;
                objHighPerformer.SearchParameter = new SelectList(new[]
                {
                new { ID = "1", Name = "Top Recruiters" },
                new { ID = "2", Name = "Top Buyers" },
                new { ID = "3", Name = "Top Earners" },
                },
                "ID", "Name", 1);

                RefferalByFilter objRefferalByFilter = new RefferalByFilter();
                objRefferalByFilter.searchId = 1;
                objRefferalByFilter.SearchParameter = new SelectList(new[]
                {
                new { ID = "1", Name = "This Month" },
                new { ID = "2", Name = "Last 5 Month" }
                },
                "ID", "Name", 1);

                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = UserLoginId
                };

                LeadersDashboard objLeaderDashboard = new LeadersDashboard();

                DashboardViewModel objDashboardViewModel = new DashboardViewModel();
                RefferalByFilter RefferalByFilter_ThisMonth = new RefferalByFilter();
                RefferalByFilter_ThisMonth.searchId = 1;
                //  objDashboardViewModel.Referrals = objLeaderDashboard.GetRefferalCountByFilter(RefferalByFilter_ThisMonth, UserLoginId);
                RefferalByFilter RefferalByFilter_5Month = new RefferalByFilter();
                RefferalByFilter_5Month.searchId = 2;
                //objDashboardViewModel.Referrals_5Month = objLeaderDashboard.GetRefferalCountByFilter(RefferalByFilter_5Month, UserLoginId);
                //objDashboardViewModel.TOTAL_MEMBERS = objLeaderDashboard.GetTotalMemberCount(UserLoginId);
                //objDashboardViewModel.QUALIFYING_RETAIL_POINTS = objLeaderDashboard.GetQRP(UserLoginId);
                //objDashboardViewModel.Withdrawn = objLeaderDashboard.GetWithdrawnAmount(UserLoginId);
                //objDashboardViewModel.ERP = objLeaderDashboard.getERP(UserLoginId);
                //objDashboardViewModel.RP_ON_MY_PURCHASE = objLeaderDashboard.GetRpOnPurchase(UserLoginId);
                //objDashboardViewModel.INACTIVE_MEMBERS = objLeaderDashboard.GetInactiveMemberCount(UserLoginId);
                //objDashboardViewModel.Payout_Requested = objLeaderDashboard.GetPayout_Requested(UserLoginId);
                //objDashboardViewModel.EZEE_MONEY = objLeaderDashboard.GetEzzeMoney(UserLoginId);
                //added by roshan
                //objLeaderDashboard.Distribute_Designation(UserLoginId);
                //objDashboardViewModel.UserDesignation = objLeaderDashboard.UserDesignation(Convert.ToInt32(UserLoginId));
                //Ended by roshan

                // changes by lokesh
                DashboardViewModel ObjDb = new DashboardViewModel();
                ObjDb = objLeaderDashboard.GetallDashboarddatabysp(UserLoginId);
                objDashboardViewModel.Referrals = ObjDb.Referrals;
                objDashboardViewModel.Referrals_5Month = ObjDb.Referrals_5Month;
                objDashboardViewModel.TOTAL_MEMBERS = ObjDb.TOTAL_MEMBERS;
                objDashboardViewModel.QUALIFYING_RETAIL_POINTS = ObjDb.QUALIFYING_RETAIL_POINTS;
                objDashboardViewModel.Withdrawn = ObjDb.Withdrawn;
                objDashboardViewModel.ERP = ObjDb.ERP;
                objDashboardViewModel.RP_ON_MY_PURCHASE = ObjDb.RP_ON_MY_PURCHASE;
                objDashboardViewModel.INACTIVE_MEMBERS = ObjDb.INACTIVE_MEMBERS;
                objDashboardViewModel.Payout_Requested = ObjDb.Payout_Requested;
                objDashboardViewModel.EZEE_MONEY = ObjDb.EZEE_MONEY;
                objDashboardViewModel.UserDesignation = ObjDb.UserDesignation;
                objDashboardViewModel.CasbackPoints = ObjDb.CasbackPoints;
                objDashboardViewModel.CasbackEzeeMoney = ObjDb.CasbackEzeeMoney;
                // changes by lokesh
                // objLeaderDashboard.Distribute_Designation(UserLoginId);

                objDashboardViewModel.Inactive_Points = objLeaderDashboard.GetInactivePoints(UserLoginId);
                objDashboardViewModel.Pending_EzeeMoney = objLeaderDashboard.GetPending_EzeeMoney(UserLoginId);
                // objDashboardViewModel.TOTAL_MEMBERS = objLeaderDashboard.GetTotalMemberCount(UserLoginId);
                //objDashboardViewModel.QUALIFYING_RETAIL_POINTS = objLeaderDashboard.GetQRP(UserLoginId);
                objDashboardViewModel.DAYS_LEFT = objLeaderDashboard.GetDayLeft();
                objDashboardViewModel.CYCLE_START_DATE = objLeaderDashboard.GetCycleStartDate();
                objDashboardViewModel.CYCLE_LAST_DATE = objLeaderDashboard.GetCycleLastDate();
                objDashboardViewModel.MY_PURCHASES = objLeaderDashboard.GetMyPurchaseCount(UserLoginId);
                objDashboardViewModel.EXPECTED_ERP = objLeaderDashboard.ExpectedERP(UserLoginId);


                objDashboardViewModel.objInviteUser = objInviteUser;
                objDashboardViewModel.listRecentJoinees = objLeaderDashboard.GetRecentJoinees(UserLoginId);
                objDashboardViewModel.isUserActive = objLeaderDashboard.IsUserActive(UserLoginId);





                // objDashboardViewModel.objHighPerformer = objHighPerformer;
                objDashboardViewModel.objRefferalByFilter = objRefferalByFilter;
                objDashboardViewModel.TopRecruiterList = new List<HighPerformerViewModel>();
                //   objDashboardViewModel.TopRecruiterList = db.Database.SqlQuery<HighPerformerViewModel>("EXEC Leaders_TopRecruiters @UserID", idParam).ToList<HighPerformerViewModel>();
                objDashboardViewModel.TopRecruiterList = db.Database.SqlQuery<HighPerformerViewModel>("EXEC GetTopRecruiters @UserID", idParam).ToList<HighPerformerViewModel>();

                var idParam1 = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = UserLoginId
                };

                var idParam2 = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = UserLoginId
                };


                objDashboardViewModel.TopBuyersList = new List<HighPerformerViewModel>();

                // objDashboardViewModel.TopBuyersList = db.Database.SqlQuery<HighPerformerViewModel>("EXEC Leaders_TopBuyers @UserID", idParam1).ToList<HighPerformerViewModel>();

                objDashboardViewModel.TopBuyersList = db.Database.SqlQuery<HighPerformerViewModel>("EXEC GetTopBuyersList @UserID", idParam1).ToList<HighPerformerViewModel>();
                objDashboardViewModel.TopEarnersList = new List<HighPerformerViewModel>();

                objDashboardViewModel.TopEarnersList = db.Database.SqlQuery<HighPerformerViewModel>("EXEC GetTopEarnersList @UserID", idParam2).ToList<HighPerformerViewModel>();

                // comment by lokesh
                // List<NetworkUserViewModel> memberList = new List<NetworkUserViewModel>();
                //memberList = objLeaderDashboard.GetTotalMember(UserLoginId);
                //// memberList = ObjDb.userlist;
                //if (memberList.Count != 0)
                //{
                //    List<long> UserIds = memberList.Select(x => x.UserId).ToList();
                //    var PersonalDetails = db.PersonalDetails.Where(x => UserIds.Contains(x.UserLoginID)).ToList();
                //    foreach (var item in memberList)
                //    {
                //        HighPerformerViewModel objHighPerformerViewModel = new HighPerformerViewModel();
                //        objHighPerformerViewModel.money = (decimal)objLeaderDashboard.getERP_User((int)item.UserId, 1);
                //        objHighPerformerViewModel.Level = (int)item.NetworkLevel;
                //        PersonalDetail pD = PersonalDetails.FirstOrDefault(p => p.UserLoginID == item.UserId);
                //        if (pD != null)
                //        {
                //            objHighPerformerViewModel.Name = pD.FirstName + ' ' + pD.LastName;
                //        }
                //        objHighPerformerViewModel.searchBy = 3;
                //        objHighPerformerViewModel.UsersCont = 0;
                //        objDashboardViewModel.TopEarnersList.Add(objHighPerformerViewModel);
                //    }
                //    objDashboardViewModel.TopEarnersList = objDashboardViewModel.TopEarnersList.Where(p => p.money > 0).OrderByDescending(p => p.money).Take(10).ToList();
                //}
                // comment by lokesh

                obj = new { Success = 1, Message = "Successfull.", data = objDashboardViewModel };
            }
            catch (Exception ex)
            {
                obj = new { Success = 1, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
