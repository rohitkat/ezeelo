//-----------------------------------------------------------------------
// <copyright file="ProductDetails.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLayer.Models.ViewModel;
using DataAccessLayer;
using ModelLayer.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Globalization;
using System.Net;
/*
 Handed over to Mohit, Tejaswee
 */
namespace BusinessLogicLayer
{
    public class ProductDetails : ProductDisplay
    {
        //initialise base class constructor
        public ProductDetails(System.Web.HttpServerUtility server) : base(server) { }

        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get Product basic Details to be shown on preview Item page
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns></returns>
        public ProductViewBasicDetailsViewModels GetBasicDetails(long pID)
        {
            ProductViewBasicDetailsViewModels pD = new ProductViewBasicDetailsViewModels();

            //Collect Product Details like Brand name, Category etc
            pD = (
               from p in db.Products
               join b in db.Brands on p.BrandID equals b.ID
               join c in db.Categories on p.CategoryID equals c.ID
               where p.IsActive == true && c.Level == 3 && p.ID == pID

               select new ProductViewBasicDetailsViewModels
               {
                   ThumbPath = string.Empty,
                   BrandID = p.BrandID,
                   CategoryID = p.CategoryID,
                   BrandName = b.Name,
                   CategoryName = c.Name,
                   BreadthInCm = p.BreadthInCm,
                   HeightInCm = p.HeightInCm,
                   LengthInCm = p.LengthInCm,
                   ProductID = p.ID,
                   ProductName = p.Name,
                   WeightInGram = p.WeightInGram,
                   Description = p.Description

               }).FirstOrDefault();
            //if product details exists, get the color varient and thumbnail for first color
            if (pD != null)
            {
                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == pID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                //Tejaswee (5-11-2015)
                if (color != null && color.name != "N/A")
                {
                    //pD.ThumbPath  = ImageDisplay.LoadProductThumbnails(pID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    pD.ThumbPath = ImageDisplay.SetProductThumbPath(pID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //pD.ThumbPath = ImageDisplay.LoadProductThumbnails(pID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    pD.ThumbPath = ImageDisplay.SetProductThumbPath(pID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }


                //BrandThumbPath is  field added in ProductViewBasicDetailsViewModels in order to show brand image
                //But as we are not storing the image, initializes it with no thumbnail
                pD.BrandThumbPath = "/Content/No_thumbnail.png";
            }

            return pD;

        }

        public ProductViewBasicDetailsViewModels GetBasicDetails(long pID, int fID)////added long cID->int fID
        {
            ProductViewBasicDetailsViewModels pD = new ProductViewBasicDetailsViewModels();

            //Collect Product Details like Brand name, Category etc
            pD = (
               from p in db.Products
               join b in db.Brands on p.BrandID equals b.ID
               join c in db.Categories on p.CategoryID equals c.ID
               //------------------------------------
               join s in db.ShopProducts on p.ID equals s.ProductID
               join sh in db.Shops on s.ShopID equals sh.ID ////added
               //join pin in db.Pincodes on s.Shop.PincodeID equals pin.ID ////hide
               join pin in db.Pincodes on sh.PincodeID equals pin.ID

               // where p.IsActive == true && c.Level == 3 && p.ID == pID && s.IsActive == true && pin.CityID == cID ////hide by ashish
               where p.IsActive == true && c.Level == 3 && p.ID == pID && s.IsActive == true && sh.FranchiseID == fID ////added by ashish

               select new ProductViewBasicDetailsViewModels
               {
                   ThumbPath = string.Empty,
                   BrandID = p.BrandID,
                   CategoryID = p.CategoryID,
                   BrandName = b.Name,
                   CategoryName = c.Name,
                   BreadthInCm = p.BreadthInCm,
                   HeightInCm = p.HeightInCm,
                   LengthInCm = p.LengthInCm,
                   ProductID = p.ID,
                   ProductName = p.Name,
                   WeightInGram = p.WeightInGram,
                   Description = p.Description

               }).FirstOrDefault();
            //if product details exists, get the color varient and thumbnail for first color
            if (pD != null)
            {
                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == pID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                if (color != null && color.name != "N/A")
                {
                    //pD.ThumbPath = ImageDisplay.LoadProductThumbnails(pID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    pD.ThumbPath = ImageDisplay.SetProductThumbPath(pID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //pD.ThumbPath = ImageDisplay.LoadProductThumbnails(pID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    pD.ThumbPath = ImageDisplay.SetProductThumbPath(pID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }

                //BrandThumbPath is  field added in ProductViewBasicDetailsViewModels in order to show brand image
                //But as we are not storing the image, initializes it with no thumbnail
                pD.BrandThumbPath = "/Content/No_thumbnail.png";
            }

            return pD;

        }

        /// <summary>
        /// Get Stock Varients like color, size, dimension, material and shop associated with that varients, by product ID
        /// If customer id is provided, it returns the status whether it is added in wishlist to show the wishlist status on preview item page
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="lcustLoginID">Customer Login ID, to get wishlist Status</param>
        /// <returns></returns>        
        public IEnumerable<ShopProductVarientViewModel> GetStockVarients(long pID, long? lcustLoginID)
        {
            return GetProductStockVarients(pID, 0, 0, lcustLoginID);

        }

        /// <summary>
        /// Get Stock Varients like color, size, dimension, material and shop associated with that varients, By ProductID and ShopID
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="sID">Shop ID</param>
        /// <param name="lcustLoginID">Customer Login ID, to get wishlist Status</param>
        /// <returns></returns>
        /// 
        public IEnumerable<ShopProductVarientViewModel> GetStockVarients(long pID, long sID, long? lcustLoginID)
        {
            return GetProductStockVarients(pID, sID, 0, lcustLoginID);
        }
        public IEnumerable<ShopProductVarientViewModel> GetStockVarients(long pID, long sID, long? lcustLoginID, int fID)////added long cID->int fID
        {
            // if (cID > 0)////hide
            if (fID > 0)////added
                return GetProductStockVarients(pID, sID, 0, lcustLoginID, fID);////added  cID->fID
            else
                return GetProductStockVarients(pID, sID, 0, lcustLoginID);
        }

        /// <summary>
        ///  Get Stock Varients like color, size, dimension, material and shop associated with that varients, By ShopStockID
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        ///  <param name="lcustLoginID">Customer Login ID, to get wishlist Status</param>
        /// <returns></returns>
        public ShopProductVarientViewModel GetShopStockVarients(long ssID, long? lcustLoginID)
        {
            return GetProductShopStockVarients(ssID, lcustLoginID);
        }

        /// <summary>
        ///  Get Stock Varients like color, size, dimension, material and shop associated with that varients, By ShopStockID
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        ///  <param name="lcustLoginID">Customer Login ID, to get wishlist Status</param>
        /// <returns></returns>
        public List<MobileShoppingCartViewModel> GetShopStockVarients(List<ShopStockIDs> ssID)
        {
            return GetProductShopStockVarientsForMobile(ssID);
        }

        /// <summary>
        /// Get list of sellers deals in product, by Product ID
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns></returns>
        public List<ProductSellersViewModel> GetSellersDealsInProduct(long pID)
        {
            return GetProductSellers(pID, 0, 0);
        }

        /// <summary>
        /// Get list of sellers deals in product, city
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="cID">City ID</param>
        /// <returns></returns>
        public List<ProductSellersViewModel> GetSellersDealsInProduct(long pID, long cID, int? fID = null)////added  int? fID For Mutiple MCO
        {
            return GetProductSellers(pID, cID, 0, fID);////added  fID
        }

        /// <summary>
        /// Get list of sellers deals in product, city, area
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="cID">City ID</param>
        /// <param name="lID">Location i.e Area ID</param>
        /// <returns></returns>
        public List<ProductSellersViewModel> GetSellersDealsInProduct(long pID, long cID, long lID, int? fID = null)////added int? fID For Mutiple MCO
        {
            return GetProductSellers(pID, cID, lID, fID);////added fID
        }

        /// <summary>
        /// get simillar products either by Shop or Globally
        /// </summary>
        /// <param name="searchSimilarProducts"></param>
        /// <returns></returns>
        public RelatedProductsViewModel GetSimillarProducts(SearchSimilarProductViewModel searchSimilarProducts)
        {
            return GetSimillarCategoryProducts(searchSimilarProducts);

        }
        /// <summary>
        /// Get the list of products, which are frequently buyed with provided Product
        /// </summary>
        /// <param name="searchFrequentlyBuyedProductViewModel">searchFrequentlyBuyedProductViewModel object</param>
        /// <returns></returns>
        public RelatedProductsViewModel GetFrequentlyBuyedProducts(SearchFrequentlyBuyedProductViewModel searchFrequentlyBuyedProductViewModel)
        {
            return GetFrequentlyBuyedTogether(searchFrequentlyBuyedProductViewModel);
        }

        /// <summary>
        /// Get Components Rate list and Display text for Component Destribution
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        /// <param name="shID">Shop ID</param>
        /// <returns></returns>
        public StockComponentsViewModel GetStockComponentDetails(long ssID, long shID)
        {

            /*For Dependent Component like making charges ComponentUnitID should 1(Qty) and component weight should be NULL*/
            return GetStockComponents(ssID, shID);
        }

        /// <summary>
        /// get products specification details
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns></returns>
        public IEnumerable<ProductTechnicalSpecificationViewModel> GetTechnicalSpecifications(long pID)
        {
            return GetProductTechnicalSpecifications(pID);
        }

        /// <summary>
        /// Get product general description file path. e.g. ../../description.html
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns>html file path</returns>
        public string GetGeneralDescription(long pID)
        {
            //bool isPathExists = false;
            //string filePath = string.Empty;
            ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
            string existsFilePath = string.Empty;
            string DescContents = string.Empty;
            // existsFilePath = imgPath.IMAGE_HTTP + "/Content/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/" + pID + "/description.html";
            existsFilePath = imgPath.IMAGE_HTTP + "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pID + "/description.html";
            if (IsFileExists(existsFilePath))
            {
                return existsFilePath;
            }
            else
            {
                return string.Empty;
            }
            //DescContents = GetProductGenDescription(existsFilePath);
            //return DescContents;
        }

        //Overload by Tejaswee
        public string GetGeneralDescription(long pID, bool isWeb)
        {
            string DescContents = string.Empty;
            if (isWeb)
            {
                ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                string existsFilePath = string.Empty;
                existsFilePath = imgPath.IMAGE_HTTP + "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pID + "/description.html";
                DescContents = GetProductGenDescription(existsFilePath);
            }
            return DescContents;

        }

        public string GetProductGenDescription(string path)
        {
            try
            {


                System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(path));

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


        #region Private Methods
        /// <summary>
        /// Get the list of components used in ShopStock ID from database using Stored procedure
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        /// <param name="shID">Shop ID</param>
        /// <returns></returns>
        public DataTable GetStockComponentList(long ssID, long shID)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = string.Empty;
            query = "[Select_Stock_Components]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StockID", ssID);
            cmd.Parameters.AddWithValue("@ShopID", shID);

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);
                }
            }
            return dt;
        }
        /// <summary>
        /// Get the list of components used in shopStock ID and Todays rate list for these components
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        /// <param name="shID">Shop ID</param>
        /// <returns></returns>
        private StockComponentsViewModel GetStockComponents(long ssID, long shID)
        {
            StockComponentsViewModel stk = new StockComponentsViewModel();
            DataTable dt = GetStockComponentList(ssID, shID);
            stk.TodaysRate = GetTodaysRateList(dt);
            stk.ComponentValue = GetStockComponentValueList(dt, true);
            return stk;
        }
        /// <summary>
        /// Collect Todays rate list from datatable
        /// </summary>
        /// <param name="dt">Datatable containg the component list and their todays rate</param>
        /// <returns></returns>
        private List<DisplayCompTodaysRateViewModel> GetTodaysRateList(DataTable dt)
        {
            List<DisplayCompTodaysRateViewModel> lRates = new List<DisplayCompTodaysRateViewModel>();
            return lRates = (from DataRow dr in dt.Rows
                             select new DisplayCompTodaysRateViewModel()
                             {
                                 ComponentID = Convert.ToInt32(dr["ComponentID"]),
                                 DisplayName = dr["DisplayName"].ToString()

                             }).ToList();

        }
        /// <summary>
        /// Get the distribution of Components used in Shopstock ID in string format, to be displayed on preview item page
        /// e.g Gold 22 Kt(22 Gm)$Rs.25,000 + Making Charges on  Gold 22 Kt$Rs. 15,00 + VAT$Rs. 312
        /// </summary>
        /// <param name="dt">Datatable containg the component list and their todays rate</param>
        /// <param name="showCmpWeight">bool value, whether you want to show weight in bracket</param>
        /// <returns></returns>
        private StockComponentsValueViewModel GetStockComponentValueList(DataTable dt, bool showCmpWeight)
        {

            StockComponentsValueViewModel lCmpValue = new StockComponentsValueViewModel();
            lCmpValue.DisplayText = GetComponentList(dt, showCmpWeight);

            return lCmpValue;
        }
        /// <summary>
        /// generate String like Gold 22 Kt(22 Gm)$Rs.25,000  +  Making Charges on  Gold 22 Kt$Rs. 15,00  +  VAT$Rs. 312, using datatable
        /// </summary>
        /// <param name="dt">Datatable containg the component list and their todays rate</param>
        /// <param name="showCmpWeight">bool value, whether you want to show weight in bracket</param>
        /// <returns></returns>
        private string GetComponentList(DataTable dt, bool showCmpWeight)
        {

            string cmpList = string.Empty;
            string depCmpList = string.Empty;
            decimal vatperValue = 0, compTotalExcludingVat = 0;
            decimal perUnitPercent = 0, perUnitPrice = 0;

            try
            {
                // All Components Excluding Vat
                DataView dvCmplComponents = new DataView(dt);
                dvCmplComponents.RowFilter = "[ComponentName] <> 'VAT'";

                if (showCmpWeight)
                {

                    //with weight ie showCmpWeight= true
                    foreach (DataRow dr in dvCmplComponents.ToTable().Rows)
                    {
                        //first calculate for un dependent component
                        if (dr["ComponentWeight"] != DBNull.Value)
                        {
                            //e.g. Gold 22 Kt

                            //total = weight* per unit rs of component
                            compTotalExcludingVat = compTotalExcludingVat + (Convert.ToDecimal(dr["ComponentWeight"]) * Convert.ToDecimal(dr["PerUnitRateInRs"]));
                            //generate display string like Gold 22 Kt(22 Gm)$Rs.25,000  +  Making Charges on  Gold 22 Kt$Rs. 15,00  +  VAT$Rs. 312
                            cmpList = cmpList + " + " + dr["ComponentName"].ToString() + " (" + Math.Round(Convert.ToDecimal(dr["ComponentWeight"]), 2).ToString() + " " + dr["UnitName"].ToString() + ")" + "$Rs. " + Math.Round(Convert.ToDecimal(dr["ComponentWeight"]) * Convert.ToDecimal(dr["PerUnitRateInRs"]), 2).ToString();

                        }
                        else
                        {
                            //e.g. Making Charges (is 10% of per unit weight of gold used)
                            decimal.TryParse(dr["PerUnitRateInPer"].ToString(), out perUnitPercent);
                            decimal.TryParse(dr["PerUnitRateInRs"].ToString(), out perUnitPrice);
                            if (perUnitPercent > 0)
                            {
                                // if value given in percent 
                                depCmpList = depCmpList + " + " + dr["ComponentName"].ToString() + "$" + dr["PerUnitRateInPer"] + "$" + dr["DependentOnComponentID"] + "$" + dr["IsCompulsory"] + "$PERCENT";
                            }
                            else if (perUnitPrice > 0)
                            {
                                // if value given in Rs
                                depCmpList = depCmpList + " + " + dr["ComponentName"].ToString() + "$" + dr["PerUnitRateInRs"] + "$" + dr["DependentOnComponentID"] + "$" + dr["IsCompulsory"] + "$PRICE";
                            }
                        }
                    }
                }
                else
                {   //follow above procedure, but dont include component weight in display string 
                    //Gold 22 Kt$Rs.25,000  +  Making Charges on  Gold 22 Kt$Rs. 15,00  +  VAT$Rs. 312
                    //without weight
                    foreach (DataRow dr in dvCmplComponents.ToTable().Rows)
                    {
                        if (dr["ComponentWeight"] != DBNull.Value)
                        {// not a dependent component

                            compTotalExcludingVat = compTotalExcludingVat + (Convert.ToDecimal(dr["ComponentWeight"]) * Convert.ToDecimal(dr["PerUnitRateInRs"]));

                            cmpList = cmpList + " + " + dr["ComponentName"].ToString() + "$Rs. " + Math.Round(Convert.ToDecimal(dr["ComponentWeight"]) * Convert.ToDecimal(dr["PerUnitRateInRs"]), 2).ToString();

                        }
                        else
                        { //a dependent component
                            decimal.TryParse(dr["PerUnitRateInPer"].ToString(), out perUnitPercent);
                            decimal.TryParse(dr["PerUnitRateInRs"].ToString(), out perUnitPrice);
                            if (perUnitPercent > 0)
                            {
                                depCmpList = depCmpList + " + " + dr["ComponentName"].ToString() + "$" + dr["PerUnitRateInPer"] + "$" + dr["DependentOnComponentID"] + "$" + dr["IsCompulsory"] + "$PERCENT";
                            }
                            else if (perUnitPrice > 0)
                            {
                                depCmpList = depCmpList + " + " + dr["ComponentName"].ToString() + "$" + dr["PerUnitRateInRs"] + "$" + dr["DependentOnComponentID"] + "$" + dr["IsCompulsory"] + "$PRICE";
                            }
                        }
                    }
                }

                if (depCmpList.Trim().Length > 0)
                {
                    //depCmpList contains string like 
                    //if dependent in terms of %  : componentname$PerUnitRateInPer$DependentOnComponentID$IsCompulsory$PERCENT
                    //if dependent in terms of Rs : componentname$PerUnitRateInPer$DependentOnComponentID$IsCompulsory$PRICE

                    //remove space Plus space (3 charecters) from Display string like  Gold 22 Kt$Rs.25,000  +  Making Charges on  Gold 22 Kt$Rs. 15,00  +  VAT$Rs. 312
                    depCmpList = depCmpList.Remove(0, 3);

                    //first split by +
                    string[] arr = depCmpList.Split('+');
                    foreach (string str in arr)
                    {
                        //then split by $
                        string[] cmp = str.Split('$');

                        DataView dv = new DataView(dt);
                        dv.RowFilter = "ComponentID='" + Convert.ToInt32(cmp[2]) + "'";
                        DataTable dtr = new DataTable();
                        dtr = dv.ToTable();

                        if (cmp[3].Equals("1") ? true : false && cmp[4].Equals("PERCENT"))
                        {
                            //if dependent in terms of % 
                            if (!dtr.Rows[0]["ComponentName"].ToString().Equals("VAT"))
                            {//give change
                                compTotalExcludingVat = compTotalExcludingVat + ((Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(dtr.Rows[0]["PerUnitRateInRs"]) * Convert.ToDecimal(cmp[1]) / 100));
                            }

                            if (showCmpWeight)
                            {//with weight
                                cmpList = cmpList + " + " + cmp[0] + " (" + Math.Round(Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]), 2).ToString() + " " + dtr.Rows[0]["UnitName"].ToString() + ")$" + "Rs. " + Math.Round((Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(dtr.Rows[0]["PerUnitRateInRs"]) * Convert.ToDecimal(cmp[1]) / 100), 2).ToString();
                            }
                            else
                            {//without weight
                                cmpList = cmpList + " + " + cmp[0] + "$" + "Rs. " + Math.Round((Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(dtr.Rows[0]["PerUnitRateInRs"]) * Convert.ToDecimal(cmp[1]) / 100), 2).ToString();
                            }
                        }
                        else if (Convert.ToBoolean(cmp[3]) && cmp[4] == "PRICE")
                        {
                            if (!dtr.Rows[0]["ComponentName"].ToString().Equals("VAT"))
                            {
                                //give change
                                compTotalExcludingVat = compTotalExcludingVat + (Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(cmp[1]));
                            }

                            if (showCmpWeight)
                            {//with weight
                                cmpList = cmpList + " + " + cmp[0] + " (" + Math.Round(Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]), 2).ToString() + " " + dtr.Rows[0]["UnitName"].ToString() + ")$" + "Rs. " + Math.Round((Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(cmp[1])), 2).ToString();
                            }
                            else
                            {//without weight
                                cmpList = cmpList + " + " + cmp[0] + "$" + "Rs. " + Math.Round((Convert.ToDecimal(dtr.Rows[0]["ComponentWeight"]) * Convert.ToDecimal(cmp[1])), 2).ToString();
                            }
                        }
                    }
                }
                //Now Add Vat Component
                dvCmplComponents = new DataView(dt);
                dvCmplComponents.RowFilter = "[IsCompulsory] = 1 And [ComponentName] = 'VAT'";
                //now calculate Vat value
                foreach (DataRow dr in dvCmplComponents.ToTable().Rows)
                {
                    if (Convert.ToBoolean(dr["IsCompulsory"]))
                    {

                        vatperValue = Math.Round((compTotalExcludingVat * Convert.ToDecimal(dr["PerUnitRateInPer"]) / 100), 2);

                        cmpList = cmpList + " + " + dr["ComponentName"].ToString() + "(" + Math.Round(Convert.ToDecimal(dr["PerUnitRateInPer"]), 2).ToString() + "%)$Rs. " + vatperValue;
                    }

                }

                //remove space Plus space (3 charecters) from Display string like  Gold 22 Kt$Rs.25,000  +  Making Charges on  Gold 22 Kt$Rs. 15,00  +  VAT$Rs. 312
                if (cmpList.Trim().Length > 0)
                    cmpList = cmpList.Remove(0, 3);
            }

            catch (Exception exception)
            {
                throw exception;
            }
            return cmpList;
        }
        /// <summary>
        /// get the list of frequently buyed together product for given criteria like product ID, CategoryID etc
        /// </summary>
        /// <param name="searchFrequentlyBuyedProductViewModel"></param>
        /// <returns></returns>
        private RelatedProductsViewModel GetFrequentlyBuyedTogetherProducts(SearchFrequentlyBuyedProductViewModel searchFrequentlyBuyedProductViewModel)
        {
            return GetFrequentlyBuyedTogether(searchFrequentlyBuyedProductViewModel);
        }

        /// <summary>
        /// get List of sellers deals in ProductID  whose shop address belongs to provided city ID and Area ID
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="cID">City ID</param>
        /// <param name="aID">Area Stock ID</param>
        /// <returns></returns>
        private List<ProductSellersViewModel> GetProductSellers(long pID, long cID, long aID, int? fID = null)////added int? fID For Multiple MCO
        {
            List<ProductSellersViewModel> lSellers = new List<ProductSellersViewModel>();
            PriceAndOffers ofr = new PriceAndOffers(System.Web.HttpContext.Current.Server);
            // if (fID.Length==0)
            // { 
            //search by prodct ID only
            // if (cID == 0 && aID == 0)////hide
            if (cID == 0 && aID == 0 && fID == null)
            {
                var lProductSellers = (
                                from sp in db.ShopProducts
                                join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                join s in db.Shops on sp.ShopID equals s.ID
                                where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true
                                      && ss.StockStatus == true && ss.Qty > 0 // if stock is not available then no seller should be visible on preview page : condition added by snehal on 25/02/2016 
                                group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name } into temp
                                select new ProductSellersViewModel
                                {

                                    ProductID = temp.Key.ProductID,
                                    ShopID = temp.Key.ShopID,
                                    ShopName = temp.Key.Name,
                                    ShopProductID = temp.Key.ShopProductID,
                                    MRP = temp.Min(x => x.MRP),
                                    SaleRate = temp.Min(x => x.RetailerRate)
                                }).ToList();
                //get the other details like varients
                foreach (var item in lProductSellers)
                {
                    item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();
                    var shopStockDetails = (from ss in db.ShopStocks
                                            join U in db.Units on ss.PackUnitID equals U.ID
                                            join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                            join s in db.Sizes on pv.SizeID equals s.ID
                                            join D in db.Dimensions on pv.DimensionID equals D.ID
                                            join C in db.Colors on pv.ColorID equals C.ID
                                            join M in db.Materials on pv.MaterialID equals M.ID
                                            where ss.ID == item.ShopStockID
                                            select new
                                            {
                                                SizeName = s.Name,
                                                DimensionName = D.Name,
                                                MaterialName = M.Name,
                                                ColorName = C.Name,
                                                StockQty = ss.Qty,
                                                HtmlColorCode = C.HtmlCode,
                                                PackSize = ss.PackSize,
                                                PackSizeUnit = U.Name

                                            }).FirstOrDefault();

                    if (shopStockDetails != null)
                    {

                        item.Size = shopStockDetails.SizeName;
                        item.Color = shopStockDetails.ColorName;
                        item.Dimension = shopStockDetails.DimensionName;
                        item.Material = shopStockDetails.MaterialName;
                        item.PackSize = shopStockDetails.PackSize;
                        item.PackUnit = shopStockDetails.PackSizeUnit;
                        item.StockQty = shopStockDetails.StockQty;
                        item.HtmlColorCode = shopStockDetails.HtmlColorCode;
                    }
                    item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
                    item.LogoPath = ImageDisplay.LoadShopLogo(item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                }
                lSellers = lProductSellers;
            }
            //Get sellers deals in provided City
            // else if (cID > 0 && aID == 0)////hide
            else if (cID > 0 && aID == 0 && fID == null)
            {
                var lProductSellers = (
                              from sp in db.ShopProducts
                              join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                              join s in db.Shops on sp.ShopID equals s.ID
                              join pin in db.Pincodes on s.PincodeID equals pin.ID
                              where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true && pin.CityID == cID
                                    && ss.StockStatus == true && ss.Qty > 0  // if stock is not available then no seller should be visible on preview page : condition added by snehal on 25/02/2016 
                              group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name, ss.ID } into temp
                              select new ProductSellersViewModel
                              {
                                  ProductID = temp.Key.ProductID,
                                  ShopID = temp.Key.ShopID,
                                  ShopName = temp.Key.Name,
                                  ShopProductID = temp.Key.ShopProductID,
                                  MRP = temp.Min(x => x.MRP),
                                  SaleRate = temp.Min(x => x.RetailerRate),
                                  ShopStockID = temp.Key.ID,
                              }).ToList();
                foreach (var item in lProductSellers)
                {
                    //Sujata
                    //item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();

                    //Tejaswee
                    // item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();

                    var shopStockDetails = (from ss in db.ShopStocks
                                            join U in db.Units on ss.PackUnitID equals U.ID
                                            join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                            join s in db.Sizes on pv.SizeID equals s.ID
                                            join D in db.Dimensions on pv.DimensionID equals D.ID
                                            join C in db.Colors on pv.ColorID equals C.ID
                                            join M in db.Materials on pv.MaterialID equals M.ID
                                            where ss.ID == item.ShopStockID
                                            select new
                                            {
                                                SizeName = s.Name,
                                                DimensionName = D.Name,
                                                MaterialName = M.Name,
                                                ColorName = C.Name,
                                                StockQty = ss.Qty,
                                                HtmlColorCode = C.HtmlCode,
                                                PackSize = ss.PackSize,
                                                PackSizeUnit = U.Name


                                            }).FirstOrDefault();

                    if (shopStockDetails != null)
                    {

                        item.Size = shopStockDetails.SizeName;
                        item.Color = shopStockDetails.ColorName;
                        item.Dimension = shopStockDetails.DimensionName;
                        item.Material = shopStockDetails.MaterialName;
                        item.PackSize = shopStockDetails.PackSize;
                        item.PackUnit = shopStockDetails.PackSizeUnit;
                        item.StockQty = shopStockDetails.StockQty;
                        item.HtmlColorCode = shopStockDetails.HtmlColorCode;
                    }
                    item.LogoPath = ImageDisplay.LoadShopLogo(item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                    item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
                }
                lSellers = lProductSellers;
            }
            else if (fID != null && aID == 0)////adde
            {
                var lProductSellers = (
                             from sp in db.ShopProducts
                             join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                             join s in db.Shops on sp.ShopID equals s.ID
                             join pin in db.Pincodes on s.PincodeID equals pin.ID
                             where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true
                                  && s.FranchiseID == fID// added by ashish
                                   && ss.StockStatus == true && ss.Qty > 0  // if stock is not available then no seller should be visible on preview page : condition added by snehal on 25/02/2016 
                             group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name, ss.ID } into temp
                             select new ProductSellersViewModel
                             {
                                 ProductID = temp.Key.ProductID,
                                 ShopID = temp.Key.ShopID,
                                 ShopName = temp.Key.Name,
                                 ShopProductID = temp.Key.ShopProductID,
                                 MRP = temp.Min(x => x.MRP),
                                 SaleRate = temp.Min(x => x.RetailerRate),
                                 ShopStockID = temp.Key.ID,
                             }).ToList();
                foreach (var item in lProductSellers)
                {
                    //Sujata
                    //item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();

                    //Tejaswee
                    // item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();

                    var shopStockDetails = (from ss in db.ShopStocks
                                            join U in db.Units on ss.PackUnitID equals U.ID
                                            join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                            join s in db.Sizes on pv.SizeID equals s.ID
                                            join D in db.Dimensions on pv.DimensionID equals D.ID
                                            join C in db.Colors on pv.ColorID equals C.ID
                                            join M in db.Materials on pv.MaterialID equals M.ID
                                            where ss.ID == item.ShopStockID
                                            select new
                                            {
                                                SizeName = s.Name,
                                                DimensionName = D.Name,
                                                MaterialName = M.Name,
                                                ColorName = C.Name,
                                                StockQty = ss.Qty,
                                                HtmlColorCode = C.HtmlCode,
                                                PackSize = ss.PackSize,
                                                PackSizeUnit = U.Name


                                            }).FirstOrDefault();

                    if (shopStockDetails != null)
                    {

                        item.Size = shopStockDetails.SizeName;
                        item.Color = shopStockDetails.ColorName;
                        item.Dimension = shopStockDetails.DimensionName;
                        item.Material = shopStockDetails.MaterialName;
                        item.PackSize = shopStockDetails.PackSize;
                        item.PackUnit = shopStockDetails.PackSizeUnit;
                        item.StockQty = shopStockDetails.StockQty;
                        item.HtmlColorCode = shopStockDetails.HtmlColorCode;
                    }
                    item.LogoPath = ImageDisplay.LoadShopLogo(item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                    item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
                }
                lSellers = lProductSellers;
            }
            //get Sellers in perticular area only
            else if (aID > 0)
            {
                var lProductSellers = (
                              from sp in db.ShopProducts
                              join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                              join s in db.Shops on sp.ShopID equals s.ID
                              join pin in db.Pincodes on s.PincodeID equals pin.ID
                              join ar in db.Areas on pin.ID equals ar.PincodeID
                              where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true && ar.ID == aID
                              && ss.StockStatus == true && ss.Qty > 0  // if stock is not available then no seller should be visible on preview page : condition added by snehal on 25/02/2016 
                              group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name } into temp
                              select new ProductSellersViewModel
                              {
                                  ProductID = temp.Key.ProductID,
                                  ShopID = temp.Key.ShopID,
                                  ShopName = temp.Key.Name,
                                  ShopProductID = temp.Key.ShopProductID,
                                  MRP = temp.Min(x => x.MRP),
                                  SaleRate = temp.Min(x => x.RetailerRate),

                              }).ToList();
                foreach (var item in lProductSellers)
                {
                    item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();
                    var shopStockDetails = (from ss in db.ShopStocks
                                            join U in db.Units on ss.PackUnitID equals U.ID
                                            join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                            join s in db.Sizes on pv.SizeID equals s.ID
                                            join D in db.Dimensions on pv.DimensionID equals D.ID
                                            join C in db.Colors on pv.ColorID equals C.ID
                                            join M in db.Materials on pv.MaterialID equals M.ID
                                            where ss.ID == item.ShopStockID
                                            select new
                                            {
                                                SizeName = s.Name,
                                                DimensionName = D.Name,
                                                MaterialName = M.Name,
                                                ColorName = C.Name,
                                                StockQty = ss.Qty,
                                                HtmlColorCode = C.HtmlCode,
                                                PackSize = ss.PackSize,
                                                PackSizeUnit = U.Name

                                            }).FirstOrDefault();

                    if (shopStockDetails != null)
                    {

                        item.Size = shopStockDetails.SizeName;
                        item.Color = shopStockDetails.ColorName;
                        item.Dimension = shopStockDetails.DimensionName;
                        item.Material = shopStockDetails.MaterialName;
                        item.PackSize = shopStockDetails.PackSize;
                        item.PackUnit = shopStockDetails.PackSizeUnit;
                        item.StockQty = shopStockDetails.StockQty;
                        item.HtmlColorCode = shopStockDetails.HtmlColorCode;
                    }
                    item.LogoPath = ImageDisplay.LoadShopLogo(item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                    item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
                }
                lSellers = lProductSellers;
            }

            return lSellers;

        }

        /// <summary>
        /// get varient details for provided shop stock Id
        /// Used in API also
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        ///  <param name="lcustLoginID">Customer Login ID</param>
        /// <returns></returns>
        private ShopProductVarientViewModel GetProductShopStockVarients(long ssID, long? lcustLoginID)
        {

            //get Varient details by shop stock Id
            ShopProductVarientViewModel lVarientList = new ShopProductVarientViewModel();

            lVarientList = (
                    from pv in db.ProductVarients
                    join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                    join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                    join p in db.Products on sp.ProductID equals p.ID
                    join sh in db.Shops on sp.ShopID equals sh.ID
                    join cl in db.Colors on pv.ColorID equals cl.ID
                    join sz in db.Sizes on pv.SizeID equals sz.ID
                    join dm in db.Dimensions on pv.DimensionID equals dm.ID
                    join mt in db.Materials on pv.MaterialID equals mt.ID
                    join Unt in db.Units on ss.PackUnitID equals Unt.ID
                    where ss.ID == ssID
                    && p.IsActive == true
                    && pv.IsActive == true
                    && ss.IsActive == true
                    && sp.IsActive == true
                    && sh.IsLive == true

                    select new ShopProductVarientViewModel
                    {
                        StockThumbPath = string.Empty,
                        ProductVarientID = ss.ProductVarientID,
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
                        WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                        IsAddedInWishlist = false,
                        ActualWeight = p.WeightInGram,
                        VolumetricWeight = p.LengthInCm * p.BreadthInCm * p.HeightInCm,
                        StockQty = ss.Qty,
                        StockStatus = ss.StockStatus,
                        PackSize = ss.PackSize,
                        PackUnitName = Unt.Name,
                        CategoryID = p.CategoryID,
                        IsInclusiveOfTax = ss.IsInclusiveOfTax,
                        BusinessPointPerUnit = ss.BusinessPoints //Added by Zubair for MLM on 04/01/2018


                    }).FirstOrDefault();

            if (lVarientList != null)
            {
                //get Product thumbnail

                //lVarientList.StockThumbPath = ImageDisplay.LoadProductThumbnails(lVarientList.ProductID, lVarientList.ColorName.Trim().Equals("N/A") || string.IsNullOrEmpty(lVarientList.ColorName.Trim())  ? "Default" : lVarientList.ColorName.Trim(),
                //        string.Empty, ProductUpload.THUMB_TYPE.MM);
                lVarientList.StockThumbPath = ImageDisplay.SetProductThumbPath(lVarientList.ProductID, lVarientList.ColorName.Trim().Equals("N/A") || string.IsNullOrEmpty(lVarientList.ColorName.Trim()) ? "Default" : lVarientList.ColorName.Trim(),
                        string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                //get Wishlist status, if Customer 
                if (lcustLoginID != null && lcustLoginID > 0)
                {
                    var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == lVarientList.ShopStockID).ToList();

                    if (lContainsWishlist.Count > 0)
                        lVarientList.IsAddedInWishlist = true;
                }

            }
            return lVarientList;
        }

        public decimal getCasbackPointsOnProduct(long WarehouseStockId)
        {
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("GETCashbackPointONProduct", con);
                sqlComm.Parameters.AddWithValue("@WarehouseStockID", SqlDbType.BigInt).Value = WarehouseStockId;
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return Convert.ToDecimal(dt.Rows[0][0]);
            }
            catch
            {
                return 0;
            }
        }
        public decimal getCasbackPointsOnProductFromWarehouse(long WarehouseStockId)
        {
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("GETCashbackPointONProductFromWarehouse", con);
                sqlComm.Parameters.AddWithValue("@WarehouseStockID", SqlDbType.BigInt).Value = WarehouseStockId;
                sqlComm.Parameters.AddWithValue("@CashbackPoints", SqlDbType.Decimal).Direction = ParameterDirection.Output;
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                decimal CashBackPoint = Convert.ToDecimal(sqlComm.Parameters["@CashbackPoints"].Value);
                return CashBackPoint;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        /// <summary>
        /// get varient details for provided shop stock Id
        /// Used in API also
        /// </summary>
        /// <param name="ssID">Shop Stock ID</param>
        ///  <param name="lcustLoginID">Customer Login ID</param>
        /// <returns></returns>
        private List<MobileShoppingCartViewModel> GetProductShopStockVarientsForMobile(List<ShopStockIDs> ssID)
        {

            //get Varient details by shop stock Id
            List<MobileShoppingCartViewModel> lVarientList = new List<MobileShoppingCartViewModel>();
            List<long> ShopStockIDList = new List<long>(); // for taxation 


            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add("ShopStockID");
            foreach (var val in ssID.ToList())
            {
                long v = Convert.ToInt64(val.ssID);
                DataRow dr = lDataTable.NewRow();
                dr[0] = v;
                lDataTable.Rows.Add(dr);
                ShopStockIDList.Add(v);
            }

            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(lDataTable);
            dt = dbOpr.GetRecords("ShoppingCartForMobile", paramValues);

            /*PURPOSE :- Tax Management in MobileVIEW ON PRODUCT
            * DATE:- 28-03-2016
            * DEVELOPED BY :- PRADNYAKAR BADGE
            */
            List<CalulatedTaxesRecord> ls = new List<CalulatedTaxesRecord>();
            TaxationManagement obj = new TaxationManagement(config.DB_CONNECTION);


            ls = obj.CalculateTaxForMultipalProduct(ShopStockIDList);
            /**********************************************************************/
            lVarientList = (from n in dt.AsEnumerable()
                            select new MobileShoppingCartViewModel
                            {
                                StockThumbPath = ImageDisplay.LoadProductThumbnails(n.Field<long>("ProductID"), n.Field<string>("ColorName").Trim().Equals("N/A") || string.IsNullOrEmpty(n.Field<string>("ColorName").Trim()) ? "Default" : n.Field<string>("ColorName").Trim(),
                                string.Empty, ProductUpload.THUMB_TYPE.MM),
                                ShopID = n.Field<long>("ShopID"),
                                ProductID = n.Field<long>("ProductID"),
                                ProductName = n.Field<string>("ProductName"),
                                SaleRate = n.Field<decimal>("SaleRate"),
                                MRP = n.Field<decimal>("MRP"),
                                ShopName = n.Field<string>("ShopName"),
                                ShopStockID = n.Field<long>("ShopStockID"),
                                StockQty = n.Field<int>("StockQty"),
                                //StockStatus = n.Field<int>("StockStatus") == 1 ? true : false,
                                DimensionName = n.Field<string>("Dimension"),
                                ColorName = n.Field<string>("ColorName"),
                                MaterialName = n.Field<string>("Maretrial"),
                                SizeName = n.Field<string>("Size"),
                                PackSize = n.Field<decimal>("PackSize"),
                                ActualWeight = n.Field<int>("ActualWeight"),
                                VolumetricWeight = n.Field<int>("VolumetricWeight"),
                                CategoryID = n.Field<int>("CategoryID"),
                                RetailPoint = n.Field<decimal>("RetailPoint"), //Sonali
                                WareHouseStockId = n.Field<long?>("WareHouseStockId"), //Sonali
                                CashbackPoint = n.Field<decimal>("CashbackPoints"),
                                /* Taxation Calculation
                                 * Added By Pradnyakar Badge 
                                 * 28-03-2016
                                 */
                                TaxesOnProduct = ls.Where(x => x.ShopStockID == n.Field<long>("ShopStockID")).Count() > 0 ? ls.Where(x => x.ShopStockID == n.Field<long>("ShopStockID")).ToList() : null
                            }).ToList();

            //Started By sonali on 13-02-2019
            if (lVarientList != null && lVarientList.Count > 0)
            {
                foreach (var item in lVarientList)
                {
                    item.ProductName = item.ProductName.Replace("+", " ");
                }
            }
            //Ended By sonali on 13-02-2019 




            return lVarientList;
        }





        /// <summary>
        /// get varient details for provided shop stock Id,Product ID,Shop ID,
        /// Used in API also
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="sID">Shop ID</param>
        /// <param name="ssID">Shop Stock ID</param>
        ///  <param name="lcustLoginID">Customer Login ID</param>
        /// <returns></returns>
        private List<ShopProductVarientViewModel> GetProductStockVarients(long pID, long sID, long ssID, long? lcustLoginID)
        {

            List<ShopProductVarientViewModel> lVarientList = new List<ShopProductVarientViewModel>();
            //get varients by shop ID
            if (sID > 0)
            {
                lVarientList = (
                    from pv in db.ProductVarients
                    join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                    join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                    join p in db.Products on sp.ProductID equals p.ID
                    join sh in db.Shops on sp.ShopID equals sh.ID
                    join cl in db.Colors on pv.ColorID equals cl.ID
                    join sz in db.Sizes on pv.SizeID equals sz.ID
                    join dm in db.Dimensions on pv.DimensionID equals dm.ID
                    join mt in db.Materials on pv.MaterialID equals mt.ID
                    join Unt in db.Units on ss.PackUnitID equals Unt.ID
                    where sp.ShopID == sID
                    where pv.ProductID == pID
                    where pv.IsActive == true
                    where ss.IsActive == true
                    where sp.IsActive == true
                    where sh.IsLive == true
                    where ss.RetailerRate > 0 //Added by Rumana

                    select new ShopProductVarientViewModel
                    {
                        StockThumbPath = string.Empty,
                        ProductVarientID = ss.ProductVarientID,
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
                        WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                        IsAddedInWishlist = false,
                        StockQty = ss.Qty,
                        StockStatus = ss.StockStatus,
                        PackSize = ss.PackSize,
                        PackUnitName = Unt.Name,
                        IsPriority = ss.IsPriority
                    }).OrderBy(x => x.IsPriority).OrderByDescending(x => x.StockQty).ToList(); //Added by Rumana .OrderByDescending(x => x.StockQty)
                foreach (var item in lVarientList)
                {
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    //item.StockImageList = GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);

                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();



                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        //string fConnectionString="";
                        //TaxationManagement objTaxationManagement = new TaxationManagement(fConnectionString);
                        //item.lCalulatedTaxesRecord = objTaxationManagement.CalculateTaxForProduct(item.ShopStockID);

                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID //&& ft.FranchiseID == fID
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }

                }


                return lVarientList;
            }
            //get varients by product ID
            else if (pID > 0)
            {
                lVarientList = (
                 from pv in db.ProductVarients
                 join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                 join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                 join p in db.Products on sp.ProductID equals p.ID
                 join sh in db.Shops on sp.ShopID equals sh.ID
                 join cl in db.Colors on pv.ColorID equals cl.ID
                 join sz in db.Sizes on pv.SizeID equals sz.ID
                 join dm in db.Dimensions on pv.DimensionID equals dm.ID
                 join mt in db.Materials on pv.MaterialID equals mt.ID
                 join Unt in db.Units on ss.PackUnitID equals Unt.ID

                 where pv.ProductID == pID
                 where pv.IsActive == true
                 where ss.IsActive == true
                 where sp.IsActive == true
                 where sh.IsLive == true

                 select new ShopProductVarientViewModel
                 {
                     StockThumbPath = string.Empty,
                     ProductVarientID = ss.ProductVarientID,
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
                     WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                     IsAddedInWishlist = false,
                     StockQty = ss.Qty,
                     StockStatus = ss.StockStatus,
                     PackSize = ss.PackSize,
                     PackUnitName = Unt.Name,
                     IsPriority = ss.IsPriority
                 }).OrderBy(x => x.IsPriority).OrderByDescending(x => x.StockQty).ToList(); //Added by Rumana .OrderByDescending(x => x.StockQty)
                foreach (var item in lVarientList)
                {
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    //   item.StockImageList = GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();


                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID //&& ft.FranchiseID == fID
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive  //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }
                }
                return lVarientList;
            }
            //get varients by shop stock ID
            else
            {
                lVarientList = (
                   from pv in db.ProductVarients
                   join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                   join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                   join p in db.Products on sp.ProductID equals p.ID
                   join sh in db.Shops on sp.ShopID equals sh.ID
                   join cl in db.Colors on pv.ColorID equals cl.ID
                   join sz in db.Sizes on pv.SizeID equals sz.ID
                   join dm in db.Dimensions on pv.DimensionID equals dm.ID
                   join mt in db.Materials on pv.MaterialID equals mt.ID
                   join Unt in db.Units on ss.PackUnitID equals Unt.ID
                   where pv.ProductID == pID
                   where pv.IsActive == true
                   where ss.IsActive == true
                   where sp.IsActive == true
                   where sh.IsLive == true
                   where ss.ID == ssID

                   select new ShopProductVarientViewModel
                   {
                       StockThumbPath = string.Empty,
                       ProductVarientID = ss.ProductVarientID,
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
                       WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                       IsAddedInWishlist = false,
                       StockQty = ss.Qty,
                       StockStatus = ss.StockStatus,
                       PackSize = ss.PackSize,
                       PackUnitName = Unt.Name,
                       IsPriority = ss.IsPriority
                   }).OrderBy(x => x.IsPriority).ToList();

                foreach (var item in lVarientList)
                {
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);

                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();


                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID //&& ft.FranchiseID == fID
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive  //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }
                }

                return lVarientList;

            }


        }

        ////added By Mohit
        private List<ShopProductVarientViewModel> GetProductStockVarients(long pID, long sID, long ssID, long? lcustLoginID, int fID)////added long cID->int fID
        {

            List<ShopProductVarientViewModel> lVarientList = new List<ShopProductVarientViewModel>();
            //get varients by shop ID
            if (sID > 0)
            {
                lVarientList = (
                    from pv in db.ProductVarients
                    join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                    join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                    join p in db.Products on sp.ProductID equals p.ID
                    join sh in db.Shops on sp.ShopID equals sh.ID
                    join cl in db.Colors on pv.ColorID equals cl.ID
                    join sz in db.Sizes on pv.SizeID equals sz.ID
                    join dm in db.Dimensions on pv.DimensionID equals dm.ID
                    join mt in db.Materials on pv.MaterialID equals mt.ID
                    join Unt in db.Units on ss.PackUnitID equals Unt.ID
                    //-----------------------
                    join pin in db.Pincodes on sh.PincodeID equals pin.ID
                    where sp.ShopID == sID
                    where pv.ProductID == pID
                    where pv.IsActive == true
                    where ss.IsActive == true
                    where sp.IsActive == true
                    where sh.IsLive == true
                    //where pin.CityID == cID////hide
                    where sh.FranchiseID == fID ////added
                    where ss.RetailerRate > 0 //Added by Rumana 
                    //where sp.ShopID == sID
                    //where pv.ProductID == pID
                    //where pv.IsActive == true
                    //where ss.IsActive == true
                    //where sp.IsActive == true
                    //where sh.IsLive == true
                    //where pin.CityID == cID

                    select new ShopProductVarientViewModel
                    {
                        StockThumbPath = string.Empty,
                        ProductVarientID = ss.ProductVarientID,
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
                        WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                        IsAddedInWishlist = false,
                        StockQty = ss.Qty,
                        StockStatus = ss.StockStatus,
                        PackSize = ss.PackSize,
                        PackUnitName = Unt.Name,
                        IsPriority = ss.IsPriority,
                        BusinessPointPerUnit = ss.BusinessPoints, // Added by Zubair for BusinessPoint on 04/01/2018
                        CashbackPointPerUnit =ss.CashbackPoints,
                    }).OrderBy(x => x.IsPriority).OrderByDescending(x => x.StockQty).ToList(); //Added by Rumana .OrderByDescending(x => x.StockQty)
                foreach (var item in lVarientList)
                {
                    if (item.CashbackPointPerUnit > 0)
                    {
                        item.IsDisplayCB = 1;
                    }
                    else
                    {
                        item.IsDisplayCB = 0;
                    }
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    //item.StockImageList = GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);

                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();


                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID && ft.FranchiseID == fID  //condition added by harshada on 31/12/2016 ft.FranchiseID == fID 
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive  //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }
                }
                return lVarientList;
            }
            //get varients by product ID
            else if (pID > 0)
            {
                lVarientList = (
                 from pv in db.ProductVarients
                 join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                 join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                 join p in db.Products on sp.ProductID equals p.ID
                 join sh in db.Shops on sp.ShopID equals sh.ID
                 join cl in db.Colors on pv.ColorID equals cl.ID
                 join sz in db.Sizes on pv.SizeID equals sz.ID
                 join dm in db.Dimensions on pv.DimensionID equals dm.ID
                 join mt in db.Materials on pv.MaterialID equals mt.ID
                 join Unt in db.Units on ss.PackUnitID equals Unt.ID
                 //-----------------------
                 join pin in db.Pincodes on sh.PincodeID equals pin.ID

                 where pv.ProductID == pID
                 where pv.IsActive == true
                 where ss.IsActive == true
                 where sp.IsActive == true
                 where sh.IsLive == true
                 //where sh.Pincode.CityID == cID////hide
                 where sh.FranchiseID == fID ////added
                 where ss.RetailerRate > 0 //Added by Rumana
                 //where pin.CityID == cID

                 select new ShopProductVarientViewModel
                 {
                     StockThumbPath = string.Empty,
                     ProductVarientID = ss.ProductVarientID,
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
                     WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                     IsAddedInWishlist = false,
                     StockQty = ss.Qty,
                     StockStatus = ss.StockStatus,
                     PackSize = ss.PackSize,
                     PackUnitName = Unt.Name,
                     IsPriority = ss.IsPriority,
                     BusinessPointPerUnit = ss.BusinessPoints, // Added by Zubair for BusinessPoint on 04/01/2018
                     CashbackPointPerUnit = ss.CashbackPoints
                 }).OrderBy(x => x.IsPriority).OrderByDescending(x => x.StockQty).ToList(); //Added by Rumana .OrderByDescending(x => x.StockQty)

                foreach (var item in lVarientList)
                {
                    if (item.CashbackPointPerUnit > 0)
                    {
                        item.IsDisplayCB = 1;
                    }
                    else
                    {
                        item.IsDisplayCB = 0;
                    }
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    //item.StockImageList = GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);
                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();

                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID && ft.FranchiseID == fID //condition added by harshada on 31/12/2016 ft.FranchiseID == fID 
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive  //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }
                }
                return lVarientList;
            }
            //get varients by shop stock ID
            else
            {
                lVarientList = (
                   from pv in db.ProductVarients
                   join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                   join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                   join p in db.Products on sp.ProductID equals p.ID
                   join sh in db.Shops on sp.ShopID equals sh.ID
                   join cl in db.Colors on pv.ColorID equals cl.ID
                   join sz in db.Sizes on pv.SizeID equals sz.ID
                   join dm in db.Dimensions on pv.DimensionID equals dm.ID
                   join mt in db.Materials on pv.MaterialID equals mt.ID
                   join Unt in db.Units on ss.PackUnitID equals Unt.ID
                   //-----------------------
                   join pin in db.Pincodes on sh.PincodeID equals pin.ID

                   where pv.ProductID == pID
                   where pv.IsActive == true
                   where ss.IsActive == true
                   where sp.IsActive == true
                   where sh.IsLive == true
                   where ss.ID == ssID
                   // where pin.CityID == cID////hide
                   where sh.FranchiseID == fID ////added
                   where ss.RetailerRate > 0 //Added by Rumana
                   select new ShopProductVarientViewModel
                   {
                       StockThumbPath = string.Empty,
                       ProductVarientID = ss.ProductVarientID,
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
                       WarehouseStockID = ss.WarehouseStockID, //Added by Zubair for Inventory on 27-03-2018
                       IsAddedInWishlist = false,
                       StockQty = ss.Qty,
                       StockStatus = ss.StockStatus,
                       PackSize = ss.PackSize,
                       PackUnitName = Unt.Name,
                       IsPriority = ss.IsPriority,
                       BusinessPointPerUnit = ss.BusinessPoints, // Added by Zubair for BusinessPoint on 04/01/2018
                       CashbackPointPerUnit = ss.CashbackPoints
                   }).OrderBy(x => x.IsPriority).OrderByDescending(x => x.StockQty).ToList(); //Added by Rumana .OrderByDescending(x => x.StockQty)


                foreach (var item in lVarientList)
                {
                    if (item.CashbackPointPerUnit > 0)
                    {
                        item.IsDisplayCB = 1;
                    }
                    else
                    {
                        item.IsDisplayCB = 0;
                    }
                    //if (item.ColorName != "N/A")
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    if (item.ColorName != "N/A")
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    item.StockImageList = ImageDisplay.GetStockImages(pID, string.IsNullOrEmpty(item.ColorName) || item.ColorName.Equals("N/A") ? "Default" : item.ColorName);

                    if (lcustLoginID != null && lcustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == lcustLoginID && x.ShopStockID == item.ShopStockID).ToList();

                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                    item.PayMode = (from mp in db.PaymentModes
                                    join sp in db.ShopPaymentModes on mp.ID equals sp.PaymentModeID
                                    where mp.IsActive == true && sp.IsActive == true && sp.ShopID == item.ShopID
                                    select new ShopPaymentModeViewModel
                                    {
                                        ID = mp.ID,
                                        Mode = mp.Name
                                    }).ToList();

                    //=================taxation (show taxation on preview page)==================
                    if (!item.IsInclusiveOfTax)
                    {
                        item.lCalulatedTaxesRecord = db.ProductTaxes.Where(x => x.ShopStockID == item.ShopStockID && x.IsActive == true).Count() > 0 ?
                                             (from pt in db.ProductTaxes
                                              join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                              join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                              where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                              && pt.ShopStockID == item.ShopStockID && ft.FranchiseID == fID //condition added by harshada on 31/12/2016 ft.FranchiseID == fID 
                                              select new CalulatedTaxesRecord
                                              {
                                                  ProductTaxID = pt.TaxID,
                                                  ShopStockID = pt.ShopStockID,
                                                  TaxName = tm.Name,
                                                  TaxPrefix = tm.Prefix,
                                                  TaxPercentage = ft.InPercentage,
                                                  IsGSTInclusive = pt.IsInclusive //Added by Zubair on 05-07-2017 for GST
                                              }).ToList()
                                                : null;
                    }
                }
                return lVarientList;

            }


        }
        /// <summary>
        /// Get product technical specification
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <returns></returns>
        private List<ProductTechnicalSpecificationViewModel> GetProductTechnicalSpecifications(long pID)
        {

            List<ProductTechnicalSpecificationViewModel> lSpecList = new List<ProductTechnicalSpecificationViewModel>();
            //collect product technical sspecification from Specifications related tables
            var lSpecCollection = (

                  from ps in db.ProductSpecifications
                  join ts in db.Specifications on ps.SpecificationID equals ts.ID
                  where ps.ProductID == pID && ps.IsActive == true && ts.IsActive == true

                  select new ProductTechnicalSpecificationViewModel
                  {
                      SpecificationName = ts.Name,
                      ParentSpecificationName = string.Empty,
                      ParentSpecificationID = ts.ParentSpecificationID,
                      SpecificationID = ts.ID,
                      Value = ps.Value,
                      Level = ts.Level
                  }).ToList();

            //get Parent specification Name
            foreach (var item in lSpecCollection)
            {
                item.ParentSpecificationName = item.ParentSpecificationID == null ? string.Empty : db.Specifications.Find(item.ParentSpecificationID).Name;
                lSpecList.Add(item);
            }

            return lSpecList;
        }

        /// <summary>
        /// get simillar products shopwise  oe glably within same category
        /// </summary>
        /// <param name="searchSimilarProducts"></param>
        /// <returns></returns>
        private RelatedProductsViewModel GetSimillarCategoryProducts(SearchSimilarProductViewModel searchSimilarProducts)
        {
            RelatedProductsViewModel lSimillarProducts = new RelatedProductsViewModel();
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = string.Empty;
            query = "[Select_Simillar_Products]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@CityID", searchSimilarProducts.CityID);////hide
            cmd.Parameters.AddWithValue("@FranchiseID", searchSimilarProducts.FranchiseID);////added
            cmd.Parameters.AddWithValue("@ProductID", searchSimilarProducts.ProductID);
            cmd.Parameters.AddWithValue("@CategoryID", searchSimilarProducts.CategoryID);
            cmd.Parameters.AddWithValue("@ShopID", searchSimilarProducts.ShopID);
            cmd.Parameters.AddWithValue("@PageIndex", searchSimilarProducts.PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", searchSimilarProducts.PageSize);

            cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }


            var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                select new SearchProductDetailsViewModel()
                                {
                                    ProductThumbPath = string.Empty,
                                    ProductID = Convert.ToInt32(dr["ProductID"]),
                                    Name = dr["ProductName"].ToString(),
                                    CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                    MRP = Convert.ToDecimal(dr["MRP"]),
                                    SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                    StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                    RetailPoint = Convert.ToDecimal(dr["RetailPoint"]) ,//By Sonali
                                    CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                    IsDisplayCB = Convert.ToInt32(dr["IsDisplayCB"]),
                                }).ToList();
            lSimillarProducts.ProductList = lproductList;
            //retrive colors and get the thumbnail for first color
            foreach (var item in lSimillarProducts.ProductList)
            {

                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == item.ProductID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                //if (color != null && color.name != "N/A")
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                //else
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);                

                if (color != null && color.name != "N/A")
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                else
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                //Added for SEO URL Structure RULE by AShish
                item.URLStructureName = GetURLStructureName(item.Name);
                item.Name = item.Name.Replace("+", " ");

            }
            //get product Count
            if (ds.Tables[1] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                searchCount.ProductCount = Convert.ToInt32(cmd.Parameters["@Productcount"].Value);

                lSimillarProducts.SearchCount = searchCount;

            }
            return lSimillarProducts;

        }
        /// <summary>
        /// Added for SEO URL Structure RULE by AShish
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            // string[] parts2 = Regex.Split(str, @"\+\/\-\,\(\)");
            ///////////////////
            string concat = "";
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\/\+\-\,()]", "|");
            string[] strSplit = str.Split('|');
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (concat.Length <= 30)
                {
                    concat = concat.Length == 0 ? strSplit[i].Trim() : concat + ' ' + strSplit[i].Trim();
                }
            }
            /////////****/////////



            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });
            ////////////
            //string test = concat.Substring(0, 1);

            //if (test == "-")
            //{ concat = concat.Substring(1, concat.Length); }
            //var test2 = concat[concat.Length - 1];
            //if (test2 == '-')
            //{ concat = concat.Substring(0, concat.Length - 1); }
            //////***//////
            return concat;
        }
        /// <summary>
        /// get Frequently buyed product list with selected product
        /// </summary>
        /// <param name="searchSimilarProducts"></param>
        /// <returns></returns>
        private RelatedProductsViewModel GetFrequentlyBuyedTogether(SearchFrequentlyBuyedProductViewModel searchFrequentlyBuyedProductViewModel)
        {
            RelatedProductsViewModel lBuyedTogetherProducts = new RelatedProductsViewModel();

            string query = string.Empty;
            query = "[Select_FrequentlyBuyedTogether_Products]";
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@CityID", searchFrequentlyBuyedProductViewModel.CityID);
            cmd.Parameters.AddWithValue("@FranchiseID", searchFrequentlyBuyedProductViewModel.FranchiseID);////added
            cmd.Parameters.AddWithValue("@ProductID", searchFrequentlyBuyedProductViewModel.ProductID);
            cmd.Parameters.AddWithValue("@ShopID", searchFrequentlyBuyedProductViewModel.ShopID);
            cmd.Parameters.AddWithValue("@PageIndex", searchFrequentlyBuyedProductViewModel.PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", searchFrequentlyBuyedProductViewModel.PageSize);

            cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            //Collect products in list
            var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                select new SearchProductDetailsViewModel()
                                {
                                    ProductThumbPath = string.Empty,
                                    ProductID = Convert.ToInt32(dr["ProductID"]),
                                    Name = dr["ProductName"].ToString(),
                                    CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                    MRP = Convert.ToDecimal(dr["MRP"]),
                                    SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                    StockStatus = Convert.ToInt32(dr["StockStatus"])
                                }).ToList();
            lBuyedTogetherProducts.ProductList = lproductList;

            //retrive colors and get the thumbnail for first color
            foreach (var item in lBuyedTogetherProducts.ProductList)
            {

                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == item.ProductID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                //if (color != null && color.name != "N/A")
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                //else
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                if (color != null && color.name != "N/A")
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                else
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

            }
            //get product Count
            if (ds.Tables[1] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                searchCount.ProductCount = Convert.ToInt32(cmd.Parameters["@Productcount"].Value);

                lBuyedTogetherProducts.SearchCount = searchCount;

            }
            return lBuyedTogetherProducts;

        }

        #endregion

        /// <summary>
        /// Get Product images for given color
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="clName">Color Name</param>
        /// <returns></returns>
        public static List<ImageListViewModel> GetStockImages(long pID, string clName)
        {
            List<ImageListViewModel> lImages = new List<ImageListViewModel>();
            string[] lImageFile = ImageDisplay.LoadProductImages(pID, clName, clName.Trim().Equals(string.Empty) ? "Default" : string.Empty);

            if (lImageFile != null)
            {
                lImages = (from file in lImageFile
                           select new ImageListViewModel
                           {
                               ImgName = string.Empty,
                               ImgPath = file

                           }).ToList();
            }
            return lImages;
        }


        /// <summary>
        /// This function load the images of perticular product
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <returns>Array of string which specifies image path</returns>
        public static string[] LoadProductImages(long pId, string pSubFolderName, string pSubFolderPrefix)
        {
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN

            StringBuilder lImgPath = new StringBuilder();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/Content/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/" + pId.ToString());
                //lpath.Append(string.IsNullOrEmpty(pSubFolderName) ? "/DEFAULT" : (string.IsNullOrEmpty(pSubFolderPrefix) ? "/" + pSubFolderName.Trim() : "/" + pSubFolderPrefix.Trim() + "_" + pSubFolderName.Trim()));

                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));

                if (pSubFolderName.Equals(string.Empty) || pSubFolderName.Equals("Default"))
                    lpath.Append("default");
                else
                    lpath.Append(pSubFolderName.Trim().ToLower());

                lImgPath = ImageDisplay.GetProductImages(imgPath, lpath, pSubFolderName, pSubFolderPrefix);

                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/Content/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductImages]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        private bool IsFileExists(BusinessLogicLayer.ReadConfig imgPath, string lpath, string filename)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath + "/" + filename));
                reqFTP.UseBinary = false;
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {

                    reqFTP.GetResponse();
                    if (reqFTP.ContentLength > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (WebException wex)
                {
                    return false;
                }
            }
            catch
            {
                //file not found or path not exists 
                return false;
            }
            return false;
        }

        private bool IsFileExists(string filename)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(filename) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";

                //Getting the Web Response
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                //Returns TURE if the Status code == 200
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //file not found or path not exists 
                return false;
            }
        }

        public RelatedProductsViewModel GetRecentlyViewProduct(long UserLoginId, int FranchiseId)
        {
            RelatedProductsViewModel lRecentlyViewProducts = new RelatedProductsViewModel();
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = string.Empty;
            query = "[RecentlyViewProductList]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@CityID", searchSimilarProducts.CityID);////hide
            cmd.Parameters.AddWithValue("@FranchiseID", FranchiseId);////added
            cmd.Parameters.AddWithValue("@UserLoginId", UserLoginId);


            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                select new SearchProductDetailsViewModel()
                                {
                                    ProductThumbPath = string.Empty,
                                    ProductID = Convert.ToInt32(dr["ProductID"]),
                                    Name = dr["ProductName"].ToString(),
                                    CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                    MRP = Convert.ToDecimal(dr["MRP"]),
                                    SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                    StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                    RetailPoint = Convert.ToDecimal(dr["RetailPoint"]),
                                    CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),//By Sonali
                                    IsDisplayCB = Convert.ToInt32(dr["IsDisplayCB"]),
                                }).ToList();
            lRecentlyViewProducts.ProductList = lproductList;
            //retrive colors and get the thumbnail for first color
            foreach (var item in lRecentlyViewProducts.ProductList)
            {

                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == item.ProductID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                //if (color != null && color.name != "N/A")
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                //else
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);                

                if (color != null && color.name != "N/A")
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                else
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                //Added for SEO URL Structure RULE by AShish
                item.URLStructureName = GetURLStructureName(item.Name);
                item.Name = item.Name.Replace("+", " ");

            }
            ////get product Count
            //if (ds.Tables[1] != null)
            //{
            //    SearchCountViewModel searchCount = new SearchCountViewModel();
            //    searchCount.PageCount = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
            //    searchCount.ProductCount = Convert.ToInt32(cmd.Parameters["@Productcount"].Value);

            //    lRecentlyViewProducts.SearchCount = searchCount;

            //}
            return lRecentlyViewProducts;
        }
    }
}
