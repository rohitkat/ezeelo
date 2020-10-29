using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class EzeeMoneyPayoutViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime LastFromPayoutDate { get; set; }
        public DateTime LastToPayoutDate { get; set; }
        public decimal? CoinRate { get; set; }
        public int DelOrdCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int UserCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PayableERP { get; set; }
        public decimal DiffAmount { get; set; }
        public decimal DiffERP { get; set; }
        public long EzeeMoneyPayoutId { get; set; }
        public bool isGoDisable { get; set; }
        public bool isFrzDtDisable { get; set; }
        public bool isPayEzMnyDisable { get; set; }
        public bool isGoClick { get; set; }
        public List<PayoutFreezeDataReportViewModel> listEzeeMoney { get; set; }
        public List<InactivePayoutFreezeDataReportViewModel> ListInactiveEzeeMoney { get; set; }

        public decimal? TotalInActivePoints { get; set; }  // added by amit
        public decimal? TotalInActivePointsAmount { get; set; }  // added by amit

        public decimal? TotalPayableERP { get; set; }  // amit
        public decimal? TotalPayableAmount { get; set; }  //amit
        public List<OrderWiseGrid_ERP> ListOrderWiseGrid_ERP { get; set; }

        public DateTime FromDate1 { get; set; } // lokesh
        public DateTime ToDate1 { get; set; }   // lokesh

        public List<DesignationReport> DesignationReportlist { get; set; }
        public List<SelectListItem> PayoutDateFilter { get; set; }
    }

    public class Network_User_Extend
    {
        [Key]
        public int UserId { get; set; }
        public int UserStatus { get; set; }
        public double ERPPoints_MLMWallet { get; set; }
        public double ERPPoints_DirectIncome { get; set; }
    }

    public class OrderWiseGrid_ERP // add by lokesh panwar
    {

        public string OrderCode { get; set; }
        public string CreateDate { get; set; }
        public double TransactionPoints { get; set; }
        public double CurrentLevel { get; set; }
        public string CurrentLevel_UserName { get; set; }
        public double UpLine1 { get; set; }
        public string UpLine1_UserName { get; set; }
        public double UpLine2 { get; set; }
        public string UpLine2_UserName { get; set; }
        public double UpLine3 { get; set; }
        public string UpLine3_UserName { get; set; }
        public double UpLine4 { get; set; }
        public string UpLine4_UserName { get; set; }
        public double UpLine5 { get; set; }
        public string UpLine5_UserName { get; set; }
        public double UpLine6 { get; set; }
        public string UpLine6_UserName { get; set; }
        public double UpLine7 { get; set; }
        public string UpLine7_UserName { get; set; }
        public double UpLine8 { get; set; }
        public string UpLine8_UserName { get; set; }
        public double UpLine9 { get; set; }
        public string UpLine9_UserName { get; set; }
        public double UpLine10 { get; set; }
        public string UpLine10_UserName { get; set; }
        public double UpLine11 { get; set; }
        public string UpLine11_UserName { get; set; }
        public double UpLine12 { get; set; }
        public string UpLine12_UserName { get; set; }
        public double UpLine13 { get; set; }
        public string UpLine13_UserName { get; set; }
        public double UpLine14 { get; set; }
        public string UpLine14_UserName { get; set; }
        public double UpLine15 { get; set; }
        public string UpLine15_UserName { get; set; }
        public double UpLine16 { get; set; }
        public string UpLine16_UserName { get; set; }
        public int Status { get; set; }
    }

    public class DesignationReport
    {
        public int Designationid { get; set; }

        public string Designation { get; set; }

        public int TotalMembers { get; set; }

        public int Active { get; set; }

        public double ActiveRP { get; set; }

        public double ActiveERP { get; set; }

        public double InActiveRP { get; set; }

        public double InActiveERP { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }




}
