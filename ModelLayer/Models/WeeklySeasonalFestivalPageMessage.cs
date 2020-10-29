using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
   public class WeeklySeasonalFestivalPageMessage
    {
       //-- Use for GetPageMessages API --//
        public long MessageTypeID { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public int? MinimumOrderInRupee { get; set; }
        public string WeeklyHoliday { get; set; }
        public string SeasonalMsgFrmMonth { get; set; }
        public string SeasonalMsgToMonth { get; set; }
        public string FestivalMsgFrmDate { get; set; }
        public string FestivalMsgToDate { get; set; }
    }
}
