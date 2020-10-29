//-----------------------------------------------------------------------
// <copyright file="ExecuteCommand.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    /// Executes the Given Command or Stored Procedure this class basicaly deals with the DataBase
    /// </summary>
    internal class ExecuteCommand
    {
        /// <summary>
        /// stores the connection string provided for this object
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// Initialises a new instance of the ExecuteCommand class
        /// </summary>
        /// <param name="connectionString">Connection string of the database</param>
        internal ExecuteCommand(string connectionString)
        {
            this.connectionString = connectionString;
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
        /// ExecuteSP Excecutes stored procedure and returns the number of rows afftected
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="execute">Execution Type of the command</param>
        /// <returns>the number of rows affected</returns>
        internal int ExeccuteSP(string spName, List<string> paramNames, List<object> paramValues,
            List<SqlDbType> paramType, ExecuteParam execute)
        {
            int value = -1;

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);

            SqlCommand cmd = new SqlCommand(spName, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < paramNames.Count; i++)
            {
                cmd.Parameters.Add(paramNames[i], paramType[i]);

                cmd.Parameters[paramNames[i]].SqlDbType = paramType[i];
                cmd.Parameters[paramNames[i]].Value = paramValues[i];
            }

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (execute == ExecuteParam.NonQuery)
                {
                    value = cmd.ExecuteNonQuery();
                }
                else
                {
                    value = Convert.ToInt32(cmd.ExecuteScalar());
                }

                conn.Close();

                return value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error !\n" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        /// <summary>
        /// ExecuteSP Excecutes stored procedure and returns the object value
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="execute">Execution Type of the command</param>
        /// <returns>the number of rows affected</returns>
        internal object ExeccuteSPScalar(string spName, List<string> paramNames, List<object> paramValues,
            List<SqlDbType> paramType, ExecuteParam execute)
        {
            object value = -1;
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < paramNames.Count; i++)
            {
                cmd.Parameters.Add(paramNames[i], paramType[i]);

                cmd.Parameters[paramNames[i]].SqlDbType = paramType[i];
                cmd.Parameters[paramNames[i]].Value = paramValues[i];
            }

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (execute == ExecuteParam.NonQuery)
                    value = cmd.ExecuteNonQuery();
                else
                    value = cmd.ExecuteScalar();
                conn.Close();

                return value;
            }
            catch (Exception ex)
            {
                throw new Exception("Error !\n" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// ExecuteSP Excecutes stored proceodeure
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="direction">List of param directin of the sotred proceudre</param>
        /// <param name="exec">ExecutuinType</param>
        /// <param name="outParam">Dictionary of output parametres</param>
        /// <returns></returns>
        internal int ExeccuteSP(string spName, List<string> paramNames, List<object> paramValues,
            List<SqlDbType> paramType, List<ParameterDirection> direction,
            ExecuteParam exec, List<int> outSize, ref Dictionary<string, object> outParam)
        {
            int value = -1, k = 0;

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < paramNames.Count; i++)
            {
                cmd.Parameters.Add(paramNames[i], paramType[i]);
                cmd.Parameters[paramNames[i]].SqlDbType = paramType[i];

                if (direction[i] != ParameterDirection.Output)
                    cmd.Parameters[paramNames[i]].Value = paramValues[i];
                else
                {
                    cmd.Parameters[i].Direction = ParameterDirection.Output;
                    cmd.Parameters[i].Size = outSize[k++];
                }
            }

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (exec == ExecuteParam.NonQuery)
                    value = cmd.ExecuteNonQuery();
                else
                    value = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

                for (int j = 0; j < direction.Count; j++)
                {
                    if (direction[j] == ParameterDirection.Output)
                        outParam.Add(paramNames[j], cmd.Parameters[paramNames[j]].Value);
                }

                return value;
            }

            catch (Exception ex)
            {
                throw new Exception("Error !\n" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// ExecuteSP Excecutes stored proceodeure
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="direction">List of param directin of the sotred proceudre</param>
        /// <param name="conn">Sql connection object</param>
        internal object ExeccuteSP(string spName, string paramName, object paramValue,
            SqlDbType paramType, ExecuteParam execute)
        {
            object obj = null;
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramName != null || paramValue != null)
            {
                cmd.Parameters.Add(paramName, paramType);
                cmd.Parameters[paramName].SqlDbType = paramType;
                cmd.Parameters[paramName].Value = paramValue;
            }

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (execute == ExecuteParam.NonQuery)
                    obj = cmd.ExecuteNonQuery();
                else
                    obj = cmd.ExecuteScalar();
                conn.Close();

                return obj;
            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }


        /// <summary>
        /// ExecuteSP Excecutes stored proceodeure
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="direction">List of param directin of the sotred proceudre</param>
        /// <param name="conn">Sql connection object</param>
        internal object ExeccuteSP(string spName, ExecuteParam execute)
        {
            object obj = null;
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;


            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (execute == ExecuteParam.NonQuery)
                    obj = cmd.ExecuteNonQuery();
                else
                    obj = cmd.ExecuteScalar();
                conn.Close();

                return obj;
            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }


        /// <summary>
        /// ExecuteSP Excecutes stored procedure
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="direction">List of param directin of the sotred proceudre</param>
        /// <param name="conn">Sql connection object</param>
        internal DataTable ExeccuteSP(string spName, object value, string paramName, SqlDbType paramType)
        {
            DataTable dt = new DataTable();
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(paramName, paramType).Value = value;

            try
            {
                SqlDataAdapter sqlDt = new SqlDataAdapter(cmd);
                sqlDt.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }

        internal DataTable ExcecuteCommand(string command, System.Data.CommandType cmdType)
        {
            try
            {
                DataTable dt = new DataTable();
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
                SqlCommand cmd = new SqlCommand(command, conn);
                cmd.CommandType = cmdType;// == CommandType.PROCEDURE ? CommandType.StoredProcedure : CommandType.QUERY;
                SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
                sqlDa.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception("DATA BASE ERROR!\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Found!\n\n" + ex.Message);
            }
        }

        internal DataTable ExeccuteSP(string spName, List<string> paramNames, List<object> paramValues, List<SqlDbType> paramType)
        {

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < paramNames.Count; i++)
                cmd.Parameters.Add(paramNames[i], paramType[i]).Value = paramValues[i];


            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(dt);
                return dt;

            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }

        /// <summary>
        /// ExecuteSP Excecutes stored procedure
        /// </summary>
        /// <param name="spName">StoredProcedure Name</param>
        /// <param name="paramNames">List of param Names in the stored Proceudre </param>
        /// <param name="paramValues">List of pram Vlaues of the stored procuire</param>
        /// <param name="paramType">List of param types of the stored procedure</param>
        /// <param name="direction">List of param directin of the sotred proceudre</param>
        /// <param name="conn">Sql connection object</param>
        internal DataSet ExeccuteSPDataSet(string spName, object value, string paramName, SqlDbType paramType)
        {
            DataSet ds = new DataSet();
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(paramName, paramType).Value = value;

            try
            {
                SqlDataAdapter sqlDt = new SqlDataAdapter(cmd);
                sqlDt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }

        internal DataSet ExcecuteCommandDataSet(string command, System.Data.CommandType cmdType)
        {
            try
            {
                DataSet ds = new DataSet();
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
                SqlCommand cmd = new SqlCommand(command, conn);
                cmd.CommandType = cmdType;// == CommandType.PROCEDURE ? CommandType.StoredProcedure : CommandType.QUERY;
                SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
                sqlDa.Fill(ds);
                return ds;
            }
            catch (SqlException ex)
            {
                throw new Exception("DATA BASE ERROR!\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Found!\n\n" + ex.Message);
            }
        }

        internal DataSet ExeccuteSPDataSet(string spName, List<string> paramNames, List<object> paramValues, List<SqlDbType> paramType)
        {

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < paramNames.Count; i++)
                cmd.Parameters.Add(paramNames[i], paramType[i]).Value = paramValues[i];


            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            { throw new Exception("Error !\n" + ex.Message); }
            finally { conn.Close(); }
        }

        internal object RunCommand(string command, ExecuteParam execute)
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this.connectionString);
            try
            {
                object output = new object();
                SqlCommand cmd = new SqlCommand(command, conn);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (execute == ExecuteParam.NonQuery)
                    output = cmd.ExecuteNonQuery();
                else
                    output = cmd.ExecuteScalar();

                conn.Close();

                return output;
            }
            catch (SqlException ex)
            {
                throw new Exception("DATA BASE ERROR!\n\n" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Found!\n\n" + ex.Message);
            }
            finally { conn.Close(); }
        }
    }
}                
