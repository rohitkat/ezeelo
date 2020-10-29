using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace API.Controllers
{
    public class GeneratePayubizHashKeyController : ApiController
    {
        [Route("api/GeneratePayubizHashKey")]
        public object Post(OrderInfoForPayuBiz model)
        {

            object obj = new object();
            IDictionary<string, string> arr = new Dictionary<string, string>();
            try
            {
                // string[] arr = new string[] { "payment_hash", "vas_for_mobile_sdk_hash", "payment_related_details_for_mobile_sdk_hash", "verify_payment_hash", "delete_user_card_hash", "get_user_cards_hash", "edit_user_card_hash", "save_user_card_hash", "check_offer_status_hash" };
                //string txnid = Generatetxnid();
                //

                if (model.Key == null)
                    model.Key = string.Empty;
                if (model.Txnid == null)
                    model.Txnid = string.Empty;
                if (model.Amount == null)
                    model.Amount = 0;
                if (model.Product_info == null)
                    model.Product_info = string.Empty;
                if (model.Firstname == null)
                    model.Firstname = string.Empty;
                if (model.Email == null)
                    model.Email = string.Empty;
                if (model.Salt == null)
                    model.Salt = string.Empty;
                string udf1 = string.Empty;
                string udf2 = string.Empty;
                string udf3 = string.Empty;
                string udf4 = string.Empty;
                string udf5 = string.Empty;
                arr.Add("txnid", model.Txnid);
                //Code refer from PHP 29-10-2018
                //$payhash_str = $key . '|' . checkNull($txnid) . '|' .checkNull($amount)  . '|' .checkNull($productinfo)  . '|' . checkNull($firstname) . '|' . checkNull($email) . '|' . checkNull($udf1) . '|' . checkNull($udf2) . '|' . checkNull($udf3) . '|' . checkNull($udf4) . '|' . checkNull($udf5) . '||||||' . $salt;
                string payhash_str = model.Key + "|" + model.Txnid + "|" + model.Amount + "|" + model.Product_info + "|" + model.Firstname + "|" + model.Email + "|" + udf1 + "|" + udf2 + "|" + udf3 + "|" + udf4 + "|" + udf5 + "||||||" + model.Salt;
                string paymentHash = Generatehash512(payhash_str).ToLower();
                arr.Add("payment_hash", paymentHash);

                string cmnMobileSdk = "vas_for_mobile_sdk";
                //$mobileSdk_str = $key . '|' . $cmnMobileSdk . '|default|' . $salt;
                string mobileSdk_str = model.Key + "|" + cmnMobileSdk + "|default|" + model.Salt;
                string mobileSdk = Generatehash512(mobileSdk_str).ToLower();
                arr.Add("vas_for_mobile_sdk_hash", mobileSdk);

                //string cmnPaymentRelatedDetailsForMobileSdk1 = "payment_related_details_for_mobile_sdk";
                ////$detailsForMobileSdk_str1 = $key  . '|' . $cmnPaymentRelatedDetailsForMobileSdk1 . '|default|' . $salt ;
                //string detailsForMobileSdk_str1 = model.Key + "|" + cmnPaymentRelatedDetailsForMobileSdk1 + "|default|" + model.Salt;
                //string detailsForMobileSdk1 = Generatehash512(detailsForMobileSdk_str1).ToLower();
                //arr.Add("payment_related_details_for_mobile_sdk_hash", detailsForMobileSdk1);

                //string cmnVerifyPayment = "verify_payment";
                //string verifyPayment_str = model.Key + "|" + cmnVerifyPayment + "|" + txnid + "|" + model.Salt;
                //string verifyPayment = Generatehash512(verifyPayment_str);
                //arr.Add("verify_payment_hash", verifyPayment);

                if (!string.IsNullOrEmpty(model.User_credentials))
                {
                    string cmnNameDeleteCard = "delete_user_card";
                    //$deleteHash_str = $key  . '|' . $cmnNameDeleteCard . '|' . $user_credentials . '|' . $salt ;
                    string deleteHash_str = model.Key + "|" + cmnNameDeleteCard + "|" + model.User_credentials + "|" + model.Salt;
                    string deleteHash = Generatehash512(deleteHash_str).ToLower();
                    arr.Add("delete_user_card_hash", deleteHash);

                    string cmnNameGetUserCard = "get_user_cards";
                    //$getUserCardHash_str = $key  . '|' . $cmnNameGetUserCard . '|' . $user_credentials . '|' . $salt ;
                    string getUserCardHash_str = model.Key + "|" + cmnNameGetUserCard + "|" + model.User_credentials + "|" + model.Salt;
                    string getUserCardHash = Generatehash512(getUserCardHash_str).ToLower();
                    arr.Add("get_user_cards_hash", getUserCardHash);

                    string cmnNameEditUserCard = "edit_user_card";

                    string editUserCardHash_str = model.Key + "|" + cmnNameEditUserCard + "|" + model.User_credentials + "|" + model.Salt;
                    string editUserCardHash = Generatehash512(editUserCardHash_str).ToLower();
                    arr.Add("edit_user_card_hash", editUserCardHash);

                    string cmnNameSaveUserCard = "save_user_card";
                    string saveUserCardHash_str = model.Key + "|" + cmnNameSaveUserCard + "|" + model.User_credentials + "|" + model.Salt;
                    string saveUserCardHash = Generatehash512(saveUserCardHash_str).ToLower();
                    arr.Add("save_user_card_hash", saveUserCardHash);

                    string cmnPaymentRelatedDetailsForMobileSdk = "payment_related_details_for_mobile_sdk";
                    string detailsForMobileSdk_str = model.Key + "|" + cmnPaymentRelatedDetailsForMobileSdk + "|" + model.User_credentials + "|" + model.Salt;
                    string detailsForMobileSdk = Generatehash512(detailsForMobileSdk_str).ToLower();
                    arr.Add("payment_related_details_for_mobile_sdk_hash", detailsForMobileSdk);
                }

                if (!string.IsNullOrEmpty(model.Offerkey))
                {
                    string cmnCheckOfferStatus = "check_offer_status";
                    string checkOfferStatus_str = model.Key + "|" + cmnCheckOfferStatus + "|" + model.Offerkey + "|" + model.Salt;
                    string checkOfferStatus = Generatehash512(checkOfferStatus_str).ToLower();
                    arr.Add("check_offer_status_hash", checkOfferStatus);
                }
                obj = new { Success = 1, Message = "Successfull.", data = arr };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }

            //string key = "z1u2iD";
            //string salt = "M3bxdMaT";
            //string hashString = key + "|" + txnid + "|" + model.Amount + "|" + model.Product_info + "|" + model.Firstname + "|" + model.Email + "|||||||||||" + salt;
            ////string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            //string hash = Generatehash512(hashString);
            return obj;
        }

        private string Generatetxnid()
        {
            Random rnd = new Random();
            string x = rnd.Next(Int32.MaxValue).ToString();
            string strHash = Generatehash512(x + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }



        private string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }

        public class OrderInfoForPayuBiz
        {
            public decimal Amount { get; set; }
            public string Email { get; set; }
            public string Product_info { get; set; }
            public string Firstname { get; set; }
            public string Key { get; set; }
            public string Salt { get; set; }
            public string Txnid { get; set; }
            public string User_credentials { get; set; }
            public string SURL { get; set; }
            public string FURL { get; set; }
            public string Offerkey { get; set; }
        }

    }
}
