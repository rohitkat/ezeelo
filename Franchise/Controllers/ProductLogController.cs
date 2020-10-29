using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Franchise.Controllers
{
    public class ProductLogController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //int FranchiseID = 2;
        private int pageSize = 50;


        public class WebmethodParams
        {
            public long ShopStockId { get; set; }
            public string txtFromDate { get; set; }
            public string txtToDate { get; set; }
           
        }

        //
        // GET: /ProductLog/
        public ActionResult Index()
        {
           int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            

            ViewBag.shopID =  new SelectList((from s in db.Shops
                               where s.FranchiseID == FranchiseID
                               && s.IsActive==true && s.IsLive==true
                               select new
                               {
                                   ShopId=s.ID,
                                   ShopName=s.Name
                               }).OrderBy(x=>x.ShopName),"ShopId","ShopName");
            return View();
        }

        public ActionResult Select_product(int? page, int ShopId, string txtFromDate, string txtToDate)//, int PageIndex, int PageSize)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);


            int TotalCount = 0;
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.ShopId = ShopId;
            ViewBag.txtFromDate = txtFromDate;
            ViewBag.txtToDate = txtToDate;

            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string conn = readCon.DB_CONNECTION;
            SqlConnection con = new SqlConnection(conn);
            SqlCommand sqlComm = new SqlCommand("Select_Product_With_Pricing", con);
            sqlComm.Parameters.AddWithValue("@ShopId", SqlDbType.Int).Value = ShopId;
            DateTime frmDate = CommonFunctions.GetProperDateTime(txtFromDate);
            DateTime toDate = CommonFunctions.GetProperDateTime(txtToDate);
            sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = frmDate;
            sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = toDate;
            sqlComm.Parameters.AddWithValue("@PageIndex", SqlDbType.Int).Value = pageNumber;
            sqlComm.Parameters.AddWithValue("@PageSize", SqlDbType.Int).Value = pageSize;
            sqlComm.Parameters.Add("@RecordCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            //sqlComm.Parameters["@RecordCount"].Direction=ParameterDirection.Output;
            sqlComm.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            TotalCount = Convert.ToInt32(sqlComm.Parameters["@RecordCount"].Value);
            List<ProductLogViewModel> lProductLog = new List<ProductLogViewModel>();
            lProductLog = BusinessLogicLayer.Helper.CreateListFromTable<ProductLogViewModel>(dt);

            ViewBag.shopID = new SelectList((from s in db.Shops
                                             where s.FranchiseID == FranchiseID
                                             && s.IsActive == true && s.IsLive == true
                                             select new
                                             {
                                                 ShopId = s.ID,
                                                 ShopName = s.Name
                                             }).OrderBy(x => x.ShopName), "ShopId", "ShopName");

            //return View("Index", lProductLog);
            ViewBag.TotalCount = TotalCount;
            return View("Index", lProductLog);
            //return View("Index", lProductLog.ToList().ToPagedList(pageNumber, pageSize));
        }



        public ActionResult Select_product_log(WebmethodParams myParam)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);

            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string conn = readCon.DB_CONNECTION;
            SqlConnection con = new SqlConnection(conn);
            SqlCommand sqlComm = new SqlCommand("Select_Product_With_Pricing_Log", con);
            sqlComm.Parameters.AddWithValue("@ShopStockId", SqlDbType.Int).Value = myParam.ShopStockId;
            DateTime frmDate = CommonFunctions.GetProperDateTime(myParam.txtFromDate);
            DateTime toDate = CommonFunctions.GetProperDateTime(myParam.txtToDate);
            sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = frmDate;
            sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = toDate;
            sqlComm.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<ProductLogViewModel> lProductLog = new List<ProductLogViewModel>();
            lProductLog = BusinessLogicLayer.Helper.CreateListFromTable<ProductLogViewModel>(dt);

            ViewBag.shopID = new SelectList((from s in db.Shops
                                             where s.FranchiseID == FranchiseID
                                             && s.IsActive == true && s.IsLive == true
                                             select new
                                             {
                                                 ShopId = s.ID,
                                                 ShopName = s.Name
                                             }).OrderBy(x => x.ShopName), "ShopId", "ShopName");

            return PartialView("_Select_product_log", lProductLog);
        }
	}
}