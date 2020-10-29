//-----------------------------------------------------------------------
// <copyright file="CustomerOrder.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using ModelLayer.Models;
using System.Web.ModelBinding;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.Configuration;
using System.Collections;

namespace BusinessLogicLayer
{
    public class CityHelpLineNo :CustomerManagement
    {
        public CityHelpLineNo(System.Web.HttpServerUtility server)
            : base(server)
        {
        }

        private static EzeeloDBContext db = new EzeeloDBContext();
        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
        //public static int GetCityHelpLineNo(long ShopID)
        //{
        //    int Result = 0;
        //    try
        //    {
        //        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
        //        SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
        //        con.Open();
        //        SqlCommand sqlComm = new SqlCommand("SelectCityHelpLineNo", con);
        //        sqlComm.CommandType = CommandType.StoredProcedure;
        //        sqlComm.Parameters.AddWithValue("@ShopId", ShopID);
        //        Result=sqlComm.ExecuteNonQuery();
        //        con.Close();
        //    }
        //    catch (Exception)
        //    {
        //        Result = 0;
        //        throw;
        //    }
        //    return Result;
        //}
        public static List<CityHelpLineNoViewModel> GetCityHelpLineNo(long ShopID)
        {
            List<CityHelpLineNoViewModel> lCityHelpLineNoViewModel = new List<CityHelpLineNoViewModel>();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);

                SqlCommand sqlComm = new SqlCommand("SelectCityHelpLineNo", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@ShopID", ShopID);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                //con.Close();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CityHelpLineNoViewModel lCityHelpLineNo = new CityHelpLineNoViewModel();
                        lCityHelpLineNo.HelpLineNumber = dt.Rows[i]["HelpLineNumber"].ToString();
                        lCityHelpLineNo.ManagmentNumber = dt.Rows[i]["ManagmentNumber"].ToString();
                        lCityHelpLineNoViewModel.Add(lCityHelpLineNo);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return lCityHelpLineNoViewModel;
        }

    }
}
