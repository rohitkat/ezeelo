using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using Franchise.Models.ViewModel;
using Franchise.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Franchise.Controllers
{
    public class CartController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;
        private  static int PendingCount;
        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
            //if (!Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel))
            //{
            //    if (Session["ID"] != null)
            //    {
            //        Session["ID"] = null;
            //    }
            //    TempData["ServerMsg"] = "You are not CustomerCare Person";
            //    Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidCustomerCare"]);
            //}
        }

        // GET: /Cart/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, string CartID, string User, decimal? Amount,
                                  int? Status, string Stage,
                                  int? page, string SearchString = ""
                                  )        
        {
            //if((string.IsNullOrEmpty(FromDate)) || (string.IsNullOrEmpty(ToDate)))
            //{
            //    FromDate = DateTime.Now.ToString("MM/dd/yy");
            //    ToDate = DateTime.Now.ToString("MM/dd/yy");
            //}
            //DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(DateTime.Now.ToShortDateString());
            //DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(DateTime.Now.ToShortDateString());

            //if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            //{
            //    lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
            //    lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
            //}

            SessionDetails();
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.CartID = CartID;
            ViewBag.User = User;
            ViewBag.Amount = Amount;
            ViewBag.SelectedStatus = Status;
            ViewBag.SelectedStage = Stage;

            List<CartViewModel> lCartViewModels = new List<CartViewModel>();
            List<CartViewModel> lGroupCartViewModelsByCartID = new List<CartViewModel>();

            var StatusList = from ModelLayer.Models.Enum.ORDER_STATUS d in Enum.GetValues(typeof(ModelLayer.Models.Enum.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };
            ViewBag.Status = new SelectList(StatusList.Where(x => x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING ||
                                                         x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED ||
                                                         x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED), "ID", "Name", Status);
            #region code
      
            try
            {
                List<CartViewModel> lCartViewModelLists = new List<CartViewModel>();
                lCartViewModelLists = (from cartvm in db.Carts
                                   join trkCart in db.TrackCarts on cartvm.ID equals trkCart.CartID
                                   join ul in db.UserLogins on cartvm.UserLoginID equals ul.ID
                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                   join shpStk in db.ShopStocks on trkCart.ShopStockID equals shpStk.ID
                                   //where cartvm.MCOID == franchiseID   //-----Commented By Ashwini 13-Jan-2016--------//                                      
                                   select new CartViewModel
                                   {
                                       ID = cartvm.ID,
                                       CartName = cartvm.Name,
                                       UserLoginID = cartvm.UserLoginID,
                                       CustomerName = pd.FirstName,
                                       CustomerMobile = ul.Mobile,
                                       CustomerEmail = ul.Email,
                                       CartAmount = (trkCart.Qty != null) ? (decimal)(shpStk.RetailerRate * trkCart.Qty) : 0,
                                       CartStage = trkCart.Stage,
                                       Status = cartvm.Status,
                                       CartPassword = cartvm.CartPassword,

                                       CustomerOrderID = cartvm.CustomerOrderID,
                                       CityID = cartvm.CityID,
                                       MCOID = cartvm.MCOID,
                                       IsPlacedByCustomer = cartvm.IsPlacedByCustomer,

                                       IsActive = cartvm.IsActive,
                                       CreateDate = cartvm.CreateDate,
                                       CreateBy = cartvm.CreateBy,
                                       ModifyDate = cartvm.ModifyDate,
                                       ModifyBy = cartvm.ModifyBy,
                                       NetworkIP = trkCart.NetworkIP,
                                       DeviceType = trkCart.DeviceType,
                                       DeviceID = trkCart.DeviceID
                                   }).ToList();


                //--------------------Added by Ashwini Meshram 13-Jan-2017 to show Cart Details using franchiseID-----------------------------//
                long lfID = 0;
                foreach (var item in lCartViewModelLists)
                {

                    var lCartNames = item.CartName.Split('C').ToList();
                    lfID = Convert.ToInt64(lCartNames[0]);

                    if(franchiseID==lfID)
                    {
                        lCartViewModels.Add(item);
                    }
                }
                //---------------------------------------------------------------------------------------------------------------------------//

                // lCartViewModels = lCartViewModels.Where(x => x.CartName.Substring(0, 4).Equals(franchiseID.ToString("00000"))).ToList();

                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    //DateTime lFromDate = DateTime.Now;
                    //if (DateTime.TryParse(FromDate, out lFromDate)) { }

                    //DateTime lToDate = DateTime.Now;
                    //if (DateTime.TryParse(ToDate, out lToDate)) { }
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                    lCartViewModels = lCartViewModels.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                }


                lGroupCartViewModelsByCartID = (from cartvm in lCartViewModels
                                                group cartvm by cartvm.ID into grps
                                                select new CartViewModel
                                                {
                                                    ID = grps.Key,
                                                    CartName = grps.FirstOrDefault(x => x.ID == grps.Key).CartName,
                                                    UserLoginID = grps.FirstOrDefault(x => x.ID == grps.Key).UserLoginID,
                                                    CustomerName = grps.FirstOrDefault(x => x.ID == grps.Key).CustomerName,
                                                    CustomerMobile = grps.FirstOrDefault(x => x.ID == grps.Key).CustomerMobile,
                                                    CustomerEmail = grps.FirstOrDefault(x => x.ID == grps.Key).CustomerEmail,
                                                    CartAmount = grps.Where(x => x.ID == grps.Key).Sum(y => y.CartAmount),
                                                    CartStage = grps.FirstOrDefault(x => x.ID == grps.Key).CartStage,
                                                    Status = grps.FirstOrDefault(x => x.ID == grps.Key).Status,

                                                    StatusVal = StatusList.FirstOrDefault(x => x.ID == grps.FirstOrDefault(y => y.ID == grps.Key).Status).Name,

                                                    CartPassword = grps.FirstOrDefault(x => x.ID == grps.Key).CartPassword,

                                                    CustomerOrderID = grps.FirstOrDefault(x => x.ID == grps.Key).CustomerOrderID,
                                                    CityID = grps.FirstOrDefault(x => x.ID == grps.Key).CityID,
                                                    MCOID = grps.FirstOrDefault(x => x.ID == grps.Key).MCOID,
                                                    IsPlacedByCustomer = grps.FirstOrDefault(x => x.ID == grps.Key).IsPlacedByCustomer,

                                                    IsActive = grps.FirstOrDefault(x => x.ID == grps.Key).IsActive,
                                                    CreateDate = grps.FirstOrDefault(x => x.ID == grps.Key).CreateDate,
                                                    CreateBy = grps.FirstOrDefault(x => x.ID == grps.Key).CreateBy,
                                                    ModifyDate = grps.FirstOrDefault(x => x.ID == grps.Key).ModifyDate,
                                                    ModifyBy = grps.FirstOrDefault(x => x.ID == grps.Key).ModifyBy,
                                                    NetworkIP = grps.FirstOrDefault(x => x.ID == grps.Key).NetworkIP,
                                                    DeviceType = grps.FirstOrDefault(x => x.ID == grps.Key).DeviceType,
                                                    DeviceID = grps.FirstOrDefault(x => x.ID == grps.Key).DeviceID,
                                                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            #endregion

            var StageList = (from cartvm in lCartViewModels
                             group cartvm by cartvm.CartStage into grps
                             select new
                             {
                                 Text = grps.Key,
                                 Value = grps.Key
                             }).ToList();
            ViewBag.Stage = new SelectList(StageList, "Text", "Value", Stage);

            #region filter
            if (!string.IsNullOrEmpty(CartID))
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(x => x.CartName.Equals(CartID)).ToList();
            }
            
            if (!string.IsNullOrEmpty(User))
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(
                                                                             x =>( (x.CustomerEmail != null && x.CustomerEmail.Contains(User)) ||
                                                                              ( x.CustomerMobile != null && x.CustomerMobile.Contains(User)) ||
                                                                              ( x.CustomerName != null && x.CustomerName.Contains(User)))
                                                                                 ).ToList();



                //lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(
                //                                                                  x =>( x.CustomerEmail ).Contains(User) ||
                //                                                                  x.CustomerMobile.Contains(User) ||
                //                                                                  x.CustomerName.Contains(User)
                //                                                                 ).ToList();
            }
            if (Amount != null)
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(x => x.CartAmount == Amount).ToList();
            }
            if (Status != null)
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(x => x.Status == Status).ToList();
            }
            if (!string.IsNullOrEmpty(Stage))
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(x => x.CartStage.Equals(Stage)).ToList();
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                lGroupCartViewModelsByCartID = lGroupCartViewModelsByCartID.Where(x => x.Adress.Equals(SearchString)).ToList();
            }
            #endregion
            return View(lGroupCartViewModelsByCartID.OrderByDescending(x => x.ID).ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: /Cart/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: /Cart/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,UserLoginID,Status,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Cart cart)
        {
            cart.IsCouponApply = false;//Added by Sonali on 19-02-2019
            cart.IsWalletApply = false;//Added by Sonali on 19-02-2019
            cart.CouponAmount = 0;//Added by Sonali on 19-02-2019
            cart.CouponCode = string.Empty;//Added by Sonali on 19-02-2019
            cart.WalletUsed = 0;//Added by Sonali on 19-02-2019
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        // GET: /Cart/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: /Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Edit([Bind(Include = "ID,Name,UserLoginID,Status,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Cart cart)
        {
            cart.IsCouponApply = false;//Added by Sonali on 19-02-2019
            cart.IsWalletApply = false;//Added by Sonali on 19-02-2019
            cart.CouponAmount = 0;//Added by Sonali on 19-02-2019
            cart.CouponCode = string.Empty;//Added by Sonali on 19-02-2019
            cart.WalletUsed = 0;//Added by Sonali on 19-02-2019
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cart);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "TrackCartReport/CanRead")]
        public ActionResult TrackCart(long CartID)
        {
            List<CartViewModel> lCartViewModels = new List<CartViewModel>();

            var Status = from ModelLayer.Models.Enum.ORDER_STATUS d in Enum.GetValues(typeof(ModelLayer.Models.Enum.ORDER_STATUS))
                         select new { ID = (int)d, Name = d.ToString() };
            ViewBag.CartID = CartID;

            try
            {
                lCartViewModels = (from cartvm in db.Carts
                                   join trkCart in db.TrackCarts on cartvm.ID equals trkCart.CartID
                                   join ul in db.UserLogins on cartvm.UserLoginID equals ul.ID
                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                   join shpStk in db.ShopStocks on trkCart.ShopStockID equals shpStk.ID
                                   join prdVar in db.ProductVarients on shpStk.ProductVarientID equals prdVar.ID
                                   join co in db.Colors on prdVar.ColorID equals co.ID
                                   join sz in db.Sizes on prdVar.SizeID equals sz.ID
                                   join shpPrd in db.ShopProducts on shpStk.ShopProductID equals shpPrd.ID
                                   join prd in db.Products on shpPrd.ProductID equals prd.ID
                                   where cartvm.ID == CartID
                                   select new CartViewModel
                                   {
                                       ID = cartvm.ID,
                                       CartName = cartvm.Name,
                                       UserLoginID = cartvm.UserLoginID,
                                       CustomerName = pd.FirstName,
                                       CustomerMobile = ul.Mobile,
                                       CustomerEmail = ul.Email,
                                       CartAmount = (trkCart.Qty != null) ? (decimal)(shpStk.RetailerRate * trkCart.Qty) : 0,
                                       CartStage = trkCart.Stage,
                                       Status = cartvm.Status,
                                       CartPassword = cartvm.CartPassword,
                                       IsActive = cartvm.IsActive,
                                       CreateDate = cartvm.CreateDate,
                                       CreateBy = cartvm.CreateBy,
                                       ModifyDate = cartvm.ModifyDate,
                                       ModifyBy = cartvm.ModifyBy,
                                       NetworkIP = trkCart.NetworkIP,
                                       DeviceType = trkCart.DeviceType,
                                       DeviceID = trkCart.DeviceID,


                                       //- Extra Added
                                       ProductName = prd.Name,
                                       ProductSize = sz.Name,
                                       ProductColor = co.Name,
                                       Adress = pd.Address,
                                       Email = ul.Email,
                                       Price = shpStk.MRP,
                                       SaleRate = shpStk.RetailerRate,
                                       LandingPrice = shpStk.WholeSaleRate,
                                       City = trkCart.City,
                                       Qty = trkCart.Qty,

                                       CustomerOrderID = cartvm.CustomerOrderID
                                   }).ToList();

                if (lCartViewModels != null && lCartViewModels.Count > 0 && lCartViewModels.FirstOrDefault().CustomerOrderID != null)
                {
                    ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(lCartViewModels.FirstOrDefault().CustomerOrderID);
                    if (lCustomerOrder != null)
                    {
                        lCartViewModels.ForEach(x => x.OrderCode = lCustomerOrder.OrderCode);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return View(lCartViewModels.ToList());
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public string GetCartPassword(long ID)
        {
            string lCartPassword = "";
            try
            {
                Cart lCart = db.Carts.Find(ID);
                if (lCart == null)
                {
                    throw new Exception("Error: Cart not Found");
                }
                if (lCart.CustomerOrderID != null)
                {
                    ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(lCart.CustomerOrderID);
                    if(lCart.IsPlacedByCustomer == true)
                    {
                        throw new Exception("Error: " + "Customer" + " already placed this Cart with " + lCustomerOrder.OrderCode + ".");
                    }
                    if (lCustomerOrder != null)
                    {
                        throw new Exception("Error: " + lCart.Name + " is already placed with " + lCustomerOrder.OrderCode + ".");
                    }
                }
                if (lCart.IsActive == true)
                {
                    PersonalDetail lPersonalDetail = db.PersonalDetails.FirstOrDefault(x => x.ID == lCart.ModifyBy);
                    if (lPersonalDetail != null)
                    {
                        throw new Exception("Error: " + lPersonalDetail.FirstName + " is processing this Cart.");
                    }
                    else
                    {
                        throw new Exception("Error: Some else is processing this Cart.");
                    }
                }
                UserLogin lUserLogin = null;
                try
                {
                    lUserLogin = db.UserLogins.Find(lCart.UserLoginID);
                    if (lUserLogin == null)
                    {
                        throw new Exception("Error: Invalid Username of Cart.");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error: Getting Username of Cart.");
                }
                lCart.CartPassword = CalculateMD5Hash(GenerateRandomString());
                lCart.IsActive = true;
                lCart.ModifyBy = Convert.ToInt64(Session["PERSONAL_ID"]);
                lCart.ModifyDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Entry(lCart).State = EntityState.Modified;
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(lUserLogin.Email))
                    {
                        return "Success: " + lUserLogin.Email + ":" + lCart.CartPassword;
                    }
                    else if (!string.IsNullOrEmpty(lUserLogin.Mobile))
                    {
                        return "Success: " + lUserLogin.Mobile + ":" + lCart.CartPassword;
                    }
                    else
                    {
                        throw new Exception("Error: Getting Username of Cart.");
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return lCartPassword;
        }

        public string UnlockCart(long ID)
        {
            try
            {
                Cart lCart = db.Carts.Find(ID);
                if (lCart == null)
                {
                    throw new Exception("Error: Cart not Found");
                }
                lCart.IsActive = false;
                lCart.CartPassword = null;
                lCart.ModifyBy = Convert.ToInt64(Session["PERSONAL_ID"]);
                lCart.ModifyDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Entry(lCart).State = EntityState.Modified;
                    db.SaveChanges();
                    return "Success:";
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return "Success:";
        }

        private string GenerateRandomString()
        {
            Random rnd = new Random();
            int month = rnd.Next(1, 13); // creates a number between 1 and 12
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            int card = rnd.Next(52);
            return month.ToString("0") + dice.ToString("0") + card.ToString("0");
        }
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string countPendingCart()
        {
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);

            //--------------------Added by Ashwini Meshram 13-Jan-2017 to show Cart Details using franchiseID-----------------------------//
            List<CartViewModel> lCartViewModelLists = new List<CartViewModel>();
            lCartViewModelLists = (from cartvm in db.Carts
                                   join trkCart in db.TrackCarts on cartvm.ID equals trkCart.CartID
                                   join ul in db.UserLogins on cartvm.UserLoginID equals ul.ID
                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                   join shpStk in db.ShopStocks on trkCart.ShopStockID equals shpStk.ID
                                   where cartvm.Status == (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING
                                   //where cartvm.ID == 90835
                                   select new CartViewModel
                                   {
                                       ID = cartvm.ID,
                                       CartName = cartvm.Name,
                                       Status = cartvm.Status
                                   }).ToList();


            long lfID = 0;
            string lCName=string.Empty;
            List<CartViewModel> lCartViewModels = new List<CartViewModel>();
            foreach (var item in lCartViewModelLists)
            {

                var lCartNames = item.CartName.Split('C').ToList();
                lfID = Convert.ToInt64(lCartNames[0]);

                if (franchiseID == lfID)
                {
                    lCName = lCartNames[0];
                }
            }
            //--------------------Added by Ashwini Meshram 13-Jan-2017 to show Cart Details using franchiseID-----------------------------//
            int lPendingCartCount = db.Carts.Where(x => x.Name.StartsWith(lCName)) //-----------Change Condition by Ashwini for franchiseID Where(x => x.MCOID==franchiseID)==>Where(x => x.Name.StartsWith(lCName)--------//
                                          .Where(x => x.TrackCarts.Count() > 0)
                                            .Count(x => x.Status == (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING);          
            return lPendingCartCount.ToString();
        }

    }
}
