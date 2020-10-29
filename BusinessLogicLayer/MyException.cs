//---------------------------------------------------------------------------
// <copyright file="MyException.cs" company="NSP Futuretech Pvt.Ltd.">
// Copyright (c) NSP Futuretech Pvt.Ltd.. All rights reserved.
// </copyright>
// <author>Aslam and Saba</author>
// <Date> 20-06-2011 </Date>
//---------------------------------------------------------------------------

namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Custom Exception class that inherits all the properties and behaviour of the Exception class
    /// </summary>
    public class MyException : Exception
    {
        /// <summary>
        /// stores the path of the method or class in which exception occured
        /// </summary>
        private string exceptionPath = string.Empty;

        /// <summary>
        /// stores the message detailing about exception
        /// </summary>
        private string exceptionMsg = string.Empty;

        /// <summary>
        /// stores the Exception Code
        /// </summary>
        private string exceptionCode = "0000";

        /// <summary>
        /// Initializes a new instance of the MyException class
        /// </summary>
        /// <param name="path">Path in which exception occured</param>
        /// <param name="message">Message deatailing about exception </param>
        public MyException(string path, string message)
            : base()
        {
            this.exceptionPath = path;
            this.exceptionMsg = message;
        }

        /// <summary>
        /// Gets the Exception Path 
        /// </summary>
        public string EXCEPTION_PATH
        {
            get
            {
                return this.exceptionPath;
            }
        }

        /// <summary>
        /// Gets the Exception Message 
        /// </summary>
        public string EXCEPTION_MSG
        {
            get
            {
                return this.exceptionMsg;
            }
        }
    }
}
