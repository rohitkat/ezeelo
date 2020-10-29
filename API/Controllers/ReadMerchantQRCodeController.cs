using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models;

namespace API.Controllers
{
    public class ReadMerchantQRCodeController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        class MerchantClass
        {
            public string Name { get; set; }
            public string City { get; set; }
            public long Id { get; set; }
        }
        public object Get(string Code, long UserLoginId)
        {
            object obj = new object();
            try
            {
                //http://merchant.ezeelo.com/uid=12 QRCode Format
                Code = Code.Replace("http://merchant.ezeelo.com/uid=", "");
                UserLogin userLogin =  db.UserLogins.FirstOrDefault(p => p.ID == UserLoginId);
                if(userLogin != null)
                {
                    long Merchantid = Convert.ToInt64(Code);
                    Merchant merchant = db.Merchants.FirstOrDefault(p => p.Id == Merchantid);
                    if(merchant != null)
                    {
                        MerchantClass merchantClass = new MerchantClass();
                        merchantClass.Name = merchant.FranchiseName;
                        merchantClass.City = merchant.CityDetail.Name;
                        merchantClass.Id = merchant.Id;
                        obj = new { Success = 1, Message = "Merchant Found", data = merchantClass };
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Merchant Not Found", data = "" };
                    }
                }
                else
                {
                    obj = new { Success = 0, Message = "User Not Found", data = "" };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
