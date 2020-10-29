using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace BusinessLogicLayer
{
    public class RegisterNewUser
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        public int CreateNew_Account(CustomerRegistrationViewModel model)
        {
            /*For Nullable Email Address
                    * Pradnyakar Badge
                    * 06-01-2016
                    */
            if (model.EmailId != null)
            {
                if (model.EmailId.Trim().ToString().Equals(""))
                {
                    model.EmailId = null;
                }
            }

            /*For Nullable Mobile Address
                    * Pradnyakar Badge
                    * 20-01-2016
                    */
            if (model.MobileNo != null)
            {
                if (model.MobileNo.Trim().ToString().Equals(""))
                {
                    model.MobileNo = null;
                }
            }


            model.LastName = "  ";

            //model.MiddleName = string.Empty;
            //UpdateModel(model);


            var lRole = db.Roles.Where(x => x.Name == "Customer".ToUpper()).ToList();
            if (lRole == null)
            {
                return 101;
            }


            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    string lMessage = string.Empty;
                    // Check user details exists or not
                    // lMessage = CommonFunctions.CheckUserDetails(model.EmailId, model.MobileNo);


                    // Creating user login
                    UserLogin userLogin = new UserLogin();

                    userLogin.ID = 0;
                    userLogin.Mobile = model.MobileNo;
                    userLogin.Email = model.EmailId;
                    userLogin.Password = model.Password;
                    userLogin.IsLocked = false;
                    userLogin.CreateBy = 1;
                    userLogin.CreateDate = DateTime.UtcNow.AddHours(5.5);

                    db.UserLogins.Add(userLogin);
                    db.SaveChanges();

                    // Getting current user login id
                    Int64 id = userLogin.ID;

                    // storing personal details
                    PersonalDetail personalDetail = new PersonalDetail();
                    personalDetail.ID = 0;
                    personalDetail.UserLoginID = id;
                    //personalDetail.SalutationID = model.ID;
                    personalDetail.SalutationID = 1;
                    personalDetail.FirstName = model.FirstName;
                    personalDetail.MiddleName = model.MiddleName;
                    personalDetail.LastName = model.LastName;
                    personalDetail.IsActive = true;
                    personalDetail.CreateBy = 1;
                    personalDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);

                    db.PersonalDetails.Add(personalDetail);
                    db.SaveChanges();

                    UserRole userRole = new UserRole();

                    userRole.ID = 0;
                    userRole.RoleID = lRole.FirstOrDefault().ID;
                    userRole.UserLoginID = id;
                    userRole.IsActive = true;
                    userRole.CreateDate = DateTime.Now;
                    userRole.CreateBy = CommonFunctions.GetPersonalDetailsID(id);

                    db.UserRoles.Add(userRole);
                    db.SaveChanges();

                    // Transaction complete
                    ts.Complete();

                    //Tejaswee 28/7/2015
                    //3) Send email, message to customer

                    //SendEmailToCustomer(model.EmailId, model.FirstName);
                    //SendMessageToCustomer(model.MobileNo, model.FirstName);





                  //  HttpContext.Current.Session["UID"] = id;

                    /*To allow null Email 
                     * if email is null then put Mobile no in the session
                     * Prandyakar Badge 05-01-2016                             
                     */
                    //if (model.EmailId == null)
                    //{
                    //    HttpContext.Current.Session["UserName"] = model.MobileNo;
                    //}
                    //else
                    //{
                    //    HttpContext.Current.Session["UserName"] = model.EmailId;
                    //}

                    // Set model to NULL
                    model = null;

                    return 200;
                }
                catch (Exception exception)
                {
                    // Rollback transaction
                    ts.Dispose();
                    return 501;
                }
            }
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
    }
}
