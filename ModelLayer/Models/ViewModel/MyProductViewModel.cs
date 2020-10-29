using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MyProductViewModel
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public bool isSelected { get; set; }
    }


    public class MyProductList
    {
        public Int32 TwoLevelID { get; set; }
        public Int32 ThreeLevelID { get; set; }

        public List<MyProductViewModel> pMyProductViewModel { get; set; }
    }
}
