using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory.Common
{
    public class Constants
    {
        //Yashaswi 17-3-2018
        public enum Warehouse_Stock_Log_Status
        {
            INVOICE = 1,
            WASTAGE = 2,
            WASTAGE_ADD_IN_STOCK = 3,
            RETURN = 4,
            REPLACE = 5,
            WASTAGE_RETURN = 6,
            INWARD = 7,
            OUTWARD = 8,
            RETURN_ADD_IN_STOCK = 9
        }

        public enum Inventory_Image_Type
        {
            WASTAGE = 1,
            INVOICE = 2,
        }
        //Yashaswi 17/4/2018
        public enum Margin_Division
        {
            LEADERSHIP = 1,
            EZEELO = 2,
            LEADERS_ROYALTY = 3,
            LIFESTYLE_FUND = 4,
            LEADERSHIP_DEVELOPMENT_FUND = 5
        }

        public enum LocationStatus
        {
            Empty =0,
            Semufull=1,
            Fulfilled=2
        }

        public enum StorageType
        {
            Rake = 1,
            Palete = 2

        }

        public enum Inventory_Alphabete
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            E = 5,
            F = 6,
            G = 7,
            H = 8,
            I = 9,
            J = 10,
            K = 11,
            L = 12,
            M = 13,
            N = 14,
            O = 15,
            //P = 16, // skip P from rack because P is reserved only for Pallete
            Q = 17,
            R = 18,
            S = 19,
            T = 20,
            U = 21,
            V = 22,
            W = 23,
            X = 24,
            Y = 25,
            Z = 26,
            AA = 27,
            BB = 28,
            CC = 29,
            DD = 30,
            EE = 31,
            FF = 32,
            GG = 33,
            HH = 34,
            II = 35,
            JJ = 36,
            KK = 37,
            LL = 38,
            MM = 39,
            NN = 40
        }
        public class Alphabete
        {
            public Inventory_Alphabete Inventory_Alphabete { get; set; }
        }
    }
}