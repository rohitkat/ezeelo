  using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        [HttpGet]
        public ActionResult Index()
        {
            Session.Abandon();
            
            Session["ID"] = null;
            // return View("Login");
            return PartialView("_LeadersLogin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string callFrom)
        {
            MLMAdminLogin oLogin = new MLMAdminLogin();

           // MLMUser oUser = new MLMUser();
            Session["ID"] = null;
            if (model.UserName == null)
            {
                TempData["Message"] = "Login ID is Required";
                return View("_LeadersLogin", model);
            }
            if (model.Password == null)
            {
                TempData["Message"] = "Password is Required";
                return View("_LeadersLogin", model);
            }
            bool IsEmailValid = false, IsMobileValid = false;

            IsEmailValid = this.IsValidEmailId(model.UserName);

            if (IsEmailValid == false)
                IsMobileValid = this.IsValidMobile(model.UserName);

            if (IsEmailValid == false && IsMobileValid == false)
            {
               
                TempData["Message"] = "Invalid UserName/Password!!";

                return View("_LeadersLogin", model);
            }
            else 
            {
                var obj = db.MLMAdminLogins.Where(h => h.Email.Equals(model.UserName) && h.Password.Equals(model.Password)).FirstOrDefault();
                var objUser = db.UserLogins.Where(h => h.Email.Equals(model.UserName) && h.Password.Equals(model.Password)).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.Role.ToString() == "admin" || obj.Role.ToString() == "superadmin" || obj.Role.ToString() == "leaders") 
                    {
                        Session["ID"] = obj.ID.ToString();
                        Session["Username"] = obj.Name.ToString();
                        Session["RoleName"] = obj.Role.ToString();

                        List<MLMUser> userList = db.MLMUsers.ToList();
                        TempData["UserCount"] = userList.Count();
                        Session["Count"] = userList.Count();

                        EzeeloCustomerOrderController oEzeeloOrder = new EzeeloCustomerOrderController();
                        List<CustomerOrderViewModel> listOrder = oEzeeloOrder.GetEzeeloOrderList().ToList();
                        Session["EzeeloOrderCount"] = listOrder.Count();
                       

                        LeadersCustomerOrderController oLeadersOrder = new LeadersCustomerOrderController();
                       List<LeadersOrderViewModel> listLeadersOrder = oLeadersOrder.GetLeadersOrderList().ToList();
                       Session["LeaderOrderCount"] = listLeadersOrder.Count();

                       EzeeloUsersController oEzeeloUser = new EzeeloUsersController();
                       List<UserLogin> ezeeloUserList = oEzeeloUser.GetEzeeloUsers().ToList();
                       Session["EzeeloUsersCount"] = ezeeloUserList.Count();

                       
                        return RedirectToAction("Index", "Home", new { area="Admin"});
 
                    }
                    else if((obj.Role.ToString() == "accounts"))
                    {
                        Session["ID"] = obj.ID.ToString();
                        Session["Username"] = obj.Name.ToString();
                        Session["RoleName"] = obj.Role.ToString();

                        return RedirectToAction("Index", "AccountSection", new { area = "Admin" });
                    }
                    //else if ((obj.Role.ToString() == "leaders"))
                    //{
                    //    Session["ID"] = obj.ID.ToString();
                    //    Session["Username"] = obj.Name.ToString();
                    //    Session["RoleName"] = obj.Role.ToString();
                    //}

                   
                }

                else if (objUser != null)
                {
                    Session["ID"] = objUser.ID.ToString();
                    Session["UserEmail"] = objUser.Email.ToString();

                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
                return PartialView("_LeadersLogin");
               

            }
           
           
        }
            private bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            //Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(pInputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        private bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address
            Regex regex = new Regex(@"^[7-9]{1}[0-9]{9}$");
            Match match = regex.Match(pInputMobile);
            if (match.Success)
                return true;
            else
                return false;
        }


        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", new { area="Admin"});
        }

   


}

	}
