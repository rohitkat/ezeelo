using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            this.AccountTransactions = new List<AccountTransaction>();
            this.APITokens = new List<APIToken>();
            this.BusinessDetails = new List<BusinessDetail>();
            this.CustomerOrders = new List<CustomerOrder>();
            this.CustomerShippingAddresses = new List<CustomerShippingAddress>();
            this.CustomerValletBalances = new List<CustomerValletBalance>();
            this.Employees = new List<Employee>();
            this.LoginAttempts = new List<LoginAttempt>();
            this.LoginSecurityAnswers = new List<LoginSecurityAnswer>();
            this.PersonalDetails = new List<PersonalDetail>();
            this.TransactionInputProcessAccounts = new List<TransactionInputProcessAccount>();
            this.UserAdditionalMenus = new List<UserAdditionalMenu>();
            this.UserRoles = new List<UserRole>();
            this.UserThemes = new List<UserTheme>();
            this.WishLists = new List<WishList>();
            //// this.DeliveryBoyAttendances = new List<DeliveryBoyAttendance>();//Add by Ashish Nagrale  // Hide from Ashish for Live

        }

        public long ID { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string OnlineUserHash { get; set;}
        public virtual List<AccountTransaction> AccountTransactions { get; set; }
        public virtual List<APIToken> APITokens { get; set; }
        public virtual List<BusinessDetail> BusinessDetails { get; set; }
        public virtual List<CustomerOrder> CustomerOrders { get; set; }
        public virtual List<CustomerShippingAddress> CustomerShippingAddresses { get; set; }
        public virtual List<CustomerValletBalance> CustomerValletBalances { get; set; }
        public virtual List<Employee> Employees { get; set; }
        public virtual List<LoginAttempt> LoginAttempts { get; set; }
        public virtual List<LoginSecurityAnswer> LoginSecurityAnswers { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<PersonalDetail> PersonalDetails { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        public virtual PersonalDetail PersonalDetail3 { get; set; }
        public virtual ICollection<TransactionInputProcessAccount> TransactionInputProcessAccounts { get; set; }
        public virtual List<UserAdditionalMenu> UserAdditionalMenus { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
        public virtual List<UserTheme> UserThemes { get; set; }
        public virtual List<WishList> WishLists { get; set; }

        //------Added by Shaili on 21-07-19----------------//
        public virtual ICollection<MerchantRating> MerchantRating { get; set; }
        //------------End--------------------------------//
        //// public virtual ICollection<DeliveryBoyAttendance> DeliveryBoyAttendances { get; set; }//Add by Ashish Nagrale  // Hide from Ashish for Live
    }
}
