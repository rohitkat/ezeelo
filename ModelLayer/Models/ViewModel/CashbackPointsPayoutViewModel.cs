using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class CashbackPointsPayoutViewModel
    {
        public List<SelectListItem> PayoutDateFilter { get; set; }
        public int EzeeMoneyPayoutId { get; set; }
        public bool OnlyActiveUser { get; set; }
        public List<Orderwise> orderwise { get; set; }
        public List<Userwise> userwise { get; set; }
    }

    public class Orderwise
    {
        public string OrderCode { get; set;}
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public decimal PayableAmout { get; set; }
        public decimal RetailPointsTotal { get; set; }
        public decimal CashbackPointsTotal { get; set; }
    }

    public class Userwise
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public decimal CashbackPoints { get; set; }
        public decimal CashbackAmount { get; set; }
    }
}
