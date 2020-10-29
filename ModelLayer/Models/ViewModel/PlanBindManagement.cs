using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models.ViewModel
{
    public class PlanBindManagement
    {
        [Required(ErrorMessage = "Level Is required")]
        public int level { get; set; }
        [Required(ErrorMessage = "Please Select The Type")]
        [Range(1, 2, ErrorMessage = "Please Select The Type")]
        public int type { get; set; }
        public bool IsActive { get; set; }
        public long ID { get; set; }
        public List<PlanCommission> planCommission { get; set; }

    }

    public class PlanCommission
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int CategoryLevel { get; set; }
        public int? parentCategoryID { get; set; }
        public bool IsActive { get; set; }
        public bool isSelect { get; set; }
    }

}