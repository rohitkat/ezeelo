using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Administrator.Models;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class DesignBlockTypeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /DesignBlockType/
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanRead")]
        public ActionResult Index()
        {
            return View(db.DesignBlockTypes.ToList().OrderBy(x=>x.Name));
        }

        // GET: /DesignBlockType/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanRead")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignBlockType designblocktype = db.DesignBlockTypes.Find(id);
            if (designblocktype == null)
            {
                return HttpNotFound();
            }
            return View(designblocktype);
        }

        // GET: /DesignBlockType/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanRead")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /DesignBlockType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,ImageWidth,ImageHeight,MaxLimit,IsActive")] DesignBlockType designblocktype)
        {
            designblocktype.CreateDate = DateTime.UtcNow.AddHours(5.30);
            designblocktype.CreatedBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            designblocktype.DeviceID = "x";
            designblocktype.DeviceType = "Net Browser";
            designblocktype.NetworkIP = CommonFunctions.GetClientIP();

            if (ModelState.IsValid)
            {
                db.DesignBlockTypes.Add(designblocktype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(designblocktype);
        }

        // GET: /DesignBlockType/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanRead")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesignBlockType designblocktype = db.DesignBlockTypes.Find(id);
            if (designblocktype == null)
            {
                return HttpNotFound();
            }
            return View(designblocktype);
        }

        // POST: /DesignBlockType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DesignBlockType/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,ImageWidth,ImageHeight,MaxLimit,IsActive")] DesignBlockType designblocktype)
        {
            DesignBlockType lDesignBlockType = db.DesignBlockTypes.Find(designblocktype.ID);
            if (lDesignBlockType == null)
            {
                return View("Error");
            }
            designblocktype.CreateDate = Convert.ToDateTime(lDesignBlockType.CreateDate);
            designblocktype.CreatedBy = Convert.ToInt64(lDesignBlockType.CreatedBy);
            designblocktype.ModifyDate = DateTime.UtcNow.AddHours(5.30);
            designblocktype.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

            if (ModelState.IsValid)
            {
                db.Entry(lDesignBlockType).CurrentValues.SetValues(designblocktype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(designblocktype);
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
