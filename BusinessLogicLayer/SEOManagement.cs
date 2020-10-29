using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This Class is common for managing the seo related contents
    /// </summary>
    public class SEOManagement
    {
        private ReadConfig readConfig;
        
        
        /// <summary>
            /// Determines which type of seo content to be fetched
            /// </summary>
        public enum CONTENT_FOR
        {
            /// <summary>
            /// Determines seo content for CATEGORY to be fetched
            /// </summary>
            CATEGORY,
            
            /// <summary>
            /// Determines seo content for PRODUCT to be fetched
            /// </summary>
            PRODUCT,
            
            /// <summary>
            /// Determines seo content for SHOP to be fetched
            /// </summary>
            SHOP
        }

        /// <summary>
        /// Determines which level of category to be searched for getting SEO content
        /// </summary>
        public enum CATEGEORY_LEVEL
        {
            /// <summary>
            /// Catgeory Level1
            /// </summary>
            LEVEL1,
            
            /// <summary>
            /// Category Level2
            /// </summary>
            LEVEL2,
            
            /// <summary>
            /// Caetgeory Level3
            /// </summary>
            LEVEL3,

            /// <summary>
            /// No content for category
            /// </summary>
            NONE
        }

        /// <summary>
        /// Initialises an instance of SEOManagement
        /// </summary>
        /// <param name="server">System.Web.HttpServerUtility Object which represents current running server</param>
        public SEOManagement(System.Web.HttpServerUtility server)
        {
            this.readConfig = new ReadConfig(server);
        }

        /// <summary>
        /// Returns SEO related content for given criteria.
        /// <para>this method automatically detects SEO content server from web config file and searches for SEO content within predefined folder paths within SEO server given</para>
        /// </summary>
        /// <param name="name">Name of the category, product, shop, etc for which seo content to be fetched. <para>content file name should be of the name provided</para></param>
        /// <param name="contentType">Specify which kind of seo content to be fetched</param>
        /// <param name="catLevel">category level to be fetched if category content required; specify none if no category required</param>
        /// <returns></returns>
        public string GetSEOContent(string name, CONTENT_FOR contentType, CATEGEORY_LEVEL catLevel)
        {
            try
            {                
                string contentUri = this.GetUri(contentType, catLevel)+name+".txt";

                System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(contentUri));

                if (webReq.GetResponse().ContentLength > 0)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(webReq.GetResponse().GetResponseStream());

                    return sr.ReadToEnd();
                }

                return string.Empty;
               
            }
            catch 
            {
                // no content found or theres something wrong getting seo content
                return string.Empty; // return string.empty coz there should not be page blocker error due to seo exceptions
            }
        }

        private string GetUri(CONTENT_FOR contentType, CATEGEORY_LEVEL catLevel)
        {
            try 
            {
                switch (contentType)
                {
                    case CONTENT_FOR.CATEGORY: return this.readConfig.SEO_SERVER + "/cat_desc" + (catLevel != CATEGEORY_LEVEL.NONE ? "/" + catLevel.ToString() : string.Empty);

                    case CONTENT_FOR.PRODUCT: return this.readConfig.SEO_SERVER + "/prod_desc";
                 
                    case CONTENT_FOR.SHOP: return this.readConfig.SEO_SERVER + "/shop_desc";
                    
                    default: return string.Empty;
                }
            }
            catch { return string.Empty; }
        }


    }
}
