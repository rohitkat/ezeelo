//-----------------------------------------------------------------------
// <copyright file="CustomerWishlist.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------

using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;

/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class CustomerWishlist : CustomerManagement
    {
        public CustomerWishlist(System.Web.HttpServerUtility server) : base(server)
        {
        }

        /// <summary>
        /// Get List of shopStock Id's and related details for provided customer login ID
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns></returns>
        public List<ProductStockDetailViewModel> GetWishlist(long lCustLoginID,long FranchiseId)//Added FranchiseId for GetWishlist by Sonali on 19-04-2019
        {

            List<ProductStockDetailViewModel> lProductStocks = new List<ProductStockDetailViewModel>();

            lProductStocks = (
                    from pv in db.ProductVarients
                    join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                    join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                    join p in db.Products on sp.ProductID equals p.ID
                    join sh in db.Shops on sp.ShopID equals sh.ID
                    join f in db.Franchises on sh.FranchiseID equals f.ID//Added by sonali for display pincode in wishlist on 18-04-2019
                    join pin in db.Pincodes on sh.PincodeID equals pin.ID//Added by sonali for display pincode in wishlist on 18-04-2019
                    join c in db.Cities on pin.CityID equals c.ID//Added by sonali for display pincode in wishlist on 18-04-2019
                    join cl in db.Colors on pv.ColorID equals cl.ID
                    join sz in db.Sizes on pv.SizeID equals sz.ID
                    join dm in db.Dimensions on pv.DimensionID equals dm.ID
                    join mt in db.Materials on pv.MaterialID equals mt.ID
                    join wl in db.WishLists on ss.ID equals wl.ShopStockID
                    where wl.UserLoginID == lCustLoginID && f.ID == FranchiseId//Added FranchiseId for GetWishlist by Sonali on 19-04-2019
                    where p.IsActive == true
                    where pv.IsActive == true
                    where ss.IsActive == true
                    where sp.IsActive == true
                    where sh.IsLive == true
                    where f.IsActive == true//Added by sonali for display pincode in wishlist on 18-04-2019
                    where pin.IsActive == true//Added by sonali for display pincode in wishlist on 18-04-2019
                    where c.IsActive == true//Added by sonali for display pincode in wishlist on 18-04-2019

                    select new ProductStockDetailViewModel
                    {
                        StockThumbPath = string.Empty,
                        ColorID = pv.ColorID,
                        ColorCode = cl.HtmlCode,
                        ColorName = cl.Name,
                        DimensionName = dm.Name,
                        DimensionID = pv.DimensionID,
                        SizeName = sz.Name,
                        SizeID = pv.SizeID,
                        MaterialName = mt.Name,
                        MaterialID = pv.MaterialID,
                        ShopID = sp.ShopID,
                        ProductID = sp.ProductID,
                        ProductName = p.Name,
                        SaleRate = ss.RetailerRate,
                        MRP = ss.MRP,
                        ShopName = sh.Name,
                        ShopStockID = ss.ID,
                        CategoryID = p.CategoryID,
                        BrandID = p.BrandID,
                        StockQty = ss.Qty,
                        StockStatus = ss.StockStatus,
                        FranchiseID = f.ID,//Added by sonali for display pincode in wishlist on 18-04-2019
                        CityID = c.ID,//Added by sonali for display pincode in wishlist on 18-04-2019
                        CityName = c.Name,//Added by sonali for display pincode in wishlist on 18-04-2019
                        PincodeId = pin.ID,//Added by sonali for display pincode in wishlist on 18-04-2019
                        Pincode = pin.Name //Added by sonali for display pincode in wishlist on 18-04-2019
                    }).ToList();

            foreach (var item in lProductStocks)
            {
                //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Equals("N/A") ? "Default" : item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.MM);

                //item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Equals("N/A") ? "Default" : item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.LL);

                //Tejaswee (5-11-2015)
                item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName.Equals("N/A") ? "Default" : item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                item.StockSmallImagePath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName.Equals("N/A") ? "Default" : item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                item.ProductName = item.ProductName.Replace("+", " ");//Added by Sonali on 13-02-2019
            }
            return lProductStocks;
        }

        /// <summary>
        /// Add product(ShopStockID) in wishlist.
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <param name="lShopStockID">Shop Stock ID </param>
        /// <returns></returns>
        public int SetWishlist(long lCustLoginID, long lShopStockID)
        {
            int oprStatus = 0;
            try
            {
                if (db.WishLists.Where(x => x.UserLoginID == lCustLoginID && x.ShopStockID == lShopStockID).ToList().Count == 0)
                {

                    WishList lWishList = new WishList();
                    lWishList.UserLoginID = lCustLoginID;
                    lWishList.ShopStockID = lShopStockID;
                    lWishList.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                    lWishList.CreateDate = System.DateTime.UtcNow.AddHours(5.5);

                    //BusinessLogicLayer.ErrorLog.ErrorLogFile("Enter " + lCustLoginID + lShopStockID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


                    db.WishLists.Add(lWishList);
                    db.SaveChanges();
                    //BusinessLogicLayer.ErrorLog.ErrorLogFile("Enter " + "101", ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                    oprStatus = 101;
                }
                else
                {
                    oprStatus = 107;
                }
            }
            catch (Exception exception)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("<P:BusinessLogicLayer><C:CustomerWishlist><M:SetWishlist> " + exception.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                oprStatus = 500;

            }
            return oprStatus;


        }

        /// <summary>
        /// Reove Product(ShopstockID) from wishlist
        /// </summary>
        /// <param name="lCustLoginID">customer Login ID</param>
        /// <param name="lShopStockID">ShopstockID</param>
        /// <returns>returns 103: Successfully Deleted 106: Record Nor exists </returns>
        public int RemoveFromWishlist(long lCustLoginID, long lShopStockID)
        {

            int oprStatus = 0;
            //int ID = 0;
            var lWishLists = db.WishLists.Where(x => x.UserLoginID == lCustLoginID && x.ShopStockID == lShopStockID).FirstOrDefault();
            if (lWishLists != null && lWishLists.ID > 0)
            {
                WishList lWishlist = db.WishLists.Find(lWishLists.ID);
                db.WishLists.Remove(lWishlist);
                db.SaveChanges();
                oprStatus = 103;
            }
            else
                oprStatus = 106;



            return oprStatus;

        }

    }
}
