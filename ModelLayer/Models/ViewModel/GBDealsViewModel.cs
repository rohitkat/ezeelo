using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    /*=================================================================================
     * Author : Tejaswee Taktewale
     * Date   : 18-feb-2016
     * Description : This file is used to display offer div such as rice festival, diwali offer on home page
     =================================================================================*/

    public class GBDealsViewModel
    {
        
        public int CatID { get; set; }
        public long ShopID { get; set; }
        public String DealThumbPath { get; set; }
    }
}
