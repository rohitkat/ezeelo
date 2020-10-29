using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public abstract class ValidateCoupon
    {
        public abstract DataTable CouponValidation(string couponCode, long shopId, long itemId);

        //public abstract DataTable CouponValidation(string couponCode, long shopId, long itemId, long userLoginId, long cityID);//hide
        public abstract DataTable CouponValidation(string couponCode, long shopId, long itemId, long userLoginId, long cityID, int franchiseID);//added cityID->franchiseID old

        public abstract DataTable CouponValidationForBillAmount(string couponCode, long shopId, double billAmount);

       // public abstract DataTable CouponValidationForBillAmount(string couponCode, long shopId, double billAmount, long userLoginId,long cityID);//hide
        public abstract DataTable CouponValidationForBillAmount(string couponCode, long shopId, double billAmount, long userLoginId, long cityID, int? franchiseID=null);//added int? franchiseID
    }

    public class Coupon : ValidateCoupon, ICouponCookie
    {
        /*Abstract Implementation*/
        public override DataTable CouponValidation(string couponCode, long shopId, long itemId)
        {
            List<object> paramValues = new List<object>();

            paramValues.Add(couponCode);
            paramValues.Add(shopId);
            paramValues.Add(DBNull.Value);
            paramValues.Add(itemId);
            //paramValues.Add(cityId);
            paramValues.Add(DBNull.Value);

            BusinessLogicLayer.ReadConfig obj = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(obj.DB_CONNECTION);

            DataTable dt = dbOpr.GetRecords("SELECT_VOUCHERS_VALIDITY", paramValues);

            return dt;
        }

        public override DataTable CouponValidation(string couponCode, long shopId, long itemId, long custId, long cityID, int franchiseID)//added cityID->franchiseID old
        {
            List<object> paramValues = new List<object>();

            paramValues.Add(couponCode);
            paramValues.Add(shopId);
            paramValues.Add(custId);
            paramValues.Add(itemId);
            paramValues.Add(cityID);
            paramValues.Add(franchiseID);//added cityID->franchiseID old
            paramValues.Add(DBNull.Value);
            
            

            BusinessLogicLayer.ReadConfig obj = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(obj.DB_CONNECTION);
            DataTable dt = dbOpr.GetRecords("SELECT_VOUCHERS_VALIDITY", paramValues);

            return dt;
        }

        public override DataTable CouponValidationForBillAmount(string couponCode, long shopId, double billAmount)
        {
            List<object> paramValues = new List<object>();

            paramValues.Add(couponCode);
            paramValues.Add(shopId);
            paramValues.Add(DBNull.Value);
            paramValues.Add(DBNull.Value);
           // paramValues.Add(cityId);
            paramValues.Add(DBNull.Value);


            BusinessLogicLayer.ReadConfig obj = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(obj.DB_CONNECTION);

            DataTable dt = dbOpr.GetRecords("SELECT_VOUCHERS_VALIDITY", paramValues);

            return dt;
        }

        public override DataTable CouponValidationForBillAmount(string couponCode, long shopId, double billAmount, long custId, long cityID, int? franchiseID=null)//Added  int? franchiseID for multiple MCO
        {
            List<object> paramValues = new List<object>();

            paramValues.Add(couponCode);
            paramValues.Add(shopId);
             paramValues.Add(custId);
            paramValues.Add(DBNull.Value);
            paramValues.Add(cityID);
            paramValues.Add(franchiseID);//added franchiseID
            paramValues.Add(DBNull.Value);

            BusinessLogicLayer.ReadConfig obj = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);

            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(obj.DB_CONNECTION);
            DataTable dt = dbOpr.GetRecords("SELECT_VOUCHERS_VALIDITY", paramValues);

            return dt;
        }
        
        /*Interface Implementation*/
        public void SetCouponCookie(string couponCode, long shopStockId, double couponValueRs, double couponValuePercent)
        {
            //bool isCouponUsed = this.GetCouponCookie(couponCode);

            //if(!isCouponUsed)
            //{
            if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null && HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
            {
                HttpContext.Current.Response.Cookies["CouponManagementCookie"].Value = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value + ", " + shopStockId + "$" + couponCode + "$" + couponValueRs + "$" + couponValuePercent;
            }
            else
            {
                HttpContext.Current.Response.Cookies["CouponManagementCookie"].Value = shopStockId + "$" + couponCode + "$" + couponValueRs + "$" + couponValuePercent;
            }
            HttpContext.Current.Response.Cookies["CouponManagementCookie"].Expires = System.DateTime.Now.AddDays(30);
            //}
            //else
            //{
            //    HttpContext.Current.Response.Cookies["CouponManagementCookie"].Value = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value;
            //    HttpContext.Current.Response.Cookies["CouponManagementCookie"].Expires = System.DateTime.Now.AddDays(30);
            //    // Coupon is already used...
            //}
        }

        public bool GetCouponCookie(string couponCode)
        {
            if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null && HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
            {
                string cookieValue = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value;
                if (cookieValue.Contains("$" + couponCode + "$"))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }

    public interface ICouponCookie
    {
        void SetCouponCookie(string couponCode, long productId, double couponValueRs, double couponValuePercent);
        bool GetCouponCookie(string couponCode);
    }
}
