using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopDetailsPostViewModel
    {
        public long ID { get; set; }

        public long BusinessDetailsID { get; set; }

        public string BusinessName { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }

        public string Lattitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public string NearestLandmark { get; set; }

        public int PincodeID { get; set; }

        public string Pincode { get; set; }

        public Nullable<int> AreaID { get; set; }
        
        public string ContactPerson { get; set; }
      
        public string Email { get; set; }
        
        public string Mobile { get; set; }
        
        public string Landline { get; set; }
        
        public string FAX { get; set; }
        
        public string VAT { get; set; }
        
        public string TIN { get; set; }
        
        public string PAN { get; set; }
      
        public bool CurrentItSetup { get; set; }

        public Nullable<System.TimeSpan> OpeningTime { get; set; }
        
        public Nullable<System.TimeSpan> ClosingTime { get; set; }

        public string WeeklyOff { get; set; }

        public bool IsLive { get; set; }
        
        public bool IsManageInventory { get; set; }
        
        public bool IsActive { get; set; }
        
        public System.DateTime CreateDate { get; set; }
        
        public long CreateBy { get; set; }
        
        public Nullable<System.DateTime> ModifyDate { get; set; }
        
        public Nullable<long> ModifyBy { get; set; }
        
        public string NetworkIP { get; set; }
        
        public string DeviceType { get; set; }
        
        public string DeviceID { get; set; }

    }
}
