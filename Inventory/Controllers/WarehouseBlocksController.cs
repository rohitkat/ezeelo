using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Inventory.Controllers
{   
    public class WarehouseBlocksController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        //
        // GET: /WarehouseBlocks/

        public ActionResult Index()
        {
            long WarehouseID = 0;

            if (Session["USER_NAME"] != null)
            {  }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            Session["WarehouseBlockLocations"] = null;
            Session["BlockLevels"] = null;

            List<WarehouseBlockViewModel> objB = new List<WarehouseBlockViewModel>();
            if(WarehouseID>0)
            {
                objB = (from wb in db.WarehouseBlocks
                        join wz in db.WarehouseZones on wb.WarehouseZoneID equals wz.ID
                        join z in db.Zones on wz.ZoneID equals z.ID
                        where wz.WarehouseID == WarehouseID
                        select new WarehouseBlockViewModel
                        {
                            ID = wb.ID,
                            Name = wb.Name,
                            ZoneName = wz.Name + " (" + z.Name + ")",
                            Rows = wb.Rows,
                            Height = wb.Height,
                            Length = wb.Length,
                            Width = wb.Width,
                            Weight = wb.Weight
                        }).ToList();
            }
                return View("Index",objB);
           
        }

        //
        // GET: /WarehouseBlocks/Details/5

        public ViewResult Details(long id)
        {
            WarehouseBlockViewModel objBlockViewModel = new WarehouseBlockViewModel();
            WarehouseBlock warehouseblock = db.WarehouseBlocks.Single(x => x.ID == id);
            objBlockViewModel.lWarehouseBlock = warehouseblock;
            return View(objBlockViewModel);
        }

        //
        // GET: /WarehouseBlocks/Create

        public ActionResult Create()
        {
            long WarehouseID = 0;
            int count = 0;
            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            
            WarehouseBlockViewModel objBlockViewModel = new WarehouseBlockViewModel();
            //List<WarehouseBlockLocation> objlocation = new List<WarehouseBlockLocation>();
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();

            if (Session["BlockLevels"] != null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];
                count = lWarehouseBlockLevelViewModel.Count();
                lWarehouseBlockLevelViewModel = lWarehouseBlockLevelViewModel.OrderByDescending(x => x.AlphabeteID).ToList();
                Session["BlockLevels"] = lWarehouseBlockLevelViewModel;
            }
            count = Convert.ToInt32(count + 1);
            objBlockViewModel.LevelName = NextLevel(count);
            objBlockViewModel.Columns = 1;
            objBlockViewModel.AlphabeteID = count;
            if (Session["WarehouseZoneID"] != null && Session["NextBlock"]!=null)
            {
                objBlockViewModel.WarehouseZoneID = Convert.ToInt64(Session["WarehouseZoneID"]);
                objBlockViewModel.Name = Session["NextBlock"].ToString();               
            }
           
            ViewBag.PossibleWarehouseZones = db.WarehouseZones.Where(x => x.WarehouseID == WarehouseID);
            objBlockViewModel.Rows = 1;
            return View(objBlockViewModel);
        } 
        
         public string NextLevel(int count)
        {                  
            string NextLevel = string.Empty;         
                  
                var alphabete = GetEnumValue<Inventory.Common.Constants.Inventory_Alphabete>(count); ;
                NextLevel = Convert.ToString(alphabete);
           
            return NextLevel;
         }

         public T GetEnumValue<T>(int intValue) where T : struct, IConvertible
         {
             if (!typeof(T).IsEnum)
             {
                 throw new Exception("T must be an Enumeration type.");
             }
             T val = ((T[])Enum.GetValues(typeof(T)))[0];

             foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
             {
                 if (Convert.ToInt32(enumValue).Equals(intValue))
                 {
                     val = enumValue;
                     break;
                 }
             }
             return val;
         }

         public PartialViewResult AddLevel(int AlphabeteID, string LevelName, int Columns, long WarehouseZoneId, string BlockName)
        {
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();            
            WarehouseBlockLevelViewModel objd = new WarehouseBlockLevelViewModel();
            List<WarehouseBlockLocationViewModel> lWarehouseBlockLocations = new List<WarehouseBlockLocationViewModel>();

            string WarehouseZone = db.WarehouseZones.Where(x => x.ID == WarehouseZoneId).Select(x => x.Name).FirstOrDefault();
            int count = 0;
            if (Session["BlockLevels"] != null && Session["WarehouseBlockLocations"]!=null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];
                count = lWarehouseBlockLevelViewModel.Count();

                lWarehouseBlockLocations = (List<WarehouseBlockLocationViewModel>)Session["WarehouseBlockLocations"];
            }
            //else
            //{
            //    Session["BlockLevels"] = null;
            //    Session["WarehouseBlockLocations"] = null;
            //}

            if (count==40)
            {
                Session["Error"] = "Can't add more than 40 levels.";
                Session["BlockLevels"] = lWarehouseBlockLevelViewModel;
                return PartialView("BlockLevels", lWarehouseBlockLevelViewModel);
            }
                      

            var vAlphabeteID = lWarehouseBlockLevelViewModel.Find(x => x.AlphabeteID == AlphabeteID);

            if (vAlphabeteID == null)
            {
                //Add row/level in WarehouseBlockLevel
                objd.AlphabeteID = AlphabeteID;
                objd.Name = LevelName;
                objd.Columns = Columns;
                lWarehouseBlockLevelViewModel.Add(objd);

                //Add Columns in WarehouseBlockLocation
               for(int i=1; i<=Columns; i++)
               {
                   WarehouseBlockLocationViewModel objLocation = new WarehouseBlockLocationViewModel();
                   objLocation.ColumnID = i;
                   objLocation.AlphabeteID = AlphabeteID;
                   objLocation.WarehouseZoneName = WarehouseZone;
                   objLocation.BlockName = BlockName;
                   objLocation.LevelName = LevelName;
                   objLocation.LocationFullName = WarehouseZone + "-" + BlockName + "-" + LevelName + i;
                   objLocation.LocationShortname = LevelName + i; 

                   lWarehouseBlockLocations.Add(objLocation);
                   lWarehouseBlockLocations = lWarehouseBlockLocations.OrderByDescending(x => x.ColumnID).OrderByDescending(x => x.AlphabeteID).ToList();

                   Session["WarehouseBlockLocations"] = lWarehouseBlockLocations;
               }
            }
            else
            {
                foreach (var item in lWarehouseBlockLevelViewModel)
                {
                    if (item.AlphabeteID == AlphabeteID)
                    {
                        item.Columns = Columns;
                    }
                }               
            }
           

            lWarehouseBlockLevelViewModel = lWarehouseBlockLevelViewModel.OrderByDescending(x => x.AlphabeteID).ToList();
            Session["BlockLevels"] = lWarehouseBlockLevelViewModel;
            return PartialView("BlockLevels", lWarehouseBlockLevelViewModel);
        }
        
        public JsonResult GetNextAlphabete(int AlphabeteID)
        {
            WarehouseBlockViewModel objBlockViewModel = new WarehouseBlockViewModel();
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();
            int count = 0;
            if (Session["BlockLevels"] != null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];
                count = lWarehouseBlockLevelViewModel.Count();
                lWarehouseBlockLevelViewModel = lWarehouseBlockLevelViewModel.OrderByDescending(x => x.AlphabeteID).ToList();
                Session["BlockLevels"] = lWarehouseBlockLevelViewModel;
            }
            count = Convert.ToInt32(count + 1);
            objBlockViewModel.LevelName = NextLevel(count);
            objBlockViewModel.Columns = 1;
            objBlockViewModel.AlphabeteID = count;

            return Json(objBlockViewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditLevel(int AlphabeteID)
        {
            WarehouseBlockViewModel objd = new WarehouseBlockViewModel();
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();

            if (Session["BlockLevels"] != null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];
                var id = lWarehouseBlockLevelViewModel.First(x => x.AlphabeteID == AlphabeteID);
                objd.AlphabeteID = AlphabeteID;
                objd.LevelName = id.Name;
                objd.Columns = id.Columns;
            }                    

            return Json(objd, JsonRequestBehavior.AllowGet);
        }

        
        public PartialViewResult Remove(long AlphabeteID)
        {
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();
            List<WarehouseBlockLocationViewModel> lWarehouseBlockLocations = new List<WarehouseBlockLocationViewModel>();
            
            if (Session["BlockLevels"] != null && Session["WarehouseBlockLocations"]!=null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];               
                var id = lWarehouseBlockLevelViewModel.First(x => x.AlphabeteID == AlphabeteID);

                //Remove level
                lWarehouseBlockLevelViewModel.Remove(id);
                lWarehouseBlockLevelViewModel = lWarehouseBlockLevelViewModel.OrderByDescending(x => x.AlphabeteID).ToList();
                Session["BlockLevels"] = lWarehouseBlockLevelViewModel;

                //Remove locations
                lWarehouseBlockLocations = (List<WarehouseBlockLocationViewModel>)Session["WarehouseBlockLocations"];
                var locations = lWarehouseBlockLocations.Where(x => x.AlphabeteID == AlphabeteID).ToList();
                foreach (var a in locations)
                {
                    lWarehouseBlockLocations.Remove(a);
                }               
                lWarehouseBlockLocations = lWarehouseBlockLocations.OrderByDescending(x => x.AlphabeteID).ToList();
                Session["WarehouseBlockLocations"] = lWarehouseBlockLocations;
            }

            return PartialView("BlockLevels", lWarehouseBlockLevelViewModel);
        }


        //
        // GET: /WarehouseBlocks/GenerageStorageLocations

        public ActionResult GenerageStorageLocations()
        {
            long WarehouseID = 0;
            int count = 0;
            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            WarehouseBlockViewModel objBlockViewModel = new WarehouseBlockViewModel();
            //List<WarehouseBlockLocation> objlocation = new List<WarehouseBlockLocation>();
            List<WarehouseBlockLevelViewModel> lWarehouseBlockLevelViewModel = new List<WarehouseBlockLevelViewModel>();

            if (Session["BlockLevels"] != null)
            {
                lWarehouseBlockLevelViewModel = (List<WarehouseBlockLevelViewModel>)Session["BlockLevels"];
                count = lWarehouseBlockLevelViewModel.Count();
                lWarehouseBlockLevelViewModel = lWarehouseBlockLevelViewModel.OrderByDescending(x => x.AlphabeteID).ToList();
                Session["BlockLevels"] = lWarehouseBlockLevelViewModel;
            }
            count = Convert.ToInt32(count + 1);
            objBlockViewModel.LevelName = NextLevel(count);
            objBlockViewModel.Columns = 1;
            objBlockViewModel.AlphabeteID = count;
            ViewBag.PossibleWarehouseZones = db.WarehouseZones.Where(x => x.WarehouseID == WarehouseID);
            objBlockViewModel.Rows = 1;
            return View(objBlockViewModel);
        } 


        //
        // POST: /WarehouseBlocks/Create

        [HttpPost]
        public ActionResult Create(WarehouseBlockViewModel objBlockViewModel)
        {
            long WarehouseID = 0;

            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            WarehouseBlock warehouseblock = new WarehouseBlock();
            if (ModelState.IsValid)
            {
                db.WarehouseBlocks.Add(warehouseblock);
                db.SaveChanges();

                Session["WarehouseZoneID"] = null;
                Session["NextBlock"] = null;
                return RedirectToAction("Index");  
            }

            ViewBag.PossibleWarehouseZones = db.WarehouseZones.Where(x => x.WarehouseID == WarehouseID);
            return View(objBlockViewModel);
        }


        [HttpPost]
        public JsonResult GetNextBlock(long WarehouseZoneID)
        {
            string NextBlock = string.Empty;
            NextBlock = NextBlockName(WarehouseZoneID);          
            Session["WarehouseZoneID"] = WarehouseZoneID ;
            Session["NextBlock"] = NextBlock;
            return Json(NextBlock, JsonRequestBehavior.AllowGet);
        }

        public string NextBlockName(long warehouseZoneId)
        {
            long WarehouseID = 0;
            string NextBlock = string.Empty;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            if (WarehouseID > 0)
            {
                int count = 0;
                count = (from wb in db.WarehouseBlocks
                             join wz in db.WarehouseZones on wb.WarehouseZoneID equals wz.ID
                             where wz.WarehouseID == WarehouseID //&& wb.WarehouseZoneID == warehouseZoneId
                             select new { wb.ID }).Count();

                NextBlock = "B" + Convert.ToInt32(count + 1);
            }
            return NextBlock;
        }
        
        //
        // GET: /WarehouseBlocks/Edit/5
 
        public ActionResult Edit(long id)
        {
            WarehouseBlock warehouseblock = db.WarehouseBlocks.Single(x => x.ID == id);
            ViewBag.PossibleWarehouseZones = db.WarehouseZones;
            return View(warehouseblock);
        }

        //
        // POST: /WarehouseBlocks/Edit/5

        [HttpPost]
        public ActionResult Edit(WarehouseBlock warehouseblock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouseblock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PossibleWarehouseZones = db.WarehouseZones;
            return View(warehouseblock);
        }

        //
        // GET: /WarehouseBlocks/Delete/5
 
        public ActionResult Delete(long id)
        {
            WarehouseBlock warehouseblock = db.WarehouseBlocks.Single(x => x.ID == id);
            return View(warehouseblock);
        }

        //
        // POST: /WarehouseBlocks/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            WarehouseBlock warehouseblock = db.WarehouseBlocks.Single(x => x.ID == id);
            db.WarehouseBlocks.Remove(warehouseblock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}