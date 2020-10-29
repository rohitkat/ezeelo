
//-----------------------------------------------------------------------
// <copyright file="ISmsAndEmail" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public interface ISmsAndEmail
    {
        void SendMailToCustomer(long pUserLoginID, long pOrderID);
        void SendMailToMerchant(long pUserLoginID, long pOrderID);
        void SendSMSToCustomer(long pUserLoginID, long pOrderID);
        void SendSMSToMerchant(long pUserLoginID, long pOrderID);
    }
}
