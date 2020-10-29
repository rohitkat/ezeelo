//-----------------------------------------------------------------------
// <copyright file="Review.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
 Handed over to Mohit, Tejaswee
 */
namespace BusinessLogicLayer
{
    public class Review
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //A Enum for type of reviews
        public enum REVIEWS
        {
            /// <summary>
            /// Review for Order GBOD
            /// </summary>
            ORDER = 0,
            /// <summary>
            /// Review for Shop GBMR
            /// </summary>
            SHOP = 1,
            /// <summary>
            ///  Review for Product GBPR
            /// </summary>
            PRODUCT = 2

        }

        /// <summary>
        /// This overload created by Tejaswee 
        /// set productId because product id is required for redirecting on preview item page
        /// </summary>
        /// <param name="ownerID"></param>
        /// <param name="prodId"></param>
        /// <param name="reviewfor"></param>
        /// <returns></returns>
        public DisplayReviewsViewModel GetReviews(long ownerID,long prodId, REVIEWS reviewfor)
        {

            if (ownerID <= 0)
                return null;
            string prefix = string.Empty;
            //Review for order
            if (reviewfor.ToString() == "ORDER")
                prefix = "GBOD";
            //Review for Shop
            else if (reviewfor.ToString() == "SHOP")
                prefix = "GBMR";
            //Review for product
            else if (reviewfor.ToString() == "PRODUCT")
                prefix = "GBPR";

            DisplayReviewsViewModel reviews = new DisplayReviewsViewModel();
            List<CustomerReviewViewModel> rCollection = new List<CustomerReviewViewModel>();
            AverageRatingPointsViewModel avgPoints = new AverageRatingPointsViewModel();
            //get list of all reviews given by different customers
            //by AJ
            rCollection =
                (from CRF in db.CustomerRatingAndFeedbacks
                 join R in db.Ratings on CRF.RatingID equals R.ID
                 join BT in db.BusinessTypes on R.BusinessTypeID equals BT.ID
                 join PD in db.PersonalDetails on CRF.PersonalDetailID equals PD.ID
                 where BT.Prefix == prefix && CRF.OwnerID == ownerID
                 group CRF by new {CRF.PersonalDetailID } into a
                 select new CustomerReviewViewModel
                 {
                     ID = a.Max(x => x.ID),
                     Comment = a.Max(x => x.Feedback),
                     AvgPointsPerCustomer = a.Max(x => x.Point),
                     ReviewDate = a.Max(x => x.CreateDate),
                     CustomerName=a.Max(x=>x.PersonalDetail.FirstName)+" "+a.Max(x=>x.PersonalDetail.LastName),
                     CustomerPersonalDetailID = a.Max(x => x.PersonalDetailID),
                     //ReviewCount=2//  CRF.OwnerID
                 }).ToList();






            //        var duplicates = companies
            //.GroupBy(c => new { c.Name, c.Email })
            //.Select(g => new { Qty = g.Count(), First = g.OrderBy(c => c.Id).First() } )
            //.Select(p => new
            //    {
            //        Id = p.First.Id,
            //        Qty = p.Qty,
            //        Name = p.First.Name,
            //        Email = p.First.Email,
            //        Address = p.First.Address
            //    });


            //getAverage of all reviews given by different customers
            var avgRatingPoint = (from r in db.Ratings
                                  join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                  join cr in db.CustomerRatingAndFeedbacks on r.ID equals cr.RatingID
                                  join pd in db.PersonalDetails on cr.PersonalDetailID equals pd.ID
                                  where bt.Prefix == prefix && cr.OwnerID == ownerID
                                  group cr by new { cr.OwnerID } into temp
                                  select new AverageRatingPointsViewModel
                                  {
                                      AvgRatingPonts = temp.Average(x => x.Point),
                                      Count = temp.Count(),
                                      OwnerID = temp.Key.OwnerID

                                  }).ToList();

            foreach (var item in avgRatingPoint)
            {
                avgPoints.Count = item.Count;
                avgPoints.AvgRatingPonts = item.AvgRatingPonts;

            }
            //return details list with average rating for provided entity
            reviews.CustomerReviewList = rCollection;
            reviews.AvgPoints = avgPoints;
            reviews.prodId = prodId;//temp
            return reviews;

        }


        public DisplayReviewsViewModel GetReviews(long ownerID, REVIEWS reviewfor)
        {

            if (ownerID <= 0)
                return null;
            string prefix = string.Empty;
            //Review for order
            if (reviewfor.ToString() == "ORDER")
                prefix = "GBOD";
            //Review for Shop
            else if (reviewfor.ToString() == "SHOP")
                prefix = "GBMR";
            //Review for product
            else if (reviewfor.ToString() == "PRODUCT")
                prefix = "GBPR";

            DisplayReviewsViewModel reviews = new DisplayReviewsViewModel();
            List<CustomerReviewViewModel> rCollection = new List<CustomerReviewViewModel>();
            AverageRatingPointsViewModel avgPoints = new AverageRatingPointsViewModel();
            //get list of all reviews given by different customers
            //by AJ
            rCollection =
                (from CRF in db.CustomerRatingAndFeedbacks
                 join R in db.Ratings on CRF.RatingID equals R.ID
                 join BT in db.BusinessTypes on R.BusinessTypeID equals BT.ID
                 join PD in db.PersonalDetails on CRF.PersonalDetailID equals PD.ID
                 where BT.Prefix == prefix && CRF.OwnerID == ownerID
                 group CRF by new { CRF.PersonalDetailID } into a
                 select new CustomerReviewViewModel
                 {
                     ID = a.Max(x => x.ID),
                     Comment = a.Max(x => x.Feedback),
                     AvgPointsPerCustomer = a.Max(x => x.Point),
                     ReviewDate = a.Max(x => x.CreateDate),
                     CustomerName = a.Max(x => x.PersonalDetail.FirstName) + " " + a.Max(x => x.PersonalDetail.LastName),
                     CustomerPersonalDetailID = a.Max(x => x.PersonalDetailID),
                     //ReviewCount=2//  CRF.OwnerID
                 }).ToList();






            //        var duplicates = companies
            //.GroupBy(c => new { c.Name, c.Email })
            //.Select(g => new { Qty = g.Count(), First = g.OrderBy(c => c.Id).First() } )
            //.Select(p => new
            //    {
            //        Id = p.First.Id,
            //        Qty = p.Qty,
            //        Name = p.First.Name,
            //        Email = p.First.Email,
            //        Address = p.First.Address
            //    });


            //getAverage of all reviews given by different customers
            var avgRatingPoint = (from r in db.Ratings
                                  join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                  join cr in db.CustomerRatingAndFeedbacks on r.ID equals cr.RatingID
                                  join pd in db.PersonalDetails on cr.PersonalDetailID equals pd.ID
                                  where bt.Prefix == prefix && cr.OwnerID == ownerID
                                  group cr by new { cr.OwnerID } into temp
                                  select new AverageRatingPointsViewModel
                                  {
                                      AvgRatingPonts = temp.Average(x => x.Point),
                                      Count = temp.Count(),
                                      OwnerID = temp.Key.OwnerID

                                  }).ToList();

            foreach (var item in avgRatingPoint)
            {
                avgPoints.Count = item.Count;
                avgPoints.AvgRatingPonts = item.AvgRatingPonts;

            }
            //return details list with average rating for provided entity
            reviews.CustomerReviewList = rCollection;
            reviews.AvgPoints = avgPoints;
           
            return reviews;

        }
        //---Also used in API for Merchant App Comment By Mohit On 23/01/2016
        public decimal GetAvgRating(long ownerID, REVIEWS reviewfor)
        {
            decimal avgRating = 0;
            if (ownerID <= 0)
                return 0;
            string prefix = string.Empty;
            //Review for order
            if (reviewfor.ToString() == "ORDER")
                prefix = "GBOD";
            //Review for Shop
            else if (reviewfor.ToString() == "SHOP")
                prefix = "GBMR";
            //Review for Product
            else if (reviewfor.ToString() == "PRODUCT")
                prefix = "GBPR";
            //getAverage of all reviews given by different customers
            var avgRatingPoint = (from r in db.Ratings
                                  join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                                  join cr in db.CustomerRatingAndFeedbacks on r.ID equals cr.RatingID
                                  join pd in db.PersonalDetails on cr.PersonalDetailID equals pd.ID
                                  where bt.Prefix == "GBPR" && cr.OwnerID == ownerID
                                  group cr by new { cr.OwnerID } into temp
                                  select new AverageRatingPointsViewModel
                                  {
                                      AvgRatingPonts = temp.Average(x => x.Point),
                                      Count = temp.Count(),
                                      OwnerID = temp.Key.OwnerID

                                  }).ToList();
            return avgRating;
        }



    }
}
