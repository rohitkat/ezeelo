using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
namespace Administrator.Controllers
{
    public class TestPlanController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : AdvertisementController" + Environment.NewLine);
        //
        // GET: /TestPlan/
       
        public ActionResult Index(int? page)
        {
            try
            {
                ViewBag.Levelone = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name");

                var categories = db.Categories.Where(c => c.Level == 3);
                return View(categories.ToList().ToPagedList(page ?? 1, 20));

            }

            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
        

        public class CategoryList
        {
            public Int64 ID { get; set; }
            public string Name { get; set; }
            public bool Iselected { get; set; }

        }
	}
}