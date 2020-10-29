using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ChatController : Controller
    {
        //
        // GET: /Chat/
        public ActionResult Index()
        {
            return View();
        }


        #region -----GENERAL METHOD -----

        public bool CheckChatUserRegistration1(string mobile, string email)
        {
           // long SessionID = (long)Session["UID"];

            try
            {
                //SessionDetails();
                BusinessLogicLayer.GetDataFromDB obj = new BusinessLogicLayer.GetDataFromDB();
                DataTable dt = new DataTable();
                long id = 0;// customerCareSessionViewModel.UserLoginID;
                string code = mobile;
                string code2 = email;
                DateTime date = new DateTime();
                int mode = 2;
               // bool exist;
                dt = obj.Call_GetDataFromDB_Procedure(id, code, code2, date, mode, System.Web.HttpContext.Current.Server);
                if (dt.Rows.Count > 0)
                {
                    string str = dt.Rows[0].ItemArray[0].ToString();
                    if(str=="True")
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }   


            }
            catch (Exception ex)
            {

            }
            return false;
            
        }
        public JsonResult CheckChatUserRegistration(string mobile, string email)
        {
           // long SessionID = (long)Session["UID"];

                ChatUserName chatusername = new ChatUserName();
                BusinessLogicLayer.GetDataFromDB obj = new BusinessLogicLayer.GetDataFromDB();
                DataTable dt = new DataTable();
                long id = 0;// customerCareSessionViewModel.UserLoginID;
                string code = mobile;
                string code2 = email;
                DateTime date = new DateTime();
                int mode = 2;
                // bool exist;
                dt = obj.Call_GetDataFromDB_Procedure(id, code, code2, date, mode, System.Web.HttpContext.Current.Server);
                if (dt.Rows.Count > 0)
                {
                   
                    string str = dt.Rows[0].ItemArray[0].ToString();
                    if (str == "True")
                    {
                        chatusername.Name = dt.Rows[0][1].ToString();
                        chatusername.UserLoginId = dt.Rows[0][2].ToString();
                        // return true;
                    }
                    else
                        chatusername.Name = "False";
                       // return false;
                }
                else
                {
                    chatusername.Name = "False";
                   // return false;
                }
                return Json(chatusername, JsonRequestBehavior.AllowGet);

            
        }
        public JsonResult RegisterChatUser(string chatname,string mobile,string email)
        {
            ChatUserName chatusername = new ChatUserName();
            RegisterNewUser regUser = new RegisterNewUser();
            CustomerRegistrationViewModel model = new CustomerRegistrationViewModel();
            model.MobileNo = mobile;
            model.EmailId = email;
            model.Password = CreatePassword(10);
            model.FirstName = chatname;
            regUser.CreateNew_Account(model);
            //-------------------------------


            BusinessLogicLayer.GetDataFromDB obj = new BusinessLogicLayer.GetDataFromDB();
            DataTable dt = new DataTable();
            long id = 0;// customerCareSessionViewModel.UserLoginID;
            string code = mobile;
            string code2 = email;
            DateTime date = new DateTime();
            int mode = 2;
            // bool exist;
            dt = obj.Call_GetDataFromDB_Procedure(id, code, code2, date, mode, System.Web.HttpContext.Current.Server);
            if (dt.Rows.Count > 0)
            {

                string str = dt.Rows[0].ItemArray[0].ToString();
                if (str == "True")
                {
                    chatusername.Name = dt.Rows[0][1].ToString();
                    chatusername.UserLoginId = dt.Rows[0][2].ToString();
                    // return true;
                }
                else
                    chatusername.Name = "False";
                // return false;
            }
            else
            {
                chatusername.Name = "False";
                // return false;
            }

            return Json(chatusername, JsonRequestBehavior.AllowGet);
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        #endregion

        public class ChatUserName
        {
            public string Name { get; set; }
            public string UserLoginId { get; set; }
        }
	}
}