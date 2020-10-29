using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    // created by amit for Userlist
  public  class MLMUserViewModel        
    {
        public long UserID { get; set; }
        
        public string FullName { get; set; }
       
        public string Email { get; set; }

        public string Mobile { get; set; }
        public DateTime CreateDate { get; set; }
        

        public long Id_Ref { get; set; }
       

        public string Ref_Id { get; set; }

        public DateTime Join_date_ref { get; set; }

        public Nullable<bool> Status_ref { get; set; }
      
        public DateTime Activate_date_ref { get; set; }
        public string Refered_Id_ref { get; set; }
        public Nullable<bool> request { get; set; }
        public Nullable<bool> request_active { get; set; }
       // public string ProfilePicture { get; set; }

        public decimal? RP { get; set; }
        public decimal? LeftQRP { get; set; }
        public decimal? ERP { get; set; }

        public string ParentName { get; set; }   //added by amit on 3-12-18

        public string Designation { get; set; }  // added by lokesh

        public int Level { get; set; }          // added by lokesh

        public int NoOftimeActive { get; set; }  // added by lokesh


    }
}
