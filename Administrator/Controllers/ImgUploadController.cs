using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class ImgUploadController : Controller
    {
        //
        // GET: /ImgUpload/
        //public ActionResult Index()
        //{
        //    return View();
        //}


        public ActionResult View1()
        {
            return View();
        }


        [HttpPost]
        public ActionResult View1(HttpPostedFileBase Files)
        {
            //ImageUploadDemo img = new ImageUploadDemo(System.Web.HttpContext.Current.Server);
            //img.CopyImagesToFTP(string.Empty, string.Empty);
            //CommonFunctions.UploadProductImages(Files, "Dell 1234", 1000, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
            //CommonFunctions.UploadShopImages(Files, 90,ProductUpload.IMAGE_TYPE.NonApproved);
            //CommonFunctions.UploadShopLogo(Files, 90, ProductUpload.IMAGE_TYPE.NonApproved);
            //BusinessLogicLayer.ImageApproval.ProductImagesApproval(90);
            BusinessLogicLayer.ImageApproval.ShopImagesApproval(90);
           // Administrator.Models.AdminCommonFunctions.UploadProductImages(Files, "Dell 1234", 20, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
            return View("View1");
        }
	}
}