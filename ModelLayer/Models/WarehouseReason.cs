using ModelLayer.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ModelLayer.Models
{
    [Table("warehousereason")]
    public class WarehouseReason
    {
        [Key]
        public long ID { get; set; }
        [Required]
        public string Reason { get; set; }
        public long? ParentReasonId { get; set; }
        public bool IsActive { get; set; }      
        public DateTime CreateDate { get; set; }        
        public long? CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        [NotMapped]
        public SelectList WarehouseReasonlist { get; set; }
        public int ParentCategoryId { get; set; } //Yashaswi 10/5/2018
        [NotMapped]
        public List<Reason_ParentCategory> CategoryList { get; set; }//Yashaswi 10/5/2018
    }
}