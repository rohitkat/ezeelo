using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace BusinessLogicLayer
{
    public class CorporateGift
    {
      // private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public ShopProductVarientViewModelCollection GetProductDetails(long itemId, long shopStockId, string fConnectionString)
        {
            ShopProductVarientViewModelCollection shopProductCollection = new ShopProductVarientViewModelCollection();
            try
            {
                ShoppingCartInitialization obj = new ShoppingCartInitialization();

                string[] prodDet = new string[] {shopStockId + "$" + itemId + "$" + 1 };
                
                shopProductCollection = obj.GetCookie(prodDet, fConnectionString);

            }
            catch (Exception)
            {
                
                throw;
            }
            return shopProductCollection;
        }


    }
}
