using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryPartner.Models.ViewModel
{
    public class EmployeeAssignmentViewModel
    {
        public long ID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<long> OwnerID { get; set; }
        public bool IsActive { get; set; }
        public string LoginStatus { get; set; }
        public string EmployeeList { get; set; }
        public string AssignStatus { get; set; }
    }
}