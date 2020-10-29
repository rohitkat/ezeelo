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
using Administrator.Models;
using System.Text;

namespace Administrator.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpire]
    //[CustomAuthorize(Roles = "WeeklyHolidayAndMinOrderMessages/CanRead")]
    public class WeeklyHolidayAndMinOrderMessagesController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
     Environment.NewLine
     + "ErrorLog Controller : WeeklyHolidayAndMinOrderMessagesController" + Environment.NewLine);



        // GET: /WeeklySeasonalFestivalMessages/

       // [CustomAuthorize(Roles = "WeeklyHolidayAndMinOrderMessages/CanRead")]

        // GET: /WeeklyHolidayAndMinOrderMessages/
        public async Task<ActionResult> Index()
        {
            //var weekly_seasona_festival_message = db.Weekly_Seasona_Festival_Message.Include(w => w.Franchise).Include(w => w.MessageType).Include(w => w.Weekly_Seasona_Festival_Message2);
            var franchise = db.Franchises.Where(x => x.IsActive ==true && x.ID!=1).ToList();
            ViewBag.Franchise = new SelectList(db.Franchises.Where(x => x.IsActive == true && x.ID != 1), "ID", "ContactPerson");
            var weekly_seasona_festival_message = db.Weekly_Seasona_Festival_Message;
            return View(await weekly_seasona_festival_message.ToListAsync());
        }

        // GET: /WeeklyHolidayAndMinOrderMessages/Details/5
      /*  public async Task<ActionResult> Details(long? id)
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

        // GET: /WeeklyHolidayAndMinOrderMessages/Create
        public ActionResult Create()
        {
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber");
            ViewBag.MessageTypeID = new SelectList(db.MessageTypes, "ID", "MessageType1");
            ViewBag.MessageTypeID = new SelectList(db.Weekly_Seasona_Festival_Message, "ID", "Message");
            return View();
        }

        // POST: /WeeklyHolidayAndMinOrderMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="ID,FranchiseID,MessageTypeID,Message,MinimumOrderInRupee,WeeklyHoliday,SeasonalMsgFrmMonth,SeasonalMsgToMonth,FestivalMsgFrmDate,FestivalMsgToDate,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Weekly_Seasona_Festival_Message weekly_seasona_festival_message)
        {
            if (ModelState.IsValid)
            {
                db.Weekly_Seasona_Festival_Message.Add(weekly_seasona_festival_message);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", weekly_seasona_festival_message.FranchiseID);
            ViewBag.MessageTypeID = new SelectList(db.MessageTypes, "ID", "MessageType1", weekly_seasona_festival_message.MessageTypeID);
            ViewBag.MessageTypeID = new SelectList(db.Weekly_Seasona_Festival_Message, "ID", "Message", weekly_seasona_festival_message.MessageTypeID);
            return View(weekly_seasona_festival_message);
        }

        // GET: /WeeklyHolidayAndMinOrderMessages/Edit/5
        public async Task<ActionResult> Edit(long? id)
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
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", weekly_seasona_festival_message.FranchiseID);
            ViewBag.MessageTypeID = new SelectList(db.MessageTypes, "ID", "MessageType1", weekly_seasona_festival_message.MessageTypeID);
            ViewBag.MessageTypeID = new SelectList(db.Weekly_Seasona_Festival_Message, "ID", "Message", weekly_seasona_festival_message.MessageTypeID);
            return View(weekly_seasona_festival_message);
        }

        // POST: /WeeklyHolidayAndMinOrderMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="ID,FranchiseID,MessageTypeID,Message,MinimumOrderInRupee,WeeklyHoliday,SeasonalMsgFrmMonth,SeasonalMsgToMonth,FestivalMsgFrmDate,FestivalMsgToDate,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Weekly_Seasona_Festival_Message weekly_seasona_festival_message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weekly_seasona_festival_message).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", weekly_seasona_festival_message.FranchiseID);
            ViewBag.MessageTypeID = new SelectList(db.MessageTypes, "ID", "MessageType1", weekly_seasona_festival_message.MessageTypeID);
            ViewBag.MessageTypeID = new SelectList(db.Weekly_Seasona_Festival_Message, "ID", "Message", weekly_seasona_festival_message.MessageTypeID);
            return View(weekly_seasona_festival_message);
        }

        // GET: /WeeklyHolidayAndMinOrderMessages/Delete/5
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

        // POST: /WeeklyHolidayAndMinOrderMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Weekly_Seasona_Festival_Message weekly_seasona_festival_message = await db.Weekly_Seasona_Festival_Message.FindAsync(id);
            db.Weekly_Seasona_Festival_Message.Remove(weekly_seasona_festival_message);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }*/

        public JsonResult FranchiseList(int FranchiseID)
        {
            List<MessageList> lst = new List<MessageList>();
            lst = GetMessageList(FranchiseID);
            //lst = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);

        }
        public class MessageList
        {
            public string Franchise { get; set; }
            public string Message { get; set; }
            public string MessageType { get; set; }
            public int? MinimumOrderInRupee { get; set; }
            public string WeeklyHoliday { get; set; }
            public string SeasonalMsgFrmMonth { get; set; }
            public string SeasonalMsgToMonth { get; set; }
            public string FestivalMsgFrmDate { get; set; }
            public string FestivalMsgToDate { get; set; }
            public bool IsActive { get; set; }
        }
        public List<MessageList> GetMessageList(int FranchiseID)
        {
            List<ModelLayer.Models.MessageType> lMessageType = new List<ModelLayer.Models.MessageType>();
            lMessageType = db.MessageTypes.ToList();
            List<MessageList> msgLst = new List<MessageList>();
            if (FranchiseID == 0)
            {
               // Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
              
                var data = db.Weekly_Seasona_Festival_Message.ToList();
                foreach (var item in data)
                {
                    string FFDate = item.FestivalMsgFrmDate.ToString();
                    string FTDate = item.FestivalMsgToDate.ToString();
                    MessageList dd = new MessageList();
                    dd.Franchise = item.Franchise.ContactPerson;
                    dd.Message = item.Message;
                    dd.MessageType = lMessageType.FirstOrDefault(x => x.ID == item.MessageTypeID).MessageType1;
                    // dd.MessageType = db.MessageTypes.Where(x=>x.ID== item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.MinimumOrderInRupee = item.MinimumOrderInRupee;
                    dd.WeeklyHoliday = item.WeeklyHoliday == null ? "" : item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate = FFDate != "" ? FFDate.Substring(0, 10) : "";
                    dd.FestivalMsgToDate = FTDate != "" ? FTDate.Substring(0, 10) : "";
                    dd.IsActive = item.IsActive;
                    // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            else if (FranchiseID < 1)
            {
              //  Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
                var data = db.Weekly_Seasona_Festival_Message.ToList();
                foreach (var item in data)
                {
                    string FFDate = item.FestivalMsgFrmDate.ToString();
                    string FTDate = item.FestivalMsgToDate.ToString();
                    MessageList dd = new MessageList();
                    dd.Franchise = item.Franchise.ContactPerson;
                    dd.Message = item.Message;
                    //dd.MessageTypeID = item.MessageTypeID;
                    //dd.MessageType = db.MessageTypes.Where(x => x.ID == item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.MessageType = lMessageType.FirstOrDefault(x => x.ID == item.MessageTypeID).MessageType1;
                    dd.MinimumOrderInRupee = item.MinimumOrderInRupee;
                    dd.WeeklyHoliday = item.WeeklyHoliday == null ? "" : item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate = FFDate != "" ? FFDate.Substring(0, 10) : "";
                    dd.FestivalMsgToDate = FTDate != "" ? FTDate.Substring(0, 10) : "";
                    dd.IsActive = item.IsActive;
                    // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            else
            {
               // Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();  
                var data = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID).ToList();
                foreach (var item in data)
                {
                    string FFDate = item.FestivalMsgFrmDate.ToString();
                       
                    string FTDate = item.FestivalMsgToDate.ToString();
                    MessageList dd = new MessageList();
                    dd.Franchise = item.Franchise.ContactPerson;
                    dd.Message = item.Message;
                   // dd.MessageTypeID = item.MessageTypeID;
                    //dd.MessageType = db.MessageTypes.Where(x => x.ID == item.MessageTypeID).Select(y => y.MessageType1).FirstOrDefault().ToString();
                    dd.MessageType = lMessageType.FirstOrDefault(x => x.ID == item.MessageTypeID).MessageType1;
                    dd.MinimumOrderInRupee = item.MinimumOrderInRupee;
                    dd.WeeklyHoliday = item.WeeklyHoliday == null ? "" : item.WeeklyHoliday;
                    dd.SeasonalMsgFrmMonth = item.SeasonalMsgFrmMonth == null ? "" : item.SeasonalMsgFrmMonth;
                    dd.SeasonalMsgToMonth = item.SeasonalMsgToMonth == null ? "" : item.SeasonalMsgToMonth;
                    dd.FestivalMsgFrmDate =FFDate!=""? FFDate.Substring(0,10):"";
                    dd.FestivalMsgToDate = FTDate != "" ? FTDate.Substring(0,10) : "";
                    dd.IsActive = item.IsActive;
                    // dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    msgLst.Add(dd);
                }
            }
            return msgLst;

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
