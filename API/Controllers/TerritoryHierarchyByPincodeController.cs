using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class TerritoryHierarchyByPincodeController : ApiController
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        // GET api/territoryhierarchybypincode/440001
        /// <summary>
        /// Get hierarchy details above pincode like city, district, state for provided pincode
        /// </summary>
        /// <param name="pincode">pincode e.g 440001</param>
        /// <returns>Hierachy and Area List for provided pincode</returns>
        [ValidateModel]
        [ApiException]
        public object Get(int? cityId)
        {
            object obj = new object();
            try
            {
                if (cityId == null || cityId <= 0)
                    return obj = new { Success = 0, Message = "Invalid Pincode", data = string.Empty };


                List<FranchiseAreaPincode> FranchAreaPin = new List<FranchiseAreaPincode>();

                FranchAreaPin = FranchiseAllotedLocation.GetFranchiseAreaPincode(cityId.Value);

                if (FranchAreaPin.Count() > 0)
                    obj = new { Success = 1, Message = "Successfull.", data = FranchAreaPin };
                //  obj = new { HTTPStatusCode = "200", UserMessage = "Successfull.", Data = FranchAreaPin };
                else if (FranchAreaPin.Count() == 0)
                    obj = new { Success = 0, Message = "No Record found.", data = string.Empty };
                // obj = new { HTTPStatusCode = "204", UserMessage = "No Record found." };
                else
                    //{ 
                    obj = new { Success = 0, Message = "Failed.", data = string.Empty };
                //obj = new { HTTPStatusCode = "400", UserMessage = "Failed." };
                //}
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
            //For invalid pincode

            //TerritoryByPincodeViewModel territory = new TerritoryByPincodeViewModel();

            //db.Configuration.ProxyCreationEnabled = false;

            //territory = (from ct in db.Cities
            //             join dt in db.Districts on ct.DistrictID equals dt.ID
            //             join st in db.States on dt.StateID equals st.ID
            //             where ct.ID == cityId
            //             select new TerritoryByPincodeViewModel
            //             {
            //                 CityID = ct.ID,
            //                 CityName = ct.Name,
            //                 DistrictID = dt.ID,
            //                 DistrictName = dt.Name,
            //                 StateID = st.ID,
            //                 StateName = st.Name,
            //                 PincodeList = ct.Pincodes
            //             }).FirstOrDefault();

            ////Get area list belongs to this Pincode ID
            //if (territory != null)
            //{
            //    List<int> PincodeIds = territory.PincodeList.Select(x => x.ID).ToList();
            //    if (PincodeIds != null && PincodeIds.Count > 0)
            //    {
            //        territory.Pincodes = new List<TerritoryPincodeViewModel>();
            //        List<Area> AreaList = db.Areas.Include("FranchiseLocations").Where(x => PincodeIds.Contains(x.PincodeID) && x.IsActive).ToList();
            //        territory.AreaList = new List<TeretoryViewModel>();
            //        if (AreaList != null && AreaList.Count > 0)
            //        {
            //            foreach (var item in AreaList)
            //            {
            //                if (item.FranchiseLocations.Where(x => x.IsActive).Any())
            //                {
            //                    TeretoryViewModel teretorymodel = new TeretoryViewModel();
            //                    teretorymodel.ID = item.ID;
            //                    teretorymodel.Name = item.Name;
            //                    territory.AreaList.Add(teretorymodel);

            //                    TerritoryPincodeViewModel pincodemodel = (from pin in territory.PincodeList
            //                                                              where pin.ID == item.PincodeID
            //                                                              select new TerritoryPincodeViewModel { ID = pin.ID, Name = pin.Name }).FirstOrDefault();
            //                    if (territory.Pincodes.Where(x => x.ID == pincodemodel.ID).Any() == false)
            //                        territory.Pincodes.Add(pincodemodel);
            //                }
            //            }
            //        }
            //        territory.PincodeList = null;
            //    }

            //    territory.AreaList = territory.AreaList.OrderBy(x => x.Name).ToList();
            // }
            //else
            //{
            //    return obj = new { Success = 0, Message = "Territory details not found for this pincode.", data = string.Empty };
            //    // Request.CreateResponse(HttpStatusCode.BadRequest, new { HTTPStatusCode = "200", UserMessage = "Details Not found", ValidationError = "Territory details not found for this pincode." });
            //}
            //return obj = new { Success = 1, Message = "Success", data = territory };
            //Request.CreateResponse(HttpStatusCode.OK, territory);

        }


        public object Getcity()
        {
            object obj = new object();
            try
            {
                var Cities = (from f in db.Franchises
                              where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                              select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().OrderBy(x => x.Name).ToList();
                if (Cities != null && Cities.Count > 0)
                {
                    obj = new { Success = 1, Message = "City list found.", data = Cities };
                }
                else
                {
                    obj = new { Success = 0, Message = "City list not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

    }
}
