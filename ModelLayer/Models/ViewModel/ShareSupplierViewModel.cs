using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    //For Share Supplier 18-01-2019
    public class ShareSupplierViewModel
    {
        public string EVWName { get; set; }
        public string State { get; set; }
        public string SuppNameList { get; set; }
        public List<SupplierList> SuppList { get; set; }
    }
    //For Share Supplier 18-01-2019
    public class SupplierList
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
