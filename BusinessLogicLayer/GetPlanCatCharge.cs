//-----------------------------------------------------------------------
// <copyright file="GetPlanCatCharge.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using DataAccessLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{

    public class GetPlanCatCharge
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        /// <summary>
        /// This function get the category charge list of franchise
        /// </summary>
        /// <param name="ownerId">Franchise Id</param>
        /// <returns>returns the category charge list of franchise</returns>
        public List<OwnerPlanCategoryChargeViewModel> GetOwnerPlanCategoryCharge(long? ownerId)
        {
            List<OwnerPlanCategoryChargeViewModel> ownerPlanCategoryCharge = new List<OwnerPlanCategoryChargeViewModel>();
            try
            {
                ReadConfig rc = new ReadConfig(System.Web.HttpContext.Current.Server);
                DbOperations dbOpr = new GetData(rc.DB_CONNECTION);
                List<object> parametervalues = new List<object>();

                parametervalues.Add(ownerId);
                parametervalues.Add("GBFR");

                DataTable dt = dbOpr.GetRecords("SelectOwnerPlanCategoryCharge", parametervalues);

                if (dt.Rows.Count > 0)
                {
                    ownerPlanCategoryCharge = (from DataRow dr in dt.Rows
                                               select new OwnerPlanCategoryChargeViewModel
                                               {
                                                   CategoryName = Convert.ToString(dr["CategoryName"]),
                                                   ChargeName = Convert.ToString(dr["ChargeName"]),
                                                   PlanName = Convert.ToString(dr["PlanName"]),
                                                   CategoryID = (int)dr["CategoryID"],
                                                   ID = (dr["ID"] == DBNull.Value) ? 0 : (int)dr["ID"],
                                                   OwnerPlanID = (dr["OwnerPlanID"] == DBNull.Value) ? 0 : (int)dr["OwnerPlanID"],
                                                   PlanID = (int)dr["PlanID"],
                                                   ChargeID = (int)dr["ChargeID"],
                                                   ChargeInPercent = (decimal)dr["ChargeInPercent"],
                                                   ChargeInRupee = (decimal)dr["ChargeInRupee"],
                                                   IsActive = (dr["IsActive"] == DBNull.Value) ? false : (bool)dr["IsActive"],
                                                   PlanCategoryChargeID = (dr["PlanCategoryChargeID"] == DBNull.Value) ? 0 : (int)dr["PlanCategoryChargeID"]
                                               }).OrderBy(x => x.CategoryName).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return ownerPlanCategoryCharge;
        }

        /// <summary>
        /// This function get the category charge list of Merchant
        /// </summary>
        /// <param name="ownerId">Merchant Id</param>
        /// <param name="franchiseID">Franchise Id with respect to merchant id</param>
        /// <returns>returns the category charge list of Merchant</returns>
        public List<OwnerPlanCategoryChargeViewModel> SelectMerchantPlanCategoryCharge(long? ownerId, int? franchiseID)
        {
            List<OwnerPlanCategoryChargeViewModel> ownerPlanCategoryCharge = new List<OwnerPlanCategoryChargeViewModel>();
            try
            {
                ReadConfig rc = new ReadConfig(System.Web.HttpContext.Current.Server);
                DbOperations dbOpr = new GetData(rc.DB_CONNECTION);
                List<object> parametervalues = new List<object>();

                parametervalues.Add(ownerId);
                parametervalues.Add("GBMR");
                parametervalues.Add(franchiseID);

                DataTable dt = dbOpr.GetRecords("SelectMerchantPlanCategoryCharge", parametervalues);

                if (dt.Rows.Count > 0)
                {
                    ownerPlanCategoryCharge = (from DataRow dr in dt.Rows
                                               select new OwnerPlanCategoryChargeViewModel
                                               {
                                                   CategoryName = Convert.ToString(dr["CategoryName"]),
                                                   ChargeName = Convert.ToString(dr["ChargeName"]),
                                                   PlanName = Convert.ToString(dr["PlanName"]),
                                                   CategoryID = (int)dr["CategoryID"],
                                                   ID = (dr["ID"] == DBNull.Value) ? 0 : (int)dr["ID"],
                                                   OwnerPlanID = (dr["OwnerPlanID"] == DBNull.Value) ? 0 : (int)dr["OwnerPlanID"],
                                                   PlanID = (int)dr["PlanID"],
                                                   ChargeID = (int)dr["ChargeID"],
                                                   ChargeInPercent = (decimal)dr["ChargeInPercent"],
                                                   ChargeInRupee = (decimal)dr["ChargeInRupee"],
                                                   IsActive = (dr["IsActive"] == DBNull.Value) ? false : (bool)dr["IsActive"],
                                                   PlanCategoryChargeID = (dr["PlanCategoryChargeID"] == DBNull.Value) ? 0 : (int)dr["PlanCategoryChargeID"]
                                               }).OrderBy(x => x.CategoryName).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return ownerPlanCategoryCharge;
        }

        /// <summary>
        /// This function get the category charge list with respect to plan
        /// </summary>
        /// <param name="id">Plan Id</param>
        /// <returns>returns the category charge list with respect to plan</returns>
        public List<PlanCategoryChargeViewModel> GetPlanCategoryCharges(int id)
        {
            List<PlanCategoryChargeViewModel> planCategoryCharge = new List<PlanCategoryChargeViewModel>();
            try
            {
                var getvalue = (from pb in db.PlanBinds
                                where pb.PlanID == id
                                select new
                                {
                                    planBindID = pb.ID,
                                    type = pb.Type,
                                    catLevel = pb.Level,
                                }).FirstOrDefault();

                if (getvalue != null)
                {
                    ReadConfig rc = new ReadConfig(System.Web.HttpContext.Current.Server);
                    DbOperations dbOpr = new GetData(rc.DB_CONNECTION);
                    List<object> parametervalues = new List<object>();

                    parametervalues.Add(id);
                    parametervalues.Add(getvalue.catLevel);
                    parametervalues.Add(getvalue.type);
                    parametervalues.Add(getvalue.planBindID);

                    DataTable dt = dbOpr.GetRecords("SelectPlanCategoryCharges", parametervalues);


                    //string query = "[SelectPlanCategoryCharges]";
                    //SqlCommand cmd = new SqlCommand(query);
                    //cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.Parameters.AddWithValue("@PlanID", id);
                    //cmd.Parameters.AddWithValue("@Level", getvalue.catLevel);
                    //cmd.Parameters.AddWithValue("@inclusiveType", getvalue.type);
                    //cmd.Parameters.AddWithValue("@PlanBindID", getvalue.planBindID);

                    //DataTable dt = new DataTable();
                    //using (SqlConnection con = new SqlConnection(GetConnectionString()))
                    //{
                    //    using (SqlDataAdapter sda = new SqlDataAdapter())
                    //    {
                    //        cmd.Connection = con;
                    //        sda.SelectCommand = cmd;
                    //        sda.Fill(dt);
                    //    }
                    //}

                    if (dt.Rows.Count > 0)
                    {
                        planCategoryCharge = (from DataRow dr in dt.Rows
                                              select new PlanCategoryChargeViewModel
                                              {
                                                  CategoryName = Convert.ToString(dr["CategoryName"]),
                                                  ChargeName = Convert.ToString(dr["ChargeName"]),
                                                  CategoryID = (int)dr["CategoryID"],
                                                  ID = (int)dr["PlanChargesID"],
                                                  PlanID = id,
                                                  ChargeID = (int)dr["ChargeID"],
                                                  ChargeInPercent = (decimal)dr["ChargeInPercent"],
                                                  ChargeInRupee = (decimal)dr["ChargeInRupee"],
                                                  IsActive = (bool)dr["IsActive"]
                                              }).OrderBy(x => x.CategoryName).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return planCategoryCharge;
        }
    }

    public class MerchantPlan
    {
        public int ID { get; set; }
        public string ShortName { get; set; }
    }
    public class GetMerchantPlan
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public List<MerchantPlan> GetMerchantPlanAsPerFranchise(int? franchiseId)
        {
            List<MerchantPlan> merplanList = new List<MerchantPlan>();
            try
            {
                ReadConfig rc = new ReadConfig(System.Web.HttpContext.Current.Server);
                DbOperations dbOpr = new GetData(rc.DB_CONNECTION);
                List<object> parametervalues = new List<object>();

                parametervalues.Add(franchiseId);

                DataTable dt = dbOpr.GetRecords("SelectMerchantPlanAsPerFranchise", parametervalues);

                if (dt.Rows.Count > 0)
                {
                    merplanList = (from DataRow dr in dt.Rows
                                   select new MerchantPlan
                                               {
                                                   ID = (dr["PlanId"] == DBNull.Value) ? 0 : (int)dr["PlanId"],
                                                   ShortName = Convert.ToString(dr["PlanShortname"])

                                               }).OrderBy(x => x.ShortName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetMerchantPlanAsPerFranchise]", "Can't Get Merchant Plan As Per Franchise! in Method !" + Environment.NewLine + ex.Message);
            }
            return merplanList;
        }
    }
}
