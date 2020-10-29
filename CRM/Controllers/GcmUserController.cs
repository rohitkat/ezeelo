using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using CRM.Models;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using CRM.Models.ViewModel;
using BusinessLogicLayer;
using System.Web.Script.Serialization;

namespace CRM.Controllers
{
    public class GCMMsgDetailViewModel
    {
        //[Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        // [Required(ErrorMessage = "Body is required")]
        public string Body { get; set; }
        // [Required(ErrorMessage = "ImgUrl is required")]
        public string ImgUrl { get; set; }
        //[Required(ErrorMessage = "LevelToID is required")]
        public string LevelToID { get; set; } //- Category / Product
        //[Required(ErrorMessage = "Level is required")]
        public string Level { get; set; }

        //- Added by Ashwini, Date : 21-Oct-2016
        public Nullable<System.DateTime> CreateDate { get; set; }
        [Required(ErrorMessage = "FromDate is required")]
        public Nullable<DateTime> FromDate { get; set; }
        [Required(ErrorMessage = "Todate is required")]
        public Nullable<DateTime> Todate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CityID { get; set; }
    }
    public class GCMNotification
    {
        public List<GcmUser> GcmUsers { get; set; }
        public GCMMsgDetailViewModel gCMMsgDetailViewModel { get; set; }
        public List<CityViewModel> CityViewModels { get; set; }
    }
    public class Level
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class CityViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class TimeSlot
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class GcmUserController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;
        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        [SessionExpire]
        // GET: /GcmUser/
        public ActionResult Index(string FromDate, string ToDate,
                                  int? page, long? CityID, string IMEI = "", string Email = ""

                                  )
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.IMEI = IMEI;
            ViewBag.Email = Email;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            GetDropDowns(CityID);

            var GcmUsers = db.GcmUsers.ToList();
            if (IMEI != "")
            {
                GcmUsers = GcmUsers.Where(x => x.Name == IMEI).ToList();
            }
            if (Email != "")
            {
                GcmUsers = GcmUsers.Where(x => x.EmailID == Email).ToList();
            }
            if (CityID != null)
            {
                GcmUsers = GcmUsers.Where(x => x.City == CityID.ToString()).ToList();
            }
            ViewBag.TotalUsers = GcmUsers.Count;
            return View(GcmUsers.OrderByDescending(x => x.ID).ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(long id)
        {
            ViewBag.FDate = DateTime.Now.ToShortDateString();
            ViewBag.TDate = DateTime.Now.ToShortDateString();
            ViewBag.FromTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            ViewBag.ToTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            SessionDetails();
            GcmUser lGcmUser = db.GcmUsers.Find(id);
            if (lGcmUser == null)
            {
                return View("Error");
            }
            ViewBag.IMEINo = lGcmUser.Name;
            ViewBag.Email = lGcmUser.EmailID;
            ViewBag.CityID = lGcmUser.City;
            List<Level> lLevels = new List<Level>() { 
                                                        new Level { ID = -1, Name = "Select Offer Type"},
                                                        new Level { ID = 0, Name = "Offer on Product"},
                                                        new Level { ID = 1, Name = "Offer on Category"},
                                                        new Level { ID = 2, Name = "Second Level Category"},
                                                        new Level { ID = 3, Name = "Refer & Earn"}
                                                    };
            ViewBag.level = new SelectList(lLevels, "ID", "Name", -1);
            return View();
        }

        // POST: /GcmUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(long id, GCMMsgDetailViewModel gCMMsgDetailViewModel, string FDate, string TDate, string FromTimeSlot, string ToTimeSlot)
        {
            SessionDetails();
            ViewBag.FromTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            ViewBag.ToTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");

            //-for fromdate,todate and create date
            string from = FDate + " " + FromTimeSlot;
            DateTime frmd = CommonFunctions.GetProperDateTime(from);
            gCMMsgDetailViewModel.FromDate = frmd;

            string to = TDate + " " + ToTimeSlot;
            DateTime tod = CommonFunctions.GetProperDateTime(to);
            gCMMsgDetailViewModel.Todate = tod;
            gCMMsgDetailViewModel.CreateDate = DateTime.Now;

            List<Level> lLevels = new List<Level>() { 
                                                        new Level { ID =-1, Name = "Select Offer Type"},
                                                        new Level { ID = 0, Name = "Offer on Product"},
                                                        new Level { ID = 1, Name = "Offer on Category"},
                                                        new Level { ID = 2, Name = "Second Level Category"},
                                                        new Level { ID = 3, Name = "Refer & Earn"}
                                                    };
            ViewBag.level = new SelectList(lLevels, "ID", "Name");
            GcmUser lGcmUser = db.GcmUsers.Find(id);
            if (lGcmUser == null)
            {
                return View("Error");
            }
            ViewBag.IMEINo = lGcmUser.Name;
            ViewBag.Email = lGcmUser.EmailID;
            ViewBag.CityID = lGcmUser.City;

            GCMNotification gCMNotification = new GCMNotification();
            List<GcmUser> lGcmUsers = new List<GcmUser>();
            lGcmUsers.Add(lGcmUser);
            gCMNotification.GcmUsers = lGcmUsers;
            gCMNotification.gCMMsgDetailViewModel = gCMMsgDetailViewModel;
            gCMNotification.gCMMsgDetailViewModel.CityID = lGcmUser.City;

            if (gCMNotification.gCMMsgDetailViewModel.LevelToID == null)
            {
                gCMNotification.gCMMsgDetailViewModel.LevelToID = null;
            }
            int lGBDatabaseNotification = InsertGCMNotification(gCMNotification); //- GBNotification stored in database.

            //// int lGCMNotificationSend = AndroidPush(gCMNotification); //- GCMNotification hide 
            int lGCMNotificationSend = AndroidPushFCM(gCMNotification); //- FCMNotification  added for FCM 

            ViewBag.GCMNotificationSend = lGCMNotificationSend;
            ViewBag.GBDatabaseNotification = lGBDatabaseNotification;
            //gCMMsgDetailViewModel.Title=string.Empty;
            //gCMMsgDetailViewModel.Body = string.Empty;
            // return RedirectToAction("Create");
            return View();
        }
        public ActionResult BulkCreate()
        {
            SessionDetails();
            GetDropDowns(null);
            ViewBag.FDate = DateTime.Now.ToShortDateString();
            ViewBag.TDate = DateTime.Now.ToShortDateString();
            ViewBag.FromTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            ViewBag.ToTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult BulkCreate(GCMMsgDetailViewModel gCMMsgDetailViewModel, long? CityID, string FDate, string TDate, string FromTimeSlot, string ToTimeSlot)
        {
            SessionDetails();
            GetDropDowns(CityID);
            ViewBag.FromTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            ViewBag.ToTimeSlot = new SelectList(GetTimeSlots(), "Value", "Text");
            string lerrormsg = "Please Select City";
            if (CityID == null)
            {
                ViewBag.CityErrorMsg = lerrormsg;
                return View();
            }
            //-for fromdate,todate and create date
            //----------------commented by Ashwini Meshram 11-Jan-2017---------------------//
            // DateTime FromDate = Convert.ToDateTime(FDate + " " + FromTimeSlot);
            //gCMMsgDetailViewModel.FromDate = FromDate;
            // DateTime ToDate = Convert.ToDateTime(TDate + " " + ToTimeSlot);
            // gCMMsgDetailViewModel.Todate = ToDate;
            //---------------------------------------------------------------------------------------//
            //---------------------------------Added by Ashwini 11-Jan-2017--------------------------//
            string from = FDate + " " + FromTimeSlot;
            DateTime frmd = CommonFunctions.GetProperDateTime(from);
            gCMMsgDetailViewModel.FromDate = frmd;

            string to = TDate + " " + ToTimeSlot;
            DateTime tod = CommonFunctions.GetProperDateTime(to);
            gCMMsgDetailViewModel.Todate = tod;
            //----------------------------------------------------------------------------------------//
            gCMMsgDetailViewModel.CreateDate = DateTime.Now;

            GCMNotification gCMNotification = new GCMNotification();
            gCMNotification.gCMMsgDetailViewModel = gCMMsgDetailViewModel;


            List<GcmUser> lGcmUsers = db.GcmUsers.ToList();
            int lGCMNotificationSend = 0; //- By GCM
            int lGBDatabaseNotification = 0; //- In App 

            if (CityID != null)
            {
                lGcmUsers = lGcmUsers.Where(x => x.City == CityID.ToString()).ToList();
            }


            gCMNotification.GcmUsers = lGcmUsers;

            //- For Notification in GBApp that are saved permanantly in database.

            gCMNotification.gCMMsgDetailViewModel.CityID = CityID.ToString();
            if (gCMNotification.gCMMsgDetailViewModel.LevelToID == null)
            {
                gCMNotification.gCMMsgDetailViewModel.LevelToID = null;
            }
            lGBDatabaseNotification = InsertGCMNotification(gCMNotification);

            //- For GCM Notification 500 users at a time.
            for (int i = 0; i <= lGcmUsers.Count; i += 500)
            {
                int j = 500;
                GCMNotification lGCMNotification = new GCMNotification();
                lGCMNotification.gCMMsgDetailViewModel = gCMMsgDetailViewModel;
                lGCMNotification.GcmUsers = lGcmUsers.Skip(i).Take(j).ToList();
                if (lGCMNotification.GcmUsers != null)
                {
                    ////lGCMNotificationSend += AndroidPush(lGCMNotification); //Hide
                    lGCMNotificationSend += AndroidPushFCM(gCMNotification); //Added for FCM
                }
            }
            ViewBag.GCMNotificationSend = lGCMNotificationSend;
            ViewBag.GBDatabaseNotification = lGBDatabaseNotification;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// gcm notification takes max 500 gcmRegIDs, So, pass only less than 500 gcmuser list.
        /// </summary>
        /// <param name="pGCMNotification"></param>
        /// <returns></returns>
        private int AndroidPush(GCMNotification pGCMNotification)
        {
            //return -1;        

            List<string> lRegIds = pGCMNotification.GcmUsers
                // For Testing Purpose. 
                //.Where(x => x.ID == 11009 || x.ID == 22747 || x.ID == 22798 || x.ID == 11499 || x.ID == 11000)
                //.Where(x => x.ID == 11009)
                                                 .Where(x => x.GcmRegID != null)  //--Added by Ashwini 03-Feb-2016 to remove null entry of GCMRegID---//
                                                 .Select(x => x.GcmRegID.ToString()).ToList();
            if (lRegIds == null)
            {
                return 0;
            }
            string lRegdata = "";
            lRegdata = string.Join("\",\"", lRegIds);

            string Status = "Promotion";

            // applicationID means google Api key                                                                                                     
            var applicationID = "AIzaSyDSymqUriO1nwSuUSgawXBXGaMvgsy26zE";  //New ID :AIzaSyBPzzIxd5S31jESsND6fHPvSDAPQP_SsjE//
            // SENDER_ID is nothing but your ProjectID (from API Console- google code)//                                          
            var SENDER_ID = "555896632846";  //Old ID: "555896632846";   //New SenderID : "606476616623";  //
            WebRequest tRequest;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.ContentType = "application/json;charset=UTF-8";
            //tRequest.ContentType = "application/json";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            string postData = "{ \"registration_ids\": [ \"" + lRegdata + "\" ], " +
                                "\"data\": {\"message\":\"" + pGCMNotification.gCMMsgDetailViewModel.Body + "\", " +
                                "\"title\":\"" + pGCMNotification.gCMMsgDetailViewModel.Title + "\", " +
                                "\"imageurl\":\"" + pGCMNotification.gCMMsgDetailViewModel.ImgUrl + "\", " +
                                "\"cat_id\":\"" + pGCMNotification.gCMMsgDetailViewModel.LevelToID + "\", " +
                                "\"level\":\"" + pGCMNotification.gCMMsgDetailViewModel.Level + "\", " +
                                "\"cityid\": \"" + pGCMNotification.GcmUsers.FirstOrDefault().City + "\" ," +
                                 "\"franchiseId\": \"" + pGCMNotification.GcmUsers.FirstOrDefault().MCOID + "\" ," +
                                 "\"Status\": \"" + Status + "\" }}";

            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;
            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tResponse = tRequest.GetResponse();
            dataStream = tResponse.GetResponseStream();
            StreamReader tReader = new StreamReader(dataStream);
            String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.
            string lServerMsg = sResponseFromServer;      //Assigning GCM response to Label text 
            JObject json = JObject.Parse(lServerMsg);
            tReader.Close();
            dataStream.Close();
            tResponse.Close();

            var data = (JObject)JsonConvert.DeserializeObject(lServerMsg);
            string status = data["success"].Value<string>();
            return Convert.ToInt32(status);
        }

        /// <summary>
        /// gcm notification takes max 500 gcmRegIDs, So, pass only less than 500 gcmuser list.
        /// </summary>
        /// <param name="pGCMNotification"></param>
        /// <returns></returns>
       //***********FCM Notification Code****************//
        private int AndroidPushFCM(GCMNotification pGCMNotification)
        {
            List<string> lRegIds = pGCMNotification.GcmUsers
              
                                                .Where(x => x.GcmRegID != null)  //--Added by Ashwini 03-Feb-2016 to remove null entry of GCMRegID---//
                                                .Select(x => x.GcmRegID.ToString()).ToList();
            if (lRegIds == null)
            {
                return 0;
            }
            string lRegdata = "";
            lRegdata = string.Join("\",\"", lRegIds);

            string Status = "Promotion";


            string Response = "1";
            try
            {

                string applicationID = "AAAAluPov4U:APA91bFGwjdn8BMsMSV95hCjIF3_nc3c6J8MC1GJe-8MRt9y7VKU_cZgxMwB24nEJxhsn-kYWyZQNFj_pKUN_DzhQUwUCPWhWWOb6drnKNQn-7jLhgwDqPzO2Spn3WowfPY_REH5WA2b";

                string senderId = "648068775813";

                string deviceId = "eOEBmgMRfg0:APA91bHzwKlih6Wnfqz4156JQl2cfLEY74gk-nkZcI0M4AOcY7GSJJ2DWpgqSY1jSY4madX_PMVqvPbo7o1b9tyeOJ1ZhJQnikLT_ZYaNVL3vArs6fYuz5erc_50IG0l1pRjuTOxKWzN";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data1 = new
                {
                   
                    to = deviceId, // this is for topic 
                   
                    data = new
                    {
                        message = pGCMNotification.gCMMsgDetailViewModel.Body,
                        title = pGCMNotification.gCMMsgDetailViewModel.Title,
                        imageurl = pGCMNotification.gCMMsgDetailViewModel.ImgUrl,
                        cat_id = pGCMNotification.gCMMsgDetailViewModel.LevelToID,
                        level = pGCMNotification.gCMMsgDetailViewModel.Level,
                        cityid = pGCMNotification.GcmUsers.FirstOrDefault().City,
                        franchiseId = pGCMNotification.GcmUsers.FirstOrDefault().MCOID,
                        Status = Status
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data1);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.ContentLength = byteArray.Length;
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));


                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                Response = sResponseFromServer;
                            }
                        }
                    }
                }

              
                //Get response from GCM server.
                string lServerMsg = Response;    //Assigning GCM response to Label text 
                JObject json1 = JObject.Parse(lServerMsg);
                
                var data11 = (JObject)JsonConvert.DeserializeObject(lServerMsg);
                string status = data11["success"].Value<string>();
                return Convert.ToInt32(status);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Convert.ToInt32(Response);
            }

        }
        //***********End Of FCM Notification Code****************//
        public string MyDictionaryToJson(Dictionary<string, string> myDictionary)
        {
            return JsonConvert.SerializeObject(myDictionary);
        }
        public static string ToString(Dictionary<string, string> source)
        {
            var result = string.Join(", ", source.Select(m => m.Key + "=" + m.Value).ToArray());
            return result;
        }
        public void GetDropDowns(long? CityID)
        {
            var gcmCities = (from gcm in db.GcmUsers
                             group gcm by gcm.City
                                 into grps
                                 select new CityViewModel
                                 {
                                     ID = grps.Key
                                 }).ToList();
            var cities = db.Cities.Where(x => x.IsActive == true).ToList();
            foreach (var cty in gcmCities)
            {
                try
                {
                    long lCityID = Convert.ToInt64(cty.ID);
                    cty.Name = cities.FirstOrDefault(x => x.ID == lCityID).Name;
                }
                catch (Exception)
                {
                    if (cty.ID != null)
                    {
                        cty.Name = cty.ID.ToString();
                    }
                }
            }
            ViewBag.CityID = new SelectList(gcmCities.Where(x => x.Name.Trim().Length > 0), "ID", "Name", CityID);

            List<Level> lLevels = new List<Level>() { 
                                                        new Level { ID =-1, Name = "Select Offer Type"},
                                                        new Level { ID = 0, Name = "Offer on Product"},
                                                        new Level { ID = 1, Name = "Offer on Category"},
                                                        new Level { ID = 2, Name = "Second Level Category"},
                                                        new Level { ID = 3, Name = "Refer & Earn"}
                                                    };
            ViewBag.level = new SelectList(lLevels, "ID", "Name", -1);
        }
        private int InsertGCMNotification(GCMNotification pGCMNotification)
        {
            var lGcmUserIDs = pGCMNotification.GcmUsers.Select(x => x.ID.ToString()).ToList();
            DataTable dt = ListToDatatable(lGcmUserIDs);
            long lGcmMsgDetailID = InsertGCMMsgDetail(pGCMNotification);
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("InsertUpdateGCMNotification", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@GcmMessageDetailID", lGcmMsgDetailID);
                SqlParameter lsqlparam = sqlComm.Parameters.AddWithValue("@dt", dt);
                sqlComm.Parameters.Add("@CountInsertedRows", SqlDbType.Int).Direction = ParameterDirection.Output;
                lsqlparam.SqlDbType = SqlDbType.Structured;
                con.Open();
                sqlComm.ExecuteNonQuery();
                int CountRowsInserted = Convert.ToInt32(sqlComm.Parameters["@CountInsertedRows"].Value.ToString());
                con.Close();
                return CountRowsInserted;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        private long InsertGCMMsgDetail(GCMNotification pGCMNotification)
        {
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Insert_Update_GCMMsgDetails", con);
                sqlComm.Parameters.AddWithValue("@Title", SqlDbType.Char).Value = pGCMNotification.gCMMsgDetailViewModel.Title;
                sqlComm.Parameters.AddWithValue("@Body", SqlDbType.Char).Value = pGCMNotification.gCMMsgDetailViewModel.Body;
                sqlComm.Parameters.AddWithValue("@ImgURL", SqlDbType.Char).Value = pGCMNotification.gCMMsgDetailViewModel.ImgUrl;
                sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.CreateDate;
                sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.FromDate;
                sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.Todate;
                sqlComm.Parameters.AddWithValue("@CityID", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.CityID;
                sqlComm.Parameters.AddWithValue("@OfferTypeID", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.Level;
                sqlComm.Parameters.AddWithValue("@ProdIDCatID", SqlDbType.DateTime2).Value = pGCMNotification.gCMMsgDetailViewModel.LevelToID;
                sqlComm.Parameters.AddWithValue("@IsActive", true);
                sqlComm.Parameters.Add("@ID", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                con.Open();
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.ExecuteNonQuery();
                int ID = Convert.ToInt32(sqlComm.Parameters["@ID"].Value.ToString());
                con.Close();
                return ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DataTable ListToDatatable(List<string> str)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("GCMUserID");
            foreach (var s in str)
            {
                DataRow dr = dt.NewRow();
                dr["GCMUserID"] = s;
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static List<TimeSlot> GetTimeSlots()
        {
            List<TimeSlot> lTimeSlots = new List<TimeSlot>();
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j <= 45; j += 15)
                {
                    TimeSlot lTimeSlot = new TimeSlot();
                    if (i < 12)
                    {
                        lTimeSlot.Text = i.ToString("00") + ":" + j.ToString("00") + " AM";
                    }
                    else if (i == 12)
                    {
                        lTimeSlot.Text = "12" + ":" + j.ToString("00") + "AM";
                    }
                    else
                    {
                        lTimeSlot.Text = (i - 12).ToString("00") + ":" + j.ToString("00") + " PM";
                    }
                    lTimeSlot.Value = i.ToString("00") + ":" + j.ToString("00");
                    lTimeSlots.Add(lTimeSlot);
                }
            }
            return lTimeSlots;
        }
    }
}


