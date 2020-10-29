using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Delivery Schedule Logic
    /// </summary>
    public class DeliveryScheduleBLL
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Data Server Connection 
        /// </summary>
        protected System.Web.HttpServerUtility server;

        /// <summary>
        /// Server Connection Inisialization
        /// </summary>
        /// <param name="s"></param>
        public DeliveryScheduleBLL(System.Web.HttpServerUtility s)
        {
            server = s;
        }

        /// <summary>
        /// Call Stored Procedure
        /// </summary>
        /// <returns>APIDeliveryScheduleViewModel List Object</returns>
        public List<APIDeliveryScheduleViewModel> Select_DeliverySchedule(Int64 CityID, string pincode, int? franchiseId=null)//added int? franchiseId for Multiple MCO/Old API
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            paramValues.Add(franchiseId);////added
            
            paramValues.Add(pincode);
            dt = dbOpr.GetRecords("Select_DeliverySchedule", paramValues);

            List<APIDeliveryScheduleViewModel> obj = new List<APIDeliveryScheduleViewModel>();

            obj = (from n in dt.AsEnumerable()
                   select new APIDeliveryScheduleViewModel
                   {
                       ScheduleDate = n.Field<string>("ScheduleDate"),
                       ScheduleId = n.Field<int>("ScheduleID"),
                       timeFrom = n.Field<string>("schuleTimeFrom"),
                       timeTo = n.Field<string>("schuleTimeTo"),
                       ScheduleDisplay = n.Field<string>("ScheduleDisplay")

                   }).ToList();


           

            return obj;
        }



        //================================= Delivery schedule for website ===================================//

        public List<DeliveryScheduleViewModel> SetDeliverySchedule(long cityId, string pincode, int? franchiseId=null)////added int? franchiseId for Multiple MCO/Old API
        {
            List<DeliveryScheduleViewModel> lDeliverySchedule = new List<DeliveryScheduleViewModel>();
            try
            {
                DeliveryScheduleBLL lDeliveryScheduleBLL = new DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);


                List<APIDeliveryScheduleViewModel> lAPIDeliveryScheduleViewModel = lDeliveryScheduleBLL.Select_DeliverySchedule(cityId, pincode, franchiseId);////added franchiseId for Multiple MCO

                foreach (var item in lAPIDeliveryScheduleViewModel)
                {
                    DeliveryScheduleViewModel obj = new DeliveryScheduleViewModel();
                    obj.date = item.ScheduleDisplay;
                    obj.delScheduleId = item.ScheduleId + "$" + item.ScheduleDate;
                    obj.time = item.timeFrom + " - " + item.timeTo;
                    lDeliverySchedule.Add(obj);
                }


            }
            catch (Exception)
            {

                throw;
            }
            return lDeliverySchedule;
        }

        public List<DeliverySchedule1> SetDeliverySchedule()
        {
            try
            {
                
                DateTime date = CheckDate(System.DateTime.Now);

                DateTime nextDate = CheckDate(date.AddDays(1));

                List<DeliverySchedule1> lDeliverySchedule = new List<DeliverySchedule1>();

                long cityId = Convert.ToInt64(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);//added
                var qry = db.DeliverySchedules.Where(x => x.IsActive == true && x.CityID == cityId && x.FranchiseID == franchiseId).Select(x => new { x.DisplayName, x.ID, x.ActualTimeFrom, x.ActualTimeTo });////added  && x.FranchiseID==franchiseId

                foreach (var item in qry)
                {
                    DeliverySchedule1 obj = new DeliverySchedule1();
                    if (date.TimeOfDay < item.ActualTimeFrom)
                    {
                        obj.date = date.ToString("dd/MM/yyyy") + " " + item.DisplayName;
                        obj.time = item.DisplayName;
                        obj.delScheduleId = item.ID + "$" + date.ToString("d");
                        lDeliverySchedule.Add(obj);
                    }
                }
                foreach (var item in qry)
                {
                    DeliverySchedule1 obj = new DeliverySchedule1();
                    obj.date = nextDate.ToString("dd/MM/yyyy") + " " + item.DisplayName;
                    obj.time = item.DisplayName;
                    obj.delScheduleId = item.ID + "$" + nextDate.ToString("d");
                    lDeliverySchedule.Add(obj);
                }
                if (lDeliverySchedule.Count() < 6)
                {
                    DateTime nextDate1 = CheckDate(nextDate.AddDays(1));
                    foreach (var item in qry)
                    {
                        if (lDeliverySchedule.Count() >= 6)
                        {
                            break;
                        }
                        DeliverySchedule1 obj = new DeliverySchedule1();
                        obj.date = nextDate1.ToString("dd/MM/yyyy") + " " + item.DisplayName;
                        obj.time = item.DisplayName;
                        obj.delScheduleId = item.ID + "$" + nextDate1.ToString("d");
                        lDeliverySchedule.Add(obj);

                    }
                }
                return lDeliverySchedule;
                //ViewBag.DeliveryScheduleID = new SelectList(db.DeliverySchedules, "ID", "DisplayName");
               // ViewBag.DeliveryScheduleID = new SelectList(lDeliverySchedule, "delScheduleId", "date");
            }
            catch (Exception)
            {

                throw;
            }
        }

       private DateTime CheckDate(DateTime date)
        {
            DateTime date1 = date;
            bool exists = false;
            try
            {
                exists = db.DeliveryHolidays.Any(t => t.Date == date.Date);
                if (exists == false)
                {
                    date1 = date;
                }
                else
                {
                    DateTime d = date.AddDays(1);
                    date1 = CheckDate(d);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return date1;
        }

    }




    public class DeliverySchedule1
    {
        public string date { get; set; }

        public string time { get; set; }

        public string delScheduleId { get; set; }

    }
}
