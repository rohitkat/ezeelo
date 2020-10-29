using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ModelLayer;
namespace ModelLayer.Models.ViewModel
{

    public class RoleManagerViewModel
    {
        public List<CustomeRoleModel> rolemenuCollection { get; set; }
    }


    public class CustomeRoleModel
    {

        public int RoleMenuID { get; set; }
        public int MenuID { get; set; }
        public int RoleID { get; set; }
        public string MenuName { get; set; }
        public string DisplayName { get; set; }
        public long? MenuParentID { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
        public bool CanImport { get; set; }
        public bool IsActive { get; set; }

    }


}