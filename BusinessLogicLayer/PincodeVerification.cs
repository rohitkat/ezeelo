//-----------------------------------------------------------------------
// <copyright file="PincodeVerification.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------

using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    /// <summary>
    /// This class contains methods related to Delivery Pincodes 
    /// </summary>
    public class PincodeVerification : DeliveryManagement
    {
         /// <summary>
         /// Check product is deliverable at selected pincode
         /// </summary>
         /// <param name="lPinCode">6 digit pincode</param>
         /// <returns></returns>       
        public bool IsDeliverablePincode(string lPinCode)
        {
            return IsVerifiedPincode(lPinCode);
               
        }
        /// <summary>
        /// Check product is deliverable at selected pincode
        /// </summary>
        /// <param name="lPinCode">6 digit pincode</param>
        /// <returns></returns>
        private bool IsVerifiedPincode(string lPinCode)
        {
            
            var lPincodeList = (from pc in db.Pincodes
                                join dp in db.DeliveryPincodes on pc.ID equals dp.PincodeID
                                join dpartner in db.DeliveryPartners on dp.DeliveryPartnerID equals dpartner.ID
                                where dp.DeliveryPartnerID == 1 && pc.IsActive == true && dp.IsActive == true && dpartner.IsLive == true
                                && pc.Name == lPinCode 
                                select new
                                {
                                    lpin = pc.Name 
                                }).ToList();
        
            return lPincodeList.Count() >0? true:false;      
            
        }

        // start Yashaswi allow only selected franchise area      

        public bool IsDeliverableArea(long AreaId, long SelectedArea)
        {
            return IsVerifiedArea(AreaId, SelectedArea);

        }
        private bool IsVerifiedArea(long AreaId, long SelectedAreaID)
        {

            var lArea = db.Areas
                //.Where(a => (db.Areas.Where(aa => aa.ID == SelectedAreaID).Select(aa => aa.PincodeID)).Contains(a.PincodeID))
                    .Join(db.FranchiseLocations.Where(f => (db.FranchiseLocations.Where(fl => fl.AreaID == SelectedAreaID).Select(fl => fl.FranchiseID).Contains(f.FranchiseID)))
                    , a => a.ID, f => f.AreaID, (a, f) => new { ID = a.ID, Name = a.Name }
                    ).ToList();
            if (lArea != null)
            {
                return lArea.Any(a => a.ID == AreaId);
            }
            else
            {
                return false;
            }

        }
        //End
    }
  
}
