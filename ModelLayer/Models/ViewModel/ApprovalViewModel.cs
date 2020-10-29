using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ApprovalViewModel
    {
        [Required(ErrorMessage = "Charge Stage is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Charge Stage")]
        public int ChargeStageID { get; set; }

        [Required(ErrorMessage = "Charge is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Charge")]
        public int ChargeID { get; set; }
        public virtual ChargeStage ChargeStage { get; set; }
        public virtual Charge Charge { get; set; }

        //[Required(ErrorMessage = "Fees is Required")]
        [Range(0, double.MaxValue, ErrorMessage = "Invalid Fees")]
        public decimal? Fees { get; set; }
        public Int64 UserLoginID { get; set; }

        [Required(ErrorMessage = "Owner Name is Required")]        
        public string FromName { get; set; } 
        public Nullable<int> FromBusinessTypeID { get; set; }
        public Nullable<long> FromPersonalDetailId { get; set; }
        public Nullable<int> ToBusinessTypeID { get; set; }
        public Nullable<long> ToPersonalDetailID { get; set; }
        public Nullable<decimal> TransactionAmount { get; set; }
        public bool IsApproved { get; set; }
    }
}
