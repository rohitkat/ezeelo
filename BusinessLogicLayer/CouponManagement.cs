using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class CouponManagement : Coupon
    {
        public DataTable CheckCouponCode(string couponCode, long shopId, long itemId)
        {
            DataTable dtCouponDetails = CouponValidation(couponCode, shopId, itemId);
            return dtCouponDetails;
        }

        public DataTable CheckCouponCode(string couponCode, long shopId, long itemId, long customerId, long cityId, int franchiseID)// added long cityId->int franchiseID old
        {
            if (couponCode.Equals(string.Empty))
                return new DataTable();
            DataTable dtCouponDetails = CouponValidation(couponCode, shopId, itemId, customerId, cityId, franchiseID);//added cityId->franchiseID old
            return dtCouponDetails;
        }

        //Shop wise coupon apply
        //public double VerifyCouponAgainstCart(List<ShopWiseDeliveryCharges> lShopWiseDeliveryCharges, string CouponCode, out string userMessage, out int validityCode, long custLoginId)
        //{

        //    double voucherAmount = 0;
        //    userMessage = string.Empty;
        //    validityCode = 0;
        //    try
        //    {
        //        if (custLoginId > 0)
        //        {
        //            foreach (var item in lShopWiseDeliveryCharges)
        //            {
        //                CouponDetailsViewModel objCouponDetailsViewModel = this.CheckCouponCodeOnBillAmount(CouponCode, item.ShopID, Convert.ToDouble(item.OrderAmount), custLoginId);
        //                if (objCouponDetailsViewModel.Result == 1)
        //                {
        //                    if (objCouponDetailsViewModel.IsFreeDelivery)
        //                    {
        //                        //delivery charges free for that shop
        //                        item.DeliveryCharge = 0;
        //                    }

        //                    userMessage = objCouponDetailsViewModel.UserMessage;
        //                    validityCode = objCouponDetailsViewModel.Result;

        //                    voucherAmount = objCouponDetailsViewModel.VoucherAmount;
        //                    HttpContext.Current.Session["OrderCouponCode"] = CouponCode;
        //                    HttpContext.Current.Session["OrderCouponAmount"] = voucherAmount;
        //                    break;
        //                }
        //                else
        //                {
        //                    userMessage = objCouponDetailsViewModel.UserMessage;
        //                    validityCode = objCouponDetailsViewModel.Result;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in lShopWiseDeliveryCharges)
        //            {
        //                CouponDetailsViewModel objCouponDetailsViewModel = this.CheckCouponCodeOnBillAmount(CouponCode, item.ShopID, Convert.ToDouble(item.OrderAmount));
        //                if (objCouponDetailsViewModel.Result == 1)
        //                {
        //                    if (objCouponDetailsViewModel.IsFreeDelivery)
        //                    {
        //                        //delivery charges free for that shop
        //                        item.DeliveryCharge = 0;
        //                    }

        //                    userMessage = objCouponDetailsViewModel.UserMessage;
        //                    voucherAmount = objCouponDetailsViewModel.VoucherAmount;
        //                    HttpContext.Current.Session["OrderCouponCode"] = CouponCode;
        //                    HttpContext.Current.Session["OrderCouponAmount"] = voucherAmount;
        //                    break;
        //                }
        //                else
        //                {
        //                    userMessage = objCouponDetailsViewModel.UserMessage;
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["OrderCouponCode"] = null;
        //        HttpContext.Current.Session["OrderCouponAmount"] = null;
        //        voucherAmount = 0;
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[ShoppingCartController][POST:ContinuePaymentProcess]",
        //            BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);


        //    }
        //    return voucherAmount;
        //}
        
        
        //Coupon apply in whole cart
        public double VerifyCouponAgainstCart(List<ShopWiseDeliveryCharges> lShopWiseDeliveryCharges, string CouponCode, out string userMessage, out int validityCode, long custLoginId, long cityId, int? franchiseId=null)// added int? franchiseID for Multiple MCO
        {
                       
            double voucherAmount = 0;
            userMessage = string.Empty;
            validityCode = 0;
            double orderTot = 0.0;
            try
            {
                if (custLoginId > 0)
                {
                    foreach (var item in lShopWiseDeliveryCharges)
                    {
                        orderTot = orderTot + Convert.ToDouble(item.OrderAmount);
                    }
                    CouponDetailsViewModel objCouponDetailsViewModel = this.CheckCouponCodeOnBillAmount(CouponCode, lShopWiseDeliveryCharges.FirstOrDefault().ShopID, orderTot, custLoginId, cityId, franchiseId);//added franchiseID
                    if (objCouponDetailsViewModel.Result == 1)
                    {

                        userMessage = objCouponDetailsViewModel.UserMessage;
                        validityCode = objCouponDetailsViewModel.Result;

                        voucherAmount = objCouponDetailsViewModel.VoucherAmount;
                        HttpContext.Current.Session["OrderCouponCode"] = CouponCode;
                        HttpContext.Current.Session["OrderCouponAmount"] = voucherAmount;
                    }
                    else
                    {
                        userMessage = objCouponDetailsViewModel.UserMessage;
                        validityCode = objCouponDetailsViewModel.Result;
                        HttpContext.Current.Session["OrderCouponCode"] = null;
                        HttpContext.Current.Session["OrderCouponAmount"] = null;
                    }


                }
                else
                {
                    foreach (var item in lShopWiseDeliveryCharges)
                    {
                        orderTot = orderTot + Convert.ToDouble(item.OrderAmount);
                    }

                    CouponDetailsViewModel objCouponDetailsViewModel = this.CheckCouponCodeOnBillAmount(CouponCode, lShopWiseDeliveryCharges.FirstOrDefault().ShopID, orderTot);
                    if (objCouponDetailsViewModel.Result == 1)
                    {
                        userMessage = objCouponDetailsViewModel.UserMessage;
                        voucherAmount = objCouponDetailsViewModel.VoucherAmount;
                        HttpContext.Current.Session["OrderCouponCode"] = CouponCode;
                        HttpContext.Current.Session["OrderCouponAmount"] = voucherAmount;
                    }
                    else
                    {
                        userMessage = objCouponDetailsViewModel.UserMessage;
                        HttpContext.Current.Session["OrderCouponCode"] = null;
                        HttpContext.Current.Session["OrderCouponAmount"] = null;
                    }


                }
              
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["OrderCouponCode"] = null;
                HttpContext.Current.Session["OrderCouponAmount"] = null;
                voucherAmount = 0;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:ContinuePaymentProcess]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);


            }
            return voucherAmount;
        }
        public CouponDetailsViewModel CheckCouponCodeOnBillAmount(string couponCode, long shopId, double billAmount)
        {
            DataTable dtCouponDetails = CouponValidationForBillAmount(couponCode, shopId, billAmount);
            CouponDetailsViewModel CouponDetails = new CouponDetailsViewModel();
            CouponDetails = GetCoupenConditions(dtCouponDetails, billAmount);
            //if finalAmount(BillAmount-VoucherAmount) is >= 500 delivery charges = 0
            //Commented by Tejaswee
            //if (CouponDetails != null && billAmount - CouponDetails.VoucherAmount >= 500)
            //    CouponDetails.IsFreeDelivery = true;
            return CouponDetails;
        }

        public CouponDetailsViewModel CheckCouponCodeOnBillAmount(string couponCode, long shopId, double billAmount, long customerId,long cityId, int? franchiseId=null)//// added  int? franchiseID
        {
            CouponDetailsViewModel CouponDetails = new CouponDetailsViewModel();

            if (couponCode.Equals(string.Empty))
                return CouponDetails;

            DataTable dtCouponDetails = CouponValidationForBillAmount(couponCode, shopId, billAmount, customerId, cityId, franchiseId);//added franchiseID
            CouponDetails = GetCoupenConditions(dtCouponDetails, billAmount);
            //if finalAmount(BillAmount-VoucherAmount) is >= 500 delivery charges = 0
            //Commented by Tejaswee
            //if (CouponDetails != null && billAmount - CouponDetails.VoucherAmount >= 500)
            //    CouponDetails.IsFreeDelivery = true;

            return CouponDetails;
        }

        private CouponDetailsViewModel GetCoupenConditions(DataTable dtCouponDetails, double billAmount)
        {
            CouponDetailsViewModel CouponDetails = new CouponDetailsViewModel();
            if (dtCouponDetails.Rows.Count > 0)
            {
                CouponDetails = (from DataRow dr in dtCouponDetails.Rows
                                 select new CouponDetailsViewModel()
                                 {
                                     VoucherAmount = Convert.ToDouble(dr["Voucher_Amount"]),
                                     VoucherPercent = Convert.ToDouble(dr["Voucher_Percent"]),
                                     MinimumPurchaseAmount = Convert.ToDouble(dr["Min_Purchase_Amt"]),
                                     MinimumPurchaseQuantity = Convert.ToDouble(dr["Min_Purchase_Qty"]),
                                     IsFreeDelivery = Convert.ToBoolean(dr["Is_Free_Delivery"]),
                                     Result = Convert.ToInt32(dr["VALIDITY CODE"])
                                 }).FirstOrDefault();

            }
            else
            {
                CouponDetails = new CouponDetailsViewModel { Result = 0, VoucherAmount = 0, VoucherPercent = 0, MinimumPurchaseAmount = 0, MinimumPurchaseQuantity = 0, IsFreeDelivery = false };

            }

            if (CouponDetails.Result == 1)
            {
                if (billAmount >= CouponDetails.MinimumPurchaseAmount)
                {
                    CouponDetails.IsCoupenUsed = false;
                    CouponDetails.UserMessage = "Voucher is valid";
                }
                else
                {
                    CouponDetails.Result = 0;
                    CouponDetails.IsCoupenUsed = false;
                    CouponDetails.UserMessage = "Voucher is valid for minimum purchase amount of Rs. " + CouponDetails.MinimumPurchaseAmount.ToString("0.00");
                }
            }
            else if (CouponDetails.Result == 0 || CouponDetails.Result == 9)
            {
                CouponDetails.IsCoupenUsed = false;
                CouponDetails.UserMessage = "Voucher is not valid";
            }
            else if (CouponDetails.Result == 4)
            {
                CouponDetails.IsCoupenUsed = false;
                CouponDetails.UserMessage = "Voucher expired";
            }
            else if (CouponDetails.Result == 5)
            {
                CouponDetails.IsCoupenUsed = false;
                CouponDetails.UserMessage = "Voucher is not valid for provided Shop";
            }
            else if (CouponDetails.Result == 6)
            {
                CouponDetails.IsCoupenUsed = false;
                CouponDetails.UserMessage = "Voucher is not valid for provided product";
            }
            else if (CouponDetails.Result == 7)
            {
                CouponDetails.IsCoupenUsed = false;
                CouponDetails.UserMessage = "Voucher is not valid for provided category";
            }
            else if (CouponDetails.Result == 8)
            {
                CouponDetails.IsCoupenUsed = true;
                CouponDetails.UserMessage = "Voucher is used by this customer";
            }
            return CouponDetails;
        }
    }
}
