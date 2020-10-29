using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public class ConfigurationSettings : IConfigurations
    {
        protected System.Web.HttpServerUtility server;

        /// <summary>
        /// Private member stores full path of the configuration file
        /// </summary>
        protected string configFilePath = string.Empty;
        
        /// <summary>
        /// Enumerator to Provide which configuration file we are dealing with
        /// </summary>
        public enum CONFIG_FILE
        {
            /// <summary>
            /// Represents the Web.Config File
            /// </summary>
            WEB_CONFIG,

            /// <summary>
            /// Represnts the System Configuration File
            /// </summary>
            SYSTEM_CONFIG,

            /// <summary>
            /// Represents EMAILS settings File
            /// </summary>
            EMAILS,

            /// <summary>
            /// Represents SMS settings File
            /// </summary>
            SMS,

            /// <summary>
            /// NO Config File Specified
            /// </summary>
            NONE
        }

        public ConfigurationSettings(System.Web.HttpServerUtility server)
        {
            this.server = server;

            //this.SetConfigPath(configFile);
        }

        private void SetConfigPath(CONFIG_FILE configFile)
        {

            switch (configFile)
            {

                case CONFIG_FILE.EMAILS:
                    this.configFilePath = this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["EMAILS_XML"].ToString();
                    break;

                case CONFIG_FILE.SMS:
                    this.configFilePath = this.server.MapPath("~") + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["SMS"].ToString();
                    break;

                case CONFIG_FILE.SYSTEM_CONFIG:
                case CONFIG_FILE.WEB_CONFIG:
                case CONFIG_FILE.NONE:
                default: this.configFilePath = string.Empty; break;
            }
        }
    }
}
