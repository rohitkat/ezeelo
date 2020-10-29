//-----------------------------------------------------------------------
// <copyright file="ProductDisplay.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
/*
 Handed over to Mohit, Tejaswee
 */

namespace BusinessLogicLayer
{
    //This is a base class for product details and product List
    public class ProductDisplay : IProductManagement
    {
        protected System.Web.HttpServerUtility server;

        public ProductDisplay(System.Web.HttpServerUtility server)
        {
            this.server = server;
        }
        
    }
}
