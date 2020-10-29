﻿/*=============================================================================================================
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
    interface IDataOperations
    {   
        # region -- VIRTUAL METHODS --

        /// <summary>
        /// Performs the operation of either INSERT or UPDATE to the given data
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <param name="opr">Operation to be performed on the table</param>
        /// <returns>Integer Value which stored Procedure's Output</returns>
        int SetRecords(string spName, List<object> paramValues, Enumerators.DB_OPERATIONS opr);// { return 0; }

        /// <summary>
        /// Performs the operation of either INSERT or UPDATE to the given data
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <param name="identity">Stores Autogenerted Primary Key Value</param>
        /// <param name="opr">Operation to be performed on the table</param>
        /// <returns>Integer Value which stores Procedure Output</returns>
        int SetRecords(string spName, List<object> paramValues, ref string identity, Enumerators.DB_OPERATIONS opr);// { return 0; }

        /// <summary>
        /// Selects Records from the given table as per parameter passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        System.Data.DataTable GetRecords(string spName, List<object> paramValues);// { return new System.Data.DataTable(); }

        /// <summary>
        /// Selects Records from the given table as per parameter passed
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        System.Data.DataSet GetRecordsDataSet(string spName, List<object> paramValues);// { return new System.Data.DataSet(); }

        /// <summary>
        /// Selects Records from the given table as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValue">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        System.Data.DataTable GetRecords(string spName, object paramValue);// { return new System.Data.DataTable(); }

        /// <summary>
        /// Selects Records from the given table as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValue">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        System.Data.DataSet GetRecordsDataSet(string spName, object paramValue);// { return new System.Data.DataSet(); }

        /// <summary>
        /// Serches the Members and its Ration Cards as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <returns>DataTable Filled with Selected Records </returns>
        System.Data.DataTable GetRecords(string spName);// { return new System.Data.DataTable(); }

        /// <summary>
        /// Serches the Members and its Ration Cards as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <returns>DataSet Filled with Selected Records within multiple tables if any</returns>
        System.Data.DataSet GetRecordsDataSet(string spName);// { return new System.Data.DataSet(); }

        /// <summary>
        /// Removes Records from the given table as per parameter values specified
        /// </summary>
        /// <param name="spName">Name of the Stored Procedure which is to be executed</param>
        /// <param name="paramValues">Parameter Values which are passed to the Stored Procedure and Sequence is Same</param>
        /// <returns>Output of the Stored Procedure </returns>
        int DeleteRecord(string spName, List<object> paramValues);// { return 0; }

       
        #endregion
    }
}
