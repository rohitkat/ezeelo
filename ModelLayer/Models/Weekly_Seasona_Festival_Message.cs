using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Weekly_Seasona_Festival_Message
    {
        public Weekly_Seasona_Festival_Message()
        {
            this.Weekly_Seasona_Festival_Message1 = new List<Weekly_Seasona_Festival_Message>();
        }

        public long ID { get; set; }
        public int FranchiseID { get; set; }
        public long MessageTypeID { get; set; }
        public string Message { get; set; }
        public Nullable<int> MinimumOrderInRupee { get; set; }
        public string WeeklyHoliday { get; set; }
        public string SeasonalMsgFrmMonth { get; set; }
        public string SeasonalMsgToMonth { get; set; }
        public Nullable<System.DateTime> FestivalMsgFrmDate { get; set; }
        public Nullable<System.DateTime> FestivalMsgToDate { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual MessageType MessageType { get; set; } //added
        public virtual ICollection<Weekly_Seasona_Festival_Message> Weekly_Seasona_Festival_Message1 { get; set; }
        public virtual Weekly_Seasona_Festival_Message Weekly_Seasona_Festival_Message2 { get; set; }

    }
}
