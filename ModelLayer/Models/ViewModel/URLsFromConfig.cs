using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ModelLayer.Models.ViewModel
{
    public class URLsFromConfig
    {
        public string GetURL(string input)
        {
            try
            {
                string output = "";
                switch (input)
                {
                    case "CUSTOMER": //Customer
                        output = WebConfigurationManager.AppSettings["EZEELO_CUSTOMER_URL"];
                        break;
                    case "API": //Api
                        output = WebConfigurationManager.AppSettings["EZEELO_API_URL"];
                        break;
                    case "LEADERS": //Leaders
                        output = WebConfigurationManager.AppSettings["EZEELO_LEADER_URL"];
                        break;
                    case "PARTNER": //Partner
                        output = WebConfigurationManager.AppSettings["EZEELO_PARTNER_URL"];
                        break;
                    case "MERCHANT": //Merchant
                        output = WebConfigurationManager.AppSettings["EZEELO_MERCHANT_URL"];
                        break;
                    case "IMG": //IMG
                        output = WebConfigurationManager.AppSettings["EZEELO_IMG_URL"];
                        break;
                    default:
                        output = "";
                        break;
                }
                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Yashaswi 17/12/2018 Default City Change 
        public static string GetDefaultData(string input)
        {
            try
            {
                string output = "";
                switch (input)
                {
                    case "CITY_NAME": 
                        output = WebConfigurationManager.AppSettings["DFLT_CITY_NAME"];
                        break;
                    case "CITY_ID": 
                        output = WebConfigurationManager.AppSettings["DFLT_CITY_ID"];
                        break;
                    case "PINCODE_NAME":
                        output = WebConfigurationManager.AppSettings["DFLT_PINCODE_NAME"];
                        break;
                    case "PINCODE_ID": 
                        output = WebConfigurationManager.AppSettings["DFLT_PINCODE_ID"];
                        break;
                    case "AREA_NAME":
                        output = WebConfigurationManager.AppSettings["DFLT_AREA_NAME"];
                        break;
                    case "AREA_ID": 
                        output = WebConfigurationManager.AppSettings["DFLT_AREA_ID"];
                        break;
                    case "HELPLINE_NO": 
                        output = WebConfigurationManager.AppSettings["DFLT_HELPLINE_NO"];
                        break;
                    case "FRANCHISE_ID": 
                        output = WebConfigurationManager.AppSettings["DFLT_FRANCHISE_ID"];
                        break;
                    default:
                        output = "";
                        break;
                }
                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
