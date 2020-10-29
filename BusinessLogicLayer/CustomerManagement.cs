//-----------------------------------------------------------------------
// <copyright file="CustomerManagement.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    //base class for Cuctomer Related Details
    public class CustomerManagement:ICustomerManagement 
    {
        protected System.Web.HttpServerUtility server;
        protected EzeeloDBContext db = new EzeeloDBContext();
        public CustomerManagement(System.Web.HttpServerUtility server)
        {
            this.server = server;        
        }
    }
}
