using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class LeadersTabularViewParameterViewModel
    {
        public int LevelId { get; set; }
        public SelectList LevelList { get; set; }
        public string SearchBy { get; set; }
        public bool Active { get; set; }
        public bool InActive { get; set; }
        public bool Name { get; set; }
        public bool RefId { get; set; }
        public bool EMail { get; set; }
        public bool JoinDate { get; set; }
        public bool ParaenttName { get; set; }
        public int pageNo { get; set; }
        public List<TabularView> list { get; set; }

        public List<Downline> listDownline { get; set; }

    }
    public class TabularView

    {
        public List<LeadersTabularViewParameterViewModel> lobj = new List<LeadersTabularViewParameterViewModel>();
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string RefferalId { get; set; }
        public string Email { get; set; }

        public string Mobile { get; set; } // added by amit on 10-11-18
        public double ERPbySP { get; set; } // added by amit on 10-11-18

        public int Level { get; set; }
        public decimal TotalRetailPoints { get; set; }
        public decimal QRP { get; set; }
        public long ParentId { get; set; }
        public string ParentName { get; set; }
        public int status { get; set; }

        public string Designation { get; set; } // added by lokesh 30/04/2019




        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastTras { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoinDate { get; set; }

        public IEnumerable<TabularView> DownLineLevel { get; set; }
        public IEnumerable<TabularView> DownLineLevel2 { get; set; }
        public IEnumerable<TabularView> DownLineLevel3 { get; set; }
        public IEnumerable<TabularView> DownLineLevel4 { get; set; }
        public IEnumerable<TabularView> DownLineLevel5 { get; set; }
        public IEnumerable<TabularView> DownLineLevel6 { get; set; }
        public IEnumerable<TabularView> DownLineLevel7 { get; set; }


    }


    public class Downline{
        public int Levelno { get; set; }
        public int TotalDownline { get; set; }
        public decimal TotalRP { get; set; }
        public int Activeusers { get; set; }
    }
}
