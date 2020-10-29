using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;



using System.Data.Entity;
using System.Net;
using System.Collections;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text;
using System.Security.Cryptography;






namespace BusinessLogicLayer
{
    public class TrackCartBusiness
    {
        private static EzeeloDBContext db = new EzeeloDBContext();
        public static void InsertCartDetails(long? CartID, int? Qty, long UserLoginID, long ShopStockID, string Mobile, string Stage, string Lattitude, string Longitude, string DeviceType, string DeviceID, String City, string IMEI_NO,int? FranchiseID=null)//added int? FranchiseID for Multiple MCO
        {
           // Result = 0;
            try
            {
                DateTime CreateDate = DateTime.UtcNow;
                string NetworkIP = CommonFunctions.GetClientIP();
                long CreateBy = 0;
                if (UserLoginID != 0)
                { 
                    CreateBy = Convert.ToInt64(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand sqlComm = new SqlCommand("CP_Insert_TrackCart", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@CartID", CartID); //- Added by Avi Verma. Date : 07-Sep-2016.
                sqlComm.Parameters.AddWithValue("@UserLoginID", UserLoginID);
                sqlComm.Parameters.AddWithValue("@ShopStockID", ShopStockID);
                sqlComm.Parameters.AddWithValue("@Qty", Qty); //- Added by Avi Verma. Date : 07-Sep-2016.
                sqlComm.Parameters.AddWithValue("@Mobile", Mobile);
                sqlComm.Parameters.AddWithValue("@Stage", Stage);
                sqlComm.Parameters.AddWithValue("@Lattitude", Lattitude);
                sqlComm.Parameters.AddWithValue("@Longitude", Longitude);
                sqlComm.Parameters.AddWithValue("@NetworkIP", NetworkIP);
                sqlComm.Parameters.AddWithValue("@DeviceType", DeviceType);
                sqlComm.Parameters.AddWithValue("@DeviceID", DeviceID);
                sqlComm.Parameters.AddWithValue("@CreateDate", CreateDate);
                sqlComm.Parameters.AddWithValue("@CreateBy", CreateBy);
                sqlComm.Parameters.AddWithValue("@City", City);

                sqlComm.Parameters.AddWithValue("@FranchiseID", FranchiseID);//--added by Ashish for multiple franchise in same city--//

                sqlComm.Parameters.AddWithValue("@IMEI_NO", IMEI_NO);
                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                //Result = Convert.ToInt32(parm.Value);
                con.Close();
            }
            catch (Exception)
            {
              //  Result = 103; //Exception Found
                throw;
            }
            //return Result;
        }

        //save cart value in track cart table
        public void SaveDetailOnPaymentProcess(long? CartID, int? Qty, long UserLoginID, string Mobile, string stage, string City, int? FranchiseID=null)// added int? FranchiseID for Multiple MCO
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null && HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value != null && HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value != string.Empty)
                {
                    string cookieValue = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                    string[] ProDetails = cookieValue.Split(',');

                    foreach (string item in ProDetails)
                    {
                        if (item != string.Empty)
                        {
                            string[] indivItmDet = item.Split('$');
                            TrackCartBusiness.InsertCartDetails(CartID, Qty, UserLoginID, Convert.ToInt64(indivItmDet[0]), Mobile, stage, "", "", "", "", City, "", FranchiseID);// added FranchiseID
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCartID"></param>
        /// <param name="pCustomerOrderID"></param>
        /// <param name="pOrderCode"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public Cart UpdateCart(long pCartID, long? pCustomerOrderID, string pOrderCode, int Status)
        {
            Cart lCart = new Cart();
            ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();
            try
            {
                lCart = db.Carts.Find(pCartID);
                if(lCart == null)
                {
                    throw new Exception("Error : Cart not found.");
                }
                if(pCustomerOrderID != null)
                {
                    lCustomerOrder = db.CustomerOrders.Find(pCustomerOrderID);
                }
                else if(!string.IsNullOrEmpty(pOrderCode))
                {
                    lCustomerOrder = db.CustomerOrders.FirstOrDefault(x => x.OrderCode == pOrderCode);
                }
                if(lCustomerOrder == null)
                {
                    throw new Exception("Error : Order not found.");
                }
                lCart.CustomerOrderID = lCustomerOrder.ID;
                lCart.IsPlacedByCustomer = true;
                lCart.Status = Status;
                lCart.IsActive = false;
                lCart.ModifyBy = lCustomerOrder.CreateBy;
                db.Entry(lCart).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessLogicLayer -> TrackCartBusiness][UpdateCart]" + ex.Message,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return lCart;
        }
          
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="CityID"></param>
        /// <param name="MCOID"></param>
        /// <returns></returns>
        //public Cart CreateVirtualAbandonedCart(long UserLoginID, long CityID, int? MCOID)
        //{
        //    Cart lCart = new Cart();
        //    try
        //    {
        //        Franchise lFranchise = new Franchise();
        //        if (MCOID != null)
        //        {
        //            int lMCOID = Convert.ToInt32(MCOID);
        //            lFranchise = db.Franchises.Find(lMCOID);
        //}
        //        else
        //        {
        //            lFranchise = db.Franchises.FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == CityID);
        //        }

        //        lCart.Name = GetNextCartName(CityID, lFranchise.ID);
        //        lCart.UserLoginID = UserLoginID;
        //        lCart.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING;
        //        lCart.CartPassword = null;
        //        lCart.CustomerOrderID = null;
        //        lCart.CityID = CityID;
        //        lCart.MCOID = null;
        //        lCart.IsPlacedByCustomer = null;
        //        lCart.IsActive = true;
        //        lCart.CreateDate = DateTime.UtcNow.AddHours(5.5);
        //        lCart.CreateBy = CommonFunctions.GetPersonalDetailsID(lCart.UserLoginID);
        //        db.Carts.Add(lCart);
        //        db.SaveChanges();
        //        //ControllerContext.HttpContext.Response.Cookies["CartName"].Value = lCart.Name;
        //        //ControllerContext.HttpContext.Response.Cookies["CartID"].Value = lCart.ID.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[TrackCartBusiness][CreateVirtualAbandonedCart]" + ex.Message,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //        lCart = null;
        //    }
        //    return lCart;
        //}
        public Cart CreateVirtualAbandonedCart(long UserLoginID, long CityID, int? MCOID, string DeviceId, string DeviceType)//DeviceId and DeviceType Sonali-28-09-2018
        {
            Cart lCart = new Cart();
            try
            {
                Franchise lFranchise = new Franchise();
                if (MCOID != null)
                {
                    int lMCOID = Convert.ToInt32(MCOID);
                    lFranchise = db.Franchises.Find(lMCOID);
                }
                else
                {
                    lFranchise = db.Franchises.FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == CityID);
                }

                lCart.Name = GetNextCartName(CityID, lFranchise.ID);
                lCart.UserLoginID = UserLoginID;
                lCart.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING;
                lCart.CartPassword = null;
                lCart.CustomerOrderID = null;
                lCart.CityID = CityID;
                lCart.MCOID = null;
                lCart.IsPlacedByCustomer = null;
                lCart.IsActive = true;
                lCart.CreateDate = DateTime.UtcNow.AddHours(5.5);
                lCart.CreateBy = CommonFunctions.GetPersonalDetailsID(lCart.UserLoginID);
                lCart.DeviceID = DeviceId;
                lCart.DeviceType = DeviceType;
                lCart.IsCouponApply = false;//Added by Sonali on 19-02-2019
                lCart.IsWalletApply = false;//Added by Sonali on 19-02-2019
                lCart.CouponCode = string.Empty;//Added by Sonali on 19-02-2019
                lCart.CouponAmount = 0;//Added by Sonali on 19-02-2019
                lCart.WalletUsed = 0;//Added by Sonali on 19-02-2019
                db.Carts.Add(lCart);
                db.SaveChanges();
                //ControllerContext.HttpContext.Response.Cookies["CartName"].Value = lCart.Name;
                //ControllerContext.HttpContext.Response.Cookies["CartID"].Value = lCart.ID.ToString();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[TrackCartBusiness][CreateVirtualAbandonedCart]" + ex.Message,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                lCart = null;
            }
            return lCart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CityID"></param>
        /// <param name="MCOID"></param>
        /// <returns></returns>
        private string GetNextCartName(long CityID, int? MCOID)
        {
            //- MCOID Preceding  with 00000, CART, YYMMDD, SequenceNumber Preceding with 00000000

            Franchise lFranchise = new Franchise();
            if(MCOID != null)
            {
                int lMCOID = Convert.ToInt32(MCOID);
                lFranchise = db.Franchises.Find(lMCOID);
            }
            else
            {
                lFranchise = db.Franchises.FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == CityID);
            }
            
            string lCartName = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);

            string lCartPrefix = "MCOID" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");
            if (lFranchise != null)
            {
                string lFranchiseID = lFranchise.ID.ToString("00000");
                lCartPrefix = lFranchiseID + "C" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");
            }
            try
            {
                 OrderManagement lOrderManagement = new OrderManagement();
                 int lCartID = lOrderManagement.GetNextCart();
                 if (lCartID > 0)
                 {
                     lCartName = lCartPrefix + lCartID.ToString("00000000");
                     return lCartName;
                 }
            }
            catch (Exception)
            {
                //throw;
            }
            return null;
        }



    }
}
