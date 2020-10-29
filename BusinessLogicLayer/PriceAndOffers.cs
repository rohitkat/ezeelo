//-----------------------------------------------------------------------
// <copyright file="PriceAndOffers.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
/*
 Handed over to Mohit, Tejaswee
 */
namespace BusinessLogicLayer
{
    public class PriceAndOffers : ProductDisplay
   {
        /// <summary>
        /// initialize base class constructor
        /// </summary>
        /// <param name="server"></param>
        public PriceAndOffers(System.Web.HttpServerUtility server) : base(server) { }
        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get todays deals  like flat discount offers, free and Component offer
        /// </summary>
        /// <returns></returns>
        public BestDealHeadingCollection GetTodaysDeals()
        {
            BestDealHeadingCollection lDealsCollection = new BestDealHeadingCollection();
            //flat discount offer
            var FlatDiscountCollection = db.Offers.Where(x => x.IsActive == true && x.IsFree == false)
                               .Select(x => new {x.ID , x.ShortName, ModelLayer.Models.Enum.OfferType.OFFER_TYPE.FLAT_DISCOUNT_OFFER}).ToList();
            foreach (var item in FlatDiscountCollection)
            {
                BestDealHeadings lDeals = new BestDealHeadings();
                lDeals.OfferType = item.FLAT_DISCOUNT_OFFER.ToString();
                lDeals.OfferID = item.ID;
                lDeals.OfferName = item.ShortName;
                lDealsCollection.bestDealHeadingsCollection.Add(lDeals);
  
            }
            //free offer
            var FreeOfferCollection = db.Offers.Where(x => x.IsActive == true && x.IsFree == true)
                             .Select(x => new { x.ID, x.ShortName, ModelLayer.Models.Enum.OfferType.OFFER_TYPE.FREE_OFFER}).ToList();
            foreach (var item in FreeOfferCollection)
            {
                BestDealHeadings lDeals = new BestDealHeadings();
                lDeals.OfferType = item.FREE_OFFER.ToString();
                lDeals.OfferID = item.ID;
                lDeals.OfferName = item.ShortName;
                lDealsCollection.bestDealHeadingsCollection.Add(lDeals);

            }

            //Component offer
            var ComponentOfferCollection = db.ComponentOffers.Where(x => x.IsActive == true)
                            .Select(x => new { x.ID, x.ShortName, ModelLayer.Models.Enum.OfferType.OFFER_TYPE.COMPONENT_OFFER}).ToList();
            foreach (var item in ComponentOfferCollection)
            {
                BestDealHeadings lDeals = new BestDealHeadings();
                lDeals.OfferType = item.COMPONENT_OFFER.ToString();
                lDeals.OfferID = item.ID;
                lDeals.OfferName = item.ShortName;
                lDealsCollection.bestDealHeadingsCollection.Add(lDeals);
            }
            return lDealsCollection;
        
        }

        /// <summary>
        /// This method is used to get product-stock list deals offer
        /// </summary>
        /// <param name="offerType">Offer Type</param>
        /// <param name="offerID">Offer ID</param>
        /// <returns>object of type ProductStockVarientViewModel</returns>
        public ProductStockVarientViewModel GetProducts(BestDealProductSearchViewModel searchDealProducts)
        {
            ProductStockVarientViewModel lProducts = new ProductStockVarientViewModel();
           
            //get list of products involved in selected offer
            DataSet ds = GetAllProducts(searchDealProducts);

            if (ds.Tables[0] != null)
            {
                lProducts.ProductInfo = (from DataRow dr in ds.Tables[0].Rows
                                         select new ProductStockDetailViewModel()
                                         {

                                             ProductID = Convert.ToInt64(dr["ProductID"]),
                                             ProductName = dr["ProductName"].ToString(),
                                             BrandID = Convert.ToInt32(dr["BrandID"]),
                                             BrandName = dr["BrandName"].ToString(),
                                             CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                             CategoryName = dr["CategoryName"].ToString(),
                                             CityID = Convert.ToInt32(dr["CityID"]),
                                             ColorID = Convert.ToInt32(dr["ColorID"]),
                                             ColorName = dr["Color"].ToString(),
                                             ColorCode = dr["HtmlCode"].ToString(),
                                             DimensionID = Convert.ToInt32(dr["DimensionID"]),
                                             DimensionName = dr["Dimension"].ToString(),
                                             MaterialID = Convert.ToInt32(dr["MaterialID"]),
                                             MaterialName = dr["Material"].ToString(),
                                             ShopID = Convert.ToInt64(dr["ShopID"]),
                                             ShopName = dr["ShopName"].ToString(),
                                             SizeID = Convert.ToInt32(dr["SizeID"]),
                                             SizeName = dr["Size"].ToString(),
                                             ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                             ShortDescription = dr["ProductDescription"].ToString(),
                                             MRP = Convert.ToDecimal(dr["MRP"]),
                                             SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                             FranchiseID=Convert.ToInt32(dr["FranchiseID"])////added 

                                         }).ToList();
                //get Specification details for product
                foreach (var item in lProducts.ProductInfo)
                {
                    item.ProductDescription = (
                        from ps in db.ProductSpecifications
                        join s in db.Specifications on ps.SpecificationID equals s.ID
                        where ps.ProductID == item.ProductID && ps.IsActive == true && s.IsActive == true
                        select new ProductDescriptionViewModel()
                        {
                            ProductID = ps.ProductID,
                            ProductSpecificationID = ps.ID,
                            SpecificationID = s.ID,
                            SpecificationName = s.Name,
                            SpecificationValue = ps.Value
                        }).ToList();
                   //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : string.Empty,
                        //item.ColorName.Trim().Equals("N/A") ? string.Empty : item.ColorName.Trim(), ProductUpload.THUMB_TYPE.MM);
                     item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : string.Empty,
                        item.ColorName.Trim().Equals("N/A") ? string.Empty : item.ColorName.Trim(), ProductUpload.IMAGE_TYPE.Approved);
                }
            }
            //get Page size and product count
            if (ds.Tables[1] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(ds.Tables[1].Rows[0]["PageCount"]);
                searchCount.ProductCount = Convert.ToInt32(ds.Tables[1].Rows[0]["Productcount"]);
                lProducts.searchCount = searchCount;
            }

            return lProducts;
        }
        /// <summary>
        /// Get Best/ Todays deals products from stored procedure
        /// </summary>
        /// <param name="searchDealProducts">Search deal product object</param>
        /// <returns></returns>
        private DataSet GetAllProducts(BestDealProductSearchViewModel searchDealProducts)
        {
            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);        
            string query = "[Search_ProductStocks]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@CityID", searchDealProducts.CityID);////hide
            cmd.Parameters.AddWithValue("@FranchiseID", searchDealProducts.FranchiseID);////added
            cmd.Parameters.AddWithValue("@OfferID", searchDealProducts.OfferID);
            cmd.Parameters.AddWithValue("@OfferType", searchDealProducts.OfferType);
            cmd.Parameters.AddWithValue("@CategoryID", searchDealProducts.CategoryID);
            cmd.Parameters.AddWithValue("@BrandID", searchDealProducts.BrandID);
            cmd.Parameters.AddWithValue("@PageIndex", searchDealProducts.PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", searchDealProducts.PageSize);
            cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }
            DataTable dt = new DataTable("SeachCount");
            dt.Columns.Add("PageCount", typeof(int));
            dt.Columns.Add("ProductCount", typeof(int));
            dt.Rows.Add(cmd.Parameters["@PageCount"].Value, cmd.Parameters["@Productcount"].Value);
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Get List of Offers on provided shopstock ID
        /// </summary>
        /// <param name="ssID">ShopStockID</param>
        /// <returns></returns>
        public ProductOffersViewModel GetStockOffers(long ssID)
        {
            ProductOffersViewModel productOffersViewModel = new ProductOffersViewModel();

            //Discount calculated from MRP and Salerate Difference
            productOffersViewModel.FlatOffer = (
               from ss in db.ShopStocks
               where ss.ID == ssID
               where ss.IsActive == true
               where ss.MRP > 0
               select new ProductFlatDiscountOfferViewModel
               {
                   FlatDiscount = ((ss.MRP - ss.RetailerRate) / ss.MRP) * 100,
                   Description = "Flat Discount"
               }).FirstOrDefault();

            if (productOffersViewModel.FlatOffer != null)
            {
                if (productOffersViewModel.FlatOffer.FlatDiscount == 0)
                {

                    productOffersViewModel.FlatOffer = null;
                }
            }
            //Flat or free offer given by shop
            ProductSpecialOfferViewModel productSpecialOfferViewModel = new ProductSpecialOfferViewModel();
            var productSpecialOffer = (
                   from oz in db.OfferZoneProducts
                   join of in db.Offers on oz.OfferID equals of.ID
                   where oz.ShopStockID == ssID
                   where oz.IsActive == true
                   select new ProductSpecialOfferViewModel
                   {
                       OfferID = of.ID,
                       Title = of.ShortName,
                       Description = of.Description,
                       DiscountInPercent = of.DiscountInPercent,
                       DiscountInRs = of.DiscountInRs,
                       IsFree = of.IsFree,
                       FreeQty = of.FreeOty,
                       MinPurchaseQty = of.MinPurchaseQty

                   }).ToList();

            if (productSpecialOffer.Count() > 0)
            {
                productSpecialOfferViewModel = productSpecialOffer.FirstOrDefault();
                //Free stock list; if above offer is free
                if (productSpecialOfferViewModel.IsFree)
                {
                    productSpecialOfferViewModel.FreeStockList = db.OfferZoneProducts.ToList().Select(x => new { x.FreeStockID, x.IsActive, x.OfferID }).Where(x => x.IsActive == true && x.OfferID == productSpecialOfferViewModel.OfferID).Select(x => Convert.ToInt64(x.FreeStockID)).ToList();
                }
                productOffersViewModel.FlatFreOffer = productSpecialOfferViewModel;
            }
            //Component Offer     
            productOffersViewModel.ComponentOffer = ComponentOfferOnStock(ssID);

            return productOffersViewModel;
        }

        /// <summary>
        /// Get final amount after applying Component Offer
        /// </summary>
        /// <param name="ssID">Shop Stock ID </param>
        /// <param name="shID">Shop ID</param>
        /// <param name="lSaleRate">Stock Sale Rate before Calculating Component Offer</param>
        /// <param name="lSavedRs">Saved amount in Rs. after calculating Component offer</param>
        /// <param name="lSavedPercent">Saved amount in % ( with respect lSaleRate) after calculating Component offer</param>
        /// <returns>Final payable amount</returns>
        public decimal GetComponentOfferAmount(long ssID, long shID, decimal lSaleRate, ref decimal lSavedRs, ref decimal lSavedPercent)
        {
            decimal lFinalAmt = CalculateComponentOffer(ssID, shID, lSaleRate, GetComonentOffer(ssID), ref lSavedRs, ref lSavedPercent);

            return lFinalAmt;
        }

       #region Private Methods    

        private decimal CalculateComponentOffer(long ssID, long shID, decimal lSaleRate, DataTable dt, ref decimal lSavedRs, ref decimal lSavedPercent)
        {
            decimal lFinalAmt = 0;
            try
            {

                ProductDetails lProdDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                CommonFunctions cf = new CommonFunctions();

                if (dt.Rows.Count > 0)                                                              //If datatable contains component offer row
                {
                    decimal offerPrice = 0, offerPercent = 0;

                    dt.Columns.Add("OFFER_DESC");

                    DataTable dtCmpList = lProdDetails.GetStockComponentList(ssID,shID);
                    
                    //no Component for stock
                    if (dtCmpList.Rows.Count == 0)
                    {
                        return lSaleRate;
                    }
                    dtCmpList.Columns.Add("Modified_Rate", typeof(decimal));

                    //For First Time
                    dtCmpList = GetCmpListWithModifiedRate(dtCmpList);

                    foreach (DataRow dr in dt.Rows)
                    {

                        decimal.TryParse(dr["IN_RS"].ToString(), out offerPrice);
                        decimal.TryParse(dr["IN_PERCENT"].ToString(), out offerPercent);

                        // Component offer in Rs.
                        if (offerPrice > 0)
                        {
                            dr["OFFER_DESC"] = "Rs. " + offerPrice + " OFF ON " + dr["CMP_NAME"];
                            dtCmpList = GetNewSaleRateAffectedByOfferPrice(dtCmpList, offerPrice, dr["Cmp_ID"].ToString());
                        }
                        else // Component offer in Percent
                        {
                            dr["OFFER_DESC"] = offerPercent + "% OFF ON " + dr["CMP_NAME"];
                            dtCmpList = GetNewSaleRateAffectedByOfferPercent(dtCmpList, offerPercent, dr["Cmp_ID"].ToString());
                        }
                    }
                    var SumofModifiedRate = dtCmpList.AsEnumerable().Sum(x => x.Field<decimal>("Modified_Rate"));

                    if (SumofModifiedRate != null)
                        lFinalAmt = Convert.ToDecimal(SumofModifiedRate);
                    else
                        lFinalAmt = 0;

                    if (lSaleRate > SumofModifiedRate)
                    {


                        decimal priceDiff = lSaleRate - SumofModifiedRate;
                        if (priceDiff > 0)
                        {
                            lSavedRs = Math.Round(priceDiff);
                            lSavedPercent = Math.Round(priceDiff / lSaleRate * 100);                           
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return lFinalAmt;

        }

        private DataTable GetCmpListWithModifiedRate(DataTable dtCmpList)
        {
            decimal saleRateExclVat = 0;
            try
            {
                foreach (DataRow r in dtCmpList.Rows)
                {
                    decimal perUnitPrice = 0, perUnitPercent = 0, weight = 0, newCmpRate = 0;
                    int dCmpId = 0;

                    if (Convert.ToBoolean(r["IsCompulsory"]) && r["ComponentName"].ToString() != "VAT")
                    {
                        decimal.TryParse(r["PerUnitRateInRs"].ToString(), out perUnitPrice);
                        decimal.TryParse(r["PerUnitRateInPer"].ToString(), out perUnitPercent);
                        decimal.TryParse(r["ComponentWeight"].ToString(), out weight);
                        int.TryParse(r["DependentOnComponentID"].ToString(), out dCmpId);

                        if (dCmpId == 0)
                        {
                            if (perUnitPrice > 0)
                            {
                                newCmpRate = (weight * perUnitPrice);
                            }
                        }
                        else
                        {// if dependent cmp id is present
                            DataView dvDepCmpRow = new DataView(dtCmpList);
                            dvDepCmpRow.RowFilter = "[ComponentID]='" + dCmpId + "'";
                            if (dvDepCmpRow.ToTable().Rows.Count > 0)
                            {

                                decimal depCmpWt = 0, depCmpPerUnitPrice = 0, depCmpSaleRate = 0, CmpSaleRate = 0;

                                decimal.TryParse(dvDepCmpRow.ToTable().Rows[0]["PerUnitRateInRs"].ToString(), out depCmpPerUnitPrice);
                                decimal.TryParse(dvDepCmpRow.ToTable().Rows[0]["ComponentWeight"].ToString(), out depCmpWt);


                                if (perUnitPrice > 0)
                                {
                                    CmpSaleRate = depCmpWt * perUnitPrice;
                                }
                                else
                                {
                                    //percent
                                    depCmpSaleRate = depCmpPerUnitPrice * depCmpWt;
                                    CmpSaleRate = depCmpSaleRate * perUnitPercent / 100;
                                }
                                newCmpRate += CmpSaleRate;
                            }
                        }

                        r["Modified_Rate"] = newCmpRate.ToString();
                        saleRateExclVat += newCmpRate;
                    }
                }

                //For Vat


                foreach (DataRow r in dtCmpList.Rows)
                {
                    decimal perUnitVatPercent = 0;

                    if (Convert.ToBoolean(r["IsCompulsory"]) && r["ComponentName"].ToString() == "VAT")
                    {
                        decimal.TryParse(r["PerUnitRateInPer"].ToString(), out perUnitVatPercent);
                        r["Modified_Rate"] = GetVatValue(saleRateExclVat, perUnitVatPercent);
                        break;
                    }
                }
            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetCmpListWithModifiedRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetCmpListWithModifiedRate>", exception.Message);
            }

            return dtCmpList;
        }

        private DataTable GetNewSaleRateAffectedByOfferPrice(DataTable dtCmpList, decimal offerPrice, string cmpOfferId)
        {

            try
            {
                dtCmpList = GetSaleRateExclVatForOfferPrice(dtCmpList, offerPrice, cmpOfferId);
                var SumofModifiedRate = dtCmpList.Select("COMPONENT_NAME<>'VAT'").CopyToDataTable().AsEnumerable().Sum(x => x.Field<decimal>("Modified_Rate"));
                foreach (DataRow r in dtCmpList.Rows)
                {
                    if (r["COMPONENT_NAME"].ToString() == "VAT")
                    {
                        r["Modified_Rate"] = GetVatValue(dtCmpList, SumofModifiedRate);
                        break;
                    }
                }

            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetNewSaleRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetNewSaleRateAffectedByOfferPrice>", exception.Message);
            }
            return dtCmpList;
        }

        private DataTable GetSaleRateExclVatForOfferPrice(DataTable dtCmpList, decimal offerPrice, string cmpOfferId)
        {
            decimal perUnitPrice = 0, perUnitPercent = 0, modifiedCmpRate = 0, CmpSaleRate = 0;
            int dCmpId = 0;
            try
            {
                foreach (DataRow r in dtCmpList.Rows)
                {
                    if (Convert.ToBoolean(r["IS COMPULSORY"]) && r["COMPONENT_NAME"].ToString() != "VAT")
                    {
                        decimal.TryParse(r["Modified_Rate"].ToString(), out modifiedCmpRate);

                        //decimal.TryParse(r["WEIGHT"].ToString(), out weight);
                        int.TryParse(r["D_CMP_ID"].ToString(), out dCmpId);

                        if (dCmpId == 0)
                        {


                            if (r["ComponentID"].ToString() == cmpOfferId)
                            {
                                if (modifiedCmpRate > offerPrice)
                                {
                                    CmpSaleRate = modifiedCmpRate - offerPrice;
                                }
                                else
                                {
                                    CmpSaleRate = 0;
                                }
                                r["Modified_Rate"] = CmpSaleRate;
                            }
                        }
                        else
                        {
                            //this component is depend on other component

                            //for offer making charges
                            if (r["ComponentID"].ToString() == cmpOfferId)
                            {
                                CmpSaleRate = modifiedCmpRate - offerPrice;
                                r["Modified_Rate"] = CmpSaleRate;
                            }
                            else
                            {

                                //when offer on dependent component
                                DataView dvDepCmpRow = new DataView(dtCmpList);
                                dvDepCmpRow.RowFilter = "[COMPONENT_ID]='" + dCmpId + "'";
                                if (dvDepCmpRow.ToTable().Rows.Count > 0)
                                {
                                    decimal.TryParse(r["PER_UNIT_PRICE"].ToString(), out perUnitPrice);
                                    decimal.TryParse(r["PER_UNIT_PERCENT"].ToString(), out perUnitPercent);
                                    decimal modifiedDepCmpRate = 0;

                                    decimal.TryParse(dvDepCmpRow.ToTable().Rows[0]["Modified_Rate"].ToString(), out modifiedDepCmpRate);
                                    if (dvDepCmpRow.ToTable().Rows[0]["ComponentID"].ToString() == cmpOfferId)
                                    {
                                        //No need to calculate for Rs.

                                        // Calculation for Percent
                                        if (perUnitPercent > 0)
                                        {
                                            //percent 
                                            CmpSaleRate = modifiedDepCmpRate * perUnitPercent / 100;
                                            r["Modified_Rate"] = CmpSaleRate;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetSaleRateExclVat>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetSaleRateExclVatForOfferPrice>", exception.Message);
            }
            return dtCmpList;
        }
        private decimal GetVatValue(DataTable dt, decimal newSaleRate)
        {
            decimal vatValue = 0, vatPer = 0;
            try
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToBoolean(r["IsCompulsory"]) && r["ComponentName"].ToString() == "VAT")
                    {

                        decimal.TryParse(r["PerUnitRateInPer"].ToString(), out vatPer);
                        vatValue = newSaleRate * vatPer / 100;
                        break;
                    }
                }
            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetNewSaleRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetVatValue(DataTable dt, decimal newSaleRate)>", exception.Message);
            }
            return vatValue;
        }
        private decimal GetVatValue(decimal newSaleRate, decimal vatPer)
        {
            decimal vatValue = 0;
            try
            {
                vatValue = newSaleRate * vatPer / 100;
            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetNewSaleRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetVatValue(decimal newSaleRate, decimal vatPer)>", exception.Message);
            }
            return vatValue;
        }
        private DataTable GetNewSaleRateAffectedByOfferPercent(DataTable dtCmpList, decimal offerPercent, string cmpOfferId)
        {

            try
            {

                dtCmpList = GetSaleRateExclVatForOfferPercent(dtCmpList, offerPercent, cmpOfferId);
                var SumofModifiedRate = dtCmpList.Select("ComponentName<>'VAT'").CopyToDataTable().AsEnumerable().Sum(x => x.Field<decimal>("Modified_Rate"));
                foreach (DataRow r in dtCmpList.Rows)
                {
                    if (r["ComponentName"].ToString() == "VAT")
                    {
                        r["Modified_Rate"] = GetVatValue(dtCmpList, SumofModifiedRate);
                        break;
                    }
                }
            }
            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetNewSaleRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetNewSaleRateAffectedByOfferPercent>", exception.Message);
            }
            return dtCmpList;
        }
        private DataTable GetSaleRateExclVatForOfferPercent(DataTable dtCmpList, decimal offerPercent, string cmpOfferId)
        {
            decimal perUnitPercent = 0, modifiedCmpRate = 0, CmpSaleRate = 0, offerPercentValue = 0;
            int dCmpId = 0;
            try
            {
                foreach (DataRow r in dtCmpList.Rows)
                {
                    if (Convert.ToBoolean(r["IsCompulsory"]) && r["ComponentName"].ToString() != "VAT")
                    {
                        decimal.TryParse(r["Modified_Rate"].ToString(), out modifiedCmpRate);

                        //decimal.TryParse(r["WEIGHT"].ToString(), out weight);
                        int.TryParse(r["DependentOnComponentID"].ToString(), out dCmpId);

                        if (dCmpId == 0)
                        {

                            if (r["ComponentID"].ToString() == cmpOfferId)
                            {
                                offerPercentValue = modifiedCmpRate * offerPercent / 100;
                                CmpSaleRate = modifiedCmpRate - offerPercentValue;
                                r["Modified_Rate"] = CmpSaleRate;
                            }
                        }
                        else
                        {
                            //this component is depend on other component

                            //for offer making charges
                            if (r["ComponentID"].ToString() == cmpOfferId)
                            {
                                offerPercentValue = modifiedCmpRate * offerPercent / 100;
                                CmpSaleRate = modifiedCmpRate - offerPercentValue;
                                r["Modified_Rate"] = CmpSaleRate;
                            }
                            else
                            {
                                //when offer on dependent component
                                DataView dvDepCmpRow = new DataView(dtCmpList);
                                dvDepCmpRow.RowFilter = "[ComponentID]='" + dCmpId + "'";
                                if (dvDepCmpRow.ToTable().Rows.Count > 0)
                                {
                                    decimal.TryParse(r["PerUnitRateInPer"].ToString(), out perUnitPercent);
                                    decimal modifiedDepCmpRate = 0;

                                    decimal.TryParse(dvDepCmpRow.ToTable().Rows[0]["Modified_Rate"].ToString(), out modifiedDepCmpRate);
                                    if (dvDepCmpRow.ToTable().Rows[0]["ComponentID"].ToString() == cmpOfferId)
                                    {
                                        //No need to calculate for Rs.

                                        // Calculation for Percent
                                        if (perUnitPercent > 0)
                                        {
                                            //percent 
                                            CmpSaleRate = modifiedDepCmpRate * perUnitPercent / 100;

                                            r["Modified_Rate"] = CmpSaleRate;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
            }

            //catch (LogFile.MyException myException)
            //{
            //    throw new LogFile.MyException(myException.EXCEPTION_PATH + "<M:GetNewSaleRate>", myException.EXCEPTION_MSG);
            //}
            catch (Exception exception)
            {
                throw new MyException("<P:BusinessLogicLayer><C:PriceAndOffers><M:GetNewSaleRateAffectedByOfferPercent>", exception.Message);
            }
            return dtCmpList;
        }
        /// <summary>
        /// Get the Description for offer given on Shopstock ID in datatable from database
        /// </summary>
        /// <param name="ssID">ShopStock ID</param>
        /// <returns></returns>
        private DataTable GetComonentOffer(long ssID)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

            string query = string.Empty;
            query = "[Select_ComponentOffer_OnStock]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StockID", ssID);
          

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
        /// Get the Description for offer available on shop stock id
        /// </summary>
        /// <param name="ssID">Shop stock ID</param>
        /// <returns></returns>
        private ProductComponentOfferViewModel ComponentOfferOnStock(long ssID)
        {
            ProductComponentOfferViewModel ComponentOffer = new ProductComponentOfferViewModel();
            //get offer from StockComponentOffer & ComponentOffer table
            var cOffer = (
                   from sc in db.StockComponentOffers
                   join co in db.ComponentOffers on sc.ComponentOfferID equals co.ID
                   where sc.ShopStockID == ssID
                   where sc.IsActive == true
                   where co.IsActive == true
                   select new ProductComponentOfferViewModel
                   {
                       OfferID = co.ID,
                       Title = co.ShortName,
                       Description = co.Description,
                       ComponentID = co.ComponentID,
                       OfferInPercent = co.OfferInPercent,
                       OfferInRs = co.OfferInRs

                   }).ToList();
            if (cOffer.Count() > 0)
            {
                //get Component name from Component table

                ComponentOffer = cOffer.FirstOrDefault();
                ComponentOffer.ComponentName = db.Components.Find(ComponentOffer.ComponentID).Name;

            }
            else
            {
                ComponentOffer = null;
            }
            return ComponentOffer;
        }
       #endregion

   }
}
