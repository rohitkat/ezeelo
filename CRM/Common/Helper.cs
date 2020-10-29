using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CRM.Common
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

        //------------------------------------------------------------------------- End Convert data table to object list --//


        //----------------------------------------------------------------------- Start Convert object list to data table --//
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        //----------------------------------------------------------------------- End Convert object list to data table --//

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