//-----------------------------------------------------------------------
// <copyright file="Teretory.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
/*
 Handed over to Pradnyakar SIr
 */
namespace BusinessLogicLayer
{
  public class Teretory
    {
      private EzeeloDBContext db = new EzeeloDBContext();       
      /// <summary>
      /// Enum for terretory level.
      /// </summary>
      public enum REGION
      {
          STATE = 1,
          DISTRICT = 2,
          CITY=3,
          PINCODE=4,
          AREA=5
      }
      /// <summary>
      /// Get the list of Terretory depending on region(enum) and parent ID for 1st level region i. e State parent is null
      /// </summary>
      /// <param name="region">Enum REGION : STATE = 1,DISTRICT = 2,CITY=3,PINCODE=4,AREA=5</param>
      /// <param name="parentID">Parent Terretory ID. e.g for pincode city is the parent</param>
      /// <returns></returns>
      public List<TeretoryViewModel> GetList(REGION region,long? parentID)
      {
          List<TeretoryViewModel> listTeretory = new List<TeretoryViewModel>();
         
          if (region.ToString() == "STATE")
              //get StateList
              listTeretory = (from s in db.States
                      where s.IsActive == true
                      select new TeretoryViewModel
                      {
                          ID = s.ID,
                          Name = s.Name
                      }).ToList();
          else if (region.ToString() == "DISTRICT")
              //get District List
              listTeretory = (from s in db.Districts
                              where s.IsActive == true && s.StateID == parentID
                              select new TeretoryViewModel
                              {
                                  ID = s.ID,
                                  Name = s.Name
                              }).ToList();
          else if (region.ToString() == "CITY")
              //get City List
              listTeretory = (from s in db.Cities
                              where s.IsActive == true && s.DistrictID == parentID
                              select new TeretoryViewModel
                              {
                                  ID = s.ID,
                                  Name = s.Name
                              }).ToList();
          else if (region.ToString() == "PINCODE")
              //get Pincode List
              listTeretory = (from s in db.Pincodes
                              where s.IsActive == true && s.CityID == parentID
                              select new TeretoryViewModel
                              {
                                  ID = s.ID,
                                  Name = s.Name
                              }).ToList();
          else if (region.ToString() == "AREA")
              //get Area List
              listTeretory = (from s in db.Areas
                              where s.IsActive == true && s.PincodeID == parentID
                              select new TeretoryViewModel
                              {
                                  ID = s.ID,
                                  Name = s.Name
                              }).ToList();
          

          return listTeretory.OrderBy(x=>x.Name).ToList();

      }

    }
}
