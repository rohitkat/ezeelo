using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Email_Type
    {
        public Email_Type()
        {
            this.Sent_Mail = new List<Sent_Mail>();
        }

        public int Mail_Type_ID { get; set; }
        public string Mail_Name { get; set; }
        public string Mail_Containt { get; set; }
        public virtual ICollection<Sent_Mail> Sent_Mail { get; set; }
    }
}
