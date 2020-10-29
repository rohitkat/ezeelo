using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class LogTable
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string RecordXML { get; set; }
        public long TableRowID { get; set; }
        public string Command { get; set; }
        public long RowOwnerID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
    }
}
