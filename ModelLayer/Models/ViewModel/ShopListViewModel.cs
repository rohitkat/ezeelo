using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class ShopListViewModel
    {
       [Required(ErrorMessage = "Please provide list of shops")]
       public List<long> shopList { get; set; }
    }
}
