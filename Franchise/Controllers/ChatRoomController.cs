using ModelLayer.Models;
using BusinessLogicLayer;
using Franchise.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Franchise.Models.ViewModel;


namespace Franchise.Controllers
{
    public class ChatRoomController : Controller
    {
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
                //Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }
        //
        // GET: /Chat/
        [SessionExpire]
        [CustomAuthorize(Roles = "Chat/CanRead")]
        public ActionResult Index()
        {

            ChatDataList lst = new ChatDataList();
            try
            {
                SessionDetails();
                BusinessLogicLayer.GetDataFromDB obj = new BusinessLogicLayer.GetDataFromDB();
                DataTable dt = new DataTable();
                long id = customerCareSessionViewModel.UserLoginID;
                string code = "";
                string code2 = "";
                DateTime date = new DateTime();
                int mode = 1;
                dt = obj.Call_GetDataFromDB_Procedure(id, code, code2, date, mode, System.Web.HttpContext.Current.Server);

                lst.chatDataListDetail = (from n in dt.AsEnumerable()
                                          select new ChatDataListDetail
                                          {
                                              Name = n.Field<string>("Name")
                                          }).ToList();
                ViewBag.Name = lst.chatDataListDetail.FirstOrDefault().Name;

            }
            catch (Exception ex)
            {

            }
            return View();
        }
	}
}