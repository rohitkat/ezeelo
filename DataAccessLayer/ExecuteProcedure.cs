//-----------------------------------------------------------------------
// <copyright file="ExecuteProcedure.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Prashant Bhoyar</author>
//-----------------------------------------------------------------------

namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    /// <summary>
    /// Exceutes All the Stored Procedures and commands of MS SQL Server Database
    /// </summary>
    public class ExecuteProcedure
    {
        /// <summary>
        /// stores the connection string provided for this object
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// Initializes a new instance of the ExecuteProcedure class
        /// </summary>
        /// <param name="conectionString">Connection String of the Database</param>
        public ExecuteProcedure(string conectionString)
        {
            this.connectionString = conectionString;
        }

        /// <summary>
        /// Enum to Identify the execution type of given command
        /// </summary>
        internal enum ExecuteParam
        {
            /// <summary>
            /// To exceute Non Query Type Command
            /// </summary>
            NonQuery,

            /// <summary>
            /// To Execute Scalar Command
            /// </summary>
            Scalar
        }
               
        /// <summary>
        /// Executes Given Command and returns the result data into DataTable
        /// </summary>
        /// <param name="command">Comand Name [StoredProcedure or TextQuery]</param>
        /// <param name="cmdType">Command Type</param>
        /// <returns>DataTable filled with records selected by given command</returns>
        internal DataTable GetData(string command, CommandType cmdType)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);
                return exeProc.ExcecuteCommand(command, cmdType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excecutes stored Procedure Having Single Parameter and returns the Result Into DataTable
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValue">Parameter Value</param>
        /// <returns>Data Table filled with Records selected by given command</returns>
        internal DataTable GetData(string spName, object paramValue)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                DataTable dt = this.GetParamInfo(spName, exeProc);

                if (dt.Rows.Count > 0)
                {
                    return exeProc.ExeccuteSP(spName, paramValue, dt.Rows[0]["PARAM NAME"].ToString(), this.GetSqlDbType(dt.Rows[0]["PARAM TYPE"].ToString().ToUpper().Trim()));
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excecutes stored Procedure and Returns the Result Into DataTable
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValues">List of Parameter Values</param>
        /// <returns>Data Table filled with Records selected by given command</returns>
        internal DataTable GetData(string spName, List<object> paramValues)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                List<string> paramNames = new List<string>();
                List<SqlDbType> paramType = new List<SqlDbType>();

                DataTable dt = this.GetParamInfo(spName, exeProc);

                foreach (DataRow dtRow in dt.Rows)
                {
                    paramNames.Add(dtRow["PARAM NAME"].ToString());
                    paramType.Add(this.GetSqlDbType(dtRow["PARAM TYPE"].ToString().ToUpper().Trim()));
                }

                return exeProc.ExeccuteSP(spName, paramNames, paramValues, paramType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Executes Given Command and returns the result data into DataTable
        /// </summary>
        /// <param name="command">Comand Name [StoredProcedure or TextQuery]</param>
        /// <param name="cmdType">Command Type</param>
        /// <returns>DataTable filled with records selected by given command</returns>
        internal DataSet GetDataSet(string command, CommandType cmdType)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);
                return exeProc.ExcecuteCommandDataSet(command, cmdType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excecutes stored Procedure Having Single Parameter and returns the Result Into DataTable
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValue">Parameter Value</param>
        /// <returns>Data Table filled with Records selected by given command</returns>
        internal DataSet GetDataSet(string spName, object paramValue)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                DataTable dt = this.GetParamInfo(spName, exeProc);

                if (dt.Rows.Count > 0)
                {
                    return exeProc.ExeccuteSPDataSet(spName, paramValue, dt.Rows[0]["PARAM NAME"].ToString(), this.GetSqlDbType(dt.Rows[0]["PARAM TYPE"].ToString().ToUpper().Trim()));
                }
                else
                {
                    return new DataSet();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excecutes stored Procedure and Returns the Result Into DataTable
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValues">List of Parameter Values</param>
        /// <returns>Data Table filled with Records selected by given command</returns>
        internal DataSet GetDataSet(string spName, List<object> paramValues)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                List<string> paramNames = new List<string>();
                List<SqlDbType> paramType = new List<SqlDbType>();

                DataTable dt = this.GetParamInfo(spName, exeProc);

                foreach (DataRow dtRow in dt.Rows)
                {
                    paramNames.Add(dtRow["PARAM NAME"].ToString());
                    paramType.Add(this.GetSqlDbType(dtRow["PARAM TYPE"].ToString().ToUpper().Trim()));
                }

                return exeProc.ExeccuteSPDataSet(spName, paramNames, paramValues, paramType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excecutes stored Procedure and returns the scalar data
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValues">List of Parameter Values</param>
        /// <returns>Object Scalar Data</returns>
        internal object GetScalarData(string spName, List<object> paramValues)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                List<string> paramNames = new List<string>();
                List<SqlDbType> paramType = new List<SqlDbType>();

                DataTable dt = this.GetParamInfo(spName, exeProc);

                foreach (DataRow dtRow in dt.Rows)
                {
                    paramNames.Add(dtRow["PARAM NAME"].ToString());
                    paramType.Add(this.GetSqlDbType(dtRow["PARAM TYPE"].ToString().ToUpper().Trim()));
                }

                return exeProc.ExeccuteSPScalar(spName, paramNames, paramValues, paramType, ExecuteCommand.ExecuteParam.Scalar);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Executes the given Stored Procedure
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValues">List of parameter Values</param>
        /// <param name="execute">Execution Type</param>
        /// <returns>Returns Number of Records Affected</returns>
        internal int CallProcedure(string spName, List<object> paramValues, ExecuteParam execute)
        {
            ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

            ExecuteCommand.ExecuteParam exec;

            List<string> paramNames = new List<string>();
            List<SqlDbType> paramType = new List<SqlDbType>();

            DataTable dt = this.GetParamInfo(spName, exeProc);

            foreach (DataRow dtRow in dt.Rows)
            {
                
                paramNames.Add(dtRow["PARAM NAME"].ToString());
                paramType.Add(this.GetSqlDbType(dtRow["PARAM TYPE"].ToString().ToUpper().Trim()));
            }

            int value = -1;

            if (execute == ExecuteParam.NonQuery)
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }
            else if (execute == ExecuteParam.Scalar)
            {
                exec = ExecuteCommand.ExecuteParam.Scalar;
            }
            else
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }

            value = exeProc.ExeccuteSP(spName, paramNames, paramValues, paramType, exec);

            return value;
        }

        /// <summary>
        /// Executes the given Stored Procedure and Returns the Dictionary Containing 
        /// Values of the OUTPUT Parameter having Param Name as KEY
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramValues">List of parameter Values</param>
        /// <returns>OUTPUT Dictionary contins param names as key</returns>
        internal Dictionary<string, object> CallProcedure(string spName, List<object> paramValues)
        {
            ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

            List<string> paramNames = new List<string>();
            List<SqlDbType> paramType = new List<SqlDbType>();
            List<System.Data.ParameterDirection> direction = new List<ParameterDirection>();
            List<int> outSize = new List<int>();

            DataTable dt = this.GetParamInfo(spName, exeProc);

            foreach (DataRow dtRow in dt.Rows)
            {
                paramNames.Add(dtRow["PARAM NAME"].ToString());
                paramType.Add(this.GetSqlDbType(dtRow["PARAM TYPE"].ToString().ToUpper().Trim()));

                if (Convert.ToBoolean(dtRow["ISOUT"]))
                {
                    direction.Add(ParameterDirection.Output);
                    outSize.Add(Convert.ToInt32(dtRow["LENGTH"].ToString()));
                }
                else
                {
                    direction.Add(ParameterDirection.Input);
                }
            }

            int value = -1;

            Dictionary<string, object> outvalues = new Dictionary<string, object>();

            value = exeProc.ExeccuteSP(spName, paramNames, paramValues, paramType, direction, ExecuteCommand.ExecuteParam.NonQuery, outSize, ref outvalues);

            return outvalues;
        }
        
        /// <summary>
        /// Executes the Given command and returns the Selected Value or Number of rows affected
        /// </summary>
        /// <param name="command">Command Name</param>
        /// <param name="execute">Excution Type</param>
        /// <returns>Object Having Selected values or nummber of rows Affected</returns>
        internal object ExcecuteCommand(string command, ExecuteParam execute)
        {
            try
            {
                ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

                return exeProc.RunCommand(command, execute == ExecuteParam.NonQuery ? ExecuteCommand.ExecuteParam.NonQuery :
                    ExecuteCommand.ExecuteParam.Scalar);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Returns the SqlDbType enum of the Given String Name of the Datatype
        /// </summary>
        /// <param name="dbType">String Name of the SQL DataType</param>
        /// <returns>Returns the enum SqlDbType</returns>
        private SqlDbType GetSqlDbType(string dbType)
        {
            switch (dbType)
            {
                case "BIGINT": return SqlDbType.BigInt;
                case "BINARY": return SqlDbType.Binary;
                case "BIT": return SqlDbType.Bit;
                case "CHAR": return SqlDbType.Char;
                case "DATE": return SqlDbType.Date;
                case "DATETIME": return SqlDbType.DateTime;
                case "DATETIME2": return SqlDbType.DateTime2;
                case "DATETIMEOFFSET": return SqlDbType.DateTimeOffset;
                case "DECIMAL": return SqlDbType.Decimal;
                case "FLOAT": return SqlDbType.Float;
                case "IMAGE": return SqlDbType.Image;
                case "INT": return SqlDbType.Int;
                case "MONEY": return SqlDbType.Money;
                case "NCHAR": return SqlDbType.NChar;
                case "NTEXT": return SqlDbType.NText;
                case "NUMERIC": return SqlDbType.Real;
                case "NVARCHAR": return SqlDbType.NVarChar;
                case "REAL": return SqlDbType.Real;
                case "SMALLDATETIME": return SqlDbType.SmallDateTime;
                case "SMALLINT": return SqlDbType.SmallInt;
                case "SMALLMONEY": return SqlDbType.SmallMoney;
                case "SQL_VARIENT": return SqlDbType.Variant;
                case "TEXT": return SqlDbType.Text;
                case "TIME": return SqlDbType.Time;
                case "TIMESTAMP": return SqlDbType.Timestamp;
                case "TINYINT": return SqlDbType.TinyInt;
                case "UNIQUEIDENTIFIER": return SqlDbType.UniqueIdentifier;
                case "VARBINARY": return SqlDbType.VarBinary;
                case "VARCHAR": return SqlDbType.VarChar;
                case "XML": return SqlDbType.Xml;

                default: return SqlDbType.Structured;
            }
        }

        /// <summary>
        /// Executes the given Stored Procedure
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramNames">List of Parameter Names</param>
        /// <param name="paramValues">List of parameter Values</param>
        /// <param name="paramType">List of Parameter Types</param>
        /// <param name="execute">Execution Type</param>
        /// <returns>Returns Number of Records Affected</returns>
        private int CallProcedure(string spName, List<string> paramNames, List<object> paramValues,
             List<SqlDbType> paramType, ExecuteParam execute)
        {
            ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

            ExecuteCommand.ExecuteParam exec;

            int value = -1;

            if (execute == ExecuteParam.NonQuery)
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }
            else if (execute == ExecuteParam.Scalar)
            {
                exec = ExecuteCommand.ExecuteParam.Scalar;
            }
            else
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }

            value = exeProc.ExeccuteSP(spName, paramNames, paramValues, paramType, exec);

            return value;
        }

        /// <summary>
        /// Executes the given Stored Procedure
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramNames">List of Parameter Names</param>
        /// <param name="paramValues">List of parameter Values</param>
        /// <param name="paramType">List of Parameter Types</param>
        /// <param name="direction">Direction of parameter (in or out)</param>
        /// <param name="outputSize">Size of the output parameters</param>
        /// <param name="execute">Execution Type [NonQuery or Scalar]</param>
        /// <returns>Returns the dictionary of output parametres Values [Prameter Name as Key]</returns>
        private Dictionary<string, object> CallProcedure(string spName, List<string> paramNames, List<object> paramValues,
            List<SqlDbType> paramType, List<System.Data.ParameterDirection> direction, List<int> outputSize, ExecuteParam execute)
        {
            ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

            ExecuteCommand.ExecuteParam exec;

            if (execute == ExecuteParam.NonQuery)
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }
            else if (execute == ExecuteParam.Scalar)
            {
                exec = ExecuteCommand.ExecuteParam.Scalar;
            }
            else
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }

            Dictionary<string, object> outParam = new Dictionary<string, object>();

            exeProc.ExeccuteSP(spName, paramNames, paramValues, paramType, direction, exec, outputSize, ref outParam);

            return outParam;
        }

        /// <summary>
        /// Executes the Given Stored Procedure 
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="paramValue">Parameter value</param>
        /// <param name="paramType">Parameter Type</param>
        /// <param name="execute">Exceution Type</param>
        /// <returns>Returns Value Selected By Stored Procedure</returns>
        private object CallProcedure(string spName, string paramName, object paramValue,
            SqlDbType paramType, ExecuteParam execute)
        {
            ExecuteCommand exeProc = new ExecuteCommand(this.connectionString);

            ExecuteCommand.ExecuteParam exec;

            if (execute == ExecuteParam.NonQuery)
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }
            else if (execute == ExecuteParam.Scalar)
            {
                exec = ExecuteCommand.ExecuteParam.Scalar;
            }
            else
            {
                exec = ExecuteCommand.ExecuteParam.NonQuery;
            }

            return exeProc.ExeccuteSP(spName, paramName, paramValue, paramType, exec);
        }

        /// <summary>
        /// Returns the Parameters Information of the Given Stored Procedure [like NAME,DATATYPE,LENGTH,OUTPUT TYPE]
        /// </summary>
        /// <param name="spName">Name of The Stored Procedure</param>
        /// <param name="exeProc">Object of the ExecuteCommand class</param>
        /// <returns>returns the DataTable Filled with Parameter Informaton of the Given Stored Procedure</returns>
        private DataTable GetParamInfo(string spName, ExecuteCommand exeProc)
        {
            try
            {
                return exeProc.ExcecuteCommand("select s.colId as [ID],s.name as [PARAM NAME], t.name as [PARAM TYPE]," +
                    "s.length as [LENGTH], s.isoutparam as [ISOUT] " +
                    "from  syscolumns s inner join systypes t on s.xusertype = t.xusertype " +
                    " where id = (select id from sysobjects where name = '" + spName +
                    "') and t.name!='sysname' order by s.colId asc", CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
