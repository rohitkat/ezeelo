using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using BusinessLogicLayer;

namespace Merchant.Models
{
    public static class MerchantCommonFunction
    {
        public static string FindProductDefaultImageLocation(long ProductID)
        {
            string imageLocation = string.Empty;
            try
            {
                imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "DEFAULT", string.Empty, ProductUpload.THUMB_TYPE.SD);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
            }
            return imageLocation;
        }
        public static string FindProductDefaultImageLocation1(long ProductID)
        {
            string imageLocation = string.Empty;
            try
            {
                imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                //ViewBag.ImageURL = src;
                //imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "DEFAULT", string.Empty, ProductUpload.THUMB_TYPE.SD);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
            }
            return imageLocation;
        }

        public static string FindProductDefaultImageLocation2(long ProductID)
        {
            string imageLocation = string.Empty;
            try
            {
                imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                //ViewBag.ImageURL = src;
                //imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "DEFAULT", string.Empty, ProductUpload.THUMB_TYPE.SD);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
            }
            return imageLocation;
        }
    }
}