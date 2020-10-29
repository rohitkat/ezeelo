//-----------------------------------------------------------------------
// <copyright file="CustomerShippingDetails.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerShippingDetails : CustomerManagement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server"></param>
        public CustomerShippingDetails(System.Web.HttpServerUtility server) : base(server) { }
        /// <summary>
        /// This method gives the list of shipping addresses maintained by customer.
        /// </summary>
        /// <param name="userLoginID"> customer Login ID</param>
        /// <returns>List of shipping addresses.</returns>
        public List<CustomerShippingAddressesViewModel> GetCustomerShippingAddresses(long userLoginID)
        {
            List<CustomerShippingAddressesViewModel> lCustAddresses = new List<CustomerShippingAddressesViewModel>();
            lCustAddresses = (from cust in db.CustomerShippingAddresses
                              join pin in db.Pincodes on cust.PincodeID equals pin.ID
                              join ct in db.Cities on pin.CityID equals ct.ID
                              join ds in db.Districts on ct.DistrictID equals ds.ID
                              join st in db.States on ds.StateID equals st.ID
                              where cust.UserLoginID == userLoginID //By sonali && cust.IsActive == true
                              orderby cust.CreateDate descending
                              select new CustomerShippingAddressesViewModel
                              {
                                  AddressID = cust.ID,
                                  AreaID = cust.AreaID,
                                  IsDeliveryAddress = cust.IsDeliveryAddress,   //By sonali                            
                                  IsSelected = false,
                                  PrimaryMobile = cust.PrimaryMobile,
                                  SecondaryMobile = cust.SecondaryMobile,
                                  ShippingAddress = cust.ShippingAddress,
                                  UserLoginID = cust.UserLoginID,
                                  PincodeID = cust.PincodeID,
                                  PincodeName = pin.Name,
                                  CityID = ct.ID,
                                  CityName = ct.Name,
                                  DistrictID = ds.ID,
                                  DistrictName = ds.Name,
                                  StateID = st.ID,
                                  StateName = st.Name,
                                  FirstName = cust.FirstName,//Sonali_27-10-2018
                                  LastName = cust.LastName//Sonali_27-10-2018
                              }).ToList();

            foreach (var item in lCustAddresses)
            {
                if (item.AreaID != null)
                {
                    item.AreaName = db.Areas.Find(item.AreaID).Name;
                }
                else//Added for AreaId is null by Sonali on 13-05-2019
                {
                    var areas = db.Areas.Where(x => x.IsActive && x.PincodeID == item.PincodeID).FirstOrDefault();
                    if (areas != null)
                    {
                        item.AreaID = areas.ID;
                        item.AreaName = areas.Name;
                    }
                }
                if (string.IsNullOrEmpty(item.FirstName))
                {
                    var personalDetails = db.PersonalDetails.Where(x => x.IsActive && x.UserLoginID == item.UserLoginID).FirstOrDefault();
                    if (personalDetails != null)
                    {
                        item.FirstName = personalDetails.FirstName;
                        item.LastName = personalDetails.LastName;
                    }
                }
                if (item.SecondaryMobile == null)
                {
                    item.SecondaryMobile = string.Empty;
                }//Closed for AreaId is null by Sonali on 13-05-2019
            }
            //By sonali
            //if (lCustAddresses.ToList().Count>0)
            //    lCustAddresses.FirstOrDefault().IsSelected = true;
            return lCustAddresses;
        }

        /// <summary>
        /// This method inserts Customer's Shipping address
        /// </summary>
        /// <param name="Addr">object of CustomerShippingAddress</param>
        /// <returns>Operation status</returns>
        public int CreateCustomerShippingAddress(CustomerShippingAddress Addr)
        {

            int oprStatus = 0;

            try
            {
                Addr.CreateBy = CommonFunctions.GetPersonalDetailsID(Addr.UserLoginID);
                Addr.CreateDate = CommonFunctions.GetLocalTime();
                Addr.IsActive = true;
                Addr.DeviceType = Addr.DeviceType;
                db.CustomerShippingAddresses.Add(Addr);
                db.SaveChanges();
                oprStatus = 101;
            }
            catch (Exception exception)
            {
                oprStatus = 500;
                BusinessLogicLayer.ErrorLog.ErrorLogFile("<P:BusinessLogicLayer><C:CustomerShippingDetails><M:CreateCustomerShippingAddress>" + exception.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

            }
            finally
            {

                db.Dispose();
            }
            return oprStatus;

        }
        /// <summary>
        /// Edit Customer Shipping address
        /// </summary>
        /// <param name="Addr">CustomerShippingAddress</param>
        /// <returns>operation status</returns>
        public int EditCustomerShippingAddress(CustomerShippingAddress Addr)
        {

            int oprStatus = 0;

            try
            {
                CustomerShippingAddress lAddr = db.CustomerShippingAddresses.Where(x => x.ID == Addr.ID && x.UserLoginID == Addr.UserLoginID).FirstOrDefault();

                if (lAddr == null)
                    return 104;
                Addr.UserLoginID = lAddr.UserLoginID;//Added By Sonali for Api 10-10-2018
                Addr.CreateBy = lAddr.CreateBy;
                Addr.CreateDate = lAddr.CreateDate;
                Addr.IsActive = true;
                Addr.DeviceID = lAddr.DeviceID;//Added By Sonali for Api 10-10-2018
                Addr.DeviceType = Addr.DeviceType;
                Addr.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                Addr.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                Addr.ModifyBy = CommonFunctions.GetPersonalDetailsID(Addr.UserLoginID);
                Addr.ModifyDate = CommonFunctions.GetLocalTime();

                db.Entry(lAddr).CurrentValues.SetValues(Addr);
                db.SaveChanges();
                oprStatus = 102;

            }
            catch (Exception exception)
            {
                oprStatus = 500;
                BusinessLogicLayer.ErrorLog.ErrorLogFile("<P:BusinessLogicLayer><C:CustomerShippingDetails><M:EditCustomerShippingAddress> :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            finally
            {

                db.Dispose();
            }
            return oprStatus;

        }
        /// <summary>
        /// Delete customer Shipping Address
        /// </summary>
        /// <param name="addrID">ID</param>
        /// <returns>Operation Status</returns>
        public int DeleteCustomerShippingAddress(int addrID)
        {

            int oprStatus = 0;

            try
            {
                CustomerShippingAddress custAddr = db.CustomerShippingAddresses.Find(addrID);
                //Added By Sonali for Api 10-10-2018
                if (custAddr == null)
                {
                    oprStatus = 103;
                }
                else
                {
                    db.CustomerShippingAddresses.Remove(custAddr);
                    db.SaveChanges();
                    oprStatus = 200;
                }
                //End
            }
            catch (Exception exception)
            {
                oprStatus = 500;
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Delete Shipping Addess: Business Logic Layer :" + exception.InnerException + exception.Message, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

                //throw new MyException("<P:BusinessLogicLayer><C:CustomerShippingDetails><M:DeleteCustomerShippingAddress>", exception.Message);
            }
            finally
            {

                db.Dispose();
            }
            return oprStatus;

        }

    }
}
