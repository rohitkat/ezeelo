using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  [Table("Network_User")]
  public  class NetworkUserViewModel
    {
      [Key]
      public long UserId { get; set; }     
      public int? NetworkLevel { get; set; }      
    }
  public class NetworkUserStatusViewModel
  {
      public long UserId { get; set; }      
      public int? NetworkLevel { get; set; }
      public bool IsActive { get; set; }
  }

}
