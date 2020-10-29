//-----------------------------------------------------------------------
// <copyright file="LoginSuccess" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Net;


namespace API.Models
{
    public class LoginSuccess : ActionFilterAttribute
    {
        //public long UserLoginId { get; set; }// Added by Sonali for create token through UserLoginId on 16-04-2019
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                ////Commented by Zubiar temporarily on 15-11-2017 on behalf of Android developer Deepanshu
                //string reqBy = actionExecutedContext.Request.Headers.GetValues("ReqBy").First();
                string IMEI = actionExecutedContext.Request.Headers.GetValues("IMEI").First();
                string reqBy = "m";
                // string IMEI = "867634024784884";
                var UserLoginId = actionExecutedContext.Request.Properties["UserLoginId"];// Added by Sonali for create token through UserLoginId on 16-04-2019

                //string UserLoginId = actionExecutedContext.Request.Properties["UserLoginId"].ToString();
                if (reqBy == "m" && IMEI.Length > 5 && UserLoginId != null)
                {
                    long UserId = Convert.ToInt64(UserLoginId.ToString());// Added by Sonali for create token through UserLoginId on 16-04-2019
                    ////End Comment

                    //BusinessLogicLayer.ErrorLog.ErrorLogFile("IMEI " + IMEI , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


                    APIToken apiToken = new APIToken();
                    string tokenKey = string.Empty;
                    var Token = db.APITokens.Where(x => x.UserLoginID == UserId && x.IsActive).FirstOrDefault();// Added UserLoginId checked by Sonali on 16-04-2019
                    if (Token == null)
                    {
                        var TokenIMEI = db.APITokens.Where(x => x.IMEI == IMEI).FirstOrDefault();
                        if (TokenIMEI == null)
                        {
                            //insert 
                            Guid randomId = Guid.NewGuid();
                            tokenKey = randomId.ToString().Substring(0, 15).ToUpper();
                            apiToken.UserLoginID = UserId;//Save UserLoginId by Sonali on 16-04-2019
                            apiToken.IMEI = IMEI;
                            apiToken.TokenCode = tokenKey;
                            apiToken.IsActive = true;
                            apiToken.CreateBy = 1;
                            apiToken.CreateDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                            db.APITokens.Add(apiToken);
                            db.SaveChanges();
                            tokenKey = apiToken.TokenCode;
                            //BusinessLogicLayer.ErrorLog.ErrorLogFile("tokenID " + apiToken.ID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                        }
                        else
                        {
                            Guid randomId = Guid.NewGuid();
                            tokenKey = randomId.ToString().Substring(0, 15).ToUpper();
                            //APIToken aToken = db.APITokens.Where(x => x.IMEI == IMEI).FirstOrDefault();// Added UserLoginId checked by Sonali on 16-04-2019
                            TokenIMEI.UserLoginID = UserId;//Save UserLoginId by Sonali on 16-04-2019
                            TokenIMEI.IMEI = IMEI;
                            TokenIMEI.TokenCode = tokenKey;
                            TokenIMEI.IsActive = true;
                            TokenIMEI.CreateBy = 1;
                            TokenIMEI.ModifyDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                            db.SaveChanges();
                            tokenKey = TokenIMEI.TokenCode;
                        }
                    }
                    else
                    {
                        APIToken oldToken = db.APITokens.Where(x => x.UserLoginID == UserId && x.IsActive).FirstOrDefault();
                        oldToken.UserLoginID = null;
                        oldToken.IsActive = false;
                        oldToken.ModifyDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                        db.SaveChanges();

                        //Update 
                        Guid randomId = Guid.NewGuid();
                        tokenKey = randomId.ToString().Substring(0, 15).ToUpper();
                        APIToken aToken = db.APITokens.Where(x => x.IMEI == IMEI).FirstOrDefault();// Added UserLoginId checked by Sonali on 16-04-2019
                        if(aToken != null)
                        {
                            aToken.UserLoginID = UserId;//Save UserLoginId by Sonali on 16-04-2019
                            aToken.IMEI = IMEI;
                            aToken.TokenCode = tokenKey;
                            aToken.IsActive = true;
                            aToken.CreateBy = 1;
                            aToken.ModifyDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                            db.SaveChanges();
                            tokenKey = aToken.TokenCode;
                        }
                        else
                        {
                            apiToken.UserLoginID = UserId;//Save UserLoginId by Sonali on 16-04-2019
                            apiToken.IMEI = IMEI;
                            apiToken.TokenCode = tokenKey;
                            apiToken.IsActive = true;
                            apiToken.CreateBy = 1;
                            apiToken.CreateDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                            db.APITokens.Add(apiToken);
                            db.SaveChanges();
                            tokenKey = apiToken.TokenCode;
                        }
                        

                        

                    }
                    actionExecutedContext.Response.Content.Headers.Add("Token", tokenKey);
                    ////Commented by Zubiar temporarily on 15-11-2017 on behalf of Android developer Deepanshu
                }
                else
                {
                    if (UserLoginId == null)
                    {
                        actionExecutedContext.Response.Content.Headers.Add("Token", string.Empty);//Added by Sonali for create token through UserLoginId on 16-04-2019
                    }
                    else
                    {
                        throw new Exception("UnAutorises Access");
                    }

                }
                ////End Comment
            }
            catch (Exception ex)
            {

                if (actionExecutedContext.Exception != null && actionExecutedContext.Exception.ToString().Length > 1)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Exception " + ex.Message + actionExecutedContext.Exception, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                    actionExecutedContext.Response =
                    actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, new { HTTPStatusCode = "500", UserMessage = "Internal server error", ServerError = ex.Message + " " + actionExecutedContext.Exception });

                }
                else
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Exception " + ex.Message + ex.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    System.Net.HttpStatusCode.BadRequest, new { HTTPStatusCode = "400", UserMessage = "The value provided for one of the HTTP headers is not in the correct format." });

                }
                return;
            }

        }
    }
}