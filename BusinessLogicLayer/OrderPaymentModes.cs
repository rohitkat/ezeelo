using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 Handed over to Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
   public class OrderPaymentModes
    {

       EzeeloDBContext db = new EzeeloDBContext();
       /// <summary>
       /// Get Payment modes available by shop on paymentProcess form.
       /// </summary>
       /// <param name="shopList">List of Shop IDs included in order</param>
       /// <returns>List of payment modes</returns>
       public AvailablePaymentModesViewModel GetPaymentModes(ShopListViewModel shopListViewModel )
       {
           List<long> shopList = shopListViewModel.shopList; 
          
         var  payModeList = (from shopPay in db.ShopPaymentModes
                          join pay in db.PaymentModes on shopPay.PaymentModeID equals pay.ID
                          where shopList.Contains(shopPay.ShopID) && pay.IsActive == true && shopPay.IsActive == true  
                          select new 
                          {
                              PaymentMode = pay.Name                              

                          }).ToList();

           //var  GBPayModeList = (from pay in db.PaymentModes 
           //               where pay.IsActive == true 
           //               select new 
           //               {
           //                   PaymentMode = pay.Name                              

           //               }).ToList();

         AvailablePaymentModesViewModel paymodes = new AvailablePaymentModesViewModel();
         paymodes.AvailablePaymentModeList = payModeList.Select(x => x.PaymentMode).Distinct().ToList();
         //paymodes.GBPaymentModeList = GBPayModeList.Select(x => x.PaymentMode).ToList();

         return paymodes;
       }

    }
}
