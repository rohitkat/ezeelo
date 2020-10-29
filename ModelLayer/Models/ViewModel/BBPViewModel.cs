using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    //For Admin
    public class BBPViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime LastFromDate { get; set; }
        public DateTime LastToDate { get; set; }
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public long PayoutDateFilterID { get; set; }
        public BBPPayoutViewModel bBPPayout { get; set; }
        public List<BBPPayoutDetailsViewModel> List_details { get; set; }
        public List<BBPPayoutUserWiseViewModel> List_userwise { get; set; }
        public List<BBPPayoutOrderWiseViewModel> List_orderwise { get; set; }
        public List<BBPInactivePoints> List_InactivePoints { get; set; }
        public long? FilterID { get; set; }
        public bool OnlyPaid { get; set; }
    }
    public class BBPPayoutViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int TotalSubscriber { get; set; }
        public int ActiveSubscriber { get; set; }
        public int TotalDeliveries { get; set; }
        public int PendingDeliveries { get; set; }
        public decimal CoinRate { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableERP { get; set; }
        public decimal PendingERP { get; set; }
        public decimal TotalPayableERP { get; set; }
    }
    public class BBPPayoutDetailsViewModel
    {
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool Status { get; set; }
        public decimal ERP { get; set; }
        public decimal PendingERP { get; set; }
        public decimal TotalERP { get; set; }
        public bool IsSelected { get; set; }
    }
    public class BBPPayoutUserWiseViewModel
    {
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public long UplineUserLoginId { get; set; }
        public string UplineName { get; set; }
        public int level { get; set; }
        public decimal ERP { get; set; }
    }
    public class BBPPayoutOrderWiseViewModel
    {
        public string OrderCode { get; set; }
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public decimal RetailPoints { get; set; }
        public decimal? UpLine1 { get; set; }
        public long? UpLine1_UserLoginId { get; set; }
        public string UpLine1_Name { get; set; }
        public decimal? UpLine2 { get; set; }
        public long? UpLine2_UserLoginId { get; set; }
        public string UpLine2_Name { get; set; }
        public decimal? UpLine3 { get; set; }
        public long? UpLine3_UserLoginId { get; set; }
        public string UpLine3_Name { get; set; }
        public decimal? UpLine4 { get; set; }
        public long? UpLine4_UserLoginId { get; set; }
        public string UpLine4_Name { get; set; }
        public decimal? UpLine5 { get; set; }
        public long? UpLine5_UserLoginId { get; set; }
        public string UpLine5_Name { get; set; }
        public decimal? UpLine6 { get; set; }
        public long? UpLine6_UserLoginId { get; set; }
        public string UpLine6_Name { get; set; }
        public decimal? UpLine7 { get; set; }
        public long? UpLine7_UserLoginId { get; set; }
        public string UpLine7_Name { get; set; }
        public decimal? UpLine8 { get; set; }
        public long? UpLine8_UserLoginId { get; set; }
        public string UpLine8_Name { get; set; }
        public decimal? UpLine9 { get; set; }
        public long? UpLine9_UserLoginId { get; set; }
        public string UpLine9_Name { get; set; }
        public decimal? UpLine10 { get; set; }
        public long? UpLine10_UserLoginId { get; set; }
        public string UpLine10_Name { get; set; }
        public decimal? UpLine11 { get; set; }
        public long? UpLine11_UserLoginId { get; set; }
        public string UpLine11_Name { get; set; }
        public decimal? UpLine12 { get; set; }
        public long? UpLine12_UserLoginId { get; set; }
        public string UpLine12_Name { get; set; }
        public decimal? UpLine13 { get; set; }
        public long? UpLine13_UserLoginId { get; set; }
        public string UpLine13_Name { get; set; }
        public decimal? UpLine14 { get; set; }
        public long? UpLine14_UserLoginId { get; set; }
        public string UpLine14_Name { get; set; }
        public decimal? UpLine15 { get; set; }
        public long? UpLine15_UserLoginId { get; set; }
        public string UpLine15_Name { get; set; }
        public decimal? UpLine16 { get; set; }
        public long? UpLine16_UserLoginId { get; set; }
        public string UpLine16_Name { get; set; }
    }

    public class BBPDashboardViewModel
    {
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public long PayoutDateFilterID { get; set; }
        public long TotalSubscriber { get; set; }
        public long TotalActiveSubscriber { get; set; }
        public long TotalOrder { get; set; }
        public long TotalDeliveries { get; set; }
        public long PendingDeliveries { get; set; }
        public List<BBPSubscribers> Subscribers { get; set; }
        public List<BBPOrders> Orders { get; set; }
        public List<BBPPayoutDetailsViewModel> InactiveSubscriber { get; set; }
        public bool NearToInactiveStatus { get; set; }
        public string Msg { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }


    public class BBPSubscribers
    {
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string PlanName { get; set; }
        public bool Status { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime LastTransDate { get; set; }
    }

    public class BBPUserStatusReport
    {
        public long SrNo { get; set; }
        public long UserloginId { get; set; }
        public string OrderCode { get; set; }
        public string OrderType { get; set; }
        public decimal RetailPoints { get; set; }
        public string OrderStatus { get; set; }
        public string OrderPlacedDate { get; set; }
        public string OrderDeliveredDate { get; set; }
        public string ActiveUpto { get; set; }
    }
    public class BBPUserStatusData
    {
        public List<BBPUserStatusReport> report { get; set; }
        public List<BBPUserStatusResult> result { get; set; }
        public List<BBPUserStatusOrdReport> order { get; set; }
    }
    public class BBPUserStatusResult
    {
        public int UserStatus { get; set; }
        public string Reason { get; set; }
    }
    public class BBPUserStatusOrdReport
    {
        public string OrderCode { get; set; }
        public decimal RetailPoints { get; set; }
        public string OrdStatus { get; set; }
    }
    public class BBPOrders
    {
        public long UserloginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string OrderCode { get; set; }
        public decimal RetailPoints { get; set; }
        public decimal OrderAmount { get; set; }
        public string OrderStatus { get; set; }
        public string FranchiseName { get; set; }
        public DateTime OrderDate { get; set; }
    }

    //for user
    
    public class BBPWeeklyEarning
    {
        public long SrNo { get; set; }
        public string Duration { get; set; }
        public bool Status { get; set; }
        public decimal ERP { get; set; }
        public decimal EzeeMoney { get; set; }
        public decimal InactivePoint { get; set; }
        public decimal Balance { get; set; }
    }

    public class BBPNetworkViewVeiwModel
    {
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public int PayoutDateFilterId { get; set; }
        public List<BBPNetworkViewMain> main { get; set; }
        public List<BBPNetworkView> details { get; set; }
    }

    public class BBPNetworkViewMain
    {
        public int Level { get; set; }
        public int count { get; set; }
        public decimal ERP { get; set; }
    }
    public class BBPNetworkView
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public decimal ERP { get; set; }
    }

    public class BBPTabularviewModel
    {
        public List<BBPTabularview> list { get; set; }
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public int PayoutDateFilterId { get; set; }
    }
    public class BBPTabularview
    {
        public long SrNo { get; set; }
        public string Name { get; set; }
        public string RefferalID { get; set; }
        public string Level { get; set; }
        public string ParentName { get; set; }
        public string Booster_Purchased { get; set; }
        public string PurchaseDate { get; set; }
    }

    public class BBPUserDashboard
    {
        public int TotalMember { get; set; }
        public int TotalSubscriber { get; set; }
        public decimal TotalERP { get; set; }
        public decimal InactivePoints { get; set; }
        public bool Status { get; set; }
    }

    public class BBPInactivePoints
    {
        public long UserLoginID { get; set; }
        public string PayoutWeek { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public decimal InactivePoints { get; set; }
        public string RefPayoutWeek { get; set; }
    }
}
