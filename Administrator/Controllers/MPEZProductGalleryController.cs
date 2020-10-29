using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class MPEZProductGalleryController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(MarketPlaceEZProductGallery model, List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1)
        {
            return View();
        }
    }
}