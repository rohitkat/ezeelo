using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ApplicationVersionController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        // GET api/applicationversion
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/applicationversion/5
        /// <summary>
        /// Provide latest version of App, published in App Stored in mobile
        /// </summary>
        /// <returns></returns>
        //[ApiException]
        //public object Get()
        //{
        //    List<AppVersionViewModel> VersionName = new List<AppVersionViewModel>();

        //    VersionName = GetLatestAppVersion.GetAppVersion();

        //    object obj = new object();
        //    if (VersionName.Count() > 0)
        //    {
        //        obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = VersionName };
        //    }
        //    else if (VersionName.Count() == 0)
        //    {
        //        obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
        //    }
        //    else
        //    {
        //        obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
        //    }


        //    return obj;
        //}

        [ApiException]
        public object Get()
        {
            object obj = new object();
            try
            {
                ApplicationVersion objAppVersion = db.ApplicationVersion.OrderByDescending(x => x.Id).FirstOrDefault();
                if (objAppVersion != null)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = objAppVersion };
                }
                else
                    obj = new { Success = 0, Message = "Record not found.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [HttpPost]
        public object Post(AppVersionViewModel appVersionViewModel)
        {
            object obj = new object();
            try
            {
                if (appVersionViewModel == null || string.IsNullOrEmpty(appVersionViewModel.VersionCode) || string.IsNullOrEmpty(appVersionViewModel.VersionName))
                {
                    return obj = new { Success = 0, Message = "Invalid input", data = string.Empty };
                }
                ApplicationVersion objAppVersion = db.ApplicationVersion.FirstOrDefault();
                objAppVersion.Code = appVersionViewModel.VersionCode;
                objAppVersion.Name = appVersionViewModel.VersionName;
                db.SaveChanges();
                obj = new { Success = 1, Message = "Success", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

    }
}
