using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class FranchiseMenuListViewModel
    {
        public Int64 LevelOneID { get; set; }
        public Int64 LevelTwoID { get; set; }
        public Int64 LevelThreeID { get; set; }
        public string LevelOneName { get; set; }
        public string LevelTwoName { get; set; }
        public string LevelThreeName { get; set; }
        //public bool LevelThreeIsActive { get; set; }
        public string ImagePath { get; set; }
        public string Thumb_ImagePath { get; set; }

        public int? LevelOneSequenceOrder { get; set; }
        public int? LevelTwoSequenceOrder { get; set; }
        public int? LevelthreeSequenceOrder { get; set; }
    }
}
