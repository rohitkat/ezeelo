using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class SiteMapViewModel
    {
        public long ID { get; set; }
        public string RouteName { get; set; }
        public int Level { get; set; }
        public bool IsManaged { get; set; }
        public string URL { get; set; }

        //- Added by Avi Verma. Date : 05-May-2017. Reason : SEO Team (Bhushan) required 2nd level category in sitemap.xml.
        public Nullable<long> ParentCategoryID { get; set; }
    }
}
