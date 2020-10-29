using API.Models;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    public class ChangePasswordController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        // POST api/changepassword
        /// <summary>
        /// Change user login password.
        /// </summary>
        /// <param name="changePassword">Credentials required for change password</param>
        /// <returns>Opreation status.</returns>
        /// --Sonali 07-09-2018 --
        [ValidateModel]
        [TokenVerification]
        [ApiException]
        [ResponseType(typeof(APIResponse))]
        public object Post(ChangePassword changePassword)
        {
            object obj = new object();
            try
            {
                if (changePassword == null)
                    return obj = new { Success = 0, Message = "The value provided for parameter is not in the correct format.", data = string.Empty };
                //  return CreatedAtRoute("DefaultApi", null, new APIResponse { HTTPStatusCode = "400", UserMessage = "The value provided for parameter is not in the correct format." });
                if (changePassword.CustLoginID == null || changePassword.CustLoginID <= 0 || string.IsNullOrEmpty(changePassword.NewPassword) || string.IsNullOrEmpty(changePassword.OldPassword))
                    return obj = new { Success = 0, Message = "The value provided for parameter is not in the correct format.", data = string.Empty };
                if (changePassword.NewPassword.Length < 6)
                    return obj = new { Success = 0, Message = "Password length must be atleast 6 characters long.", data = string.Empty };
                //  return CreatedAtRoute("DefaultApi", null, new APIResponse { HTTPStatusCode = "400", UserMessage = "Password length must be atleast 6 characters long." });

                UserLogin ul = db.UserLogins.Where(x => x.ID == changePassword.CustLoginID && x.Password == changePassword.OldPassword).FirstOrDefault();
                if (ul == null)
                    return obj = new { Success = 1, Message = "Customer with given CustLoginID is not available.", data = string.Empty };
                // return CreatedAtRoute("DefaultApi", null, new APIResponse { HTTPStatusCode = "409", UserMessage = "Customer with given CustLoginID is not available." });

                if (changePassword.NewPassword == ul.Password)
                    return obj = new { Success = 1, Message = "New password cannot be same as old password.", data = string.Empty };
                //return CreatedAtRoute("DefaultApi", null, new APIResponse { HTTPStatusCode = "400", UserMessage = "New password cannot be same as old password." });

                ul.Password = changePassword.NewPassword;
                db.SaveChanges();
                obj = new { Success = 1, Message = "Passwod Changed Successfully.", data = string.Empty };
                // return CreatedAtRoute("DefaultApi", null, new APIResponse { HTTPStatusCode = "200", UserMessage = "Passwod Changed Successfully." });
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


    }
}
