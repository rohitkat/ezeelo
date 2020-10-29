using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
     [Table("Zone")]
   public class   Zone
   {
         public static long WarehouseID;    

       [Key]
        public  long  ID{get;set;}
        public String Name{get;set;}
        public string 	Abbreviation {get ;set ;}
       

    }
}
