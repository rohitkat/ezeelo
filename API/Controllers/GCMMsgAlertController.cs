using System;
using System.Collections.Generic;
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


namespace API.Controllers
{
    public class GCMMsgAlertController : ApiController
    {
        [HttpPost]
        public object PostGCMMsgDetails(GCMMsgTemplateViewModel gCMMsgTemplateViewModel)
        {
            try
            {
                if(gCMMsgTemplateViewModel.UserLoginId==0)
                {
                    return new { HttpStatusCode = "400", UserMessage = "Invalid request. Please enter Valid  UserLoginID" };
                }
                List<GCMMsgTemplateTransactionViewModel> gCMMsgTemplateTransactionViewModel = new List<GCMMsgTemplateTransactionViewModel>();
                DataTable ldt = new DataTable();
                ldt = GetGCMMsgTemplate(gCMMsgTemplateViewModel);
                gCMMsgTemplateTransactionViewModel = BusinessLogicLayer.Helper.CreateListFromTable<GCMMsgTemplateTransactionViewModel>(ldt);
                foreach(var item in gCMMsgTemplateTransactionViewModel)
                {
                    if (item.Name =="ORD_PLAC")
                    {
                        item.Name="ORDER PLACED";
                    }
                    else if(item.Name=="ORD_CONF")
                    {
                        item.Name = "ORDER CONFIRMED";
                    }
                    else if(item.Name=="ORD_DIL")
                    {
                        item.Name = "ORDER DELIVERED";
                    }
                    else if (item.Name == "ORD_CANC")
                    {
                        item.Name = "ORDER CANCELLED";
                    }
                }

                gCMMsgTemplateTransactionViewModel = gCMMsgTemplateTransactionViewModel.Skip(gCMMsgTemplateViewModel.index).Take(8).ToList();              
                if (gCMMsgTemplateTransactionViewModel.Count == 0)
                {
                    return new { HttpStatusCode = "204", UserMessage = "No Record Found" };
                }
                return new { HttpStatusCode = "200", gCMMsgTemplateTransactionViewModel };
            }
            catch
            {
                throw;
            }
        }

        [HttpPut]
        public HttpResponseMessage PutGCMMsgAlert(GCMMsgAlertViewModel GCMMsgAlertViewModel)
        {
            try
            {       
                if(GCMMsgAlertViewModel.getGCMMsgAlertList==null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "ID is null");
                }
                DataTable ldt = new DataTable();
                var lGCMMsgAlertID = GCMMsgAlertViewModel.getGCMMsgAlertList.Select(x => x.ID).ToList();
                ldt = ListToDatatable(lGCMMsgAlertID);
                int CountRowUpdated = UpdateGCMMsgAlertDetail(ldt);
                if (CountRowUpdated == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Records Not Deleted");
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Records Deleted Successfully");
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetGCMMsgTemplate(GCMMsgTemplateViewModel gCMMsgTemplateViewModel)
        {
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Get_GCMMsgTemplateTransaction", con);
                sqlComm.Parameters.AddWithValue("@UserLoginID", SqlDbType.Int).Value = gCMMsgTemplateViewModel.UserLoginId;
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
        private int UpdateGCMMsgAlertDetail(DataTable gCMMsgAlertdt)
        {
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("[Update_GCMMsgTemplateTrasaction]", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlParameter lsqlparam = sqlComm.Parameters.AddWithValue("@dt", gCMMsgAlertdt);
                sqlComm.Parameters.Add("@CountUpdatedRows", SqlDbType.Int).Direction = ParameterDirection.Output;
                lsqlparam.SqlDbType = SqlDbType.Structured;
                con.Open();
                sqlComm.ExecuteNonQuery();
                int CountRowsUpdated = Convert.ToInt32(sqlComm.Parameters["@CountUpdatedRows"].Value.ToString());
                con.Close();
                return CountRowsUpdated;

            }
            catch
            {
                throw;
            }

        }
        private DataTable ListToDatatable<item>(List<item> dtItems)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            foreach (item s in dtItems)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = s;
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
   
}
public class GCMMsgTemplateViewModel
{
    public long UserLoginId { get; set; }
    public int index { get; set; }
}
public class GCMMsgTemplateTransactionViewModel
{
    public long ID { get; set; }
    public string ModifyTemplate { get; set; }
    public Nullable<System.DateTime> CreateDate { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string OrderCode { get; set; }
    public long CustomerOrderID { get; set; }
    public string PaymentMode { get; set; }

  
}

public class GCMMsgAlertViewModel
{
    public List<GetGCMMsgAlertID> getGCMMsgAlertList { get; set; }
}
public class GetGCMMsgAlertID
{
    public long ID { get; set; }
   
}

