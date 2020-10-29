﻿using API.Models;
//-----------------------------------------------------------------------
// <copyright file="MerchantReviewsController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Mohit Sinha</author>
//-----------------------------------------------------------------------
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace API.Controllers
{
    public class MerchantReviewsController : ApiController
    {
        public HttpResponseMessage Get(long ShopId)
        {
            MerchantApp lMerchantApp = new MerchantApp(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
           // List<DisplayReviewsViewModel> lDisplayReviewsViewModel = new List<DisplayReviewsViewModel>();
            DisplayReviewsViewModel reviews = new DisplayReviewsViewModel();
            BusinessLogicLayer.Review objReview = new Review();
            reviews = objReview.GetReviews(ShopId, BusinessLogicLayer.Review.REVIEWS.SHOP);
            return Request.CreateResponse(HttpStatusCode.OK, reviews);
        }

    }
}