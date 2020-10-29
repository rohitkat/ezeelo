using ModelLayer.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class GetFCMUserDataController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();

        [HttpGet]
        [Route("api/GetFCMUserData/getData")]
        public object getData(long UserLoginId, string FCMRegistartionId, string DeviceId, string DeviceType)
        {
            object obj = new object();
            try
            {
                if (!db.UserLogins.Any(p => p.ID == UserLoginId))
                {
                    obj = new { Success = 0, Message = "Not valid user login id." };
                }
                else
                {
                    FCMUser objFCMuser = db.FCMUsers.FirstOrDefault(p => p.UserLoginId == UserLoginId && p.FCMRegistartionId == FCMRegistartionId);
                    if (objFCMuser == null)
                    {
                        FCMUser objFCMuser_ = db.FCMUsers.FirstOrDefault(p => p.UserLoginId == UserLoginId);
                        if (objFCMuser_ != null)
                        {
                            //Update
                            objFCMuser_.FCMRegistartionId = FCMRegistartionId;
                            objFCMuser_.DeviceId = DeviceId;
                            objFCMuser_.DeviceType = DeviceType;
                            objFCMuser_.ModifiedDate = DateTime.Now;
                            objFCMuser_.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            db.SaveChanges();
                        }
                        else
                        {
                            //Insert
                            FCMUser user = new FCMUser();
                            user.UserLoginId = UserLoginId;
                            user.FCMRegistartionId = FCMRegistartionId;
                            user.DeviceId = DeviceId;
                            user.DeviceType = DeviceType;
                            user.CreateDate = DateTime.Now;
                            user.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            db.FCMUsers.Add(user);
                            db.SaveChanges();
                        }
                    }
                    obj = new { Success = 1, Message = "Successfull." };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message };
            }
            return obj;
        }
    }
}
