using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Data;
using System.Web.Configuration;

namespace Inventory.Controllers
{
    public class CategoryHomeController : Controller
    {


        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: CategoryHome
        public ActionResult Index( )
        {

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            long franchiseId = 0;
            if(Session["FRANCHISE_ID"]!=null)
            {
                franchiseId = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID).Select(y => y.FranchiseID).FirstOrDefault();
            }
            long ShopID = 0;
            if (Session["ShopID"]!=null)
            {
                 ShopID = db.Shops.Where(x => x.FranchiseID == franchiseId).Select(s => s.ID).FirstOrDefault();
            }
                string WarehouseName = db.Warehouses.Where(x => x.ID == WarehouseID && x.IsActive == true).Select(x => x.Name).FirstOrDefault();
            Session["WarehouseName"] = WarehouseName;
            Session["Category"] = "IsCategory";




       
          
            Session["FRANCHISE_ID"] = franchiseId;

            Session["ShopID"] = ShopID;


            Warehouse model = new Warehouse();
          
            //model.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            //ViewBag.WarehouseList = db.Warehouses.ToList();
            return RedirectToAction("Home", "CategoryHome");
        }



        public ActionResult Home()
        {
            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.DVList = db.Warehouses.Where(x => x.Entity == "DV").ToList();
            ViewBag.FVList = db.Warehouses.Where(x => x.Entity == "FV").ToList();
      
            //model.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(int result)
        {
            //infiAgent.Models.User User = (infiAgent.Models.User)Session["USER_NAME"];
            //if (User.UserCode == null || User.UserCode == string.Empty)
            //{
            //    //return Content("<script language='javascript' type='text/javascript'>'http://www.google.co.uk'</script>");
            //    return JavaScript("window.location.href = '/Cust/OrderSummary'");//'" + Url.Action("OrderSummary", "Cust") + "'"
            //}
            Warehouse model = new Warehouse();
            Session["WarehouseID"] = result;
          

            long franchiseId = 0;
            if (Session["FRANCHISE_ID"] != null)
            {
                franchiseId = db.WarehouseFranchises.Where(x => x.WarehouseID == result).Select(y => y.FranchiseID).FirstOrDefault();
            }
            long ShopID = 0;
            if (Session["ShopID"] != null)
            {
                ShopID = db.Shops.Where(x => x.FranchiseID == franchiseId).Select(s => s.ID).FirstOrDefault();
            }
          
            Session["FRANCHISE_ID"] = franchiseId;

            Session["ShopID"] = ShopID;

            string WarehouseName = db.Warehouses.Where(x => x.ID== result && x.IsActive == true).Select(x => x.Name).FirstOrDefault();
            Session["WarehouseName"] = WarehouseName;
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }


        
//        @if(SignInManager.IsSignedIn(User))
//        {
//            // normal stuff
//        } 
//else if (!Context.Request.Path.ToString().Contains("/About")) 
//{
//    // If we aren't processing a request for the target page, 
//    // then redirect to it.
//    Context.Response.Redirect("/About");
//}


}
}