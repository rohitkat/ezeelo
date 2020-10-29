using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Gandhibagh.Models;
//using Administrator.Models;

namespace Gandhibagh.Controllers
{
    public class WishListController : Controller
    {
        //
        // GET: /WishList/
        //[SessionExpire]
        public ActionResult Index()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();

            List<ProductStockDetailViewModel> listProductStockDetailViewModel = new List<ProductStockDetailViewModel>();
            try
            {
                if (Session["UID"] != null)
                {
                    long lCustID = 0;
                    CustomerWishlist cw = new CustomerWishlist(System.Web.HttpContext.Current.Server);
                    long lFranchiseId = 0; // Added by donali for Get wishlist by FranchiseId on 19-04-2019
                    //Session["UID"] = 1;
                    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                    {
                        string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                        string[] arr = cookieValue.Split('$');
                        lFranchiseId = Convert.ToInt32(arr[2]);////added
                    }
                    long.TryParse(Session["UID"].ToString(), out lCustID);

                    listProductStockDetailViewModel = cw.GetWishlist(lCustID, lFranchiseId);// Added by donali for Get wishlist by FranchiseId on 19-04-2019
                    return View(listProductStockDetailViewModel);
                }
                else
                {
                    //return View()
                    return RedirectToAction("login", "Login", new { callfrom = "normal" });
                }
                // return View(listProductStockDetailViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading Wishlist items!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[WishListController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View(listProductStockDetailViewModel);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading Wishlist items!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[WishListController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View(listProductStockDetailViewModel);
            }
        }

        public JsonResult RemoveFromWishList(string shopStockID)
        {
            System.Threading.Thread.Sleep(1000);
            long lCustID = 0, lShopStockID = 0;
            //Session["UID"] = 1;
            long.TryParse(Session["UID"].ToString(), out lCustID);
            long.TryParse(shopStockID, out lShopStockID);
            CustomerWishlist cw = new CustomerWishlist(System.Web.HttpContext.Current.Server);
            int oprStatus = cw.RemoveFromWishlist(lCustID, lShopStockID);
            return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}