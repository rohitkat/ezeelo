using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class NotificationTestController : ApiController
    {
        [HttpGet]
        [Route("api/NotificationTest/SentNotification")]
        public object SentNotification(int Type,string FCMId)
        {
            object obj = new object();
            try
            {
                string code = "";
                switch (Type)
                {
                    case 1://Placed
                        code = "placed";
                        break;
                    case 2://Delivered
                        code = "delivered";
                        break;
                    case 3://Return
                        code = "return";
                        break;
                    case 4://Cancel
                        code = "cancel";
                        break;
                    case 5://Payout
                        code = "payout";
                        break;
                    case 6://Inactive Points Payout
                        code = "inactive_points_payout";
                        break;
                    case 7://Withdrawn request send
                        code = "withdrawn_send";
                        break;
                    case 8://Withdrawn request accept
                        code = "withdrawn_accept";
                        break;
                    case 9://Withdrawn request approve
                        code = "withdrawn_approve";
                        break;
                    case 10://Withdrawn request cancel
                        code = "withdrawn_cancel";
                        break;
                    case 11://QRP comlete
                        code = "qrp_complete";
                        break;
                    default:
                       return obj = new { Success = 1, Message = "Not a valid code.", data = string.Empty };
                      
                }
                (new BusinessLogicLayer.SendFCMNotification()).DemoSendNotification(code, FCMId);
                obj = new { Success = 1, Message = "Notification Send Successfully.", data = string.Empty };
            }
            catch(Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
