using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BusinessLogicLayer
{
    /// <summary>
    /// Class which reads the configuration file settings stored 
    /// </summary>
    public class ReadConfig : ConfigurationSettings
    {
        /// <summary>
        /// Gets the SEO Server Path from web Config File
        /// </summary>
        public string EwalletRefundMailId //Added by Rumana on 18-05-2019
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["Ewallet_Refund_Mail"].ToString(); }
        }

        /// <summary>
        /// initialises an instance of ReadConfig class
        /// </summary>
        /// <param name="server">HttpServerUtility Object</param>
        public ReadConfig(System.Web.HttpServerUtility server) : base(server) { }

        /// <summary>
        /// Calculates the complete path of the log folder
        /// </summary>
        public string LOG_FOLDER_PATH
        {
            get { return this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["LOG_FOLDER"].ToString(); }
        }


        /// <summary>
        /// Gets the SEO Server Path from web Config File
        /// </summary>
        public string SEO_SERVER
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["SEO_CONTENT_SERVER"].ToString(); }
        }

        /// <summary>
        /// Calculates Emails Folder Path where all emails template files are stored
        /// </summary>
        public string EMAILS_FOLDER_PATH
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["EMAILS_FOLDER"].ToString(); }
            //get { return this.server.MapPath("~") + System.Web.Configuration.WebConfigurationManager.AppSettings["EMAILS_FOLDER"].ToString(); }
        }

        /// <summary>
        /// Returns the Full path of the GATEWAYS xml file
        /// </summary>
        public string GATEWAYS_XML_PATH
        {
            get { return this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["GATEWAYS"].ToString(); }
        }


        /// <summary>
        /// Returns the Full path of the GATEWAYS xml file
        /// </summary>
        public string DEFAULT_EMAIL
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_EMAIL"].ToString(); }
        }
        /// <summary>
        /// Returns Full Path of the SMS xml file
        /// </summary>
        public string SMS_XML_PATH
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["SMS_XML"].ToString(); }
            //get { return this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["SMS_XML"].ToString(); }
        }

        /// <summary>
        /// Returns Full Path of the Image file
        /// </summary>
        public string IMAGE_FTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["IMAGE_FTP"].ToString(); }
        }

        /// <summary>
        /// Returns Full Path of the Image file
        /// </summary>
        public string IMAGE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["IMAGE_HTTP"].ToString(); }
        }
        public string CATEGORY_IMAGE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["CATEGORY_IMAGE_HTTP"].ToString(); }
        }
        public string CATEGORY_IMAGE_FTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["CATEGORY_IMAGE_FTP"].ToString(); }
        }

        /// <summary>
        /// Returns Full Path of the Category Description Text file FTP Protocal
        /// </summary>
        public string FILE_FTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["FILE_FTP"].ToString(); }
        }

        /// <summary>
        /// Returns Full Path of the Category Description Text file HTTP Protocal
        /// </summary>
        public string FILE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["FILE_HTTP"].ToString(); }
        }

        public string HOME_IMAGE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["HOME_IMAGE_HTTP"].ToString(); }
        }
        public string HOME_IMAGE_FTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["HOME_IMAGE_FTP"].ToString(); }
        }

        public string HomePageDynamicSection_IMAGE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["HomePageDynamicSection_IMAGE_HTTP"].ToString(); }
        }

        public string HomePageDynamicSection_IMAGE_FTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["HomePageDynamicSection_IMAGE_FTP"].ToString(); }
        }

        /// <summary>
        /// Returns User Name of ftp folder
        /// </summary>
        public string USER_NAME
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["USER_NAME"].ToString(); }
        }

        /// <summary>
        /// Returns Password of ftp folder
        /// </summary>
        public string PASSWORD
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["PASSWORD"].ToString(); }
        }
        //By Sonali
        public string LOCALIMG_PATH
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["IMG_HTTP"].ToString(); }
        }
        /// <summary>
        /// Returns full path of the EMAILS xml File
        /// </summary>
        public string EMAILS_XML_PATH
        {
            //get { return this.server.MapPath("http://192.168.1.102:9292/Communication/EMAIL/"); }
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["EMAILS_XML"].ToString(); }
            // get { return this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["EMAILS_XML"].ToString(); }
        }

        /// <summary>
        /// Returns the current database connection string
        /// </summary>
        public string DB_CONNECTION
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["DB_CON"].ToString(); }
        }

        /// <summary>
        /// Provides the administrators Mobile number
        /// </summary>
        public string ADMIN_MOBILE
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["ADMIN_MOBILE"].ToString();
                }
                catch
                {
                    return "9822072922";
                }
            }
        }

        /// <summary>
        /// To send All Mail on this Email Address
        /// </summary>
        public string DEFAULT_ALL_EMAIL
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ALL_EMAIL"].ToString();
                }
                catch
                {
                    return "sales@ezeelo.com";
                }
            }
        }

        /// <summary>
        /// To Send All the SMS 
        /// </summary>
        public string DEFAULT_ALL_SMS
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ALL_SMS"].ToString();
                }
                catch { return "9422149985"; }
            }
        }



        /// <summary>
        /// Provides CRM Mobile Number
        /// </summary>
        public string CRM_MOBILE
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["CRM_MOBILE"].ToString();
                }
                catch
                {
                    return "9822072922";
                }
            }
        }
        public string HR_EMAIL
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["HR_Email"].ToString();
                }
                catch
                {
                    return "career@ezeelo.com";
                }
            }
        }
        public string GB_DEALS_IMAGE
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["GB_DEALS_IMAGE"].ToString(); }
        }

        /// <summary>
        /// Provides CRM Email
        /// </summary>
        public string CRM_EMAIL
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["CRM_EMAIL"].ToString();
                }
                catch
                {
                    return "crm@ezeelo.com";
                }
            }
        }

        /// <summary>
        /// Provides Social Password set By SOnali
        /// </summary>
        public string SOCIAL_PASSWORD
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["PASSWORD_SOCIAL"].ToString();
                }
                catch
                {
                    return "social123";
                }
            }
        }
        /// <summary>
        /// Provides GST constant set By SOnali
        /// </summary>
        public string GST_CONSTANT
        {
            get
            {
                try
                {
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["GST_CONSTANT"].ToString();
                }
                catch
                {
                    return "27AAECE6662K1Z9";
                }
            }
        }

        /// <summary>
        ///Return path of DealBannerImg folder By SOoali
        /// </summary>
        public string DealBanner_IMAGE_HTTP
        {
            get { return System.Web.Configuration.WebConfigurationManager.AppSettings["DEALBANNER_IMAGE_HTTP"].ToString(); }
        }

        /// <summary>
        /// Calculates the complete path of the Payubiz log folder
        /// </summary>
        public string Payubiz_LOG_FOLDER_PATH
        {
            get { return this.server.MapPath("~") + System.Web.Configuration.WebConfigurationManager.AppSettings["Payubiz_LOG_FOLDER"].ToString(); }
        }

        /// <summary>
        /// Calculates the complete path of the Order log folder
        /// </summary>
        public string Order_LOG_FOLDER_PATH
        {
            get { return this.server.MapPath("~") + System.Web.Configuration.WebConfigurationManager.AppSettings["ORDER_LOG_FOLDER"].ToString(); }
        }

        /// <summary>
        /// Get Leaders admin email id
        /// </summary>
        public string LEADERS_ADMIN_EMAILID
        {
            get { return this.server.MapPath("~") + System.Web.Configuration.WebConfigurationManager.AppSettings["LEADER_EMAILID"].ToString(); }
        }
    }
}
