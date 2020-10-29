//-----------------------------------------------------------------------
// <copyright file="EnableTag" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Caching;

//using System.Runtime.Caching.ObjectCache;
//using System.Runtime.Caching.MemoryCache;
namespace API.Models
{
    public class EnableTag : System.Web.Http.Filters.ActionFilterAttribute
    {
        private static ConcurrentDictionary<string, EntityTagHeaderValue> etags = new ConcurrentDictionary<string, EntityTagHeaderValue>();

        public override void OnActionExecuting(HttpActionContext context)
        {
            if (context != null)
            {
                var request = context.Request;
                if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Post)
                {
                    var key = GetKey(request);
                    ICollection<EntityTagHeaderValue> etagsFromClient = request.Headers.IfNoneMatch;
                    EntityTagHeaderValue etag = null;

                    //string ETag = context.Request.Headers.GetValues("ETag").First();
                    
                    //etags.TryGetValue(key, out etag);
                    //if (etags.TryGetValue(key, out etag) && etagsFromClient.Any(t => t.Tag == etag.Tag))
                    //{
                    //    context.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
                    //    SetCacheControl(context.Response);
                    //}
                    if (etags.Count > 0)
                    {                      
                        if (etags.TryGetValue(key, out etag) )
                        {
                            //ObjectCache cache = MemoryCache.Default;
                            
                            context.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
                            SetCacheControl(context.Response);
                        }
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var request = context.Request;
            var key = GetKey(request);

            EntityTagHeaderValue etag;
            if (!etags.TryGetValue(key, out etag) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
            {
                //etag = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
                etag = new EntityTagHeaderValue("\"1\"");
                etags.AddOrUpdate(key, etag, (k, val) => etag);
            }

            context.Response.Headers.ETag = etag;
            SetCacheControl(context.Response);
        }

        private static void SetCacheControl(HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromMinutes(5),
                MustRevalidate = true,
                Private = true
            };
        }

        private static string GetKey(HttpRequestMessage request)
        {
            return request.RequestUri.ToString();
        }
    }
}