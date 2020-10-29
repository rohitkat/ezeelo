using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Role
    {
        public Role()
        {
            this.Role1 = new List<Role>();
            this.RoleMenus = new List<RoleMenu>();
            this.UserRoles = new List<UserRole>();
        }

        public int ID { get; set; }
        [Required (ErrorMessage="Role Name is required")]
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        
        [Required(ErrorMessage = "Level is required is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Level")]
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<Role> Role1 { get; set; }
        public virtual Role Role2 { get; set; }
        public virtual List<RoleMenu> RoleMenus { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
    }
}
