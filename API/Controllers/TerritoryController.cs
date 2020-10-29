//-----------------------------------------------------------------------
// <copyright file="TeretoryController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using API.Models;

namespace API.Controllers
{
    public class TerritoryController : ApiController
    {
        // GET api/teretory/1/8304
        /// <summary>
        /// Get list of territory depending on level.
        ///For State Level =1 and parentID= ContryID
        ///For District Level =2 and parentID= StateID
        ///For City Level =3 and parentID= DistrictID
        ///For Pincode Level =4 and parentID=CityID
        ///For Area Level =5 and parentID=PincodeID
        /// </summary>
        /// <param name="level">Territory Level</param>
        /// <param name="parentID">Parent Territory ID</param>
        /// <returns></returns>
         [ApiException] 
        [ValidateModel]
        public IEnumerable<TeretoryViewModel> Get(int level,long parentID)
        {
            BusinessLogicLayer.Teretory teretory = new  BusinessLogicLayer.Teretory();
            return teretory.GetList((Teretory.REGION)level, parentID);
        }

       
    }
}
