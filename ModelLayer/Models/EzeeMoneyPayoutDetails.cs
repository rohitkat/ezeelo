using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelLayer.Models
{[Table("EzeeMoneyPayoutDetails")]
    public class EzeeMoneyPayoutDetails
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public int DelOrdCount { get; set; }
        public decimal TotalOrdAmt { get; set; }
        public decimal TotalRetailPoints { get; set; }
        public decimal ERP { get; set; }
        public Boolean Status { get; set; }
        public decimal EzeeMoney { get; set; }
        public decimal QRP { get; set; }
        public long EzeeMoneyPayoutID { get; set; }
        public string OrdCode { get; set; }//Yashaswi 22-1-19
        public string TransID { get; set; }//Yashaswi 22-1-19

        public decimal? InActivePoints { get; set; }  // added by amit
        public decimal? CurrentMonthPoints { get; set; } // added by amit
        public long? Ref_EzeeMoneyPayoutID { get; set; }
        public bool IsInactivePaid { get; set; } // Added by Sonali for Inactive Payout
        public decimal? InactiveEzeeMoney { get; set; }// Added by Sonali for Inactive Payout
       
        [NotMapped]
        public decimal LastFirstMonthRP { get; set; }
        [NotMapped]
        public decimal LastSecoundMonthRP { get; set; }

    }
}
