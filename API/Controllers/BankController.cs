//-----------------------------------------------------------------------
// <copyright file="BankController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
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
using API.Models;

namespace API.Controllers
{
    public class BankController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        /// <summary>
        /// Gets the list of banks.
        /// </summary>
        [ApiException] 
        public IQueryable<Bank> GetBanks()
        {
            return db.Banks;
        }
        
        // GET api/Bank/5
        /// <summary>
        /// Get selected bank details
        /// </summary>
        /// <param name="id">Bank ID</param>
        /// <returns>Returns selected bank details</returns>
        [ApiException] 
        [ResponseType(typeof(Bank))]
        public IHttpActionResult GetBank(int id)
        {
            Bank bank = db.Banks.Find(id);
            if (bank == null)
            {
                return NotFound();
            }
            return Ok(bank);
        }

        // PUT api/Bank/5
        /// <summary>
        /// Edit details of selected bank
        /// </summary>
        /// <param name="id">Bank id for which details to be updated. </param>
        /// <param name="bank"></param>
        /// <returns>Returns operation status Success/Failure Code</returns>
         [ApiException] 
        public IHttpActionResult PutBank(int id, Bank bank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bank.ID)
            {
                return BadRequest();
            }

            db.Entry(bank).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankExists(id))
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

        // POST api/Bank
        /// <summary>
        /// Save new bank details
        /// </summary>
        /// <param name="bank"></param>
         /// <returns>Returns operation status Success/Failure Code</returns>
         [ApiException] 
        [ResponseType(typeof(Bank))]
        public IHttpActionResult PostBank(Bank bank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Banks.Add(bank);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bank.ID }, bank);
        }

        // DELETE api/Bank/5
        /// <summary>
        /// Delete perticular bank details
        /// </summary>
        /// <param name="id">Bank ID</param>
         /// <returns>operation status Success/Failure Code</returns>
         [ApiException] 
        [ResponseType(typeof(Bank))]
        public IHttpActionResult DeleteBank(int id)
        {
            Bank bank = db.Banks.Find(id);
            if (bank == null)
            {
                return NotFound();
            }

            db.Banks.Remove(bank);
            db.SaveChanges();

            return Ok(bank);
        }
         [ApiException] 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
         [ApiException] 
        private bool BankExists(int id)
        {
            return db.Banks.Count(e => e.ID == id) > 0;
        }
    }
}