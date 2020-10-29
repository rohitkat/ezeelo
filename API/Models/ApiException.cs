//-----------------------------------------------------------------------
// <copyright file="ApiException" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
/*
 Handed over on 15-09-2015(12.15 AM to 12.45 AM) to Ajit, AVi, Mohit, Manoj
 */
namespace API.Models
{
    public class ApiException : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
           
            if (actionExecutedContext.Exception != null)
            {           
                HttpResponseMessage exMessage = new HttpResponseMessage();
                BusinessLogicLayer.ErrorLog.ErrorLogFile(actionExecutedContext.ActionContext.ControllerContext.Controller.ToString() 
                    + ": " 
                    + actionExecutedContext.Exception.Message 
                    + actionExecutedContext.Exception.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                
                if (actionExecutedContext.ActionContext.ModelState.IsValid == false)
                {
                    // Return the validation errors in the response body.
                    var errors = new Dictionary<string, IEnumerable<string>>();
                    object objValidation = new object();
                    StringBuilder UserMessage = new StringBuilder();
                
                    foreach (KeyValuePair<string, ModelState> keyValue in actionExecutedContext.ActionContext.ModelState)
                    {
                        var msg = keyValue.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
                        if (msg != null)
                        {
                            if (keyValue.Key.Contains("."))
                            {
                                string[] keySplit = keyValue.Key.ToString().Split('.');
                                UserMessage.Append("The value provided for parameter '" + keySplit[keySplit.Length - 1] + "' is not valid.");

                                if (keyValue.Value.Errors != null && keyValue.Value.Errors.Count > 0 && keyValue.Value.Errors[0].Exception != null && keySplit[keySplit.Length - 1].Contains("[") && !(keyValue.Value.Errors[0].Exception.Equals(string.Empty)))
                                    UserMessage.Append(" i.e. " + keyValue.Value.Errors[0].Exception.Message);
                            }                           
                            else
                                UserMessage.Append("The value provided for parameter '" + keyValue.Key.ToString() + "' is not valid.");
                        }
                    }
                    objValidation = new { HTTPStatusCode = "400", UserMessage = "Unsupported Query Parameter", ValidationError = UserMessage.ToString() };
                    actionExecutedContext.ActionContext.Response =
                    actionExecutedContext.ActionContext.Request.CreateResponse(HttpStatusCode.BadRequest, objValidation);
                }
                else
                {
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest, new { HTTPStatusCode = "500", UserMessage = "Internal server error", ServerError = actionExecutedContext.Exception.Message + ' ' + actionExecutedContext.Exception.InnerException });
                }              
              
            }
        }      
    }
}