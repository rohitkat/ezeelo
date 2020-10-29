using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public static class Helper
    {
        //----------------------------------------------------------------------- Start Convert data table to object list --//

        // function that set the given object from the given data row
        public static void SetItemFromRow<T>(T item, DataRow row)
            where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        // function that creates an object from the given data row
        public static T CreateItemFromRow<T>(DataRow row)
            where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        // function that creates a list of an object from the given data table
        public static List<T> CreateListFromTable<T>(DataTable tbl)
            where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }

        //
        public static List<T> CreateListFromTable<T>(DataSet ds)
           where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    // add to the list
                    lst.Add(CreateItemFromRow<T>(dr));
                }
            }

            // return the list
            return lst;
        }

        //------------------------------------------------------------------------- End Convert data table to object list --//




        //public static List<T> FilterList<T>(List<T> pLst, Dictionary<string, string> pParamenters)
        //    where T: new()
        //{
        //    List<T> lLst = new List<T>();
        //    foreach (var property in pParamenters)
        //    {
        //        var lProperty = pLst.GetType().GetProperty(property.Key.ToString());
        //        if (lProperty != null)
        //        {
        //            pLst = pLst.Where(x => x.<T.property.Key> == property.Value).ToList();
        //        }
        //    }
        //    return lLst;
        //}


    }
}
