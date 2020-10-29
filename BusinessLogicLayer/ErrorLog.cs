//---------------------------------------------------------------------------
// <copyright file="ErrorLog.cs" company="NSP Futuretech Pvt.Ltd.">
// Copyright (c) NSP Futuretech Pvt.Ltd.. All rights reserved.
// </copyright>
// <author>Prashant Bhoyar</author>
//---------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    /// <summary>
    /// This Class is used to Write the Error to the Log File
    /// </summary>
    public class ErrorLog : ILogging
    {
        /// <summary>
        /// String to write the path where the exception occured
        /// </summary>
        private static string exceptionStr = "<N:LogFile; C:ErrorLog;";

        /// <summary>
        /// Specifies the Module in which exception occurs 
        /// </summary>
        public enum Module
        {
            /// <summary>
            /// Constant To Represent Administrator Layer
            /// </summary>
            Administrator,

            /// <summary>
            /// API Module
            /// </summary>
            API,

            /// <summary>
            /// BissinessLogicLayer 
            /// </summary>
            BussinessLogicLayer,

            /// <summary>
            /// Represents CRM Module
            /// </summary>
            CRM,

            /// <summary>
            /// Module to represnt Data Access Layer
            /// </summary>
            DataAccessLayer,

            /// <summary>
            /// Represnts Delivery Partner Module
            /// </summary>
            DeliveryPartner,

            /// <summary>
            /// Represents Franchise Module 
            /// </summary>
            Franchise,

            /// <summary>
            /// Gandhibagh Module
            /// </summary>
            Gandhibagh,

            /// <summary>
            /// API Module 
            /// </summary>
            GBAPI,

            /// <summary>
            /// Merchant Module
            /// </summary>
            Merchant,

            /// <summary>
            /// Models 
            /// </summary>
            ModelLayer,

            /// <summary>
            /// Customer
            /// </summary>
            Customer,
            MarketPartner
        }

        /// <summary>
        /// Method to create directory  if not exist and if it is exist 
        /// <para>then to create a log file if log file is exist then to append the data </para>
        /// <para>to that log file to write the error</para>
        /// </summary>
        /// <param name="message"> Exception Message which is return by catch </param>
        /// <param name="module">Module for which error to be logged</param>
        /// <exception cref="LogFile.MyException">LogFile.MyException</exception>
        public static void ErrorLogFile(string message, Module module, System.Web.HttpServerUtility server)
        {
            try
            {
                ReadConfig readConfig = new ReadConfig(server);

                string filePath = readConfig.LOG_FOLDER_PATH;

                // add module name directory to the file path
                filePath += @"\" + module.ToString() + @"\";

                // To create a directory  if not exist and if it is exist then
                // to create a error log file if error log file is already exist then it
                // will append the data and write to that file, it will create a log file  
                // according to the system date, the file name will be datewise e.g. "16062011"  
                StreamWriter sw = new StreamWriter(
                    (Directory.Exists(filePath) ? filePath
                    : System.IO.Directory.CreateDirectory(filePath).FullName)
                    + DateTime.Now.ToString("ddMMyyyy") + ".txt",
                     true);
                sw.WriteLine(message);
                sw.WriteLine("-----------------------------------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
            }
            catch (IOException ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "File IO Error!" + ex.Message);
            }
            catch (MyException mex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine + mex.EXCEPTION_PATH, "Can't Create Log File!" + mex.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "Can't Create Log File!" + ex.Message);
            }
        }

        /// <summary>
        /// Method to create directory  if not exist and if it is exist 
        /// <para>then to create a log file if log file is exist then to append the data </para>
        /// <para>to that log file to write the error</para>
        /// </summary>
        /// <param name="message"> Exception Message which is return by catch </param>
        /// <param name="module">Module for which error to be logged</param>
        /// <param name="filePath">Complete Physical Path Of the Log folder.
        /// <para>e.g. c:\My Web App\ERROR_LOG </para>
        /// <para>Specify only the Path Upto Log Folder; file name sahll be automatically generated </para>
        /// </param>
        /// <exception cref="LogFile.MyException">LogFile.MyException</exception>
        public static void ErrorLogFile(string message, Module module, string filePath)
        {
            try
            {

                // add module name directory to the file path
                filePath += @"\" + module.ToString() + @"\";

                // To create a directory  if not exist and if it is exist then
                // to create a error log file if error log file is already exist then it
                // will append the data and write to that file, it will create a log file  
                // according to the system date, the file name will be datewise e.g. "16062011"  
                StreamWriter sw = new StreamWriter(
                    (Directory.Exists(filePath) ? filePath
                    : System.IO.Directory.CreateDirectory(filePath).FullName)
                    + DateTime.Now.ToString("ddMMyyyy") + ".txt",
                     true);
                sw.WriteLine(message);
                sw.WriteLine("-----------------------------------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
            }
            catch (IOException ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "File IO Error!" + ex.Message);
            }
            catch (MyException mex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine + mex.EXCEPTION_PATH, "Can't Create Log File!" + mex.EXCEPTION_MSG);
            }
            catch (Exception ex)
            {
                throw new MyException(exceptionStr + " M:ErrorLogFile>" + Environment.NewLine, "Can't Create Log File!" + ex.Message);
            }
        }
    }
}

