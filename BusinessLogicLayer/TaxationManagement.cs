using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class TaxationManagement : TaxRepository
    {
        private string serversting { get; set; }
        public TaxationManagement(string server)
        {
            this.serversting = server;
        }

        public string InsertUpdate_Taxation(TaxationMaster taxationMaster)
        {
            throw new NotImplementedException();
        }

        public string InsertUpdate_FranchiseTaxDetail(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(serversting);
            return (DisplayMessage(obj.SetRecords("InsertUpdate_FranchiseTaxDetail", paramValues, opr), opr.ToString(), "Franchise Tax Detail"));

        }

        private string DisplayMessage(int QryResult, string opr, string value)
        {
            switch (QryResult)
            {
                case 0: return "No Operation Performed on \" " + value + "\"";
                case 1: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 2: return "\"" + value + "\" " + opr.ToString().ToUpper() + "D Successfully!!";
                case 3: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 4: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Priority Level..!!";
                case 10: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid City..!!";
                case 11: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Franchise..!!";
                case 12: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Taxation selection..!!";
                case 13: return "The Record to perform " + opr.ToString().ToUpper() + " Entry for tax in franchse already present..!!";
                case 14: return "The Record to perform " + opr.ToString().ToUpper() + " Record not found..!!";              
                default: return "No Operation Performed Perform";
            }
        }
        public string InsertUpdate_ProductTax(Int64 ID, int TaxID, bool IsActive, DateTime CreateDate, Int64 CreateBy, DateTime ModifyDate, Int64 ModifyBy, string NetworkIP, string DeviceType, string DeviceID, DataTable dt, string Operation)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serversting))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdate_ProductTax", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@ID", SqlDbType.BigInt).Value = ID;
                    sqlComm.Parameters.AddWithValue("@ShopStockID", SqlDbType.Structured).Value = dt;
                    sqlComm.Parameters.AddWithValue("@TaxID", SqlDbType.Int).Value = TaxID;
                    sqlComm.Parameters.AddWithValue("@IsActive", SqlDbType.Bit).Value = IsActive;
                    sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime).Value = CreateDate;
                    sqlComm.Parameters.AddWithValue("@CreateBy", SqlDbType.BigInt).Value = CreateBy;
                    sqlComm.Parameters.AddWithValue("@ModifyDate", SqlDbType.DateTime).Value = ModifyDate;
                    sqlComm.Parameters.AddWithValue("@ModifyBy", SqlDbType.BigInt).Value = ModifyBy;
                    sqlComm.Parameters.AddWithValue("@NetworkIP", SqlDbType.VarChar).Value = NetworkIP;
                    sqlComm.Parameters.AddWithValue("@DeviceType", SqlDbType.VarChar).Value = DeviceType;
                    sqlComm.Parameters.AddWithValue("@DeviceID", SqlDbType.VarChar).Value = DeviceID;
                    sqlComm.Parameters.AddWithValue("@Mode", SqlDbType.VarChar).Value = Operation;
                    sqlComm.Parameters.AddWithValue("@QryResult", SqlDbType.Int).Value = 0;

                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
                return "Records Insert/Updation Successfully";
            }
            catch (Exception ex)
            {
                //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                return "Unable Records Insert/Updation Successfully";
            }
        }

        public List<ProductVarientByCategoryAndShop> ProductVarientByCategoryAndShop(long CatgeoryID, long ShopId, int TaxMasterID, bool IsInclusiveOfTax, int ShowOnlyBranded)
        {
            List<ProductVarientByCategoryAndShop> ls = new List<ProductVarientByCategoryAndShop>();
           
                DataTable dt = new DataTable();
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
                List<object> paramValues = new List<object>();
                paramValues.Add(CatgeoryID);
                paramValues.Add(ShopId);
                paramValues.Add(TaxMasterID);
                paramValues.Add(IsInclusiveOfTax);
                paramValues.Add(ShowOnlyBranded);
                dt = dbOpr.GetRecords("ProductVarientByCategoryAndShop", paramValues);

                try
                {
                    ls = (from n in dt.AsEnumerable()
                          select new ProductVarientByCategoryAndShop
                          {
                              ShopStockID = n.Field<Int64>("ShopStockID"),
                              ShopID = n.Field<Int64>("ShopID"),
                              ShopName = n.Field<string>("ShopName"),
                              ProductID = n.Field<Int64>("ProductID"),
                              ProductName = n.Field<string>("ProductName"),
                              BrandName = n.Field<string>("BrandName"), // By Zubair on 01-07-2017
                              CategoryID = n.Field<int>("CategoryID"),
                              CategoryName = n.Field<string>("CategoryName"),
                              SizeID = n.Field<int>("SizeID"),
                              SizeName = n.Field<string>("SizeName"),
                              ColorID = n.Field<int>("ColorID"),
                              ColorName = n.Field<string>("ColorName"),
                              MaterialID = n.Field<int>("MaterialID"),
                              MaterialName = n.Field<string>("MaterialName"),
                              ReorderLevel = n.Field<int>("ReorderLevel"),
                              PackUnitID = n.Field<int>("PackUnitID"),
                              UnitName = n.Field<string>("UnitName"),
                              PackSize = n.Field<decimal>("PackSize"),
                              MRP = n.Field<decimal>("MRP"),
                              RetailerRate = n.Field<decimal>("RetailerRate"),
                              QTY = n.Field<int>("QTY")
                          }).ToList();
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => new { x.ErrorMessage, x.PropertyName });

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

            return ls;
        }

        public List<ProductVarientByCategoryAndShop> ProductTaxIndex(long CatgeoryID, long ShopId, int TaxMasterID, bool IsInclusiveOfTax)
        {
            List<ProductVarientByCategoryAndShop> ls = new List<ProductVarientByCategoryAndShop>();

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(CatgeoryID);
            paramValues.Add(ShopId);
            paramValues.Add(TaxMasterID);
            dt = dbOpr.GetRecords("ProductTaxIndex", paramValues);

            try
            {
                ls = (from n in dt.AsEnumerable()
                      select new ProductVarientByCategoryAndShop
                      {
                          ProductTaxID = n.Field<Int64>("ProductTaxID"),
                          ShopStockID = n.Field<Int64>("ShopStockID"),
                          ShopID = n.Field<Int64>("ShopID"),
                          ShopName = n.Field<string>("ShopName"),
                          ProductID = n.Field<Int64>("ProductID"),
                          ProductName = n.Field<string>("ProductName"),
                          CategoryID = n.Field<int>("CategoryID"),
                          CategoryName = n.Field<string>("CategoryName"),
                          SizeID = n.Field<int>("SizeID"),
                          SizeName = n.Field<string>("SizeName"),
                          ColorID = n.Field<int>("ColorID"),
                          ColorName = n.Field<string>("ColorName"),
                          MaterialID = n.Field<int>("MaterialID"),
                          MaterialName = n.Field<string>("MaterialName"),
                          ReorderLevel = n.Field<int>("ReorderLevel"),
                          PackUnitID = n.Field<int>("PackUnitID"),
                          UnitName = n.Field<string>("UnitName"),
                          PackSize = n.Field<decimal>("PackSize"),
                          MRP = n.Field<decimal>("MRP"),
                          RetailerRate = n.Field<decimal>("RetailerRate"),
                          QTY = n.Field<int>("QTY"),
                          isSelected = n.Field<bool>("IsActive")
                      }).ToList();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            return ls;
        }

        public List<CalulatedTaxesRecord> CalculateTaxForProduct(Int64 ShopStockID)
        {

            List<CalulatedTaxesRecord> ls = new List<CalulatedTaxesRecord>();

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopStockID);
            dt = dbOpr.GetRecords("CalculateTax_sp", paramValues);

            ls = (from n in dt.AsEnumerable()
                  select new CalulatedTaxesRecord
                  {
                      ShopStockID = n.Field<Int64>("ShopStockID"),
                      TaxName = n.Field<string>("TaxName"),
                      TaxPercentage = n.Field<decimal>("TaxPercentage"),
                      TaxableAmount = n.Field<decimal>("TaxAmount"),
                      ProductTaxID = n.Field<Int64>("ProductTaxID"),
                      TaxPrefix = n.Field<string>("Prefix"),
                      IsGSTInclusive = n.Field<bool>("IsGSTInclusive") // Added By Zuair for GST on 05-07-2017
                  }).ToList();

            return ls;
        }
        public List<CalulatedTaxesRecord> CalculateTaxForMultipalProduct(List<Int64> ShopStockID)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("ShopStockID");



            foreach (Int64 pa in ShopStockID)
            {
                DataRow dr = dt.NewRow();
                dr["ShopStockID"] = pa;
                dt.Rows.Add(dr);
            }

            List<CalulatedTaxesRecord> ls = new List<CalulatedTaxesRecord>();


            DataTable ResultDt = new DataTable();
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(serversting);
            SqlCommand cmd = new SqlCommand("CalculateMultipalProductTax", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ShopstockList", SqlDbType.Structured).Value = dt;
            SqlDataAdapter sqlDt = new SqlDataAdapter(cmd);
            sqlDt.Fill(ResultDt);



            ls = (from n in ResultDt.AsEnumerable()
                  select new CalulatedTaxesRecord
                  {
                      ShopStockID = n.Field<Int64>("ShopStockID"),
                      TaxName = n.Field<string>("TaxName"),
                      TaxPercentage = n.Field<decimal>("TaxPercentage"),
                      TaxableAmount = n.Field<decimal>("TaxAmount"),
                      ProductTaxID = n.Field<Int64>("ProductTaxID"),
                      TaxPrefix = n.Field<string>("Prefix"),
                      IsGSTInclusive = n.Field<bool>("IsGSTInclusive") // Added By Sonali for GST on 19-09-2018
                  }).ToList();


            return ls;

        }

        public static string GetDisplayMessage(int QryResult, string opr, string value)
        {
            switch (QryResult)
            {
                case 0: return "No Operation Performed on \" " + value + "\"";
                case 1: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 2: return "\"" + value + "\" " + opr.ToString().ToUpper() + "D Successfully!!";
                case 3: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 4: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Priority Level..!!";
                case 10: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Franchise..!!";
                case 11: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Category..!!";
                case 12: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Shop..!!";
                case 13: return "The Record to perform " + opr.ToString().ToUpper() + " IsActive Can't null..!!";
                case 14: return "The Record to perform " + opr.ToString().ToUpper() + " Sequence Order Can't null..!!";
                case 15: return "The Record to perform " + opr.ToString().ToUpper() + " Is already Exists..!!";
                case 16: return "The Record to perform " + opr.ToString().ToUpper() + " Record Not Found..!!";
                default: return "No Operation Performed Perform";
            }
        }

        public List<IndirectTaxSetting> Select_IndirectTaxSetting(int FranchaiseID, int TaxID)
        {
            List<IndirectTaxSetting> ls = new List<IndirectTaxSetting>();

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchaiseID);
            paramValues.Add(TaxID);
            dt = dbOpr.GetRecords("Select_IndirectTaxSetting", paramValues);

            ls = (from n in dt.AsEnumerable()
                  select new IndirectTaxSetting
                  {
                      FranchiseID = n.Field<int>("FranchiseID"),
                      FranmchiseName = n.Field<string>("FranchiseName"),
                      TaxName = n.Field<string>("MainTax"),
                      DependTaxName = n.Field<string>("BaseOn"),
                      TaxPercentage = n.Field<decimal>("InPercentage"),
                      TaxableAmount = n.Field<decimal>("InRupees"),
                      IsIncludeSaleRate = n.Field<bool>("IsIncludeSaleRate"),
                      IsOnTaxSum = n.Field<bool>("IsOnTaxSum"),
                      ID = n.Field<int>("ID")

                  }).ToList();

            return ls;
        }


        public static  bool GetTaxStatus(string StrInclusiveOfTax)
        {
            try
            {
                if (StrInclusiveOfTax != string.Empty)
                {
                    string[] NewinclusiveOfTax = StrInclusiveOfTax.Split(',');
                    return Convert.ToBoolean(NewinclusiveOfTax[1]);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
    }
    interface TaxRepository
    {
        string InsertUpdate_Taxation(TaxationMaster taxationMaster);
        string InsertUpdate_FranchiseTaxDetail(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr);

        string InsertUpdate_ProductTax(Int64 ID, int TaxID, bool IsActive, DateTime CreateDate, Int64 CreateBy, DateTime ModifyDate, Int64 ModifyBy, string NetworkIP, string DeviceType, string DeviceID, DataTable dt, string Operation);

        List<ProductVarientByCategoryAndShop> ProductVarientByCategoryAndShop(long CatgeoryID, long ShopId, int TaxMasterID, bool IsInclusiveOfTax, int ShowOnlyBranded);
        List<ProductVarientByCategoryAndShop> ProductTaxIndex(long CatgeoryID, long ShopId, int TaxMasterID, bool IsInclusiveOfTax);

        List<CalulatedTaxesRecord> CalculateTaxForProduct(Int64 ShopStockID);

        List<CalulatedTaxesRecord> CalculateTaxForMultipalProduct(List<Int64> ShopStockID);

        List<IndirectTaxSetting> Select_IndirectTaxSetting(int FranchaiseID, int TaxID);

    }    
}
