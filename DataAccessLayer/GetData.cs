/*=============================================================================================================
 * <Organisation> Ezeelo Consumer Services Pvt. Ltd. </Organisation>
 * 
 * <Copyrights> 
 *  Copyrights to NSP Futuretech. Pvt. Ltd. 
 *  All contents are not subject to change before prior permission of the author or copyright owner</Copyrights>
 *  
 * <Author> Prashant N. Bhoyar </Author>
 * 
 * <CreationDate> MAY 21, 2015 5.30pm </CreationDate>
 * 
 * <Version>1.0.0</Version>
 * ============================================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GetData : DbOperations
    {
        /// <summary>
        /// initialises an Instance of GetData class, this class defines the functionality to Get data from Database
        /// </summary>
        /// <param name="dbConnection">DataBase connection string on which database operations to be performed</param>
        public GetData(string dbConnection) : base(dbConnection) { }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        public override System.Data.DataTable GetRecords(string spName, List<object> paramValues)
        {
            return exeProc.GetData(spName, paramValues);
        }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        public override System.Data.DataSet GetRecordsDataSet(string spName, List<object> paramValues)
        {
            throw new Exception("method for returning dataset not implemented;");
            //return new System.Data.DataSet();
        }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValue">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        public override System.Data.DataTable GetRecords(string spName, object paramValue)
        {
            return exeProc.GetData(spName.Trim(), paramValue);
        }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValue">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        public override System.Data.DataSet GetRecordsDataSet(string spName, object paramValue)
        {
            throw new Exception("method for returning dataset not implemented;");
            //return new System.Data.DataSet(); 
        }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        public override System.Data.DataTable GetRecords(string spName)
        {
            return exeProc.GetData(spName.Trim(), System.Data.CommandType.StoredProcedure);
            //return new System.Data.DataTable(); 
        }

        /// <summary>
        /// Executes given procedure as per parameters passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        public override System.Data.DataSet GetRecordsDataSet(string spName)
        {
            throw new Exception("method for returning dataset not implemented;");
            //    return new System.Data.DataSet(); 
        }
    }
}
