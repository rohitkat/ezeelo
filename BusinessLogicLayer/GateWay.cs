/// --- class used for SMS and Email GateWays
/// Author : Prashant
/// Version : 1.0.0     Date : Feb 3, 2015 4.30pm
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// Name Space GateWays Used for Sending Email and SMS
namespace BusinessLogicLayer
{
    /// <summary>
    /// Initialises to use the facility of gateways for sending eMial and SMS
    /// </summary>
    public class GateWay
    {
        /// <summary>
        /// Private variable of Type httpServerutility for accessing the path 
        /// of the server and files physical location
        /// </summary>
        protected System.Web.HttpServerUtility server;

        /// <summary>
        /// Initialises a Gateways
        /// </summary>
        /// <param name="server">Server Utility Object</param>
        public GateWay(System.Web.HttpServerUtility server)
        {
            this.server = server;
        }



        /// <summary>
        /// Enumerator which Identifies what EMAIL GateWay to Use
        /// </summary>
        public enum EmailGateWays
        {
            /// <summary>
            /// EMail GateWay Which is registered with eZeelo
            /// </summary>
            GANDHIBAGH,

            ///// <summary>
            ///// EMail GateWay which is registered with gmail.com
            ///// </summary>
            GMAIL
        }

        /// <summary>
        /// Enumerator which Identifies what SMS GateWay to Use
        /// </summary>
        public enum SMSGateWays
        {
            /// <summary>
            /// SMS GateWay Which is to Be used for Sending SMS
            /// </summary>
            SUMIT
        }

        /// <summary>
        /// Enumerator which Identifies the Sender Email ID
        /// </summary>
        public enum SenderMail
        {
            /// <summary>
            /// Specifies the Sender eMail ID of Type INFO
            /// </summary>
            INFO,

            /// <summary>
            /// Specifies the Sender eMail ID of tYpe Support
            /// </summary>
            SUPPORT,

            EZEELOPAYOUT,
            /// <summary>
            /// Payment
            /// </summary>

        }

        /// <summary>
        /// Enumerator Which identifies the type of the EMail to be sent
        /// </summary>
        public enum EMailTypes
        {
            /// <summary>
            /// Account Activation Link
            /// </summary>
            ACT_LINK,

            /// <summary>
            /// Account Activation Link
            /// </summary>
            ON_REQUEST_SEND_CRM_EWALLETREFUND,

            /// <summary>
            /// When merchant registers on eZeelo
            /// </summary>
            MER_WELCOME,

            /// <summary>
            /// When merchant registers on eZeelo
            /// </summary>
            MER_FINAL,

            /// <summary>
            /// When merchant registers successfully (Sign Up) on eZeelo
            /// </summary>
            MER_SIGN_UP,

            /// <summary>
            /// When merchant gets organizational approval
            /// </summary>
            MER_APPROVED,

            /// <summary>
            /// When employee gets organizational approval
            /// </summary>
            EMP_APPROVED,

            /// <summary>
            /// When franchise gets organizational approval
            /// </summary>
            FRN_APPROVED,

            /// <summary>
            /// When Delivery Partner gets organizational approval
            /// </summary>
            DP_APPROVED,

            /// <summary>
            /// When order is placed by Customer and EMail is to be Sent to the Merchant
            /// </summary>
            MER_ORD_PLACED,

            /// <summary>
            /// When Order has been cancelled (by the customer) 
            /// </summary>
            ORD_CANCELLED_CUST,

            /// <summary>
            /// When merchant adds new component/adds new unit price 
            /// </summary>
            MER_ADD_CMP,

            /// <summary>
            /// When merchant changes product price/stock prices
            /// </summary>
            MER_PRICE_CHANGE,

            /// <summary>
            /// When merchant's e-shop has low inventory 
            /// </summary>
            MER_LOW_INV,

            /// <summary>
            /// When order is cancelled by merchant due to inability to contact customer
            /// </summary>
            ORD_CANCELLED_MER,

            /// <summary>
            /// When merchant is not responding to pending order 
            /// </summary>
            MER_CNF_DELAYED,

            /// <summary>
            /// When feed back is sent to merchant 
            /// </summary>
            MER_REWD,

            /// <summary>
            /// When customer registers    
            /// </summary>
            CUST_WELCOME,

            /// <summary>
            /// When order is placed and EMail is to be sent to the customer
            /// </summary>   
            /// 

            CUST_PAYOUT_REQUEST,

            /// <summary>
            /// Email send When customer requested for wallete payout request
            /// </summary>   
            /// 

            ON_SUCCESS_CUST_PAYMENT,





           


            /// <summary>
            /// Email send When customer requested for EWALLET REFUND CANCEL OREDER IS DECLINE request
            /// </summary>   
            /// 

            EWALLET_REFUND_CANCEL_LEADER_ADMIN,

            /// <summary>
            /// Email send When customer requested for EWALLET REFUND PARTIAL CANCEL ORDER IS ACCEPTED request
            /// </summary>   
            /// 

            EWALLET_PACCEPTED_LEADERS_ADMIN,

            /// <summary>
            /// Email send When customer requested for EWALLET REFUND CANCEL OREDER IS ACCEPTED request
            /// </summary>   
            /// 

            EWALLET_ACCEPTED_LEADERS_ADMIN,

            /// <summary>
            /// Email send When customer requested for EWALLET REFUND PARTIAL CANCEL ORDER IS DECLINE request
            /// </summary>   
            /// 

            EWALLET_REFUND_PCANCEL_LEADERS_ADMIN,

            /// <summary>
            /// When customer payment to account after payout request
            /// </summary> 
            /// 
            ON_APPROVAL_ACCOUNT_TEAM_PAYOUT_REQUEST,
            /// <summary>
            /// When admin accept payout request and notify to  account 
            /// /// </summary> 
            /// 

            ON_CUST_PAYOUT_REQUEST,
            /// <summary>
            /// Send to customer when customer make payout request 
            /// /// </summary> 
            /// 

            ON_REQUEST_SEND_CRM,
            /// <summary>
            /// Send to CRM team, When customer make payout request  
            /// /// </summary> 
            /// 

            PAYOUT_REQUEST,
            /// <summary>
            /// Send to account section, When customer make payout request  
            /// /// </summary> 
            /// 

            ON_CANCEL_PAYOUT_REQUEST,
            /// <summary>
            /// When  payout request is cancelled
            /// /// </summary> 
            /// 
            CUST_ORD_PLACED,

            /// <summary>
            /// When customer cancels order 
            /// </summary>
            CUST_ORD_CANCELLED_REPLY,

            /// <summary>
            /// Confirmed Order( after merchant confirmation) 
            /// </summary>
            CUST_ORD_CNF,

            /// <summary>
            /// Added by Tejaswee
            /// Information SMS to the Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH_COD,

            /// <summary>
            /// Added by Tejaswee
            /// Information SMS to the Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH_COD_ONLINE,

            /// <summary>
            /// Add By Ashish Nagrale
            /// When order is dispatched by merchant
            /// </summary>
            CUST_ORD_DSPTCH,

            /// <summary>
            /// When order is delivered 
            /// </summary>
            CUST_ORD_DLVRD,

            /// <summary>
            /// When order is not delivered due to inability to contact customer. 
            /// </summary>
            CUST_NOT_DLVRD,

            /// <summary>
            /// When is OTP is sent whiel customer is registering
            /// </summary>
            CUST_OTP_REG,

            /// <summary>
            /// When OTP regenerate limit is exceeds
            /// </summary>
            ADM_CUST_REG_LIM_EXDS,

            /// <summary>
            /// Mail Sent for PWD Recovery
            /// </summary>
            PWD_RCVRY,

            /// <summary>
            /// Mail to be sent to customer after order is returned
            /// </summary>
            CUST_ORD_RTND,

            /// <summary>
            /// Mail to be sent to User on Forget Password
            /// </summary>
            FORGET_PASS,

            /// <summary>
            /// Mail to send Merchant ID
            /// </summary>
            MERCHANT_ID,

            /// <summary>
            /// Subscription mail to customer
            /// </summary>
            CUST_SUBSCRIPTION,
            /// <summary>
            /// sUBSCRIPTION Purchase mail to customer
            /// </summary>
            CUST_PURCHASE_SUBSCRIPTION,

            /// <summary>
            /// mail to customer as well as CRM for customer product/shop suggesion
            /// </summary>
            CUST_LOOKING_FOR,

            /// <summary>
            /// mail to merchant after registration
            /// </summary>
            MER_REGISTRATION,

            /// <summary>
            /// mail to franchise after registration
            /// </summary>
            FRN_REGISTRATION,


            FEEDBACK_MANAGEMENT,

            /// <summary>
            /// Send email to customer on feedBACK submit
            /// </summary>
            FEEDBACK_CUSTOMER,

            /// <summary>
            ///send OTP to customer on forgot passward link
            /// </summary>
            CUST_OTP,

            /// <summary>
            /// Subscription mail to customer
            /// </summary>
            CUST_NEW_SUBSCRIPTION,

            /// <summary>
            /// email to customer when total order get fulfilled
            /// </summary>
            CUST_ORD_FULFILL,

            ///// <summary>
            ///// email to customer when total order is not get fulfilled
            ///// </summary>
            //CUST_PTL_ORD_FULFILL,

            /// <summary>
            /// email to customer when he/she update the order
            /// </summary>
            CUST_CHN_ORD,

            /// <summary>
            /// Email to the Candidate who apply for the Job Or Application for Join
            /// </summary>
            CAREER_APPLICATION,

            /// <summary>
            /// Added by Manoj
            /// Send SMS to refer user
            /// </summary>
            CUST_REFER_TO,


            /// <summary>
            /// Send feedback reply email to customer from Partner.
            /// </summary>
            FEED_CUST_FROM_ADMIN,

            /// <summary>
            /// Send email for quotation request yashaswi 30/3/2018
            /// </summary>
            QUOT_REQUEST,

            /// <summary>
            /// Send email to send invoice 24/4/2018
            /// </summary>
            SEND_INVOICE,

            /// <summary>
            /// Send email to when purchase order is created 02/05/2018
            /// </summary>
            PURCHASE_ORDER,

            /// <summary>
            /// Send email to user for Yashaswi For PartnerRequestModule 9/5/2018
            /// </summary>
            PARTNER_REQUEST_USER,
            /// <summary>
            /// Send email to Admin for Yashaswi For PartnerRequestModule 9/5/2018
            /// </summary>
            PARTNER_REQUEST_ADMIN,

            /// <summary>
            /// Send email to warehouse from supplier 17/5/2018
            /// </summary>
            PURCHASE_ORDER_REFUSAL,

            Leaders_InviteUser,
            LEADERS_WALLET_PAYBACK_APPROVE, //yashaswi 12/9/2018
            LEADERS_WALLET_PAYBACK_ACCEPT, //yashaswi 12/9/2018
            PROMO_ERP_PAYOUT, //yashaswi 12/9/2018
            /// <summary>
            /// Send email to user  For Become Leader by Sonali on 07-03-2019
            /// </summary>
            BECOMELEADER_USER,
            /// <summary>
            /// Send email to admin  For Become Leader by Sonali on 07-03-2019
            /// </summary>
            BECOMELEADER_ADMIN,
            /// <summary>
            /// Send Reminder email to admin  For Become Leader by Sonali on 07-03-2019
            /// </summary>
            BECOMELEADER_ADMIN_REMINDER,
            ACCEPT_MERCHANT,
            APPROVE_MERCHANT,
            VALIDATE_MERCHANT,
            APPROVE_MERCHANT_SECOND_ACCOUNT,
            MERCHANT_NOTIFICATION
        }

        /// <summary>
        /// SMS Sending Options
        /// </summary>
        public enum SMSOptions
        {
            /// <summary>
            /// Specifies to Send SMS to Single User/Recipient only
            /// </summary>
            SINGLE,

            /// <summary>
            /// Specifies to Send SMS to Multiple Users/Recipients
            /// </summary>
            MULTIPLE,

            /// <summary>
            /// Specifies to Send Unicode SMS to Single User/Recipient only
            /// </summary>
            UNI_SINGLE,

            /// <summary>
            /// Specifies to Send Unicode SMS to Multiple Users/Recipients
            /// </summary>
            UNI_MULTIPLE,

            /// <summary>
            /// Specifies to Send Scheduled SMS
            /// </summary>
            SCHEDULED

        }

        /// <summary>
        /// Enumerator Which identifies the type of the SMS/SMS Text to be sent
        /// </summary>
        public enum SMSTypes
        {

            /// <summary>
            /// OTP for Merchant Registration / Mobile Verification
            /// </summary>
            OTP_MER_REG,


            ///<summary>
            /// message for custmer about CANCELL Ewallet Refund 
            ///</summary>
            CANCEL_EWALLET_REFUND_REQUEST,

            ///<summary>
            /// message for custmer about PARTIAL CANCEL Ewallet Refund
            ///</summary>
            CANCEL_PARTIAL_EWALLET_REFUND_REQUEST,


            ///<summary>
            /// message for custmer about Ewallet Refund
            ///</summary>
            ACCEPT_EWALLET_REFUND,

            ///<summary>
            /// message for custmer about Ewallet Refund PARTIAL
            ///</summary>
            ACCEPT_EWALLET_REFUND_PARTIAL,

            /// <summary>
            /// OTP for Customer Registration / Mobile Verification
            /// </summary>
            OTP_CUST_REG,

            /// <summary>
            ///  Notification to customer when regenerate SMS exceeds limits
            /// </summary>
            CUST_REG_LIM_EXDS,


            /// <summary>
            /// Notification to admin when regenerate SMS exceeds limits
            /// </summary>
            ADM_CUST_REG_LIM_EXDS,
            /// <summary>
            /// OTP for Request of Password Change by Any Portal user
            /// </summary>
            OTP_PWD_CHNG,

            /// <summary>
            /// OTP for Guest check out
            /// </summary>
            OTP_GUEST_CHECKOUT,

            /// <summary>
            /// SMS to Be sent to the Customer after Successfull Order placed
            /// </summary>
            CUST_ORD_PLCD,

            /// <summary>
            /// SMS to Be sent to the Merchant after Successfull Order placed
            /// </summary>
            MER_ORD_PLCD,



            /// <summary>
            /// Welcome SMS to the Merchant i.e. Seller after successfull registration/enrollment
            /// </summary>
            MER_WELCOME,

            /// <summary>
            /// SMS when Merchant completes Registration Process  
            /// </summary>
            MER_SIGN_UP,

            /// <summary>
            /// Welcome SMS to the Customer i.e. Buyer after successfull registration/enrollment
            /// </summary>
            CUST_WELCOME,

            /// <summary>
            /// Alert SMS to the Merchant after he changes price of any of the item displayed in his e-shop on the portal
            /// </summary>
            /// 


            CUST_PAYMNT_BANK,
            /// <summary>
            /// Credited amount SMS to customer bank account after successfull transaction of Payout Request
            /// </summary>
            /// 

            MER_PRC_CHNG,

            /// <summary>
            /// Alert SMS to the merchant if any of the product he sells throgh the portal reaches minimum stock
            /// </summary>
            MER_LOW_INV,

            /// <summary>
            /// Information SMS to the merchant after he gets Succeesfull Approval by the portal team
            /// </summary>
            MER_APRVD,

            /// <summary>
            /// Information SMS to the franchise after he gets Succeesfull Approval by the portal team
            /// </summary>
            FRN_APRVD,

            /// <summary>
            /// Information SMS to the Delivery Partner after he gets Succeesfull Approval by the portal team
            /// </summary>
            DP_APRVD,

            /// <summary>
            /// Information SMS to the employee after he gets Succeesfull Approval by the portal team
            /// </summary>
            EMP_APRVD,

            /// <summary>
            /// Confirmation SMS to the Customer after he cancells order
            /// </summary>
            CUST_ORD_CAN,

            /// <summary>
            /// confirmation SMSs to the Merchant after Order placed to his/her e-shop gets cancelled
            /// </summary>
            MER_ORD_CAN,

            /// <summary>
            /// Confirmation SMS to the customer when Merchant confirms his order
            /// </summary>
            CUST_ORD_CNF,

            /// <summary>
            /// Information SMS to the Customer after his order/product deliverd successfuly
            /// </summary>
            CUST_ORD_DLVRD,

            /// <summary>
            /// Added by Tejaswee
            /// Information SMS to the Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH,

            /// <summary>
            /// Information SMS to the Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH_COD,

            /// <summary>
            /// Added by Tejaswee
            /// Information SMS to the Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH_COD_ONLINE,

            ///// <summary>
            ///// Add By Ashish Nagrale
            ///// Information SMS to the COD Customer after his order/product dispatched from Merchant's SHOP successfuly
            ///// </summary>
            //CUST_ORD_DSPTCH,

            /// <summary>
            /// Add By Ashish Nagrale
            /// Information SMS to the Net Banking Customer after his order/product dispatched from Merchant's SHOP successfuly
            /// </summary>
            CUST_ORD_DSPTCH_PAID,

            /// <summary>
            /// Information SMS to the Customer when his order returened back 
            /// </summary>
            CUST_ORD_RTRND,

            /// <summary>
            /// SMS to Delivery Partner when consignment is ready to dsipatch
            /// </summary>
            DP_DSPTCH,

            /// <summary>
            /// SMS to Merchant when one of the componenet of his product changes
            /// </summary>
            MER_CMP_RATE_CHNGE,
            /// <summary>
            /// SMS to send OTP when user forgets his/her password
            /// </summary>
            OTP_FORGET_PASS,

            /// <summary>
            /// send sms to customer after placing order
            /// </summary>

            CUST_SUBSCR_ORDER_PLACE,

            /// <summary>
            /// send app download link to customer
            /// </summary>
            CUST_APP_DOWNLOAD_LINK,

            /// <summary>
            /// Subscription Purchase Sms to Customer
            /// </summary>
            CUST_SUBSCR_PURCHASE,

            /// <summary>
            /// sms to customer as well as CRM for customer product/shop suggesion
            /// </summary>
            CUST_LOOKING_FOR,

            /// <summary>
            /// sms to merchant after registration
            /// </summary>
            MER_REG,

            /// <summary>
            /// sms to franchise after registration
            /// </summary>
            FRN_REG,

            /// <summary>
            /// Send new password to customer on forget password link
            /// </summary>
            CUST_SEND_PWD,

            /// <summary>
            /// Send new password to customer on forget password link
            /// </summary>
            AFTER_PAYMENT_DONE_PAYOUT_REQUEST,

            /// <summary>
            /// sms to customer when wallet amount transfered to account after payout request
            /// </summary>
            /// 

            CUST_PAYOUT_REQUEST,

            /// <summary>
            /// sms to customer when customer request for payout
            /// </summary>
            /// 
            ACCEPT_PAYOUT_REQUEST,

            /// <summary>
            /// sms to customer when payout request is accepted by admin section
            /// </summary>
            /// 
            CANCEL_PAYMENT_PAYOUT_REQUEST,
            /// <summary>
            /// sms to customer when payout request is cancelled 
            /// </summary>
            /// 
            CUST_ORD_FULFILL,

            /// <summary>
            /// sms to customer when total order is not get fulfilled
            /// </summary>
            CUST_PTL_ORD_FULFILL,

            /// <summary>
            /// sms to customer when he/she update the order
            /// </summary>
            CUST_CHN_ORD,

            /// <summary>
            /// Added by Manoj
            /// Send SMS to refer user
            /// </summary>
            CUST_REFER_TO,

            /// <summary>
            /// Send feedback reply SMS to customer from Partner
            /// </summary>
            FEED_CUST_FROM_ADMIN,
            LEADERS_WALLET_PAYBACK_APPROVE, //YASHASWI 12/9/2018
            LEADERS_WALLET_PAYBACK_ACCEPT, //YASHASWI 12/9/2018
            PROMOERPPAYOUT, //YASHASWI 11/02/2018
            KYC_CMPT_REQUEST,
            UPLINE_FOR_BOOST_PLAN,
            MERCHANT_REG_LEADER,
            MERCHANT_REG,
            MERCHANT_REG_ACCEPT,
            MERCHANT_REG_APPROVE,
            MERCHANT_REG_APPROVE_LEADER,
            MERCHANT_REG_REJECT,
            MERCHANT_REG_REJECT_LEADER,
            MERCHANT_REG_KYC,
            MERCHANT_TRANSACTION_USER,
            MERCHANT_RECHARGE_REQUEST,
            MERCHANT_RECHARGE_WARNING,
            MERCHANT_TRANSACTION_2WARNING,
            LEADER_RECHARGE_WARNING,
            LEADER_TRANSACTION_2WARNING,
            MERCHANT_TRANSACTION_REQUEST_USER,
            MERCHANT_TRANSACTION_REQUEST_MERCHANT,
            MERCHANT_TRANSACTION_REQUEST_ACCEPT,
            MERCHANT_TRANSACTION_REQUEST_REJECT
        }


        /// <summary>
        /// Method to send Emails to the Recipients
        /// </summary>
        /// <param name="emailGateWay">Specify which Gateway to Use</param>
        /// <param name="sender">Specify the Sender Email Account</param>
        /// <param name="emailType">Specify Which Type of Email to Send</param>
        /// <param name="recipients">Email IDs of recipients </param>
        /// <param name="emailValues">Specify Values which will be Filled within blanks of Standard EMial text</param>
        /// <param name="isBodyHtml">Specifies whether Body Is HTML or Not</param>
        /// <returns>TRUE : If mail Sent Successfully; FALSE : If failed</returns>
        /// <exception cref="LogFile.MyException">LogFile.MyException</exception>
        public virtual bool SendEmail(EmailGateWays emailGateWay, SenderMail sender,
            EMailTypes emailType, string[] recipients, Dictionary<string, string> emailValues, bool isBodyHtml)
        { return false; }


        /// <summary>
        /// Method to send SMS to the Recipients
        /// </summary>
        /// <param name="smsGateWay">Specify which SMS Gateway to Use</param>
        /// <param name="smsOptions">Select SMS Sending options e.g. SINGLE, MULTIPLE, UNICODE, etc</param>
        /// <param name="smsType">Specify Which Type of SMS to Send</param>
        /// <param name="recipients">A String Array of Recipients Valid Mobile Number. 
        /// <para>Only First Value from Array shall be preffered if SINGLE option Selected</para>
        /// <para>Maximum 10 Recipients Allowed for MULTIPLE option</para></param>
        /// <param name="smsValues">Values that will be replaced against keyword within specified SMS text</param>
        /// <returns>TRUE : If SMS Sent Successfully; FALSE : If failed</returns>
        /// <exception cref="LogFile.MyException">LogFile.MyException</exception>        
        public virtual bool SendSMS(SMSGateWays smsGateWay, SMSOptions smsOptions,
            SMSTypes smsType, string[] recipients, Dictionary<string, string> smsValues)
        { return false; }
    }

}
