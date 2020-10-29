//-----------------------------------------------------------------------
// <copyright file="ShopDisplay.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
/*
 Handed over to Mohit
 */
namespace BusinessLogicLayer
{
  public class ShopDisplay
    {
      //Base class for ShopDetails
        protected System.Web.HttpServerUtility server;
      /// <summary>
      /// Initialise Server Object
      /// </summary>
        /// <param name="server">System.Web.HttpServerUtility object</param>
        public ShopDisplay(System.Web.HttpServerUtility server)
        {
            this.server = server;
       }
        /// <summary>
        /// Get Shop Details like name, website etc
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
     
        public virtual ShopDetailsViewModel GetShopBasicDetails(long shopID)
        {
      ShopDetailsViewModel lshopDetails  = new ShopDetailsViewModel();
          return lshopDetails; 
      }
      /// <summary>
        /// Get ShopDescriptionFilePath of Http Server if file exists on FTP
      /// </summary>
      /// <param name="shopID"></param>
      /// <returns></returns>
      public virtual string  GetShopDescriptionFilePath(long shopID)
      {
          
          return string.Empty;
      }
      /// <summary>
      /// Get Shop Images
      /// </summary>
      /// <param name="shopID">Shop ID</param>
      /// <returns></returns>
      public virtual List<ImageListViewModel>  GetShopImageGallery(long shopID)
      {
          List<ImageListViewModel> lImageGallery = new  List<ImageListViewModel>();
          return lImageGallery;
      }  
    }
}
