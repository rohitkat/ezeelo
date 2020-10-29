using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using Franchise.Models.ViewModel;
using Franchise.Models;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    public class EmployeeAssignmentController : Controller
    {
        //
        // GET: /EmployeeAssignment/
        public ActionResult Index()
        {
            return View();
        }
	}
}