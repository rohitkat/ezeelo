using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GenerateIMEINOController : ApiController
    {
        public object Get()
        {
            object obj = new object();
            try
            {
               // string id1 = Guid.NewGuid().ToString();

                long ticks = DateTime.Now.Ticks;
                //byte[] bytes = BitConverter.GetBytes(ticks);
                //string id = Convert.ToBase64String(bytes)
                //                        .Replace('+', '_')
                //                        .Replace('/', '-')
                //                        .TrimEnd('=');
                obj = new { Success = 1, Message = "Success", data = ticks };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
