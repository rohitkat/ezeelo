using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class GBTrackController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public void InsertGBTrack(string URL)
        {
            //string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
            //string CurrentURL4 = Request.Url.AbsolutePath;
            //string CurrentURL1 = Request.RawUrl;
            //string CurrentURL2 = Request.Url.OriginalString;
            //string CurrentURL3 = Request.Url.ToString();
            string CurrentURL = URL;
            long UserloginId = Convert.ToInt64(Session["UID"]);
            BusinessLogicLayer.GBTrackBAL.SaveGBTrack(CurrentURL, UserloginId);
        }  
    }
}
