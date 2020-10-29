using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Leaders.Common
{
    public class Constant
    {
        public enum Leaders_Image_Type
        {
            ADHAR = 1,
            PAN = 2,
            PASSBOOK = 3,
            PROFILE = 4
        }

        public enum Inventory_Image_Type
        {
            //WASTAGE = 1,
            //INVOICE = 2,
            ADHAR = 1,
            PAN = 2,
            PASSBOOK = 3,
            PROFILE = 4,
            KYC = 5
        }
    }
}