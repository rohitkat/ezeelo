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
    public class SetData : DbOperations
    {
        /// <summary>
        /// initialises an Instance of SetData class, this class defines the functionality to INSERT, UPDATE and DELETE data from Database
        /// </summary>
        /// <param name="dbConnection">DataBase connection string on which database operations to be performed</param>
        public SetData(string dbConnection) : base(dbConnection) { }

        /// <summary>
        /// Performs the operation of either INSERT or UPDATE to the given data
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <param name="opr">Operation to be performed on the table</param>
        /// <returns>Integer Value which stored Procedure's Output</returns>
        public override int SetRecords(string spName, List<object> paramValues, Enumerators.DB_OPERATIONS opr)
        {
            return this.GetNumeric((exeProc.CallProcedure(spName.Trim(), paramValues))["@QryResult"].ToString());
        }

        /// <summary>
        /// Performs the operation of either INSERT or UPDATE to the given data
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <param name="identity">Stores Autogenerted Primary Key Value</param>
        /// <param name="opr">Operation to be performed on the table</param>
        /// <returns>Integer Value which stores Procedure Output</returns>
        public override int SetRecords(string spName, List<object> paramValues, ref string identity, Enumerators.DB_OPERATIONS opr)
        {
            Dictionary<string, object> outValues = exeProc.CallProcedure(spName.Trim(), paramValues);
            identity = outValues["@IDENTITY"].ToString();

            return this.GetNumeric(outValues["@QryResult"].ToString());
        }

        /// <summary>
        /// Executes given procedure and Removes Records from table as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>Output of the Stored Procedure </returns>
        public override int DeleteRecord(string spName, List<object> paramValues)
        {
            return this.GetNumeric((exeProc.CallProcedure(spName.Trim(), paramValues))["@QryResult"].ToString());
        }

        private int GetNumeric(string str)
        {
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }
    }
}
