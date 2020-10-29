using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductListByCategoryViewModel
    {
        public int CityID { get; set; }
        public int FranchiseID { get; set; } 
        public long ShopID { get; set; }
        public string Keyword { get; set; }
        public System.Data.DataTable CategoryIDList { get; set; }
        public long ProductID { get; set; }
        public string BrandIDs { get; set; }
        public string ColorIDs { get; set; }
        public string SizeIDs { get; set; }
        public string DimensionIDs { get; set; }
        public string MaterialIDs { get; set; }
        public string SpecificationIDs { get; set; }
        public string SpecificationValues { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        [Required]
        public int PageIndex { get; set; }
        [Required]
        public int PageSize { get; set; }
        public bool IsScroll { get; set; }
        public long CustLoginID { get; set; }
        public bool SearchInCategoryOnly { get; set; }

        /*Added By Pradnyakar Badge
        for Mobile API requirement for Image type
        */
        public string ImageType { get; set; }
        /*
         for Sort type
         */
        public Nullable<int> SortType { get; set; }

        public bool IsVarientRestricted { get; set; }

        public bool isListVarient { get; set; }

        /// <summary>
        /// Added by Tejaswee 2-8-2016
        /// Contains list of shopping cart products shopStockId 
        /// used for sorting list of products depending on products added in shopping cart.
        /// </summary>
        public System.Data.DataTable ShopStockIDList { get; set; }

        /// <summary>
        /// Sort Product list accorting to give input such as alphabetically, price wise etc.
        /// </summary>
        public int SortVal { get; set; }

        public int? Version { get; set; }//// Added by Ashish For New App
    }
}
