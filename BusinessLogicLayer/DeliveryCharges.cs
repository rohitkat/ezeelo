
//-----------------------------------------------------------------------
// <copyright file="DeliveryCharges.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using System.Web;
/*
Handed over to Mohit, Tejaswee, Pradnyakar Sir
*/
namespace BusinessLogicLayer
{
    public class DeliveryCharges : DeliveryManagement
    {

        /// <summary>
        /// This method returns eZeelo Delivery Partner delivery charges for given weight against one shop and pincode,
        /// when delivery is manged by eZeelo only.
        /// </summary>
        /// <param name="pinCode">Customer Pincode</param>
        /// <param name="totalWeight">total weight against shop</param>
        /// <param name="isExpress">Delivery type</param>
        /// <returns>delivery charges</returns>
        public decimal GetDeliveryCharges(string pinCode, decimal totalWeight, bool isExpress)
        {
            decimal charges = 0;

            try
            {


                ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);

                string query = string.Empty;

                query = "[CalculateDeliveryChargesFromDeliveryPartnerSlab]";

                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PinCode", pinCode.Trim());
                cmd.Parameters.AddWithValue("@TotalWeight", totalWeight);

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0 && isExpress == true)
                            charges = Convert.ToDecimal(dt.Rows[0]["ExpressRateWithinPincodeList"]);
                        else if (dt.Rows.Count > 0 && isExpress == false)
                            charges = Convert.ToDecimal(dt.Rows[0]["NormalRateWithinPincodeList"]);
                        else
                            charges = 0;

                    }
                }

            }
            catch (Exception ex)
            {
                charges = 0;

            }
            return charges;
        }


        /// <summary>
        /// This method returns eZeelo Delivery Partner delivery charges for given weight against one shop and pincode,
        /// when delivery is manged by eZeelo only.
        /// </summary>
        /// <param name="pinCode">Customer Pincode</param>
        /// <param name="totalWeight">total weight against shop</param>
        /// <param name="isExpress">Delivery type</param>
        /// <returns>delivery charges</returns>
        public decimal GetDeliveryCharges(string pinCode, decimal totalWeight, bool isExpress, decimal orderAmt)
        {
            decimal charges = 0;

            try
            {
                /*===================================== Tejaswee (13-10-2015) ===================================
                 *If condition is added for checking delivery pincode
                 *if delivery is out of city then set delivery charge as 250 rs else go through normal process
                 =================================================================================================*/

                long cityPincodeId = Convert.ToInt64(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[0]);

                var IsSameCity = (from pin in db.Pincodes
                                  where pin.Name == pinCode && pin.CityID == cityPincodeId
                                  select pin).ToList();


                if (IsSameCity.Count() <= 0 || IsSameCity == null)
                {
                    charges = 250;
                }
                else
                {
                    if (pinCode == "221007" || pinCode == "221104" || pinCode == "221007" || pinCode == "221008" || pinCode == "221011")
                    {
                        charges = 50;
                    }
                    else
                    {
                        //For Kanpur and Nagpur, Varanasi -- Free delivery and for wardha city free delivery for order amount >= 100
                        if (orderAmt < 500 && cityPincodeId != 4968 && cityPincodeId != 10909 && cityPincodeId != 10908 && cityPincodeId != 11187 || (cityPincodeId == 7536 && orderAmt < 100))
                        {

                            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);

                            string query = string.Empty;

                            query = "[CalculateDeliveryChargesFromDeliveryPartnerSlab]";

                            SqlCommand cmd = new SqlCommand(query);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@PinCode", pinCode.Trim());
                            cmd.Parameters.AddWithValue("@TotalWeight", totalWeight);

                            DataTable dt = new DataTable();
                            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.Connection = con;
                                    sda.SelectCommand = cmd;
                                    sda.Fill(dt);

                                    if (dt.Rows.Count > 0 && isExpress == true)
                                        charges = Convert.ToDecimal(dt.Rows[0]["ExpressRateWithinPincodeList"]);
                                    else if (dt.Rows.Count > 0 && isExpress == false)
                                        charges = Convert.ToDecimal(dt.Rows[0]["NormalRateWithinPincodeList"]);
                                    else
                                        charges = 0;

                                }
                            }
                        }
                        else
                        {
                            charges = 0;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                charges = 0;

            }
            return charges;
        }


        /// <summary>
        /// Get delivery charges for shop, if shop itself may manage delivery process.
        /// </summary>
        /// <param name="pinCode">pincode</param>
        /// <param name="totalWeight">TotalWeight of product aginst a shop</param>
        /// <param name="isExpress">DeliveryType isExpress or normal</param>
        /// <param name="shopID">ShopID</param>
        /// <param name="shopOrderAmount">Shop order amount</param>
        /// <param name="deliveryPartnerID">Delivery partner ID</param>
        /// <returns>delivery charges against a shop</returns>
        public decimal GetDeliveryCharges(string pinCode, decimal totalWeight, bool isExpress, long shopID, double shopOrderAmount, out int deliveryPartnerID)
        {
            decimal charges = 0;
            deliveryPartnerID = 1;
            try
            {

                ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);

                string query = string.Empty;

                query = "[CalculateDeliveryChargesFromDeliveryPartnerSlabForShop]";

                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PinCode", pinCode.Trim());
                cmd.Parameters.AddWithValue("@TotalWeight", totalWeight);
                cmd.Parameters.AddWithValue("@ShopID", shopID);
                cmd.Parameters.AddWithValue("@ShopOrderAmount", shopOrderAmount);

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0 && isExpress == true)
                        {
                            charges = Convert.ToDecimal(dt.Rows[0]["ExpressRateWithinPincodeList"]);
                            deliveryPartnerID = Convert.ToInt32(dt.Rows[0]["DeliveryPartnerID"]);
                        }
                        else if (dt.Rows.Count > 0 && isExpress == false)
                        {
                            charges = Convert.ToDecimal(dt.Rows[0]["NormalRateWithinPincodeList"]);
                            deliveryPartnerID = Convert.ToInt32(dt.Rows[0]["DeliveryPartnerID"]);
                        }
                        else
                            charges = 0;

                    }
                }

            }
            catch (Exception ex)
            {
                charges = 0;

            }
            return charges;
        }


        /// <summary>
        /// Get Delivery Charges for list of shops involve in a Cart.
        /// </summary>
        /// <param name="ShopListAndPincode">List of shopwise order Amount</param>
        /// <returns>List of delivery charges agianst shops</returns>
        public List<ShopWiseDeliveryCharges> GetDeliveryCharges(GetShopWiseDeliveryChargesViewModel ShopListAndPincode)
        {
            List<ShopWiseDeliveryCharges> shopList = new List<ShopWiseDeliveryCharges>();
            shopList = ShopListAndPincode.ShopWiseDelivery;

            //Algorith after 22 Sept 2015 : No shopwise delivery charges, consider total weight of product irrespective of shop, and free delivery above 500 irrespective of shop
            //*Applicable till subscription plan only.
            //As per discussion we have to put Rs. 50 as delivery charges for first shop
            //There are some issues like if first merchant cancels his order, In that case its management' dicision that they will manage it.

            /*===================== Tejaswee (12/10/2015) ===============================
                     * if delivery is within city then set delivery charge 50 rs
                     * For other than city set Rs. 250 delivery charge  
            =============================================================================*/
            string deliveryAddrPincode = ShopListAndPincode.Pincode;
            long cityPincodeId;
            if (HttpContext.Current.Request.Cookies["CityCookie"] != null)
            {
                cityPincodeId = Convert.ToInt64(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[0]);
            }
            else//Else add for api as Cookie are not avalable with APP.
            {
                cityPincodeId = ShopListAndPincode.CityID;
            }


            var IsSameCity = (from pin in db.Pincodes
                              where pin.Name == deliveryAddrPincode && pin.CityID == cityPincodeId
                              select pin).ToList();

            if (IsSameCity.Count() <= 0 || IsSameCity == null)
            {
                DeliveryCharges ldelCharges = new DeliveryCharges();
                shopList.First().DeliveryCharge = 250;
                shopList.ForEach(x => x.DeliveryPartnerID = 1);
                //Yashaswi Change Delivery Charges on Minimum Order Amount   
                try
                {
                    var CartTotal = new
                    {
                        NetWeight = shopList.Sum(t => t.Weight),
                        NetOrderAmount = shopList.Sum(t => t.OrderAmount)
                    };
                    if (CartTotal != null)
                    {
                        //if (cityPincodeId == 10909)
                        //{
                        //    if (CartTotal.NetOrderAmount >= 750)
                        //    {
                        //        shopList.ForEach(x => x.DeliveryCharge = 0);
                        //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        //    }
                        //    else
                        //    {
                        //        shopList.ForEach(x => x.DeliveryCharge = 25);
                        //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        //    }
                        //}
                        //Yashaswi 21-11-2018
                        //if (cityPincodeId == 21191)
                        //{
                        //    if (CartTotal.NetOrderAmount >= 750)
                        //    {
                        //        shopList.ForEach(x => x.DeliveryCharge = 0);
                        //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        //    }
                        //    else
                        //    {
                        //        shopList.ForEach(x => x.DeliveryCharge = 25);
                        //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        //    }
                        //}
                        //started delivery charges according to admin panel by SOnali on 19_01_2019
                        DeliveryCharge objcharge = db.DeliveryCharges.Where(x => x.CityID == cityPincodeId && x.IsActive).FirstOrDefault();
                        if (objcharge != null)
                        {
                            if (CartTotal.NetOrderAmount >= objcharge.OrderAmount)
                            {
                                shopList.ForEach(x => x.DeliveryCharge = 0);
                                shopList.ForEach(x => x.DeliveryPartnerID = 1);
                            }
                            else
                            {
                                shopList.ForEach(x => x.DeliveryCharge = objcharge.Charges);
                                shopList.ForEach(x => x.DeliveryPartnerID = 1);
                            }
                        }
                        else
                        {
                            if (CartTotal.NetOrderAmount >= 750)
                            {
                                shopList.ForEach(x => x.DeliveryCharge = 0);
                                shopList.ForEach(x => x.DeliveryPartnerID = 1);
                            }
                            else
                            {
                                shopList.ForEach(x => x.DeliveryCharge = 25);
                                shopList.ForEach(x => x.DeliveryPartnerID = 1);
                            }
                        }


                    }
                }
                catch
                {

                }
            }
            else
            {
                var CartTotal = new
                {
                    NetWeight = shopList.Sum(t => t.Weight),
                    NetOrderAmount = shopList.Sum(t => t.OrderAmount)
                };

                foreach (var item in shopList)
                {
                    var CatIDSecond = db.Categories.Where(x => x.ID == item.CatID).Select(x => x.ParentCategoryID).FirstOrDefault();
                    var ScatID = db.Categories.Where(x => x.ID == CatIDSecond).Select(x => x.ParentCategoryID).FirstOrDefault();
                    item.CatID = ScatID;
                }


                var CatWiseCartTotal = new
                {
                    CatWiseNetOrderAmount = shopList.Where(x => x.CatID == 15).Sum(t => t.OrderAmount)
                };

                if (CartTotal != null)
                {

                    /*===================== Tejaswee (11/12/2015) ===============================
                     * Below conditions applicable only for varanasi city
                     *For varanasi city set delivery charge rs. 40 for order amount less than 400
                     If order amount is >= 400 then delivery charge will be free.
                     * 
                     * (15/12/2015) Delivery charges for varanasi city is same as Nagpur
                    =============================================================================*/
                    //if (cityPincodeId == 10908)
                    //{
                    //    long tot = Convert.ToInt64(CartTotal.NetOrderAmount);
                    //    shopList = this.setDeliveryCharge(shopList, 400, 40, tot);
                    //}
                    //else
                    //{
                    //if (deliveryAddrPincode == "221007" || deliveryAddrPincode == "221104" || deliveryAddrPincode == "221007" || deliveryAddrPincode == "221008" || deliveryAddrPincode == "221011")
                    //{
                    //    shopList.First().DeliveryCharge = 50;
                    //    shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //}
                    //else
                    //{
                    //    if (CartTotal.NetOrderAmount >= 500 || CatWiseCartTotal.CatWiseNetOrderAmount >= 300 || cityPincodeId == 4968 || cityPincodeId == 10909 || cityPincodeId == 10908 || cityPincodeId == 11187)
                    //    {

                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        ////*************** This Added on 20-3-2017 for Lucknow (20 delivery charge on below 250 Rs Order) By Ashish *****************
                    //        if (cityPincodeId == 11187 && CartTotal.NetOrderAmount >= 350)
                    //        {
                    //            shopList.ForEach(x => x.DeliveryCharge = 0);
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //        else if (cityPincodeId == 11187 && CartTotal.NetOrderAmount < 350)
                    //        {
                    //            shopList.First().DeliveryCharge = 25;
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //        //*************** This Added on 20-3-2017 for Kanpur (20 delivery charge on below 250 Rs Order) By Ashish *****************
                    //        if (cityPincodeId == 10909 && CartTotal.NetOrderAmount >= 350)
                    //        {
                    //            shopList.ForEach(x => x.DeliveryCharge = 0);
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //        else if (cityPincodeId == 10909 && CartTotal.NetOrderAmount < 350)
                    //        {
                    //            shopList.First().DeliveryCharge = 25;
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //    }
                    //    //For Wardha city delivery charrges is free for order amount >=100
                    //    else if (cityPincodeId == 7536 && CartTotal.NetOrderAmount >= 100)
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //    else
                    //    {
                    //        DeliveryCharges ldelCharges = new DeliveryCharges();
                    //        shopList.First().DeliveryCharge = 50;
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //}

                    //started delivery charges according to admin panel by SOnali on 19_01_2019
                    DeliveryCharge objcharge = db.DeliveryCharges.Where(x => x.CityID == cityPincodeId && x.IsActive).FirstOrDefault();
                    if (objcharge != null)
                    {
                        if (CartTotal.NetOrderAmount >= objcharge.OrderAmount)
                        {
                            shopList.ForEach(x => x.DeliveryCharge = 0);
                            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        }
                        else
                        {
                            shopList.ForEach(x => x.DeliveryCharge = objcharge.Charges);
                            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        }
                    }
                    else
                    {
                        if (CartTotal.NetOrderAmount >= 750)
                        {
                            shopList.ForEach(x => x.DeliveryCharge = 0);
                            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        }
                        else
                        {
                            shopList.ForEach(x => x.DeliveryCharge = 25);
                            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                        }
                    }
                    //if (cityPincodeId == 10909 || cityPincodeId == 21191)
                    //{
                    //    if (CartTotal.NetOrderAmount >= 750)
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //    else
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 25);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //}
                    //}
                    //Ended delivery charges according to admin panel by SOnali on 19_01_2019
                    //Yashaswi Change Delivery Charges on Minimum Order Amount                   
                    //if (cityPincodeId == 10909)
                    //{
                    //    if (CartTotal.NetOrderAmount >= 750)
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //    else
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 25);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //}
                    ////Yashaswi 21-11-2018
                    //if (cityPincodeId == 21191)
                    //{
                    //    if (CartTotal.NetOrderAmount >= 750)
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //    else
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 25);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //}
                    //DateTime deliveryChargeValidUpto = new DateTime(2018, 11, 18, 23, 59, 59);
                    ////Yashaswi Change Delivery Charges on Minimum Order Amount
                    //if (DateTime.Now > deliveryChargeValidUpto)
                    //{
                    //    if (cityPincodeId == 21191)
                    //    {
                    //        if (CartTotal.NetOrderAmount >= 750)
                    //        {
                    //            shopList.ForEach(x => x.DeliveryCharge = 0);
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //        else
                    //        {
                    //            shopList.ForEach(x => x.DeliveryCharge = 25);
                    //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //Yashaswi Change Delivery Charges for Sonipat 26/10/2018
                    //    if (cityPincodeId == 21191)
                    //    {
                    //        shopList.ForEach(x => x.DeliveryCharge = 0);
                    //        shopList.ForEach(x => x.DeliveryPartnerID = 1);
                    //    }
                    //}
                    //}
                }
            }

            //Algorithm before 22 Sept 2015 : shopwise delivery charges will be displayed on Shopping cart and check delivery free above 500 for distinct shop
            //*Uncomment following code when subscription plan ends.
            //foreach (var item in shopList)
            //{
            //    DeliveryCharges ldelCharges = new DeliveryCharges();
            //    int deliveryPartnerID;
            //    if (item.OrderAmount < 500)
            //    {
            //        decimal deliveryCharges = ldelCharges.GetDeliveryCharges(ShopListAndPincode.Pincode, item.Weight, ShopListAndPincode.IsExpress, item.ShopID, Convert.ToDouble(item.OrderAmount), out  deliveryPartnerID);
            //        item.DeliveryCharge = deliveryCharges;
            //        item.DeliveryPartnerID = deliveryPartnerID;
            //    }
            //    else
            //    {
            //        item.DeliveryCharge = 0;
            //    }

            //}
            return shopList;

        }



        //private List<ShopWiseDeliveryCharges> setDeliveryCharge(List<ShopWiseDeliveryCharges> shopList, long checkAmt, long delCharge, long CartTotal)
        //{
        //    try
        //    {
        //        if (CartTotal >= checkAmt)
        //        {
        //            shopList.ForEach(x => x.DeliveryCharge = 0);
        //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
        //        }
        //        else
        //        {
        //            DeliveryCharges ldelCharges = new DeliveryCharges();
        //            shopList.First().DeliveryCharge = delCharge;
        //            shopList.ForEach(x => x.DeliveryPartnerID = 1);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return shopList;
        //}
        private List<ShopWiseDeliveryCharges> setDeliveryCharge(List<ShopWiseDeliveryCharges> shopList, long checkAmt, long delCharge, long CartTotal)
        {
            try
            {
                if (CartTotal >= checkAmt)
                {
                    shopList.ForEach(x => x.DeliveryCharge = 0);
                    shopList.ForEach(x => x.DeliveryPartnerID = 1);
                }
                else
                {
                    DeliveryCharges ldelCharges = new DeliveryCharges();
                    shopList.First().DeliveryCharge = delCharge;
                    shopList.ForEach(x => x.DeliveryPartnerID = 1);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return shopList;
        }



        //public int GetDeliveryCharges(string pinCode)
        //{
        //    List<ShopWiseDeliveryCharges> shopList = new List<ShopWiseDeliveryCharges>();
        //    int deliveryCharge = 0;


        //    string deliveryAddrPincode = pinCode;
        //    long cityPincodeId = Convert.ToInt64(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[0]);

        //    var IsSameCity = (from pin in db.Pincodes
        //                      where pin.Name == deliveryAddrPincode && pin.CityID == cityPincodeId
        //                      select pin).ToList();

        //    if (IsSameCity.Count() <= 0 || IsSameCity == null)
        //    {
        //        DeliveryCharges ldelCharges = new DeliveryCharges();
        //        //shopList.First().DeliveryCharge = 250;
        //        deliveryCharge = 250;
        //        //shopList.ForEach(x => x.DeliveryPartnerID = 1);
        //    }
        //    else
        //    {

        //        var CartTotal = new
        //        {
        //            NetWeight = shopList.Sum(t => t.Weight),
        //            NetOrderAmount = shopList.Sum(t => t.OrderAmount)
        //        };
        //        if (CartTotal != null)
        //        {

        //            if (CartTotal.NetOrderAmount > 500)
        //            {
        //                shopList.ForEach(x => x.DeliveryCharge = 0);
        //                shopList.ForEach(x => x.DeliveryPartnerID = 1);
        //            }
        //            else
        //            {
        //                DeliveryCharges ldelCharges = new DeliveryCharges();
        //                //shopList.First().DeliveryCharge = 50;
        //                deliveryCharge = 50;
        //                //shopList.ForEach(x => x.DeliveryPartnerID = 1);
        //            }
        //        }
        //    }

        //    return deliveryCharge;

        //}

        /// <summary>
        /// Author: Tejaswee Taktewale
        /// This function is used in payment process controller to verify product deliver in same city or not in normal buy and express buy
        /// 
        /// </summary>
        /// <param name="lShoppingCartCollection"></param>
        /// <param name="pincode"></param>
        /// <param name="deliveryType"></param>
        /// <returns></returns>
        public List<ShopWiseDeliveryCharges> CalculateDeliveryCharge(ShopProductVarientViewModelCollection lShoppingCartCollection, string pincode, string deliveryType)
        {
            List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();
            try
            {
                DeliveryCharges dc = new DeliveryCharges();
                decimal orgProductWeight = 0;

                List<long> merId = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().ToList();
                for (int i = 0; i < merId.Count; i++)
                {
                    var orgProducts = lShoppingCartCollection.lShopProductVarientViewModel.Select(x => new
                    {
                        x.ShopID,
                        x.ActualWeight,
                        x.SaleRate,
                        x.PurchaseQuantity,
                        x.CategoryID
                    })
                   .Where(x => x.ShopID == merId[i]).ToList();
                    orgProductWeight = 0;
                    decimal MerchantWiseSubTotal = 0;
                    for (int j = 0; j < orgProducts.Count; j++)
                    {
                        orgProductWeight = orgProductWeight + (orgProducts[j].PurchaseQuantity * orgProducts[j].ActualWeight);
                        MerchantWiseSubTotal = MerchantWiseSubTotal + Convert.ToInt64(orgProducts[j].SaleRate * orgProducts[j].PurchaseQuantity);
                    }

                    ShopWiseDeliveryCharges lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();
                    //===================== Initialize Property ======================================
                    lShopWiseDeliveryCharges.ShopID = merId[i];

                    lShopWiseDeliveryCharges.OrderAmount = MerchantWiseSubTotal;
                    lShopWiseDeliveryCharges.Weight = orgProductWeight;
                    lShopWiseDeliveryCharges.DeliveryType = deliveryType;         //"Normal";
                    lShopWiseDeliveryCharges.CatID = orgProducts.FirstOrDefault().CategoryID;
                    listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);
                }
                decimal NetOrderAmount = listShopWiseDeliveryCharges.Sum(t => t.OrderAmount);
                GetShopWiseDeliveryChargesViewModel lGetShopWiseDeliveryChargesViewModel = new GetShopWiseDeliveryChargesViewModel();
                lGetShopWiseDeliveryChargesViewModel.ShopWiseDelivery = listShopWiseDeliveryCharges;
                lGetShopWiseDeliveryChargesViewModel.Pincode = pincode;


                if (deliveryType == "Express")
                {
                    lGetShopWiseDeliveryChargesViewModel.IsExpress = true;
                    GetDeliveryCharges(pincode, orgProductWeight, true, NetOrderAmount);
                }
                else
                {
                    lGetShopWiseDeliveryChargesViewModel.IsExpress = false;
                    listShopWiseDeliveryCharges = dc.GetDeliveryCharges(lGetShopWiseDeliveryChargesViewModel);
                }



                //lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;
            }
            catch (Exception)
            {

                throw;
            }
            return listShopWiseDeliveryCharges;
        }
        public string CheckFreeDeliveryCharge(decimal GrandOrderAmount, long cityPincodeId, long FranchiseId)
        {

            string Msg = string.Empty;

            try

            {

                //started delivery charges according to admin panel by SOnali on 19_01_2019

                DeliveryCharge objcharge = db.DeliveryCharges.Where(x => x.CityID == cityPincodeId && x.IsActive && x.FranchiseID == FranchiseId).FirstOrDefault();
                if (objcharge != null)
                {
                    if (objcharge.MinOrderAmount <= GrandOrderAmount && objcharge.OrderAmount >= GrandOrderAmount)
                        Msg = objcharge.Message;
                }

                else

                {

                    if (GrandOrderAmount <= 750 && GrandOrderAmount >= 300)

                        Msg = "Avoid Rs. 25 shipping charges by purchasing Rs. 750 above";





                }

            }

            catch (Exception ex)

            {

                throw;

            }

            return Msg;

        }
    }
}
