using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class Menu
    {
        public Menu()
        {
            this.Menu1 = new List<Menu>();
            this.RoleMenus = new List<RoleMenu>();
            this.UserAdditionalMenus = new List<UserAdditionalMenu>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Menu Name is Required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Menu Name must be between 3 - 150 characters ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Menu Display Name is Required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Menu Display Name must be between 3 - 150 characters ")]
        public string DisplayName { get; set; }
        public Nullable<int> ParentID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<Menu> Menu1 { get; set; }
        public virtual Menu Menu2 { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<RoleMenu> RoleMenus { get; set; }
        public virtual List<UserAdditionalMenu> UserAdditionalMenus { get; set; }
    }
}
