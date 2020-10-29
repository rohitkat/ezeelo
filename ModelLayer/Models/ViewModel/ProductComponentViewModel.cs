using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{

    public class ProductComponentViewModel
    {
        public List<Component> componentList { get; set; }
        public int ComponentID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Componant Name must be between 5 to 50 Chatacter")]
        public string ComponentName { get; set; }
        public long ShopID { get; set; }
        public long ProductID { get; set; }

        public string ProductName { get; set; }
        public decimal PerUnitRateInRs { get; set; }
        public decimal PerUnitRateInPer { get; set; }
        public string DependentComponentName { get; set; }
        public Nullable<int> DependentOnComponentID { get; set; }
        public Nullable<decimal> ComponentWeight { get; set; }
        public Nullable<int> ComponentUnitID { get; set; }
        public string ComponentUnitName { get; set; }
        public decimal Total { get; set; }
        public int UnitID { get; set; }
        public int Quantity { get; set; }
        public long ShopStockID { get; set; }
        public long ProductVarientID { get; set; }
        public decimal MRP { get; set; }

        //Varients Model
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int DimensionID { get; set; }
        public string DimensionName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }

    }


}
