using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
   public class UnitCalculation
    {
       public int ID { get; set; }

       public int FromUnitID { get; set; }

       public int ToUnitID { get; set; }

       public decimal MultiplyBy { get; set; }
    }
}
