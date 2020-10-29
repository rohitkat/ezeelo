using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class ChatDataList
    {
        public List<ChatDataListDetail> chatDataListDetail { get; set; }
    }

    public class ChatDataListDetail
    {
        public string Name { get; set; }

    }
}
