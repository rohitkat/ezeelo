using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class MerchantNotification
    {
        EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// 
        /// </summary>
        public void SaveNotification(int Status,string MerchantName)
        {
            string Message = "";
            switch (Status)
            {
                case 1:
                    Message = "New shop registered, Shop name is :- " + MerchantName;
                    break;
                case 2:
                    Message = MerchantName + " completed his KYC.";
                    break;
                case 3:
                    Message = MerchantName + " send account recharge request.";
                    break;
                case 4:
                    Message = MerchantName + " send profile update request";
                    break;
                case 5:
                    Message = MerchantName + " send Banner/Shop image update request.";
                    break;
                case 6:
                    Message = MerchantName + "'s account recharge reach its 30% of total recharge amount.";
                    break;
                case 7:
                    Message = MerchantName + "'s account recharge reach its 20% of total recharge amount.";
                    break;
                default:
                    break;
            }
            MerchantNotifications notifications = new MerchantNotifications();
            notifications.CreateDate = DateTime.Now;
            notifications.IsRead = false;
            notifications.Message = Message;
            notifications.Type = Status;
            db.merchantNotifications.Add(notifications);
            db.SaveChanges();
            SendMailOnNotificationEvents(Message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void SendMailOnNotificationEvents(string Message)
        {
            try
            {
                MerchantCommonValues values = db.MerchantCommonValues.FirstOrDefault();
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--MESSAGE-->", Message);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MERCHANT_NOTIFICATION, new string[] { values.AdminEmailID }, dictEmailValues, true);
            }
            catch { }
        }

        /// <summary>
        /// Get all unread notification
        /// </summary>
        /// <returns></returns>
        public List<MerchantNotifications> GetMerchantNotification()
        {
            List<MerchantNotifications> list = new List<MerchantNotifications>();
            list = db.merchantNotifications.Where(p => p.IsRead == false).OrderByDescending(p => p.CreateDate).Take(15).ToList();
            list.ForEach(p => p.Date = p.CreateDate.ToString("dd/MM/yyyy HH:mm"));
            return list;
        }
    }
}
