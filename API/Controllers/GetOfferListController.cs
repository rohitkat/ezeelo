using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetOfferListController : ApiController
    {
        public object Get(int FranchiseId)
        {
            object obj = new object();
            try
            {
                List<OfferViewModel> OfferviewModelList = new List<OfferViewModel>();
                OfferViewModel model = new OfferViewModel();
                model.OfferViewModelId = 1;
                model.Name = "Deals of the day";
                model.DescriptionList = new List<string>();
                model.DescriptionList.Add("Best Deal Guaranteed");
                model.IsSet = true;
                OfferviewModelList.Add(model);

                OfferViewModel model1 = new OfferViewModel();
                model1.OfferViewModelId = 2;
                model1.Name = "48Hrs Deals";
                model1.DescriptionList = new List<string>();
                model1.DescriptionList.Add("Limited Period Offer");
                model1.IsSet = true;
                OfferviewModelList.Add(model1);

                OfferViewModel model2 = new OfferViewModel();
                model2.OfferViewModelId = 3;
                model2.Name = "Hot Deals";
                model2.DescriptionList = new List<string>();
                model2.DescriptionList.Add("All Hottest Deal under one click");
                model2.IsSet = true;
                OfferviewModelList.Add(model2);

                OfferViewModel model3 = new OfferViewModel();
                model3.OfferViewModelId = 4;
                model3.Name = "Newly Launch";
                model3.DescriptionList = new List<string>();
                model3.DescriptionList.Add("Just launched in Market");
                model3.IsSet = true;
                OfferviewModelList.Add(model3);

                OfferViewModel model4 = new OfferViewModel();
                model4.OfferViewModelId = 5;
                model4.Name = "Trending Deals";
                model4.DescriptionList = new List<string>();
                model4.DescriptionList.Add("Latest Trending Products ");
                model4.IsSet = true;
                OfferviewModelList.Add(model4);

                OfferViewModel model5 = new OfferViewModel();
                model5.OfferViewModelId = 6;
                model5.Name = "Major Retail Points";
                model5.DescriptionList = new List<string>();
                model5.DescriptionList.Add("Product with Highest Retail Points.");
                model5.IsSet = true;
                OfferviewModelList.Add(model5);

                OfferViewModel model6 = new OfferViewModel();
                model6.OfferViewModelId = 7;
                model6.Name = "Offer by Brands";
                model6.DescriptionList = new List<string>();
                model6.DescriptionList.Add("Product By Brands");
                model6.IsSet = true;
                OfferviewModelList.Add(model6);

                OfferViewModel model7 = new OfferViewModel();
                model7.OfferViewModelId = 8;
                model7.Name = "Combo Offers";
                model7.DescriptionList = new List<string>();
                model7.DescriptionList.Add("Best Combo’s in Town");
                model7.IsSet = true;
                OfferviewModelList.Add(model7);

                OfferViewModel model8 = new OfferViewModel();
                model8.OfferViewModelId = 9;
                model8.Name = "Optional Offers";
                model8.DescriptionList = new List<string>();
                model8.DescriptionList.Add("Best Combo’s in Town");
                model8.IsSet = false;
                OfferviewModelList.Add(model8);

                if (OfferviewModelList != null && OfferviewModelList.Count > 0)
                    obj = new { Success = 1, Message = "Success", data = OfferviewModelList };
                else
                    obj = new { Success = 0, Message = "Offer list not found.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
    public class OfferViewModel
    {
        public int OfferViewModelId { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public List<string> DescriptionList { get; set; }
        public bool IsSet { get; set; }
    }
}
