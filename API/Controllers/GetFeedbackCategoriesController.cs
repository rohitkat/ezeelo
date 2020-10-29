using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Controllers
{
    public class GetFeedbackCategoriesController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get()
        {
            object obj = new object();
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var feedbackcategorylist = db.FeedbackCategaries.ToList();
                if (feedbackcategorylist != null)
                {
                    obj = new { Success = 1, Message = "Feedback Category list are found.", data = new { feedbackcategorylist } };
                }
                else
                {
                    obj = new { Success = 0, Message = "Feedback Category list are not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
