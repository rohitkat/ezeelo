using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Transactions;

namespace Administrator.Controllers
{
    public class SchemeBudgetController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /SchemeBudget/
        public ActionResult Index()
        {
            //var schemebudgets = db.SchemeBudgets.Include(s => s.ReferAndEarnSchema);
            //return View(schemebudgets.ToList());

            List<SchemeBudgetTransaction> SBT = new List<SchemeBudgetTransaction>();
            SBT = db.SchemeBudgetTransactions.Include(s => s.ReferAndEarnSchema).ToList();
            return View(SBT.ToList());
        }

        // GET: /SchemeBudget/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchemeBudget schemebudget = db.SchemeBudgets.Find(id);
            if (schemebudget == null)
            {
                return HttpNotFound();
            }
            return View(schemebudget);
        }

        // GET: /SchemeBudget/Create
        public ActionResult Create()
        {
            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name");
            return View();
        }

        // POST: /SchemeBudget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReferAndEarnSchemaID,BudgetAmount,PreRemainingAmt,PreUsedAmt,ExpiryDate,ActionStatus")] SchemeBudget schemebudget, string ExpiryDate1)
        {
            if (ModelState.IsValid)
            {
                DateTime lExpiryDate = DateTime.Now;
                if (ExpiryDate1 != "")
                {
                    lExpiryDate = CommonFunctions.GetProperDateTime(ExpiryDate1);
                    schemebudget.ExpiryDate = lExpiryDate;
                }
                using (TransactionScope ts = new TransactionScope())
                {
                    SchemeBudget SB = new SchemeBudget();
                    SchemeBudgetTransaction SBT = new SchemeBudgetTransaction();

                    if (db.SchemeBudgets.Any(x => x.ReferAndEarnSchemaID == schemebudget.ReferAndEarnSchemaID))
                    {
                        SBT = db.SchemeBudgetTransactions.Where(x=>x.ReferAndEarnSchemaID == schemebudget.ReferAndEarnSchemaID).FirstOrDefault();

                        SB.PreRemainingAmt = SBT.RemainingAmt;
                        SB.PreUsedAmt = SBT.TotalBudgetAmt - SBT.RemainingAmt;


                        if (schemebudget.ActionStatus == false) // Condition Added by Zubair on 13-09-2017 for checking deduct amount if Increment/Decrement is Uncheck
                        {
                            if (SB.PreRemainingAmt < schemebudget.BudgetAmount)
                            {
                                ViewBag.Msg = "You can not deduct Scheme budget amount, because you have a low budget Amount";
                                ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", schemebudget.ReferAndEarnSchemaID);
                                return View();
                            }
                        }

                    }
                    else
                    {
                        SB.PreRemainingAmt = 0;
                        SB.PreUsedAmt = 0;
                        if(schemebudget.ActionStatus==false)
                        {
                            ViewBag.Msg = "You can not deduct Scheme budget amount. Plese click on Increment/Decrement";
                            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", schemebudget.ReferAndEarnSchemaID);
                            return View();
                        }
                    }
                    SB.ReferAndEarnSchemaID = schemebudget.ReferAndEarnSchemaID;
                    SB.BudgetAmount = schemebudget.BudgetAmount;
                    SB.ExpiryDate = schemebudget.ExpiryDate;
                    SB.ActionStatus = schemebudget.ActionStatus;
                    SB.CreateDate = DateTime.UtcNow.AddHours(5.3);
                    SB.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    SB.NetworkIP = CommonFunctions.GetClientIP();
                    db.SchemeBudgets.Add(SB);
                    db.SaveChanges();

                    
                    if (!db.SchemeBudgetTransactions.Any(x => x.ReferAndEarnSchemaID == SB.ReferAndEarnSchemaID))
                    {
                        SBT.ReferAndEarnSchemaID = schemebudget.ReferAndEarnSchemaID;
                        SBT.TotalBudgetAmt = schemebudget.BudgetAmount;
                        SBT.RemainingAmt = schemebudget.BudgetAmount;
                        SBT.ExpiryDate = schemebudget.ExpiryDate;
                        SBT.CreateDate = DateTime.UtcNow.AddHours(5.3);
                        SBT.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        SBT.NetworkIP = CommonFunctions.GetClientIP();
                        db.SchemeBudgetTransactions.Add(SBT);
                        db.SaveChanges();
                    }
                    else
                    {
                        //SBT = db.SchemeBudgetTransactions.Find(schemebudget.ReferAndEarnSchemaID);
                        SBT = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemebudget.ReferAndEarnSchemaID).FirstOrDefault();
                        if (SB.ActionStatus == true)
                        {
                            SBT.TotalBudgetAmt = SBT.TotalBudgetAmt + schemebudget.BudgetAmount;
                            SBT.RemainingAmt = SBT.RemainingAmt + schemebudget.BudgetAmount;
                        }
                        else
                        {
                            SBT.TotalBudgetAmt = SBT.TotalBudgetAmt - schemebudget.BudgetAmount;
                            SBT.RemainingAmt = SBT.RemainingAmt - schemebudget.BudgetAmount;
                        }
                        
                        SBT.ExpiryDate = schemebudget.ExpiryDate;
                        SBT.ModifyDate = DateTime.UtcNow.AddHours(5.3);
                        SBT.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        SBT.NetworkIP = CommonFunctions.GetClientIP();
                        //db.SchemeBudgets.Add(SB);
                        db.SaveChanges();
                    }
                    ts.Complete();
                }
                return RedirectToAction("Index");
            }

            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", schemebudget.ReferAndEarnSchemaID);
            return View(schemebudget);
        }

        // GET: /SchemeBudget/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchemeBudget schemebudget = db.SchemeBudgets.Find(id);
            if (schemebudget == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", schemebudget.ReferAndEarnSchemaID);
            return View(schemebudget);
        }

        // POST: /SchemeBudget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,ReferAndEarnSchemaID,BudgetAmount,PreRemainingAmt,PreUsedAmt,ExpiryDate,ActionStatus,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] SchemeBudget schemebudget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schemebudget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", schemebudget.ReferAndEarnSchemaID);
            return View(schemebudget);
        }

        // GET: /SchemeBudget/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchemeBudget schemebudget = db.SchemeBudgets.Find(id);
            if (schemebudget == null)
            {
                return HttpNotFound();
            }
            return View(schemebudget);
        }

        // POST: /SchemeBudget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            SchemeBudget schemebudget = db.SchemeBudgets.Find(id);
            db.SchemeBudgets.Remove(schemebudget);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
