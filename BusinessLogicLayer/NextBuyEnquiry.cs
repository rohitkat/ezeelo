using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /*
     * Created By : Pradnyakar Badge
     * Create Dage : 21-12-2015
     * Purpose :- For Enqury of Customer for next bye on eZeelo
     *            so that send alert mail for the same
     */
    /// <summary>
    /// Next Buy Enqury from customer
    /// </summary>
    public class NextBuyEnquiry
    {
        
        /// <summary>
        /// Database Connection initialization
        /// </summary>
        protected string server;

        /// <summary>
        /// To Initialized Connection
        /// </summary>
        /// <param name="serverCon">http connection</param>
        public NextBuyEnquiry(string serverCon)
        {
            this.server = serverCon;
        }
        
        /// <summary>
        /// To Insert update in Enqury for next Buy
        /// </summary>
        /// <param name="paramValues"></param>
        /// <param name="opr">Insert / Update</param>
        /// <returns> String Result </returns>
        public string InsertUpdate_nextbuyenquiry(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr)
        {
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.SetData(server.ToString());
            int QryResult = dbOpr.SetRecords("InsertUpdate_nextbuyenquiry", paramValues, opr);

            if (QryResult == 0)
                return "Unable to Perform Operation";
            else if (QryResult == 1)
                return "Thank you!!";// "Request Inserted Successfully";
            else if (QryResult == 2)
                return "Request Updated Successfully";
            else if (QryResult == 4)
                return "Unable to find Request for Updated Successfully";
            else
                return "Operation Fail";
        }
        
        /// <summary>
        /// to Select Record for the Table
        /// </summary>
        public DataTable Select_NextBuyEnquiry(long id)
        {
            DataTable dt = new DataTable();
            //ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(server);
            List<object> paramValues = new List<object>();
            paramValues.Add(id);


            dt = dbOpr.GetRecords("Select_NextBuyEnquiry", paramValues);

            return dt;
        }

        /// <summary>
        /// Schedule to list list
        /// </summary>
        /// <returns>List of NextBuySchedule</returns>
        public List<NextBuySchedule> NextBuyScheduleList()
        {
            List<NextBuySchedule> obj = new List<NextBuySchedule>();
            obj.Add(new NextBuySchedule { id = 0, text = " --- Select --- " });
            obj.Add(new NextBuySchedule { id = 1, text = "Next Day" });
            obj.Add(new NextBuySchedule { id = 2, text = "Day After" });
            obj.Add(new NextBuySchedule { id = 3, text = "Next Week" });
            obj.Add(new NextBuySchedule { id = 4, text = "Week After" });
            obj.Add(new NextBuySchedule { id = 5, text = "After two week" });
            obj.Add(new NextBuySchedule { id = 6, text = "Next Month" });
            obj.Add(new NextBuySchedule { id = 7, text = "After Two Month" });

            return obj;
        }

        /// <summary>
        /// Date for alert according to the selected Schedule
        /// </summary>
        /// <param name="id">selected schedule</param>
        /// <returns>DateTime object</returns>
        public DateTime NextBuySchedule(int id)
        {
            switch (id)
            {
                case 1:
                    return DateTime.UtcNow.AddHours(29.30); //5:30 and 24 hr fro utc date
                case 2:
                    return DateTime.UtcNow.AddDays(2);
                case 3:
                    return DateTime.UtcNow.AddDays(7);
                case 4:
                    return DateTime.UtcNow.AddDays(14);
                case 5:
                    return DateTime.UtcNow.AddDays(28);
                case 6:
                    return DateTime.UtcNow.AddMonths(1);
                case 7:
                    return DateTime.UtcNow.AddMonths(2);
                default:
                    return DateTime.UtcNow.AddDays(1);
            }
        }
    
    }


     public class NextBuySchedule
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}
