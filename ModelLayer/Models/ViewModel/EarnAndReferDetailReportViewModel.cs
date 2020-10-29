using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class EarnAndReferDetailReportViewModel
    {
        public long ID { get; set; }
        public Nullable<long> ReferenceID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Nullable<decimal> EarnAmount { get; set; }
        public string SchemeName { get; set; }
        public string OrderCode { get; set; }
        public Nullable<decimal> OrderAmount { get; set; }
        public Nullable<decimal> TotalBudgetAmount { get; set; }
        public Nullable<decimal> RemainingAmount { get; set; }
        public DateTime ReferredDate { get; set; }//Added by harshada on 18/1/2017
        public string RefereeStatus { get; set; }//Added by harshada on 18/1/2017
        public string FranchiseName { get; set; }//Added by harshada on 18/1/2017
        public Nullable<DateTime> RegDate { get; set; }//Added by harshada on 18/1/2017
    }

    public class EarnAndReferReportViewModelDetails
    {
        public List<EarnAndReferDetailReportViewModel> lEarnAndReferReportViewModelDet { get; set; }
        public Nullable<decimal> RemainingAmount { get; set; }
        public Nullable<decimal> totalEarnAmount { get; set; }
        public Nullable<decimal> totalUsedAmount { get; set; }

        //=========== Tejaswee Change in new Project also ==================//

        public Nullable<decimal> SchemeRemainAmt { get; set; }
        public string ExpiryDays { get; set; }

    }
}
