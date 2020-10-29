using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class PreviewSpecification
    {


        public bool GetSpecificationShowStatus(int thirdLevelCatID, PreviewSpecificationEnum specificationName)// string specificationName)
        {

            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
               
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlParameter parm = new SqlParameter("@IsDisplay", SqlDbType.Bit);
                SqlCommand sqlComm = new SqlCommand("DisplayPreviewSpecification", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@ThirdLevelCatID", thirdLevelCatID);
                sqlComm.Parameters.AddWithValue("@SpecificationName", specificationName.ToString());
                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                bool Result = Convert.ToBoolean(parm.Value);
                con.Close();
                return Result;

            }
            catch (Exception)
            {
                
                throw;
                return true;
            }
            //return true;
        } 

    }
    public enum PreviewSpecificationEnum
    {
        SimilarProducts = 1,

        FrequentlyBuyedProducts=2,

        GeneralDescription = 3,

        TechnicalDescription = 4,

        RecentlyViewedProducts=5,

        ProductReviews=6,
        
        ShopReviews=7,

        ShopList=8
    }
}
