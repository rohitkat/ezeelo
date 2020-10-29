using API.Models;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetLeaderVideoController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();

        [ApiException]
        public object Get()
        {
            object obj = new object();
            try
            {
                LeaderVideo objLeaderVideo = db.LeaderVideos.OrderByDescending(x => x.ID).FirstOrDefault();
                if (objLeaderVideo != null)
                {
                    obj = new { Success = 1, Message = "Successfull.", data = objLeaderVideo };
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
    }
}
