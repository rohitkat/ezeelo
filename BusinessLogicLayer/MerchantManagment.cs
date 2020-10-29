//-----------------------------------------------------------------------
// <copyright file="CustomerManagement.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public class MerchantManagment
    {
        protected System.Web.HttpServerUtility server;
        protected EzeeloDBContext db = new EzeeloDBContext();
        public MerchantManagment(System.Web.HttpServerUtility server)
        {
            this.server = server;        
        }
    }
}
