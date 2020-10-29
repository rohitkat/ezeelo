//-----------------------------------------------------------------------
// <copyright file="ValidateModelAttribute" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace API.Models
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)        
        {
          
            if (actionContext.ModelState.IsValid == false)
            {
                // Return the validation errors in the response body.
                var errors = new Dictionary<string, IEnumerable<string>>();

                object objValidation = new object();              
                StringBuilder UserMessage = new StringBuilder();                

                foreach (KeyValuePair<string, ModelState> keyValue in actionContext.ModelState)
                {
                    var msg = keyValue.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
                    if (msg != null)
                    {
                        //To avoid lCust.MobileNo 
                        if (keyValue.Key.Contains("."))
                        {
                            string[] keySplit = keyValue.Key.ToString().Split('.');
                            UserMessage.Append("The value provided for parameter '" + keySplit[keySplit.Length - 1] + "' is not valid.");
                            if (keyValue.Value.Errors != null && keyValue.Value.Errors.Count > 0 && keyValue.Value.Errors[0].Exception != null && keySplit[keySplit.Length - 1].Contains("[") && !(keyValue.Value.Errors[0].Exception.Message.Equals(string.Empty)))
                                UserMessage.Append(" i.e. " + keyValue.Value.Errors[0].Exception.Message);
                        }
                        else if (keyValue.Value.Errors.Count > 0)
                        {
                            foreach (var item in keyValue.Value.Errors)
	                        {
                                UserMessage.Append(item.Exception.Message.Split('.')[0] + ".");  
	                        }                                                
                        }
                        else
                            UserMessage.Append("The value provided for parameter '" + keyValue.Key.ToString() + "' is not valid.");
                       
                    }
                }
                objValidation = new {HTTPStatusCode = "400",  UserMessage = "Unsupported Query Parameter", ValidationError =  UserMessage.ToString()};
                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, objValidation);
            }
        }
    }
}