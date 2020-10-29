using API.Models;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    public class MerchantController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();

        [ApiException]
        [ResponseType(typeof(Bank))]
        public IHttpActionResult Post(Merchant merchant, List<int> Holiday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Merchants.Add(merchant);
            db.SaveChanges();
           
            //add holidays
            foreach (var obj in Holiday)
            {
                MerchantHoliday merchantHoliday = new MerchantHoliday();
                merchantHoliday.MerchantID = merchant.Id;
                merchantHoliday.HolidayID = obj;
                db.MerchantHolidays.Add(merchantHoliday);
            }

            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = merchant.Id }, merchant);
        }
    }
}
