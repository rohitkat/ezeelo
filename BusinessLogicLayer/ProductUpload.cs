//-----------------------------------------------------------------------
// <copyright file="ProductUpload.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace BusinessLogicLayer
{
    public class ProductUpload : IProductManagement
    {

        protected System.Web.HttpServerUtility server;

        public ProductUpload(System.Web.HttpServerUtility server)
        {
            this.server = server;
        }

        public enum IMAGE_FOR
        {
            [Description("p")]
            Products,
            [Description("s")]
            Shops
        }

        public enum DIMENSION
        {
            MOBILE = 120,
            THUMB = 150,
            SMALL = 44,   //change req. by sumit for api on date:4/9/2015  prev value: 300
            LARGE
        }

        public enum IMAGE_TYPE
        {
            [Description("ai")]
            Approved,
            [Description("nai")]
            NonApproved
        }

        public enum THUMB_TYPE
        {
            SD,
            MM,
            SS,
            LL

        }

        /// <summary>
        /// Specifies whether path required is url path or Physical path  
        /// </summary>
        public enum IMG_PATH_TYPE
        {
            /// <summary>
            /// URL Path e.g. http://ezeelo.com/images/demo.png
            /// </summary>
            URL_PATH,

            /// <summary>
            /// Physical path e.g. c:\ezeelo\images\demo.png
            /// </summary>
            PHYSICAL_PATH

        }
        /// <summary>
        /// Specifies the root folder name for images
        /// </summary>
        public static string IMAGE_ROOTPATH = "c";//"Content";

        public static string DESC_FILE_NAME = "description.html";

        public static string CAREER_ROOTPATH = "Career";
        public IEnumerable<HttpPostedFileBase> Files { get; set; }

        public HttpPostedFileBase LOGO { get; set; }

        public enum DISAPPROVAL_REMARK
        {           
            APPLY_FOR_APPROVAL=0,

            /// <summary>
            /// 
            /// </summary>
            APPROVED=1,

            /// <summary>
            /// 
            /// </summary>
            PENDING_BY_ADMIN = 2,

            /// <summary>
            /// 
            /// </summary>
            STOCK_NOT_EXIST = 3,

            /// <summary>
            /// 
            /// </summary>
            VARIENT_NOT_EXIST = 4,

            /// <summary>
            /// 
            /// </summary>
            SHOP_NOT_APPROVED = 5,

            /// <summary>
            /// 
            /// </summary>
            IMAGE_NOT_EXIST = 6,

            /// <summary>
            /// 
            /// </summary>
            PRODUCT_NOT_EXIST = 7,

            /// <summary>
            /// 
            /// </summary>
            PRODUCT_WITH_SAME_NAME_ALREADY_EXIST = 8,

            /// <summary>
            /// 
            /// </summary>
            PRODUCT_LIMIT_EXCEEDED=9

            
        }

        public enum CITY
        {
            NAGPUR = 4968,
            VARANASI = 10908,
            Kanpur = 10909,   
            Wardha = 7536,
            Lucknow = 11187 ////added
        }        
    }

    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static String GetDescription(this Enum value)
        {
            var description = GetAttribute<DescriptionAttribute>(value);
            return description != null ? description.Description : null;
        }
    }
}

