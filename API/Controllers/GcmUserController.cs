using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ModelLayer.Models;

namespace API.Controllers
{
    public class GcmUserController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET api/GcmUser
        public IQueryable<GcmUser> GetGcmUsers()
        {
            return db.GcmUsers;
        }

        // GET api/GcmUser/5
        [ResponseType(typeof(GcmUser))]
        public IHttpActionResult GetGcmUser(long id)
        {
            GcmUser gcmuser = db.GcmUsers.Find(id);
            if (gcmuser == null)
            {
                return NotFound();
            }

            return Ok(gcmuser);
        }

        // PUT api/GcmUser/5
        public IHttpActionResult PutGcmUser(long id, GcmUser gcmuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gcmuser.ID)
            {
                return BadRequest();
            }

            db.Entry(gcmuser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GcmUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/GcmUser
        //[ResponseType(typeof(GcmUser))]
        //public object PostGcmUser(GCMUserViewModel gCMUserViewModel)
        //{
        //    try
        //    {
        //        GcmUser lGcmUser = db.GcmUsers.FirstOrDefault(x => x.Name == gCMUserViewModel.IMEI);
        //        if (lGcmUser != null) //- Update Old GCM Details.
        //        {
        //            lGcmUser.GcmRegID = gCMUserViewModel.GcmRegID;
        //            if (gCMUserViewModel.UserLoginID != null)
        //            {
        //                if (lGcmUser.Name == gCMUserViewModel.IMEI && lGcmUser.GcmRegID == gCMUserViewModel.GcmRegID && gCMUserViewModel.UserLoginID > 0)
        //                {
        //                    lGcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
        //                    lGcmUser.IsActive = true;
        //                }
        //                else
        //                {
        //                    lGcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
        //                    lGcmUser.IsActive = false;
        //                }
        //            }
        //            if (gCMUserViewModel.MCOID != null && gCMUserViewModel.MCOID != "")
        //            {
        //                lGcmUser.MCOID = gCMUserViewModel.MCOID;
        //            }
        //            if (gCMUserViewModel.EmailID != null && gCMUserViewModel.EmailID != "")
        //            {
        //                lGcmUser.EmailID = gCMUserViewModel.EmailID;
        //            }
        //            db.Entry(lGcmUser).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return new { HTTPStatusCode = "200", UserMessage = "User Already Exist." };
        //        }
        //        //- Create a new GCM User.
        //        GcmUser gcmUser = new GcmUser();
        //        gcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
        //        gcmUser.MCOID = gCMUserViewModel.MCOID;
        //        gcmUser.GcmRegID = gCMUserViewModel.GcmRegID;
        //        gcmUser.EmailID = gCMUserViewModel.EmailID;
        //        gcmUser.Name = gCMUserViewModel.IMEI;
        //        gcmUser.City = gCMUserViewModel.City;
        //        //gcmUser.MCOID = gCMUserViewModel.MCOID;
        //        gcmUser.IsActive = false;
        //        gcmUser.CreateDate = DateTime.Now;

        //        //if (!ModelState.IsValid)
        //        //{
        //        //    return new { HTTPStatusCode = "400", UserMessage = ModelState };
        //        //    //return BadRequest(ModelState);
        //        //}
        //        db.GcmUsers.Add(gcmUser);
        //        db.SaveChanges();

        //        return new { HTTPStatusCode = "200", UserMessage = "GCM User added successfully." };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new { HTTPStatusCode = "400", UserMessage = "Error in Creating GCM User" };
        //    }

        //}

        [HttpPost]
        public object Post(GCMUserViewModel gCMUserViewModel)
        {
            object obj = new object();
            try
            {
                if (gCMUserViewModel == null || gCMUserViewModel.UserLoginID <= 0 || string.IsNullOrEmpty(gCMUserViewModel.FCMRegId))
                {
                    obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                GcmUser lGcmUser = db.GcmUsers.FirstOrDefault(x => x.Name == gCMUserViewModel.IMEI);
                if (lGcmUser != null) //- Update Old GCM Details.
                {
                    lGcmUser.GcmRegID = gCMUserViewModel.FCMRegId;
                    if (gCMUserViewModel.UserLoginID != null)
                    {
                        if (lGcmUser.Name == gCMUserViewModel.IMEI && lGcmUser.GcmRegID == gCMUserViewModel.FCMRegId && gCMUserViewModel.UserLoginID > 0)
                        {
                            lGcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
                            lGcmUser.IsActive = true;
                        }
                        else
                        {
                            lGcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
                            lGcmUser.IsActive = false;
                        }
                    }
                    if (gCMUserViewModel.FranchiseId != null && gCMUserViewModel.FranchiseId != "")
                    {
                        lGcmUser.MCOID = gCMUserViewModel.FranchiseId;
                    }
                    if (gCMUserViewModel.EmailID != null && gCMUserViewModel.EmailID != "")
                    {
                        lGcmUser.EmailID = gCMUserViewModel.EmailID;
                    }
                    db.Entry(lGcmUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return obj = new { Success = 0, Message = "User Already Exist.", data = string.Empty };
                }
                //- Create a new GCM User.
                GcmUser gcmUser = new GcmUser();
                gcmUser.UserLoginID = gCMUserViewModel.UserLoginID;
                gcmUser.MCOID = gCMUserViewModel.FranchiseId;
                gcmUser.GcmRegID = gCMUserViewModel.FCMRegId;
                gcmUser.EmailID = gCMUserViewModel.EmailID;
                gcmUser.Name = gCMUserViewModel.IMEI;
                gcmUser.City = gCMUserViewModel.CityID;
                //gcmUser.MCOID = gCMUserViewModel.MCOID;
                gcmUser.IsActive = true;
                gcmUser.CreateDate = DateTime.Now;

                //if (!ModelState.IsValid)
                //{
                //    return new { HTTPStatusCode = "400", UserMessage = ModelState };
                //    //return BadRequest(ModelState);
                //}
                db.GcmUsers.Add(gcmUser);
                db.SaveChanges();
                obj = new { Success = 1, Message = "FCM User added successfully.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        // DELETE api/GcmUser/5
        [ResponseType(typeof(GcmUser))]
        public IHttpActionResult DeleteGcmUser(long id)
        {
            GcmUser gcmuser = db.GcmUsers.Find(id);
            if (gcmuser == null)
            {
                return NotFound();
            }

            db.GcmUsers.Remove(gcmuser);
            db.SaveChanges();

            return Ok(gcmuser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GcmUserExists(long id)
        {
            return db.GcmUsers.Count(e => e.ID == id) > 0;
        }
    }
    public class GCMUserViewModel
    {
        public string FCMRegId { get; set; }
        public string EmailID { get; set; }
        public string IMEI { get; set; }
        public string CityID { get; set; }
        public string FranchiseId { get; set; }
        public long UserLoginID { get; set; }


    }
}