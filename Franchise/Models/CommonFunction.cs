using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using BusinessLogicLayer;
using ModelLayer.Models;

namespace Franchise.Models
{
    public static class FranchiseCommonFunction
    {
        private static EzeeloDBContext db = new EzeeloDBContext();

        //public static string FindProductDefaultImageLocation(long ProductID)
        //{
        //    string imageLocation = string.Empty;
        //    try
        //    {
        //        imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "DEFAULT", string.Empty, ProductUpload.THUMB_TYPE.SD);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[FranchiseCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
        //    }
        //    return imageLocation;
        //}

        public static string FindProductDefaultImageLocation(long ProductID)
        {
            string imageLocation = string.Empty;
            try
            {
                imageLocation = ImageDisplay.LoadProductThumbnails(ProductID, "DEFAULT", string.Empty, ProductUpload.THUMB_TYPE.SD);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FranchiseCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
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
                throw new BusinessLogicLayer.MyException("[FranchiseCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
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
                throw new BusinessLogicLayer.MyException("[FranchiseCommonFunction][FindProductDefaultImageLocation]", "Can't find Image  !" + Environment.NewLine + ex.Message);
            }
            return imageLocation;
        }

        public static string GetEmployeeName(long UID)
        {
            string EmployeeName = string.Empty;
            try
            {
                PersonalDetail PD = db.PersonalDetails.Where(x => x.UserLoginID == UID).FirstOrDefault();
                if (PD != null)
                {
                    EmployeeName = PD.Salutation.Name + " " + PD.FirstName + " " + PD.MiddleName + " " + PD.LastName;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FranchiseCommonFunction][GetEmployerName]", "Can't Get Employer Name  !" + Environment.NewLine + ex.Message);
            }
            return EmployeeName.ToString().Trim();
        }
    }
}