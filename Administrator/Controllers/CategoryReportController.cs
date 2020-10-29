using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class CategoryReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 1000;
        //
        // GET: /CategoryReport/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, int Level,int Status)
        {
            bool IsActive = false;
            if (Status == 1)
            {
                IsActive = true;
            }
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            int TotalCount = 0;
            int TotalPages = 0;
            int pageNumber = page;
            string from = fromDate.ToString();
            string[] f = from.Split('/');
            string[] ftime = f[2].Split(' ');
            DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
            frmd = Convert.ToDateTime(frmd.ToShortDateString());

            // ViewBag.fromDate = frmd;
            string to = toDate.ToString();
            string[] t = to.Split('/');
            string[] ttime = t[2].Split(' ');
            DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
            tod = Convert.ToDateTime(tod.ToShortDateString());
            tod = tod.AddDays(1);
            List<CategoryReportViewModel> category = new List<CategoryReportViewModel>();
            if (Level == 0 && Status==0)
            {
                category = (from c in db.Categories
                            where (c.CreateDate >= frmd && c.CreateDate <= tod)
                            select new CategoryReportViewModel
                            {
                                ID = c.ID,
                                Name = c.Name,
                                Level = c.Level,
                                CreateDate = c.CreateDate,
                                IsActive = c.IsActive
                            }).ToList();

            }
            else if(Level>0 && Status==0)
            {
                category = (from c in db.Categories
                            where (c.CreateDate >= frmd && c.CreateDate <= tod && c.Level==Level)
                            select new CategoryReportViewModel
                            {
                                ID = c.ID,
                                Name = c.Name,
                                Level = c.Level,
                                CreateDate = c.CreateDate,
                                IsActive = c.IsActive
                            }).ToList();
            }
            else if (Level == 0 && Status > 0)
            {
                category = (from c in db.Categories
                            where (c.CreateDate >= frmd && c.CreateDate <= tod && c.IsActive == IsActive)
                            select new CategoryReportViewModel
                            {
                                ID = c.ID,
                                Name = c.Name,
                                Level = c.Level,
                                CreateDate = c.CreateDate,
                                IsActive = c.IsActive
                            }).ToList();
            }
            TotalCount = category.Count();
            ViewBag.TotalCount = TotalCount;
            category = category.OrderByDescending(x => x.CreateDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
            ViewBag.PageSize = category.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            ViewBag.TotalPages = TotalPages;
            return View(category);


        }
        //
        // GET: /CategoryReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /CategoryReport/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CategoryReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CategoryReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /CategoryReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CategoryReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CategoryReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
