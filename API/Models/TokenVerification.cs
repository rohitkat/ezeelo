//-----------------------------------------------------------------------
// <copyright file="TokenVerification" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models;

/*
 Handed over on 15-09-2015 (From 12.00 AM to 12:15 AM) to Ajit, AVi, Mohit, Manoj
 */
namespace API.Models
{
    public class TokenVerification : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            var context = actionContext.Request.Properties["MS_HttpContext"] as System.Web.HttpContextBase;
            string userIP = context.Request.UserHostAddress;
            try
            {
                string token = actionContext.Request.Headers.GetValues("Token").First();
                string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
                string IMEI = actionContext.Request.Headers.GetValues("IMEI").First();
                if (reqBy == "m" && IMEI.Length > 5)
                {                   
                    var row = db.APITokens.Where(x => x.IMEI == IMEI && x.TokenCode == token && x.IsActive == true).FirstOrDefault();
                    if (row!=null)
                    {
                        return;
                    } 
                    else
                    {
                        throw new Exception("UnAutorises Access"); 
                    }
                }
                else
                {
                    throw new Exception("UnAutorises Access");

                }
            }
            catch (System.Data.DataException ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                System.Net.HttpStatusCode.BadRequest, new { HTTPStatusCode = "500", UserMessage = "Internal server error", ServerError = ex.Message + ex.InnerException});

                return;
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                System.Net.HttpStatusCode.BadRequest, new { HTTPStatusCode = "400", UserMessage = "The value provided for one of the HTTP headers is not in the correct format." });                
                
                return;
            }
        }
     
        
    }
}