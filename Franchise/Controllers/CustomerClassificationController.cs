using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.Data.SqlClient;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    public class CustomerClassificationController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        private static string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();

        public class ForLoopClass //----------------use this class for loop purpose in below functions--------------
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }
        //
        // GET: /CustomerClassification/
        public ActionResult Index()
        
        {
            BindCities();
            string InitialFromDate = "01/4/2017"; //Fixed date considered as Ezeelo.com Start date
            String FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            String ToDate = DateTime.Now.ToString("dd/MM/yyyy");

             
            DataTable dtMainCategory = BindDataTable("CustomerClassificationFirstLevelCategory",InitialFromDate, FromDate,ToDate,null,null);
            DataTable dtVeg = BindDataTable("CustomerClassificationFruitsAndVegitablesCategory", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtGrocery = BindDataTable("CustomerClassificationGroceryCategory", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtMeat = BindDataTable("CustomerClassificationMeatAndPoultry", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtOther = BindDataTable("CustomerClassificationOtherCategory", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtPet = BindDataTable("CustomerClassificationPetCareProducts", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtOffer = BindDataTable("CustomerClassificationOfferProducts", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtFMCG = BindDataTable("CustomerClassificationFMCGProducts", InitialFromDate, FromDate, ToDate, null, null);
            DataTable dtMixed = BindDataTable("CustomerClassificationMixedCategoryProducts", InitialFromDate, FromDate, ToDate, null, null);
            
            //Bind Main Category
            List<CustomerClassificationFirstLevelCategory> lCustomerClassificationFirstLevelCategory = new List<CustomerClassificationFirstLevelCategory>();
            List<CustomerClassificationFruitsAndVegitablesCategory> lCustomerClassificationFruitsAndVegitablesCategory = new List<CustomerClassificationFruitsAndVegitablesCategory>();
            List<CustomerClassificationGroceryCategory> lCustomerClassificationGroceryCategory = new List<CustomerClassificationGroceryCategory>();
            List<CustomerClassificationMeatAndPoultry> lCustomerClassificationMeatAndPoultry = new List<CustomerClassificationMeatAndPoultry>();
            List<CustomerClassificationOtherCategory> lCustomerClassificationOtherCategory = new List<CustomerClassificationOtherCategory>();
            List<CustomerClassificationPetCareProducts> lCustomerClassificationPetCareProducts = new List<CustomerClassificationPetCareProducts>();
            List<CustomerClassificationOfferProducts> lCustomerClassificationOfferProducts = new List<CustomerClassificationOfferProducts>();
            List<CustomerClassificationFMCGProducts> lCustomerClassificationFMCGProducts = new List<CustomerClassificationFMCGProducts>();
            List<CustomerClassificationMixedCategoryProducts> lCustomerClassificationMixedCategoryProducts = new List<CustomerClassificationMixedCategoryProducts>();
                      
                
                    if (dtMainCategory != null && dtMainCategory.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtMainCategory.Rows.Count; i++)
                        {
                            CustomerClassificationFirstLevelCategory objCC = new CustomerClassificationFirstLevelCategory();
                            objCC.ImageType = dtMainCategory.Rows[i]["ImageType"].ToString();
                            objCC.ImageCount = Convert.ToInt32(dtMainCategory.Rows[i]["ImageCount"].ToString());
                            objCC.TotalCount = Convert.ToInt32(dtMainCategory.Rows[i]["TotalCount"].ToString());
                            //objCC.Color = dtMainCategory.Rows[i]["Color"].ToString();
                            objCC.Description = dtMainCategory.Rows[i]["Description"].ToString();
                            objCC.LevelID = Convert.ToInt32(dtMainCategory.Rows[i]["LevelID"].ToString());

                            lCustomerClassificationFirstLevelCategory.Add(objCC);
                        }
                    }
                

                //Bind Fruits and Vegitable Category   
                    
                    if (dtVeg != null && dtVeg.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtVeg.Rows.Count; i++)
                        {
                            CustomerClassificationFruitsAndVegitablesCategory objVeg = new CustomerClassificationFruitsAndVegitablesCategory();
                            objVeg.LevelID = Convert.ToInt32(dtVeg.Rows[i]["LevelID"].ToString());
                            objVeg.ImageType = dtVeg.Rows[i]["ImageType"].ToString();
                            objVeg.ImageCount = Convert.ToInt32(dtVeg.Rows[i]["ImageCount"].ToString());
                            objVeg.TotalVegCount = Convert.ToInt32(dtVeg.Rows[i]["TotalVegCount"].ToString());
                            objVeg.Color = dtVeg.Rows[i]["Color"].ToString();
                            objVeg.Condition1 = Convert.ToInt32(dtVeg.Rows[i]["Condition1"].ToString());
                            objVeg.Condition2 = Convert.ToInt32(dtVeg.Rows[i]["Condition2"].ToString());
                            objVeg.Condition3 = Convert.ToInt32(dtVeg.Rows[i]["Condition3"].ToString());
                            objVeg.Condition4 = Convert.ToInt32(dtVeg.Rows[i]["Condition4"].ToString());


                            lCustomerClassificationFruitsAndVegitablesCategory.Add(objVeg);
                        }
                    }                


                //Bind Grocery Category                
                 
                    if (dtGrocery != null && dtGrocery.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtGrocery.Rows.Count; i++)
                        {
                            CustomerClassificationGroceryCategory objGrocery = new CustomerClassificationGroceryCategory();
                            objGrocery.LevelID = Convert.ToInt32(dtGrocery.Rows[i]["LevelID"].ToString());
                            objGrocery.ImageType = dtGrocery.Rows[i]["ImageType"].ToString();
                            objGrocery.ImageCount = Convert.ToInt32(dtGrocery.Rows[i]["ImageCount"].ToString());
                            objGrocery.TotalGroceryCount = Convert.ToInt32(dtGrocery.Rows[i]["TotalGroceryCount"].ToString());
                            objGrocery.Color = dtGrocery.Rows[i]["Color"].ToString();
                            objGrocery.Condition1 = Convert.ToInt32(dtGrocery.Rows[i]["Condition1"].ToString());
                            objGrocery.Condition2 = Convert.ToInt32(dtGrocery.Rows[i]["Condition2"].ToString());
                            objGrocery.Condition3 = Convert.ToInt32(dtGrocery.Rows[i]["Condition3"].ToString());
                            objGrocery.Condition4 = Convert.ToInt32(dtGrocery.Rows[i]["Condition4"].ToString());

                            lCustomerClassificationGroceryCategory.Add(objGrocery);
                        }
                    }                


                //Bind Meat and Poultry                                

                    if (dtMeat != null && dtMeat.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtMeat.Rows.Count; i++)
                        {
                            CustomerClassificationMeatAndPoultry objMeat = new CustomerClassificationMeatAndPoultry();
                            objMeat.LevelID = Convert.ToInt32(dtMeat.Rows[i]["LevelID"].ToString());
                            objMeat.ImageType = dtMeat.Rows[i]["ImageType"].ToString();
                            objMeat.ImageCount = Convert.ToInt32(dtMeat.Rows[i]["ImageCount"].ToString());
                            objMeat.TotalMeatCount = Convert.ToInt32(dtMeat.Rows[i]["TotalMeatCount"].ToString());
                            objMeat.Color = dtMeat.Rows[i]["Color"].ToString();
                            objMeat.Condition1 = Convert.ToInt32(dtMeat.Rows[i]["Condition1"].ToString());
                            objMeat.Condition2 = Convert.ToInt32(dtMeat.Rows[i]["Condition2"].ToString());
                            objMeat.Condition3 = Convert.ToInt32(dtMeat.Rows[i]["Condition3"].ToString());
                            objMeat.Condition4 = Convert.ToInt32(dtMeat.Rows[i]["Condition4"].ToString());

                            lCustomerClassificationMeatAndPoultry.Add(objMeat);
                        }
                    }


                    //Bind Other Category (Festival Needs, Local Delicacies, Restaurant)                                

                    if (dtOther != null && dtOther.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtOther.Rows.Count; i++)
                        {
                            CustomerClassificationOtherCategory objOther = new CustomerClassificationOtherCategory();
                            objOther.LevelID = Convert.ToInt32(dtOther.Rows[i]["LevelID"].ToString());
                            objOther.ImageType = dtOther.Rows[i]["ImageType"].ToString();
                            objOther.ImageCount = Convert.ToInt32(dtOther.Rows[i]["ImageCount"].ToString());
                            objOther.TotalOtherCount = Convert.ToInt32(dtOther.Rows[i]["TotalOtherCount"].ToString());
                            objOther.Color = dtOther.Rows[i]["Color"].ToString();
                            objOther.Condition1 = Convert.ToInt32(dtOther.Rows[i]["Condition1"].ToString());
                            objOther.Condition2 = Convert.ToInt32(dtOther.Rows[i]["Condition2"].ToString());
                            objOther.Condition3 = Convert.ToInt32(dtOther.Rows[i]["Condition3"].ToString());
                            objOther.Condition4 = Convert.ToInt32(dtOther.Rows[i]["Condition4"].ToString());
                            lCustomerClassificationOtherCategory.Add(objOther);

                        }
                    }

                    //Bind Pet Care Products                                

                    if (dtPet != null && dtPet.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtPet.Rows.Count; i++)
                        {
                            CustomerClassificationPetCareProducts objPet = new CustomerClassificationPetCareProducts();
                            objPet.LevelID = Convert.ToInt32(dtPet.Rows[i]["LevelID"].ToString());
                            objPet.ImageType = dtPet.Rows[i]["ImageType"].ToString();
                            objPet.ImageCount = Convert.ToInt32(dtPet.Rows[i]["ImageCount"].ToString());
                            objPet.TotalPetCareCount = Convert.ToInt32(dtPet.Rows[i]["TotalPetCareCount"].ToString());
                            objPet.Color = dtPet.Rows[i]["Color"].ToString();
                            objPet.Condition1 = Convert.ToInt32(dtPet.Rows[i]["Condition1"].ToString());
                            objPet.Condition2 = Convert.ToInt32(dtPet.Rows[i]["Condition2"].ToString());
                            objPet.Condition3 = Convert.ToInt32(dtPet.Rows[i]["Condition3"].ToString());
                            objPet.Condition4 = Convert.ToInt32(dtPet.Rows[i]["Condition4"].ToString());
                            lCustomerClassificationPetCareProducts.Add(objPet);
                        }
                    }


                    //Bind Offer Products                               

                    if (dtOffer != null && dtOffer.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtOffer.Rows.Count; i++)
                        {
                            CustomerClassificationOfferProducts objOffer = new CustomerClassificationOfferProducts();
                            objOffer.LevelID = Convert.ToInt32(dtOffer.Rows[i]["LevelID"].ToString());
                            objOffer.ImageType = dtOffer.Rows[i]["ImageType"].ToString();
                            objOffer.ImageCount = Convert.ToInt32(dtOffer.Rows[i]["ImageCount"].ToString());
                            objOffer.TotalOfferCount = Convert.ToInt32(dtOffer.Rows[i]["TotalOfferCount"].ToString());
                            objOffer.Color = dtOffer.Rows[i]["Color"].ToString();
                            objOffer.Condition1 = Convert.ToInt32(dtOffer.Rows[i]["Condition1"].ToString());
                            objOffer.Condition2 = Convert.ToInt32(dtOffer.Rows[i]["Condition2"].ToString());
                            objOffer.Condition3 = Convert.ToInt32(dtOffer.Rows[i]["Condition3"].ToString());
                            objOffer.Condition4 = Convert.ToInt32(dtOffer.Rows[i]["Condition4"].ToString());
                            lCustomerClassificationOfferProducts.Add(objOffer);
                        }
                    }

                    //Bind FMCG Products                               

                    if (dtFMCG != null && dtFMCG.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtFMCG.Rows.Count; i++)
                        {
                            CustomerClassificationFMCGProducts objFMCG = new CustomerClassificationFMCGProducts();
                            objFMCG.LevelID = Convert.ToInt32(dtFMCG.Rows[i]["LevelID"].ToString());
                            objFMCG.ImageType = dtFMCG.Rows[i]["ImageType"].ToString();
                            objFMCG.ImageCount = Convert.ToInt32(dtFMCG.Rows[i]["ImageCount"].ToString());
                            objFMCG.TotalFMCGCount = Convert.ToInt32(dtFMCG.Rows[i]["TotalFMCGCount"].ToString());
                            objFMCG.Color = dtFMCG.Rows[i]["Color"].ToString();
                            objFMCG.Condition1 = Convert.ToInt32(dtFMCG.Rows[i]["Condition1"].ToString());
                            objFMCG.Condition2 = Convert.ToInt32(dtFMCG.Rows[i]["Condition2"].ToString());
                            objFMCG.Condition3 = Convert.ToInt32(dtFMCG.Rows[i]["Condition3"].ToString());
                            objFMCG.Condition4 = Convert.ToInt32(dtFMCG.Rows[i]["Condition4"].ToString());
                            lCustomerClassificationFMCGProducts.Add(objFMCG);
                        }
                    }


                    //Bind Mixed category Products                               

                    if (dtMixed != null && dtMixed.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtMixed.Rows.Count; i++)
                        {
                            CustomerClassificationMixedCategoryProducts objMixed = new CustomerClassificationMixedCategoryProducts();
                            objMixed.LevelID = Convert.ToInt32(dtMixed.Rows[i]["LevelID"].ToString());
                            objMixed.ImageType = dtMixed.Rows[i]["ImageType"].ToString();
                            objMixed.ImageCount = Convert.ToInt32(dtMixed.Rows[i]["ImageCount"].ToString());
                            objMixed.TotalMixedCount = Convert.ToInt32(dtMixed.Rows[i]["TotalMixedCount"].ToString());
                            objMixed.Color = dtMixed.Rows[i]["Color"].ToString();
                            objMixed.Condition1 = Convert.ToInt32(dtMixed.Rows[i]["Condition1"].ToString());
                            objMixed.Condition2 = Convert.ToInt32(dtMixed.Rows[i]["Condition2"].ToString());
                            objMixed.Condition3 = Convert.ToInt32(dtMixed.Rows[i]["Condition3"].ToString());
                            objMixed.Condition4 = Convert.ToInt32(dtMixed.Rows[i]["Condition4"].ToString());
                            lCustomerClassificationMixedCategoryProducts.Add(objMixed);
                        }
                    }

            

            CustomerClassificationViewModel objCCVM = new CustomerClassificationViewModel();
            objCCVM.lCustomerClassificationFirstLevelCategory = lCustomerClassificationFirstLevelCategory;
            objCCVM.lCustomerClassificationFruitsAndVegitablesCategory = lCustomerClassificationFruitsAndVegitablesCategory;
            objCCVM.lCustomerClassificationGroceryCategory = lCustomerClassificationGroceryCategory;
            objCCVM.lCustomerClassificationMeatAndPoultry = lCustomerClassificationMeatAndPoultry;
            objCCVM.lCustomerClassificationOtherCategory = lCustomerClassificationOtherCategory;
            objCCVM.lCustomerClassificationPetCareProducts = lCustomerClassificationPetCareProducts;
            objCCVM.lCustomerClassificationOfferProducts = lCustomerClassificationOfferProducts;
            objCCVM.lCustomerClassificationFMCGProducts = lCustomerClassificationFMCGProducts;
            objCCVM.lCustomerClassificationMixedCategoryProducts = lCustomerClassificationMixedCategoryProducts;

            objCCVM.InitialFromDate = InitialFromDate;
            objCCVM.FromDate = FromDate;
            objCCVM.ToDate = ToDate;

            return View(objCCVM);
        }

        private DataTable BindDataTable(string StoredProcedure, string InitialFromDate, string FromDate, string ToDate, int? CityId, int? PincodeID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand cmd = new SqlCommand(StoredProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] objParams = new SqlParameter[5];

                objParams[0] = new SqlParameter("@InitialFromDate", InitialFromDate);
                objParams[1] = new SqlParameter("@FromDate", FromDate);
                objParams[2] = new SqlParameter("@ToDate", ToDate);
                objParams[3] = new SqlParameter("@CityID", CityId);
                objParams[4] = new SqlParameter("@PinCodeID", PincodeID);
                int n;
                for (n = 0; n < objParams.Length; n++)
                {
                    cmd.Parameters.Add(objParams[n]);
                }
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {

            }
            return dt;
        }


        private DataTable BindDataTableDetail(string StoredProcedure, string InitialFromDate, string FromDate, string ToDate, int? CityId, int? PincodeID,int LevelID, string Condition)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand cmd = new SqlCommand(StoredProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] objParams = new SqlParameter[7];

                objParams[0] = new SqlParameter("@InitialFromDate", InitialFromDate);
                objParams[1] = new SqlParameter("@FromDate", FromDate);
                objParams[2] = new SqlParameter("@ToDate", ToDate);
                objParams[3] = new SqlParameter("@CityID", CityId);
                objParams[4] = new SqlParameter("@PinCodeID", PincodeID);
                objParams[5] = new SqlParameter("@LevelID", LevelID);
                objParams[6] = new SqlParameter("@Condition", Condition);
                int n;
                for (n = 0; n < objParams.Length; n++)
                {
                    cmd.Parameters.Add(objParams[n]);
                }
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        protected void BindCities()
        {
            ViewBag.CityID = new SelectList(db.Cities.Where(x => x.ID != 1 && x.IsActive == true).OrderBy(x=>x.Name), "ID", "Name");
            //ViewBag.CouponCode = new SelectList(db.CoupenLists.Where(x => x.ID != 1 && x.IsActive == true), "CoupenCode", "CoupenCode");
        }

        public JsonResult BindCities(int? CityID)
        {
            long lFranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            List<Pincode> lPincodeLists = new List<Pincode>();
            var lPincodes = db.Pincodes.Where(x => x.CityID == CityID).ToList();
            foreach (var c in lPincodes)
            {
                Pincode selectPincode = new Pincode();
                selectPincode.ID = c.ID;
                selectPincode.Name = c.Name;
                lPincodeLists.Add(selectPincode);
            }

            return Json(lPincodeLists.Distinct().OrderBy(x => x.ID).ToList(), JsonRequestBehavior.AllowGet);
        }

         //[HttpPost]
        public ActionResult Search(string InitialFromDate, string fromDate, string toDate, int? City, int? PincodeID)
        {
            CustomerClassificationViewModel objCCVM = new CustomerClassificationViewModel();

            if (City != null && City > 0)
            {
                ViewBag.CityID = new SelectList(db.Cities.Where(x => x.ID != 1 && x.IsActive == true), "ID", "Name");

                List<Pincode> lPincode = new List<Pincode>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lPincode = db.Pincodes.Where(x => x.CityID == City).ToList();

                foreach (var c in lPincode)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.City = City;
                ViewBag.lPincode = forloopclasses.ToList();
                objCCVM.CityID = ViewBag.City;               
            }
            else
            {
                ViewBag.CityID = new SelectList(db.Cities.Where(x => x.ID != 1 && x.IsActive == true), "ID", "Name");
            }

            DataTable dtMainCategory = BindDataTable("CustomerClassificationFirstLevelCategory", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtVeg = BindDataTable("CustomerClassificationFruitsAndVegitablesCategory", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtGrocery = BindDataTable("CustomerClassificationGroceryCategory", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtMeat = BindDataTable("CustomerClassificationMeatAndPoultry", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtOther = BindDataTable("CustomerClassificationOtherCategory", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtPet = BindDataTable("CustomerClassificationPetCareProducts", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtOffer = BindDataTable("CustomerClassificationOfferProducts", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtFMCG = BindDataTable("CustomerClassificationFMCGProducts", InitialFromDate, fromDate, toDate, City, PincodeID);
            DataTable dtMixed = BindDataTable("CustomerClassificationMixedCategoryProducts", InitialFromDate, fromDate, toDate, City, PincodeID);
            
            
            
            List<CustomerClassificationFirstLevelCategory> lCustomerClassificationFirstLevelCategory = new List<CustomerClassificationFirstLevelCategory>();
            List<CustomerClassificationFruitsAndVegitablesCategory> lCustomerClassificationFruitsAndVegitablesCategory = new List<CustomerClassificationFruitsAndVegitablesCategory>();
            List<CustomerClassificationGroceryCategory> lCustomerClassificationGroceryCategory = new List<CustomerClassificationGroceryCategory>();
            List<CustomerClassificationMeatAndPoultry> lCustomerClassificationMeatAndPoultry = new List<CustomerClassificationMeatAndPoultry>();
            List<CustomerClassificationOtherCategory> lCustomerClassificationOtherCategory = new List<CustomerClassificationOtherCategory>();
            List<CustomerClassificationPetCareProducts> lCustomerClassificationPetCareProducts = new List<CustomerClassificationPetCareProducts>();
            List<CustomerClassificationOfferProducts> lCustomerClassificationOfferProducts = new List<CustomerClassificationOfferProducts>();
            List<CustomerClassificationFMCGProducts> lCustomerClassificationFMCGProducts = new List<CustomerClassificationFMCGProducts>();
            List<CustomerClassificationMixedCategoryProducts> lCustomerClassificationMixedCategoryProducts = new List<CustomerClassificationMixedCategoryProducts>();


            if (dtMainCategory != null && dtMainCategory.Rows.Count > 0)
            {
                for (int i = 0; i < dtMainCategory.Rows.Count; i++)
                {
                    CustomerClassificationFirstLevelCategory objCC = new CustomerClassificationFirstLevelCategory();
                    objCC.LevelID = Convert.ToInt32(dtMainCategory.Rows[i]["LevelID"].ToString());
                    objCC.ImageType = dtMainCategory.Rows[i]["ImageType"].ToString();
                    objCC.ImageCount = Convert.ToInt32(dtMainCategory.Rows[i]["ImageCount"].ToString());
                    objCC.TotalCount = Convert.ToInt32(dtMainCategory.Rows[i]["TotalCount"].ToString());
                    //objCC.Color = dtMainCategory.Rows[i]["Color"].ToString();
                    objCC.Description = dtMainCategory.Rows[i]["Description"].ToString();

                    lCustomerClassificationFirstLevelCategory.Add(objCC);
                }
            }


            //Bind Fruits and Vegitable Category   

            if (dtVeg != null && dtVeg.Rows.Count > 0)
            {
                for (int i = 0; i < dtVeg.Rows.Count; i++)
                {
                    CustomerClassificationFruitsAndVegitablesCategory objVeg = new CustomerClassificationFruitsAndVegitablesCategory();
                    objVeg.LevelID = Convert.ToInt32(dtVeg.Rows[i]["LevelID"].ToString());
                    objVeg.ImageType = dtVeg.Rows[i]["ImageType"].ToString();
                    objVeg.ImageCount = Convert.ToInt32(dtVeg.Rows[i]["ImageCount"].ToString());
                    objVeg.TotalVegCount = Convert.ToInt32(dtVeg.Rows[i]["TotalVegCount"].ToString());
                    objVeg.Color = dtVeg.Rows[i]["Color"].ToString();
                    objVeg.Condition1 = Convert.ToInt32(dtVeg.Rows[i]["Condition1"].ToString());
                    objVeg.Condition2 = Convert.ToInt32(dtVeg.Rows[i]["Condition2"].ToString());
                    objVeg.Condition3 = Convert.ToInt32(dtVeg.Rows[i]["Condition3"].ToString());
                    objVeg.Condition4 = Convert.ToInt32(dtVeg.Rows[i]["Condition4"].ToString());

                    lCustomerClassificationFruitsAndVegitablesCategory.Add(objVeg);
                }
            }


            //Bind Grocery Category                

            if (dtGrocery != null && dtGrocery.Rows.Count > 0)
            {
                for (int i = 0; i < dtGrocery.Rows.Count; i++)
                {
                    CustomerClassificationGroceryCategory objGrocery = new CustomerClassificationGroceryCategory();
                    objGrocery.LevelID = Convert.ToInt32(dtGrocery.Rows[i]["LevelID"].ToString());
                    objGrocery.ImageType = dtGrocery.Rows[i]["ImageType"].ToString();
                    objGrocery.ImageCount = Convert.ToInt32(dtGrocery.Rows[i]["ImageCount"].ToString());
                    objGrocery.TotalGroceryCount = Convert.ToInt32(dtGrocery.Rows[i]["TotalGroceryCount"].ToString());
                    objGrocery.Color = dtGrocery.Rows[i]["Color"].ToString();
                    objGrocery.Condition1 = Convert.ToInt32(dtGrocery.Rows[i]["Condition1"].ToString());
                    objGrocery.Condition2 = Convert.ToInt32(dtGrocery.Rows[i]["Condition2"].ToString());
                    objGrocery.Condition3 = Convert.ToInt32(dtGrocery.Rows[i]["Condition3"].ToString());
                    objGrocery.Condition4 = Convert.ToInt32(dtGrocery.Rows[i]["Condition4"].ToString());

                    lCustomerClassificationGroceryCategory.Add(objGrocery);
                }
            }


            //Bind Meat and Poultry                                

            if (dtMeat != null && dtMeat.Rows.Count > 0)
            {
                for (int i = 0; i < dtMeat.Rows.Count; i++)
                {
                    CustomerClassificationMeatAndPoultry objMeat = new CustomerClassificationMeatAndPoultry();
                    objMeat.LevelID = Convert.ToInt32(dtMeat.Rows[i]["LevelID"].ToString());
                    objMeat.ImageType = dtMeat.Rows[i]["ImageType"].ToString();
                    objMeat.ImageCount = Convert.ToInt32(dtMeat.Rows[i]["ImageCount"].ToString());
                    objMeat.TotalMeatCount = Convert.ToInt32(dtMeat.Rows[i]["TotalMeatCount"].ToString());
                    objMeat.Color = dtMeat.Rows[i]["Color"].ToString();
                    objMeat.Condition1 = Convert.ToInt32(dtMeat.Rows[i]["Condition1"].ToString());
                    objMeat.Condition2 = Convert.ToInt32(dtMeat.Rows[i]["Condition2"].ToString());
                    objMeat.Condition3 = Convert.ToInt32(dtMeat.Rows[i]["Condition3"].ToString());
                    objMeat.Condition4 = Convert.ToInt32(dtMeat.Rows[i]["Condition4"].ToString());

                    lCustomerClassificationMeatAndPoultry.Add(objMeat);
                }
            }

            //Bind Other Category (Festival Needs, Local Delicacies, Restaurant)                                

            if (dtOther != null && dtOther.Rows.Count > 0)
            {
                for (int i = 0; i < dtOther.Rows.Count; i++)
                {
                    CustomerClassificationOtherCategory objOther = new CustomerClassificationOtherCategory();
                    objOther.LevelID = Convert.ToInt32(dtOther.Rows[i]["LevelID"].ToString());
                    objOther.ImageType = dtOther.Rows[i]["ImageType"].ToString();
                    objOther.ImageCount = Convert.ToInt32(dtOther.Rows[i]["ImageCount"].ToString());
                    objOther.TotalOtherCount = Convert.ToInt32(dtOther.Rows[i]["TotalOtherCount"].ToString());
                    objOther.Color = dtOther.Rows[i]["Color"].ToString();
                    objOther.Condition1 = Convert.ToInt32(dtOther.Rows[i]["Condition1"].ToString());
                    objOther.Condition2 = Convert.ToInt32(dtOther.Rows[i]["Condition2"].ToString());
                    objOther.Condition3 = Convert.ToInt32(dtOther.Rows[i]["Condition3"].ToString());
                    objOther.Condition4 = Convert.ToInt32(dtOther.Rows[i]["Condition4"].ToString());
                    lCustomerClassificationOtherCategory.Add(objOther);
                }
            }


            //Bind Pet Care Products                                

            if (dtPet != null && dtPet.Rows.Count > 0)
            {
                for (int i = 0; i < dtPet.Rows.Count; i++)
                {
                    CustomerClassificationPetCareProducts objPet = new CustomerClassificationPetCareProducts();
                    objPet.LevelID = Convert.ToInt32(dtPet.Rows[i]["LevelID"].ToString());
                    objPet.ImageType = dtPet.Rows[i]["ImageType"].ToString();
                    objPet.ImageCount = Convert.ToInt32(dtPet.Rows[i]["ImageCount"].ToString());
                    objPet.TotalPetCareCount = Convert.ToInt32(dtPet.Rows[i]["TotalPetCareCount"].ToString());
                    objPet.Color = dtPet.Rows[i]["Color"].ToString();
                    objPet.Condition1 = Convert.ToInt32(dtPet.Rows[i]["Condition1"].ToString());
                    objPet.Condition2 = Convert.ToInt32(dtPet.Rows[i]["Condition2"].ToString());
                    objPet.Condition3 = Convert.ToInt32(dtPet.Rows[i]["Condition3"].ToString());
                    objPet.Condition4 = Convert.ToInt32(dtPet.Rows[i]["Condition4"].ToString());
                    lCustomerClassificationPetCareProducts.Add(objPet);
                }
            }

            //Bind Offer Products  
            if (dtOffer != null && dtOffer.Rows.Count > 0)
            {
                for (int i = 0; i < dtOffer.Rows.Count; i++)
                {
                    CustomerClassificationOfferProducts objOffer = new CustomerClassificationOfferProducts();
                    objOffer.LevelID = Convert.ToInt32(dtOffer.Rows[i]["LevelID"].ToString());
                    objOffer.ImageType = dtOffer.Rows[i]["ImageType"].ToString();
                    objOffer.ImageCount = Convert.ToInt32(dtOffer.Rows[i]["ImageCount"].ToString());
                    objOffer.TotalOfferCount = Convert.ToInt32(dtOffer.Rows[i]["TotalOfferCount"].ToString());
                    objOffer.Color = dtOffer.Rows[i]["Color"].ToString();
                    objOffer.Condition1 = Convert.ToInt32(dtOffer.Rows[i]["Condition1"].ToString());
                    objOffer.Condition2 = Convert.ToInt32(dtOffer.Rows[i]["Condition2"].ToString());
                    objOffer.Condition3 = Convert.ToInt32(dtOffer.Rows[i]["Condition3"].ToString());
                    objOffer.Condition4 = Convert.ToInt32(dtOffer.Rows[i]["Condition4"].ToString());
                    lCustomerClassificationOfferProducts.Add(objOffer);
                }
            }


            //Bind FMCG Products                               

            if (dtFMCG != null && dtFMCG.Rows.Count > 0)
            {
                for (int i = 0; i < dtFMCG.Rows.Count; i++)
                {
                    CustomerClassificationFMCGProducts objFMCG = new CustomerClassificationFMCGProducts();
                    objFMCG.LevelID = Convert.ToInt32(dtFMCG.Rows[i]["LevelID"].ToString());
                    objFMCG.ImageType = dtFMCG.Rows[i]["ImageType"].ToString();
                    objFMCG.ImageCount = Convert.ToInt32(dtFMCG.Rows[i]["ImageCount"].ToString());
                    objFMCG.TotalFMCGCount = Convert.ToInt32(dtFMCG.Rows[i]["TotalFMCGCount"].ToString());
                    objFMCG.Color = dtFMCG.Rows[i]["Color"].ToString();
                    objFMCG.Condition1 = Convert.ToInt32(dtFMCG.Rows[i]["Condition1"].ToString());
                    objFMCG.Condition2 = Convert.ToInt32(dtFMCG.Rows[i]["Condition2"].ToString());
                    objFMCG.Condition3 = Convert.ToInt32(dtFMCG.Rows[i]["Condition3"].ToString());
                    objFMCG.Condition4 = Convert.ToInt32(dtFMCG.Rows[i]["Condition4"].ToString());
                    lCustomerClassificationFMCGProducts.Add(objFMCG);
                }
            }

            //Bind Mixed category Products                               

            if (dtMixed != null && dtMixed.Rows.Count > 0)
            {
                for (int i = 0; i < dtMixed.Rows.Count; i++)
                {
                    CustomerClassificationMixedCategoryProducts objMixed = new CustomerClassificationMixedCategoryProducts();
                    objMixed.LevelID = Convert.ToInt32(dtMixed.Rows[i]["LevelID"].ToString());
                    objMixed.ImageType = dtMixed.Rows[i]["ImageType"].ToString();
                    objMixed.ImageCount = Convert.ToInt32(dtMixed.Rows[i]["ImageCount"].ToString());
                    objMixed.TotalMixedCount = Convert.ToInt32(dtMixed.Rows[i]["TotalMixedCount"].ToString());
                    objMixed.Color = dtMixed.Rows[i]["Color"].ToString();
                    objMixed.Condition1 = Convert.ToInt32(dtMixed.Rows[i]["Condition1"].ToString());
                    objMixed.Condition2 = Convert.ToInt32(dtMixed.Rows[i]["Condition2"].ToString());
                    objMixed.Condition3 = Convert.ToInt32(dtMixed.Rows[i]["Condition3"].ToString());
                    objMixed.Condition4 = Convert.ToInt32(dtMixed.Rows[i]["Condition4"].ToString());
                    lCustomerClassificationMixedCategoryProducts.Add(objMixed);
                }
            }

           
            objCCVM.lCustomerClassificationFirstLevelCategory = lCustomerClassificationFirstLevelCategory;
            objCCVM.lCustomerClassificationFruitsAndVegitablesCategory = lCustomerClassificationFruitsAndVegitablesCategory;
            objCCVM.lCustomerClassificationGroceryCategory = lCustomerClassificationGroceryCategory;
            objCCVM.lCustomerClassificationMeatAndPoultry = lCustomerClassificationMeatAndPoultry;
            objCCVM.lCustomerClassificationOtherCategory = lCustomerClassificationOtherCategory;
            objCCVM.lCustomerClassificationPetCareProducts = lCustomerClassificationPetCareProducts;
            objCCVM.lCustomerClassificationOfferProducts = lCustomerClassificationOfferProducts;
            objCCVM.lCustomerClassificationFMCGProducts = lCustomerClassificationFMCGProducts;
            objCCVM.lCustomerClassificationMixedCategoryProducts = lCustomerClassificationMixedCategoryProducts;
                
            objCCVM.InitialFromDate = InitialFromDate;
            objCCVM.FromDate = fromDate;
            objCCVM.ToDate = toDate;

            if (PincodeID != null && PincodeID > 0)
            {
                objCCVM.PincodeID = Convert.ToInt32(PincodeID);
            }

            return View("Index", objCCVM);
        }

        public ActionResult GetDetail(string Category, int Level, string Condition, string InitialFromDate, string fromDate, string toDate, int? City, int? PincodeID, string Option)
        {           
            if (City != null && City > 0)
            {
                ViewBag.CityID = new SelectList(db.Cities.Where(x => x.ID != 1 && x.IsActive == true), "ID", "Name");

                List<Pincode> lPincode = new List<Pincode>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lPincode = db.Pincodes.Where(x => x.CityID == City).ToList();

                foreach (var c in lPincode)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.City = City;

                ViewBag.lPincode = forloopclasses.ToList();
            }
            else
            {
                ViewBag.CityID = new SelectList(db.Cities.Where(x => x.ID != 1 && x.IsActive == true), "ID", "Name");
            }

            string Range = string.Empty;
            if(Condition=="T")
            {
                Range = "Total Customer";
            }
            if (Condition == "C1")
            {
                Range = "Rs.(1-450)";
            }
            if (Condition == "C2")
            {
                Range = "Rs.(451-750)";
            }
            if (Condition == "C3")
            {
                Range = "Rs.(751-900)";
            }
            if (Condition == "C4")
            {
                Range = "Rs.(901 & more)";
            }

            ViewBag.Heading = Category +" "+ Range;

            CustomerClassificationViewModel objCCVM = new CustomerClassificationViewModel();
            List<CustomerClassificationDetail> lCustomerClassificationDetailList = new List<CustomerClassificationDetail>();
            DataTable dtDetail = new DataTable();
            objCCVM.InitialFromDate = InitialFromDate;
            objCCVM.FromDate = fromDate;
            objCCVM.ToDate = toDate;

            if (Category == "Main")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationFirstLevelCategoryDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "OnlyVeg")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationFruitsAndVegitablesCategoryDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "OnlyGrocery")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationGroceryCategoryDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "OnlyNonVeg")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationMeatAndPoultryDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "Other")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationOtherCategoryDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "PetCare")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationPetCareProductsDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "Offer")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationOfferProductsDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "FMCG")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationFMCGProductsDetail", InitialFromDate, fromDate, toDate, City, PincodeID,Level,Condition);
            }
            if (Category == "Mixed")
            {
                dtDetail = BindDataTableDetail("CustomerClassificationMixedCategoryProductsDetail", InitialFromDate, fromDate, toDate, City, PincodeID, Level, Condition);
            }

                if (dtDetail != null && dtDetail.Rows.Count > 0)
                {
                    string fileName = "Customer Classification " + ViewBag.Heading;
                    fileName = fileName.Replace(' ','_');
                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (Option == "Excel")
                    {
                        ExportExcelCsv.ExportToExcel(dtDetail, fileName);
                        return View("Index");
                    }
                    else if (Option == "Pdf")
                    {
                        ExportExcelCsv.ExportToCSV(dtDetail, fileName);
                        return View("Index");
                    }
                    else if (Option == "Csv")
                    {
                        ExportExcelCsv.ExportToPDF(dtDetail, fileName);
                        return View("Index");
                    }
                    else
                    {
                        for (int i = 0; i < dtDetail.Rows.Count; i++)
                        {
                            CustomerClassificationDetail objCC = new CustomerClassificationDetail();
                            objCC.UserLoginID = Convert.ToInt32(dtDetail.Rows[i]["UserLoginID"].ToString());
                            objCC.Name = dtDetail.Rows[i]["Name"].ToString();
                            objCC.TotalOrder = Convert.ToInt32(dtDetail.Rows[i]["TotalOrder"].ToString());
                            objCC.TotalAmount = Convert.ToDecimal(dtDetail.Rows[i]["TotalAmount"].ToString());
                            objCC.LastPurchaseDate = Convert.ToDateTime(dtDetail.Rows[i]["LastPurchaseDate"].ToString());
                            objCC.JoiningDate = Convert.ToDateTime(dtDetail.Rows[i]["JoiningDate"].ToString());
                            objCC.PrimaryMobile = dtDetail.Rows[i]["PrimaryMobile"].ToString();
                            objCC.RegisteredMobile = dtDetail.Rows[i]["Mobile"].ToString();
                            objCC.Email = dtDetail.Rows[i]["Email"].ToString();
                            objCC.Pincode = dtDetail.Rows[i]["Pincode"].ToString();
                            objCC.City = dtDetail.Rows[i]["City"].ToString();
                            objCC.ShippingAddress1 = dtDetail.Rows[i]["ShippingAddress1"].ToString();
                            objCC.ShippingAddress2 = dtDetail.Rows[i]["ShippingAddress2"].ToString();
                            objCC.ShippingAddress3 = dtDetail.Rows[i]["ShippingAddress3"].ToString();

                            lCustomerClassificationDetailList.Add(objCC);
                        }
                        objCCVM.lCustomerClassificationDetails = lCustomerClassificationDetailList;
                       
                    }
                }
                return View("GetDetail", objCCVM);
        }
        
	}
}