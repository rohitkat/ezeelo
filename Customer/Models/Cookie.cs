using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gandhibagh.Models
{
    public static class URLCookie
    {
        public static void SetCookies()
        {
            string url = HttpContext.Current.Request.Url.ToString();
            string check = HttpContext.Current.Request.Url.AbsoluteUri;
            HttpContext.Current.Response.Cookies["UrlCookie"].Value = url;
            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["UrlCookie"]);
            HttpContext.Current.Response.Cookies["UrlCookie"].Expires = System.DateTime.Now.AddDays(30);
        }
    }
}