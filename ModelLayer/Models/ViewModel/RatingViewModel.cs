using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class RatingViewModel
    {
        public List<CustomerRatingViewModel> MasterRating  { get; set; }
        public long CustomerOrderID { get; set; }

    }
}
