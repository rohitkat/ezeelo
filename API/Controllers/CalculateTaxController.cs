using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace API.Controllers
{
    /*
     * Developed By :- Pradnyakar N. Badge
     * Dated :- 05/01/2016
     * Purpose to calculate taxes for product as define in database
     * 
     */
    public class CalculateTaxController : ApiController
    {
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        // GET api/calculatetax
        /// <summary>
        ///
        /// Get detail about product taxes
        /// </summary>
        /// <param name="ShopStockIDList">List of ShopStockID</param>
        /// <returns>List of taxes of each shopstockIDs</returns>
        public object Get(Int64 ShopStockID)
        {
            object obj = new object();
            try
            {
                if (ShopStockID <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                List<CalulatedTaxesRecord> objList = new List<CalulatedTaxesRecord>();
                BusinessLogicLayer.TaxationManagement calTax = new BusinessLogicLayer.TaxationManagement(fConnectionString);
                //List<Int64> paramValues = new List<Int64>();
                //paramValues.Add(245933);
                //paramValues.Add(245932);
                objList = calTax.CalculateTaxForProduct(ShopStockID);
                if (objList != null && objList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = objList };
                }
                else
                {
                    obj = new { Success = 0, Message = "Invalid ShopstockID.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // GET api/calculatetax/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/calculatetax
        public object Post(List<Int64> ShopStockIDList)
        {
            object obj = new object();
            try
            {
                if (ShopStockIDList == null || ShopStockIDList.Count <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                List<CalulatedTaxesRecord> objList = new List<CalulatedTaxesRecord>();
                BusinessLogicLayer.TaxationManagement calTax = new BusinessLogicLayer.TaxationManagement(fConnectionString);
                //List<Int64> paramValues = new List<Int64>();
                //paramValues.Add(245933);
                //paramValues.Add(245932);
                objList = calTax.CalculateTaxForMultipalProduct(ShopStockIDList);
                if (objList != null && objList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = objList };
                }
                else
                {
                    obj = new { Success = 0, Message = "Invalid ShopstockID list.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // PUT api/calculatetax/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/calculatetax/5
        public void Delete(int id)
        {
        }
    }
}
