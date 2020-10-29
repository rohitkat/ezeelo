//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using System.Data;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This class used to give ALL BusinessOwner Name and ID
    /// On the basis Business Type ID
    /// BusinessTypeID : It Identify the Prefix of the Business Type
    /// GandhibaghSuperAdmin "GBSA"
    /// Merchant:           "GBMR"
    /// Franchise:          "GBFR"
    /// DeliveryPartner:    "GBDP"
    /// Advertiser:         "GBAD"
    /// 
    /// </summary>
    
    public class OwnerDetailByPrefix
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public Int64 ID { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// BusinessTypeID : It Identify the Prefix of the Business Type
        /// </summary>
        /// <param name="businessTypeID">Business Type ID</param>
        /// <returns>OwnerDetailByPrefix type List</returns>
        public List<OwnerDetailByPrefix> OwnerDetail(int? businessTypeID, System.Web.HttpServerUtility server)
        {
            try
            {

                BusinessType businessType = new BusinessType();
                businessType = db.BusinessTypes.Single(x => x.ID == businessTypeID);

               List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();
                /* if (businessType.Prefix.Equals("GBMR"))
                    lownerType = this.BindMerchant();

                else if (businessType.Prefix.Equals("GBFR"))
                    lownerType = this.BindFranchise();

                else if (businessType.Prefix.Equals("GBDP"))
                    lownerType = this.BindDeliveryPatner();

                else if (businessType.Prefix.Equals("GBAD"))
                    lownerType = this.BindAdvertisers();

                return lownerType;
                */

                lownerType = OwnerDetail(businessType.Prefix, server);

                return lownerType;

            }
            catch (Exception ex)
            {
                throw new Exception(" Problem in Retriving OwnerDetail :- " + ex.Message);
            }

        }

        private List<OwnerDetailByPrefix> OwnerDetail(string prefix, System.Web.HttpServerUtility server)
        {
            try
            {

                DataTable dt = new DataTable();
                ReadConfig config = new ReadConfig(server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);

                dt = dbOpr.GetRecords("OwnerDetailByPrefix", prefix);

                List<OwnerDetailByPrefix> lOwnerType = new List<OwnerDetailByPrefix>();

                for (int iteam = 0; iteam < dt.Rows.Count; iteam++)
                {
                    OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
                    dd.ID = Convert.ToInt64(dt.Rows[iteam]["OwnerID"]);
                    dd.Name = dt.Rows[iteam]["Name"].ToString();

                    lOwnerType.Add(dd);
                }

                return (lOwnerType);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrive data of Business owner :- " + ex.InnerException);
            }

        }
                
        /// <summary>
        /// List of Franchise Detail
        /// </summary>
        /// <returns>List of Name and ID</returns>
        public List<OwnerDetailByPrefix> BindFranchise()
        {
            List<OwnerDetailByPrefix> lOwnerType = new List<OwnerDetailByPrefix>();

            foreach (var iteam in db.Franchises.ToList().Where(x => x.IsActive == true))
            {
                OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
                dd.ID = iteam.ID;
                dd.Name = iteam.ContactPerson;

                lOwnerType.Add(dd);
            }

            return (lOwnerType);
        }

        /// <summary>
        /// List of Franchise Detail
        /// </summary>
        /// <returns>List of Name and ID</returns>
        public List<OwnerDetailByPrefix> BindMerchant()
        {
            List<OwnerDetailByPrefix> lOwnerType = new List<OwnerDetailByPrefix>();

            foreach (var iteam in db.Shops.ToList().Where(x => x.IsActive == true))
            {
                OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
                dd.ID = iteam.ID;
                dd.Name = iteam.Name;

                lOwnerType.Add(dd);
            }

            return (lOwnerType);
        }

        /// <summary>
        /// List of Advertiser Detail
        /// </summary>
        /// <returns>List of Name and ID</returns>
        public List<OwnerDetailByPrefix> BindAdvertisers()
        {
            List<OwnerDetailByPrefix> lOwnerType = new List<OwnerDetailByPrefix>();

            foreach (var iteam in db.Advertisers.ToList().Where(x => x.IsActive == true))
            {
                OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
                dd.ID = iteam.ID;
                dd.Name = iteam.PersonalDetail.FirstName + ' ' + iteam.PersonalDetail.MiddleName + ' ' + iteam.PersonalDetail.LastName; ;

                lOwnerType.Add(dd);
            }

            return (lOwnerType);
        }
        
        /// <summary>
        /// List of Delivery Patner Detail
        /// </summary>
        /// <returns>List of Name and ID</returns>
        public List<OwnerDetailByPrefix> BindDeliveryPatner()
        {
            List<OwnerDetailByPrefix> lOwnerType = new List<OwnerDetailByPrefix>();

            foreach (var iteam in db.DeliveryPartners.ToList().Where(x => x.IsActive == true))
            {
                OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
                dd.ID = iteam.ID;
                dd.Name = iteam.PersonalDetail.FirstName + ' ' + iteam.PersonalDetail.MiddleName + ' ' + iteam.PersonalDetail.LastName;
                lOwnerType.Add(dd);
            }

            return (lOwnerType);
        }


        

    }
   
}
