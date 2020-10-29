using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Text;
using Franchise.Models;
using BusinessLogicLayer;
using System.Globalization;
using System.Transactions;
using System.ComponentModel.DataAnnotations;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpire]
    [CustomAuthorize(Roles = "WeeklySeasonalFestivalMessages/CanRead")]
    public class WeeklySeasonalFestivalMessagesController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
     Environment.NewLine
     + "ErrorLog Controller : WeeklySeasonalFestivalMessagesController" + Environment.NewLine);

       

        // GET: /WeeklySeasonalFestivalMessages/

        [CustomAuthorize(Roles = "WeeklySeasonalFestivalMessages/CanRead")]
      
        public async Task<ActionResult> Index()
        {

             Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();
             List<ModelLayer.Models.MessageType> lMessageType = new List<ModelLayer.Models.MessageType>();
             lMessageType = db.MessageTypes.ToList();
             ViewBag.MessagesType = new SelectList(db.MessageTypes, "ID", "MessageType1");
           // var weekly_seasona_festival_message = db.Weekly_Seasona_Festival_Message.Include(w => w.Franchise).Include(w => w.Weekly_Seasona_Festival_Message2);
             var weekly_seasona_festival_message = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == franchiseID);
            return View(await weekly_seasona_festival_message.ToListAsync());
        }
         [SessionExpire]
         [CustomAuthorize(Roles = "WeeklySeasonalFestivalMessages/CanRead")]
        // GET: /WeeklySeasonalFestivalMessages/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekly_Seasona_Festival_Message weekly_seasona_festival_message = await db.Weekly_Seasona_Festival_Message.FindAsync(id);
            if (weekly_seasona_festival_message == null)
            {
                return HttpNotFound();
            }
            return View(weekly_seasona_festival_message);
        }
         [SessionExpire]
         [CustomAuthorize(Roles = "WeeklySeasonalFestivalMessages/CanRead")]
        // GET: /WeeklySeasonalFestivalMessages/Create
        public ActionResult Create()
         {
             try
             {
                 Dictionary<int, string> myDict=new Dictionary<int,string>();
                 for(int i=1; i<=12; i++)
                 myDict.Add(i, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
                 ViewBag.MessagesType = new SelectList(db.MessageTypes, "ID", "MessageType1");
                 ViewBag.SeasonalStartMonth = myDict;
                 Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();
                 return View();
             }
             catch (Exception ex)
             {
                 errStr.Append("Method Name[Http Request] :- Create[HttpGet]" + Environment.NewLine +
                                    "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                        ex.Message.ToString() + Environment.NewLine +
                              "====================================================================================="
                                    );
                 //ViewBag.Message = "Sorry! Problem in customer registration!!";
                 ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                 ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                     , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                 return View();
             }
        }

        // POST: /WeeklySeasonalFestivalMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
         [SessionExpire]
         [CustomAuthorize(Roles = "WeeklySeasonalFestivalMessages/CanRead")]
         public async Task<ActionResult> Create([Bind(Include = "ID,FranchiseID,MessageTypeID,Message,WeeklyHoliday,SeasonalMsgFrmMonth,SeasonalMsgToMonth")] Weekly_Seasona_Festival_Message weekly_seasona_festival_message, int MessagesType, string FestivalMsgFDate, string FestivalMsgTDate, int? txtMinOrderInRupee)
        {
            try
            {
                ViewBag.MessagesType = new SelectList(db.MessageTypes, "ID", "MessageType1");

                weekly_seasona_festival_message.FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                weekly_seasona_festival_message.CreateDate = DateTime.UtcNow.AddHours(5.3);
                weekly_seasona_festival_message.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                weekly_seasona_festival_message.NetworkIP = CommonFunctions.GetClientIP();
                if (FestivalMsgFDate != "" && FestivalMsgTDate != "")
                {
                    weekly_seasona_festival_message.FestivalMsgFrmDate = CommonFunctions.GetProperDate(FestivalMsgFDate);
                    weekly_seasona_festival_message.FestivalMsgToDate = CommonFunctions.GetProperDate(FestivalMsgTDate);
                }
                if (txtMinOrderInRupee >= 50 && txtMinOrderInRupee <= 500)
                {
                    weekly_seasona_festival_message.MinimumOrderInRupee = txtMinOrderInRupee;
                }
                else if (txtMinOrderInRupee != null)
                {
                    TempData["Error"] = "Please Enter Min Order between 50 and 500 Rs.";
                    TempData.Keep();
                    return RedirectToAction("Create");
                }
                if (weekly_seasona_festival_message.Message == null && txtMinOrderInRupee == null)
                {
                    // ModelState.AddModelError("MessagesType", "Please Enter Message");
                    ViewBag.ErrorMessage = "Please Enter Message";
                    TempData["Error"] = "Please Enter Message";
                    TempData.Keep();
                    return RedirectToAction("Create");
                }
                else if (weekly_seasona_festival_message.Message != null && weekly_seasona_festival_message.Message.Length > 100)
                {
                    TempData["Error"] = "Please Enter Message within 100 characters.";
                    TempData.Keep();
                    return RedirectToAction("Create");
                }
                if (MessagesType == null)
                {
                    ModelState.AddModelError("MessagesType", "Please Select Message Type");
                    ViewBag.ErrorMessage = "Please Select Message Type";
                    return View();
                }
                weekly_seasona_festival_message.MessageTypeID = Convert.ToInt32(MessagesType);
                if (ModelState.IsValid)
                {
                            db.Weekly_Seasona_Festival_Message.Add(weekly_seasona_festival_message);

                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                    }
                return View(weekly_seasona_festival_message);
            }

            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                           ex.Message.ToString() + Environment.NewLine +
                 "====================================================================================="
                       );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);


                return View();

            }
        }

        // GET: /WeeklySeasonalFestivalMessages/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
             try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekly_Seasona_Festival_Message weekly_seasona_festival_message = await db.Weekly_Seasona_Festival_Message.FindAsync(id);

            MessageType lMessageType = db.MessageTypes.Find(weekly_seasona_festival_message.MessageTypeID);
            lMessageType.MessageType1 = db.MessageTypes.Where(x => x.ID == weekly_seasona_festival_message.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault();

            ViewBag.MessagesType = new SelectList(db.MessageTypes.ToList(), "ID", "MessageType1", lMessageType.ID);

            ViewBag.FestivalMsgFDate = weekly_seasona_festival_message.FestivalMsgFrmDate;
            ViewBag.FestivalMsgTDate = weekly_seasona_festival_message.FestivalMsgToDate;
            ViewBag.txtMinOrderInRupee = weekly_seasona_festival_message.MinimumOrderInRupee;
            Dictionary<int, string> myDict = new Dictionary<int, string>();
            for (int i = 1; i <= 12; i++)
                myDict.Add(i, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
            ViewBag.SeasonalStartMonth = myDict;

            if (weekly_seasona_festival_message == null)
            {
                return HttpNotFound();
            }

            return View(weekly_seasona_festival_message);
            }

             catch (Exception ex)
             {
                 errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                        "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                            ex.Message.ToString() + Environment.NewLine +
                  "====================================================================================="
                        );
                 //ViewBag.Message = "Sorry! Problem in customer registration!!";
                 ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                 ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                     , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);


                 return View();

             }
        }

        // POST: /WeeklySeasonalFestivalMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,FranchiseID,MessageTypeID,Message,WeeklyHoliday,SeasonalMsgFrmMonth,SeasonalMsgToMonth,FestivalMsgFrmDate,FestivalMsgToDate,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Weekly_Seasona_Festival_Message weekly_seasona_festival_message, int MessagesType, string FestivalMsgFDate, string FestivalMsgTDate, int? txtMinOrderInRupee)
        {
            ViewBag.MessagesType = new SelectList(db.MessageTypes, "ID", "MessageType1");

            weekly_seasona_festival_message.FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            weekly_seasona_festival_message.ModifyDate = DateTime.UtcNow.AddHours(5.3);
            weekly_seasona_festival_message.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            weekly_seasona_festival_message.NetworkIP = CommonFunctions.GetClientIP();
            if (FestivalMsgFDate != "" && FestivalMsgTDate != "")
            {
                weekly_seasona_festival_message.FestivalMsgFrmDate = CommonFunctions.GetProperDate(FestivalMsgFDate);
                weekly_seasona_festival_message.FestivalMsgToDate = CommonFunctions.GetProperDate(FestivalMsgTDate);
            }
            if (txtMinOrderInRupee >= 50 && txtMinOrderInRupee<=500)
            {
                weekly_seasona_festival_message.MinimumOrderInRupee = txtMinOrderInRupee;
            }
            else if (txtMinOrderInRupee!=null)
            {
                TempData["Error"] = "Please Enter Min Order between 50 and 500 Rs.";
                return RedirectToAction("Edit");
            }
            if (weekly_seasona_festival_message.Message == null && txtMinOrderInRupee==null)
            {
               // ModelState.AddModelError("MessagesType", "Please Enter Message");
                ViewBag.ErrorMessage = "Please Enter Message";
                TempData["Error"] = "Please Enter Message";
                TempData.Keep();
                return RedirectToAction("Edit");
            }
            else if (weekly_seasona_festival_message.Message != null && weekly_seasona_festival_message.Message.Length > 100)
            {
                TempData["Error"] = "Please Enter Message within 100 characters.";
               // TempData.Keep();
                return RedirectToAction("Edit");
            }
            if (MessagesType == null)
            {
                ModelState.AddModelError("MessagesType", "Please Select Message Type");
                ViewBag.ErrorMessage = "Please Select Message Type";
                return RedirectToAction("Edit");
            }
            weekly_seasona_festival_message.MessageTypeID = Convert.ToInt32(MessagesType);
            if (ModelState.IsValid)
            {
                db.Entry(weekly_seasona_festival_message).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weekly_seasona_festival_message);
        }

        // GET: /WeeklySeasonalFestivalMessages/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Weekly_Seasona_Festival_Message weekly_seasona_festival_message = await db.Weekly_Seasona_Festival_Message.FindAsync(id);
            if (weekly_seasona_festival_message == null)
            {
                return HttpNotFound();
            }
            return View(weekly_seasona_festival_message);
        }

        // POST: /WeeklySeasonalFestivalMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Weekly_Seasona_Festival_Message weekly_seasona_festival_message = await db.Weekly_Seasona_Festival_Message.FindAsync(id);
            db.Weekly_Seasona_Festival_Message.Remove(weekly_seasona_festival_message);
            await db.SaveChangesAsync();
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

        public JsonResult MessageTypeList(int MessageTypeID)
        {
            List<Weekly_Seasona_Festival_Message> lst = new List<Weekly_Seasona_Festival_Message>();
            lst = GetMessageList(MessageTypeID);
            return Json(lst, JsonRequestBehavior.AllowGet);

        }
        public class MessageList
        {
            public string Message { get; set; }
            public string MessageType { get; set; }
            public string WeeklyHoliday { get; set; }
            public string SeasonalMsgFrmMonth { get; set; }
            public string SeasonalMsgToMonth { get; set; }
            public DateTime? FestivalMsgFrmDate { get; set; }
            public DateTime? FestivalMsgToDate { get; set; }
            public bool IsActive { get; set; }
        }
        public List<Weekly_Seasona_Festival_Message> GetMessageList(int MessageTypeID)
        {
            List<Weekly_Seasona_Festival_Message> msgLst = new List<Weekly_Seasona_Festival_Message>();
            if (MessageTypeID == 0)
            {
                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
                var data = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == franchiseID).ToList();
                foreach (var item in data)
                {
                    Weekly_Seasona_Festival_Message dd = new Weekly_Seasona_Festival_Message();
                    dd.Message = item.Message;
                    dd.MessageTypeID = item.MessageTypeID;
                   // dd.MessageType = db.MessageTypes.Where(x=>x.ID== item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.WeeklyHoliday = item.WeeklyHoliday==null?"":item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate = item.FestivalMsgFrmDate;
                    dd.FestivalMsgToDate = item.FestivalMsgToDate;
                    dd.IsActive = item.IsActive;
                   // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            else if (MessageTypeID<1)
            {
                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
                var data = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == franchiseID ).ToList();
                foreach (var item in data)
                {
                    Weekly_Seasona_Festival_Message dd = new Weekly_Seasona_Festival_Message();
                    dd.Message = item.Message;
                    dd.MessageTypeID = item.MessageTypeID;
                    //dd.MessageType = db.MessageTypes.Where(x => x.ID == item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.WeeklyHoliday = item.WeeklyHoliday == null ? "" : item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate = item.FestivalMsgFrmDate;
                    dd.FestivalMsgToDate = item.FestivalMsgToDate;
                    dd.IsActive = item.IsActive;
                   // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            else
            {
                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
                var data = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == franchiseID && x.MessageTypeID == MessageTypeID).ToList();
                foreach (var item in data)
                {
                    Weekly_Seasona_Festival_Message dd = new Weekly_Seasona_Festival_Message();
                    dd.Message = item.Message;
                    dd.MessageTypeID = item.MessageTypeID;
                    //dd.MessageType = db.MessageTypes.Where(x => x.ID == item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.WeeklyHoliday = item.WeeklyHoliday == null ? "" : item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate = item.FestivalMsgFrmDate;
                    dd.FestivalMsgToDate = item.FestivalMsgToDate;
                    dd.IsActive = item.IsActive;
                    // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            return msgLst;

        }



    }
}
