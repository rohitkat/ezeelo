using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    // Added by Amit on 2/8/2018
   public class NetworkReportViewModel
    {
        public long? UserID { get; set; }

        public string UserEmail { get; set; }
        public string FullName { get; set; }       //added on  5-10-18
        public string Mobile { get; set; }   //added on  12-10-18

        public string OrderCode { get; set; }
        public decimal? OrderAmount { get; set; }
        public int? BusinessPoint { get; set; }
        public decimal ERP { get; set; }  // added by amit on 21-11-2018
        public long MemberCount { get; set; } // added by amit on 21-11-2018
        public int? Downline { get; set; }
        public string OrderStatus { get; set; }

       /// <summary>
       /// added by amit for Network Explorer in Customer Module
       /// </summary>
        public string Name { get; set; }

        public string ReferalID { get; set; }
       [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
       [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }
        public string ParentName { get; set; }

        public decimal? BusinessPointTotal  { get; set; }
       [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
       [DataType(DataType.Date)]
        public DateTime? LastTransaction { get; set; }
       public string ActiveStatus { get; set; }

       public decimal PendingPoints { get; set; }     // added by amit on 11/05/2018
       public decimal RPOnMyPurchase { get; set; }  // added by amit on 21/11/2018
       public decimal InActivePoints { get; set; } // added by amit on 21/11/2018


       public int Totallevel { get; set; } // added by lokesh panwar

    }
}
