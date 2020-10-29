using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ModelLayer.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;


namespace API.Controllers
{
    public class GcmMessageDetailController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //POST api/gcmmessagedetail
        /// <summary>
        /// TO Display GCM Message Detail
        /// </summary>
        /// <param name="gCMUserDetailViewModel"></param>
        /// <returns></returns>
        public object PostGCMMsgDetails(GCMUserDetailViewModel gCMUserDetailViewModel)
        {
            try
            {
                if (gCMUserDetailViewModel.UserLoginID == 0 && (gCMUserDetailViewModel.GcmRegID == null || gCMUserDetailViewModel.GcmRegID == "") && (gCMUserDetailViewModel.IMEI == null || gCMUserDetailViewModel.IMEI == ""))
                {
                    return new { HttpStatusCode = "400", UserMessage = "Invalid request. Please enter Valid  UserLoginID,IMEI,GCMRegID" };
                }
                List<GCMMsgDetailViewModel> gCMMsgDetailViewModel = new List<GCMMsgDetailViewModel>();
                DataTable ldt = new DataTable();
                ldt = Get_GCMMsgDetails(gCMUserDetailViewModel);
                gCMMsgDetailViewModel = BusinessLogicLayer.Helper.CreateListFromTable<GCMMsgDetailViewModel>(ldt);
                if (gCMMsgDetailViewModel.Count == 0)
                {
                    return new { HttpStatusCode = "204", UserMessage = "No Record Found" };
                }
                return new { HttpStatusCode = "200", gCMMsgDetailViewModel };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable Get_GCMMsgDetails(GCMUserDetailViewModel gCMUserDetailViewModel)
        {
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Get_GCMMsgDetail", con);
                sqlComm.Parameters.AddWithValue("@userloginID", SqlDbType.Int).Value = gCMUserDetailViewModel.UserLoginID;
                sqlComm.Parameters.AddWithValue("@IMEI", SqlDbType.DateTime).Value = gCMUserDetailViewModel.IMEI;
                sqlComm.Parameters.AddWithValue("@GCMRegID", SqlDbType.DateTime).Value = gCMUserDetailViewModel.GcmRegID;
                sqlComm.Parameters.AddWithValue("@CityID", SqlDbType.DateTime).Value = gCMUserDetailViewModel.CityId;
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.DateTime).Value = gCMUserDetailViewModel.FranchiseId;
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter lda = new SqlDataAdapter(sqlComm);
                DataTable ldt = new DataTable();
                lda.Fill(ldt);
                return ldt;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
       
    }
    public class GCMUserDetailViewModel
    {
        public long UserLoginID { get; set; }
        public string IMEI { get; set; }
        public string GcmRegID { get; set; }
        public long CityId { get; set; }
        public string FranchiseId { get; set; }
    }
    public class GCMMsgDetailViewModel
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImgURL { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long CityId { get; set; }
        public string FranchiseID { get; set; }
        public int OfferTypeID { get; set; }
        public long ProdIDCatID { get; set; }
    }

