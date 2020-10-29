using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace DeliveryPartner.Models.ViewModel
{
    public class DashBoardViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Dictionary<string,int> TodayStatus { get; set; }
        public Dictionary<string, int> DonutStatus { get; set; }
    }
}