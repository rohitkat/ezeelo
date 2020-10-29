using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using System.Web.ModelBinding;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace BusinessLogicLayer
{
    /// <summary>
    /// oprStatus : 1 All Inserted Successfully
    ///             3 Product is not in CorporateCategory
    ///             4 OrderID is not Present/Placed
    ///             5 Place Order Qty and Total Qty Not Match
    ///             6 Inable to insert data in CorporateCustomerShippingDeliveryDetails
    ///             7 Inable to insert data in CorporateOrderShippingFacilityDetails
    ///             9 Unable to Parse Values in CorporateCustomerShippingDeliveryDetails model
    ///             10 Unable to Parse Values in CorporateOrderShippingFacilityDetails model
    /// </summary>

    public class CorporateGifting : OrderConform, ICorporate
    {
        /// <summary>
        /// DataBase Connection
        /// </summary>
        protected EzeeloDBContext db = new EzeeloDBContext();

        private int _totalQty { get; set; }

        /// <summary>
        /// Current Placed Order ID 
        /// </summary>
        private Int64 _orderID { get; set; }

        /// <summary>
        /// Login Id Of Order placed Person
        /// </summary>
        private Int64 _orderBy { get; set; }


        /// <summary>
        /// Operation Status
        /// </summary>
        public int oprStatus = 0;
        /// <summary>
        /// All List of Shipping Address
        /// </summary>
        public List<CorporateCustomerShippingDeliveryDetail> parseconFormOrderInsert { get; set; }

        /// <summary>
        /// List Of Facilities
        /// </summary>
        private List<CorporateOrderShippingFacilityDetail> parsecorporateFacilityDetail { get; set; }

        /// <summary>
        /// Allowed CategoryName
        /// </summary>
        private string _CategoryName = "Corporate Gifting";

        /// <summary>
        /// Connection String
        /// </summary>
        private string _Connection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CorporateGifting()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection"></param>
        public CorporateGifting(string dbConnection)
        {
            this._Connection = dbConnection.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="OrderByID"></param>
        /// <param name="orderID"></param>
        /// <param name="totalQty"></param>
        /// <param name="OrderTo"></param>
        /// <param name="facilityList"></param>
        public CorporateGifting(Int64 ProductID, Int64 OrderByID, Int64 orderID, int totalQty, List<CorporateCustomerShippingAddressViewModel> OrderTo, List<CorporateFacilityDetailsViewModel> facilityList)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (db.CustomerOrderDetails.Where(x => x.ID == orderID).Count() < 1)
                    {
                        this.oprStatus = 4;
                    }
                    else if (IsCorporate(ProductID))
                    {
                        this._orderID = orderID;
                        this._orderBy = OrderByID;
                        this._totalQty = totalQty;
                        this.OrderDetail(OrderTo, facilityList);

                        if (this.InsertUpdateOrder(parseconFormOrderInsert) && this.InserUpdateFacility(parsecorporateFacilityDetail))
                        {
                            this.oprStatus = 1;
                        }
                    }


                    ts.Complete();
                }
                catch (Exception exception)
                {
                    this.oprStatus = 0;
                    // Rollback transaction
                    // throw new Exception("NOT INSERTED");
                    ts.Dispose();

                }

            }
        }

        public CorporateGifting(Int64 ProductID, Int64 OrderByID, Int64 orderID, int totalQty, List<CorporateCustomerShippingAddressViewModel> OrderTo)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (db.CustomerOrderDetails.Where(x => x.ID == orderID).Count() < 1)
                    {
                        this.oprStatus = 4;
                    }
                    else if (IsCorporate(ProductID))
                    {
                        this._orderID = orderID;
                        this._orderBy = OrderByID;
                        this._totalQty = totalQty;

                        int deliveryQtySum = (from n in OrderTo.AsEnumerable()
                                              select new { n.Quantity }).ToList().Sum(x => x.Quantity);


                        if (this.IsOrderQuantityParse(deliveryQtySum))
                        {
                            List<CorporateCustomerShippingDeliveryDetail> conformOrderInsert = new List<CorporateCustomerShippingDeliveryDetail>();
                            parseconFormOrderInsert = this.ShippingAddessDetail(OrderTo);

                        }
                        else
                        {
                            this.oprStatus = 5;
                        }

                        if (this.InsertUpdateOrder(parseconFormOrderInsert))
                        {
                            this.oprStatus = 1;
                        }
                    }


                    ts.Complete();
                }
                catch (Exception exception)
                {
                    this.oprStatus = 0;
                    //Rollback transaction
                    // throw new Exception("NOT INSERTED");
                    ts.Dispose();

                }

            }
        }

        /// <summary>
        /// Update Actual Model using supplied View Model
        /// </summary>
        /// <param name="OrderTo">Ordered Shipping Detail</param>
        /// <param name="facilityList">Facility List</param>
        public void OrderDetail(List<CorporateCustomerShippingAddressViewModel> OrderTo, List<CorporateFacilityDetailsViewModel> facilityList)
        {
            int deliveryQtySum = (from n in OrderTo.AsEnumerable()
                                  select new { n.Quantity }).ToList().Sum(x => x.Quantity);


            if (this.IsOrderQuantityParse(deliveryQtySum))
            {
                List<CorporateOrderShippingFacilityDetail> FacilityToInsert = new List<CorporateOrderShippingFacilityDetail>();
                
                parsecorporateFacilityDetail = this.FacilitiesAdd(facilityList);

                List<CorporateCustomerShippingDeliveryDetail> conformOrderInsert = new List<CorporateCustomerShippingDeliveryDetail>();
                parseconFormOrderInsert = this.ShippingAddessDetail(OrderTo);

            }
            else
            {
                this.oprStatus = 5;
            }
        }

        private bool IsOrderQuantityParse(int deliveryQtySum)
        {
            if (deliveryQtySum == this._totalQty)
            {
                return true;
            }
            else
            {
                return false;
            }


        }


        #region ----- InheritClass Body -----
        /// <summary>
        /// Insert All Order Shipping Addresses
        /// </summary>
        /// <param name="conFormOrderInsert">All Shipping Address</param>
        /// <returns>bool</returns>
        public override bool InsertUpdateOrder(List<CorporateCustomerShippingDeliveryDetail> conFormOrderInsert)
        {
            try
            {

                db.CorporateCustomerShippingDeliveryDetails.AddRange(conFormOrderInsert);
                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                oprStatus = 6;
                return false;
            }
        }

        /// <summary>
        /// Insert All Facilities 
        /// </summary>
        /// <param name="corFacilityInsert">Facilities List</param>
        /// <returns>bool</returns>
        public override bool InserUpdateFacility(List<CorporateOrderShippingFacilityDetail> corFacilityInsert)
        {
            try
            {
                db.CorporateOrderShippingFacilityDetails.AddRange(corFacilityInsert);
                db.SaveChanges();
                return true;
            }

            catch (Exception ex)
            {
                this.oprStatus = 7;
                return false;
            }
            
        }

        #endregion

        #region ----- Interface Body Defination -----

        /// <summary>
        /// Check whether an give product is valid for such shopping
        /// </summary>
        /// <param name="ProductID">ProductID</param>
        /// <returns></returns>
        public bool IsCorporate(Int64 ProductID)
        {
            Int64 categoryID = db.Products.Where(x => x.ID == ProductID).FirstOrDefault().CategoryID;

            string levelOneCategory = (from n3 in db.Categories
                                       join n2 in db.Categories on n3.ParentCategoryID equals n2.ID
                                       join n1 in db.Categories on n2.ParentCategoryID equals n1.ID
                                       where n3.ID == categoryID
                                       select new { n1.Name }).FirstOrDefault().Name.ToString().Trim();

            if (levelOneCategory.ToUpper().Equals(this._CategoryName.ToUpper()))
            {
                return true;
            }
            else
            {
                this.oprStatus = 3;
                return false;
            }

        }

        /// <summary>
        /// Update Actual Shipping Address Model from view model
        /// </summary>
        /// <param name="shippingAddressList">Shipping Address ViewModel</param>
        /// <returns>CorporateCustomerShippingDeliveryDetail</returns>
        public List<CorporateCustomerShippingDeliveryDetail> ShippingAddessDetail(List<CorporateCustomerShippingAddressViewModel> shippingAddressList)
        {
            try
            {
                List<CorporateCustomerShippingDeliveryDetail> conFormOrderInsert = new List<CorporateCustomerShippingDeliveryDetail>();
                for (int i = 0; i < shippingAddressList.Count; i++)
                {
                    CorporateCustomerShippingDeliveryDetail obj = new CorporateCustomerShippingDeliveryDetail();
                  
                    obj.CustomerOrderDetailID = this._orderID;
                    obj.FromUserLoginID = this._orderBy;
                    obj.ToName = shippingAddressList[i].ToName.Trim();
                    obj.DeliveryCharges = shippingAddressList[i].DeliveryCharges;
                    obj.ExpectedDeliveryDate = shippingAddressList[i].ExpectedDeliveryDate;
                    obj.Quantity = shippingAddressList[i].Quantity;
                    obj.PrimaryMobile = shippingAddressList[i].PrimaryMobile;
                    obj.SecondaryMobile = shippingAddressList[i].SecondaryMobile;
                    obj.ShippingAddress = shippingAddressList[i].ShippingAddress;
                    obj.PincodeID = shippingAddressList[i].PincodeID;
                    obj.AreaID = Convert.ToInt32(shippingAddressList[i].AreaID);
                    obj.IsActive = true;
                    obj.CreateDate = DateTime.UtcNow.AddHours(5.30);
                    obj.CreateBy = CommonFunctions.GetPersonalDetailsID(2);
                    obj.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    obj.DeviceType = "x";
                    obj.DeviceID = "x";

                    conFormOrderInsert.Add(obj);
                }

                return conFormOrderInsert;
            }
            catch
            {
                this.oprStatus = 9;
                return null;
            }
        }

        /// <summary>
        /// Update Actual Facilities Model form View Model
        /// </summary>
        /// <param name="FacilityDetails">Facilities View Model</param>
        /// <returns>CorporateOrderShippingFacilityDetail</returns>
        public List<CorporateOrderShippingFacilityDetail> FacilitiesAdd(List<CorporateFacilityDetailsViewModel> FacilityDetails)
        {
            try
            {
                List<CorporateOrderShippingFacilityDetail> facilitiesToInsert = new List<CorporateOrderShippingFacilityDetail>();
                for (int i = 0; i < FacilityDetails.Count(); i++)
                {
                    CorporateOrderShippingFacilityDetail obj = new CorporateOrderShippingFacilityDetail();
                    //obj.ID = 0;
                    obj.CustomerOrderDetailID = Convert.ToInt64(FacilityDetails[i].CustomerOrderDetailID);
                    obj.CorporateshippingFacilityID = FacilityDetails[i].FacilityID;
                    obj.ShippingFacilityCharges = Convert.ToDecimal(FacilityDetails[i].ShippingFacilityCharges);
                    obj.IsActive = true;
                    obj.CreateDate = DateTime.UtcNow.AddHours(5.30);
                    obj.CreateBy = CommonFunctions.GetPersonalDetailsID(2);
                    obj.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    obj.DeviceType = "x";
                    obj.DeviceID = "x";
                    facilitiesToInsert.Add(obj);
                }

                return facilitiesToInsert;
            }
            catch
            {
                this.oprStatus = 10;
                return null;
            }
        }

        #endregion

    }

    /// <summary>
    /// Class for virtual Insert Method
    /// </summary>
    public class OrderConform
    {
        /// <summary>
        /// Insert All Order Shipping Addresses
        /// </summary>
        /// <param name="conFormOrderInsert">All Shipping Address</param>
        /// <returns>bool</returns>
        public virtual bool InsertUpdateOrder(List<CorporateCustomerShippingDeliveryDetail> conFormOrderInsert)
        { return false; }

        /// <summary>
        /// Insert All Facilities 
        /// </summary>
        /// <param name="corFacilityInsert">Facilities List</param>
        /// <returns>bool</returns>
        public virtual bool InserUpdateFacility(List<CorporateOrderShippingFacilityDetail> corFacilityInsert)
        { return false; }

    }

}
