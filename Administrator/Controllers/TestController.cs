using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System.Web.Configuration;
namespace Administrator.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        public ActionResult Index()
        {
            Int64 ProductID = 23884;
            Int64 OrderByID = 2;
            Int64 orderID = 15;
            int totalQty = 40;
            List<CorporateCustomerShippingAddressViewModel> OrderTo = new List<CorporateCustomerShippingAddressViewModel>();
            OrderTo.Add(new CorporateCustomerShippingAddressViewModel { AreaID = 1, DeliveryCharges = 50, ExpectedDeliveryDate = DateTime.Now, PincodeID = 8311, PrimaryMobile = "9874563214", Quantity = 10, SecondaryMobile = null, ShippingAddress = "Mahalgi nagar", ToName = "Pradnyaka" });
            OrderTo.Add(new CorporateCustomerShippingAddressViewModel { AreaID = 1, DeliveryCharges = 50, ExpectedDeliveryDate = DateTime.Now, PincodeID = 8311, PrimaryMobile = "9874563214", Quantity = 10, SecondaryMobile = null, ShippingAddress = "Mahalgi nagar", ToName = "Pradnyaka" });
            OrderTo.Add(new CorporateCustomerShippingAddressViewModel { AreaID = 1, DeliveryCharges = 50, ExpectedDeliveryDate = DateTime.Now, PincodeID = 8311, PrimaryMobile = "9874563214", Quantity = 10, SecondaryMobile = null, ShippingAddress = "Mahalgi nagar", ToName = "Pradnyaka" });
            OrderTo.Add(new CorporateCustomerShippingAddressViewModel { AreaID = 1, DeliveryCharges = 50, ExpectedDeliveryDate = DateTime.Now, PincodeID = 8311, PrimaryMobile = "9874563214", Quantity = 10, SecondaryMobile = null, ShippingAddress = "Mahalgi nagar", ToName = "Pradnyaka" });
            List<CorporateFacilityDetailsViewModel> facilityList = new List<CorporateFacilityDetailsViewModel>();
            facilityList.Add(new CorporateFacilityDetailsViewModel { CustomerOrderDetailID = 15, FacilityID = 1, ShippingFacilityCharges = 50 });
            CorporateGifting obj = new CorporateGifting(ProductID, OrderByID, orderID, totalQty, OrderTo, facilityList);

            ViewBag.result = "Output Result ID :- " + obj.oprStatus.ToString();
            //Console.ReadKey();
            List<CalulatedTaxesRecord> ls = new List<CalulatedTaxesRecord>();
            return View(ls);
        }
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();


        [HttpPost, ActionName("Index")]
        public ActionResult IndexResult()
        {
            BusinessLogicLayer.TaxationManagement obj = new TaxationManagement(fConnectionString);
            List<CalulatedTaxesRecord> ls = new List<CalulatedTaxesRecord>();
            List<Int64> paramValues = new List<long>();
            paramValues.Add(29456);
            paramValues.Add(29483);
            paramValues.Add(29485);
            paramValues.Add(29487);
            paramValues.Add(29491);
            ls = obj.CalculateTaxForMultipalProduct(paramValues);
            return View(ls);
        }


    }
}