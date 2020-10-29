using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Inventory.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;

namespace Inventory.Controllers

{
    public class Warehousezone1Controller : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Warehousezone1/
        public ActionResult Index()
        {

            //ViewBag.Zone = new SelectList(db.WarehouseZones, "ID", "Name");
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            List<WarehouseZoneViewModel> lWarehouseZoneViewModels = new List<WarehouseZoneViewModel>();

           lWarehouseZoneViewModels= (from z in db.Zones join wz in db.WarehouseZones on z.ID equals wz.ZoneID
                                      where wz.WarehouseID==WarehouseID                       
                         select new WarehouseZoneViewModel { 
                         ZoneID = z.ID,
                         Zone = z.Name,
                         WarehouseZoneID = wz.ID,
                         Abbreviation = wz.Name
                         }).ToList();

            return View(lWarehouseZoneViewModels);
             }    
       

        
        public ActionResult Test()
        {
            WarehouseZone OS= new WarehouseZone();
         OS.ZoneList = new SelectList(db.Zones.ToList(), "ID", "Name");
            return View(OS);
        }
        [HttpGet]
        public ActionResult Test1()
        {
            WarehouseZone ol = new WarehouseZone();
            ol.ZoneList = new SelectList(db.Zones.ToList(), "ID", "Name");
            return View(ol);
        }
      
        public ActionResult WarehouseZone()
        {
            WarehouseZone oc = new WarehouseZone();
            oc.ZoneList = new SelectList(db.Zones.ToList(), "ID", "Name");
            return View(oc);
        }
         [HttpGet]
        public JsonResult GetZone(int ZoneID)
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var COUNT = db.WarehouseZones.Where(x => x.WarehouseID == WarehouseID && x.ZoneID == ZoneID).Count();
             COUNT=COUNT+1;
             string Abbreviation = db.Zones.Where(x => x.ID == ZoneID).Select(x => x.Abbreviation).FirstOrDefault();

             String ZoneName = Abbreviation + COUNT;
             return Json(ZoneName, JsonRequestBehavior.AllowGet);
               
        }
  
        public ActionResult FirstAjax(long value)
        {
            string ab = db.Zones.Where(p => p.ID ==value).SingleOrDefault().Abbreviation;
           
            return Json(ab, JsonRequestBehavior.AllowGet);
        }   
   
  private void GetData()
  {
    string ab = db.Zones.Where(p => p.ID == 1).SingleOrDefault().Abbreviation;
    }


  
  
        ////////// GET: /Warehousezone1/Details/5
//      public ActionResult Details(long? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Zone zone = db.Zones.Find(id);
//            if (zone == null)
//            {
//                return HttpNotFound();
//            }
//            return View(zone);
        
//}
    //    protected void Page_Load(object sender, EventArgs e)   
    //{  
    //    //if (!Page.IsPostBack)  
    //    //{  
    //    // DropDownList1.DataSource = DropDownList1.SelectedItem.Value;  
    //    // DropDownList1.DataBind();  
    //    //}  
    //}  
   
        // GET: /Warehousezone1/Create
        public ActionResult Create()
        {
         
            if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            {
            }
            else
            {
                if (Session["USER_NAME"] != null)
                {                  
                    
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }

            WarehouseZone OS = new WarehouseZone();
            OS.ZoneList = new SelectList(db.Zones.ToList(), "ID", "Name");
            return View(OS);    
        }

        // POST: /Warehousezone1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(WarehouseZone objZone, string hdnName)
        {           
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                if (ModelState.IsValid)
                {
                    WarehouseZone oc = new WarehouseZone();
                    oc.WarehouseID = WarehouseID;
                    oc.ZoneID = objZone.ZoneID;
                    oc.Name = hdnName;
                    oc.CreateDate = DateTime.UtcNow;
                    oc.CreateBy = GetPersonalDetailID();
                    oc.NetworkIP = CommonFunctions.GetClientIP();
                    db.WarehouseZones.Add(oc);     
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                ViewBag.CollName = db.WarehouseZones;
                return View(WarehouseZone);
            }
          catch(Exception ex)
            {

            }
    {
        return View();
          }
      }


         public long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        private ActionResult View(Func<ActionResult> WarehouseZone)
        {
            throw new NotImplementedException();
        }
        
        // GET: /Warehousezone1/Edit/5
        //public ActionResult Edit(long? id)
        //{      
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Zone zone = db.Zones.Find(id);
        //    if (zone == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(zone);
        //}

        //// POST: /Warehousezone1/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,Warehouse_ID,SHIPPING_RECEIVING,OFFICE,STORAGE,Counter,BAKING_ASSEMBLY,RESTROOM")] Zone zone)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(zone).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(zone);
        //}

        // GET: /Warehousezone1/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zone zone = db.Zones.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            return View(zone);
        }

        // POST: /Warehousezone1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Zone zone = db.Zones.Find(id);
            db.Zones.Remove(zone);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
