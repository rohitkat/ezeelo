using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DesignBlockType
    {
        public DesignBlockType()
        {
            this.BlockItemsLists = new List<BlockItemsList>();
        }

        public long ID { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 - 50 characters ")]      
        public string Name { get; set; }
        public string ImageExtension { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Please enter valid Width")]
        public decimal ImageWidth { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Please enter valid Height")]
        public decimal ImageHeight { get; set; }
        public Nullable<int> MinLimit { get; set; }

        [Required(ErrorMessage = "MaxLimit is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid MaxLimit")]
        public int MaxLimit { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string NetworkIP { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<BlockItemsList> BlockItemsLists { get; set; }
    }
}
