using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Add Compare Prodcut Class
    /// </summary>
    public class CompareProduct : ICheckItem, IGetDetail
    {

        /// <summary>
        /// Status is 1 := Product Added All Ok  
        /// Status is 2 := Product Already Added
        /// Status is 3 := Product limit exceed
        /// Status is 4 := Product Category Not Similer
        /// status is 5 := Unable to Add Product
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected EzeeloDBContext db = new EzeeloDBContext();

        /// <summary>
        /// To set the Product Detail in the cookies
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int SetCookies(long itemID, string ImgPath, string ProductName, long ShopStockID)
        {
            CampareProductViewModel CampareProductViewModel = new CampareProductViewModel();
            if (IsCookiesNULL())
            {
                return this.IsAddedSuccessFully(itemID, ImgPath, ProductName, ShopStockID);
            }
            else
            {
                if (this.IsAlreadyAdded(ShopStockID))
                {
                    return 2;
                }
                else if (this.IsSameCategory(itemID))
                {
                    return this.IsAddedSuccessFully(itemID, ImgPath, ProductName, ShopStockID);
                }
                else
                {
                    return 4;
                }
            }
            return 0;
        }

        public int SetCookies(long itemID)
        {
            CampareProductViewModel CampareProductViewModel = new CampareProductViewModel();
            cookiesDataProperties obj = new cookiesDataProperties();
            obj.ShopStockID = 0;

            obj = getCookiesDetail(itemID);
           

            if (obj != null)
            {
                obj.ImgPath = getImagePath(itemID, obj.ColourName);
                if (IsCookiesNULL())
                {
                    return this.IsAddedSuccessFully(obj.itemID, obj.ImgPath, obj.ProductName, obj.ShopStockID);
                }
                else
                {
                    if (this.IsAlreadyAdded(obj.itemID))
                    {
                        return 2;
                    }
                    else if (this.IsSameCategory(itemID))
                    {
                        return this.IsAddedSuccessFully(obj.itemID, obj.ImgPath, obj.ProductName, obj.ShopStockID);
                    }
                    else
                    {
                        return 4;
                    }
                }
            }
            else
            {
                if (this.IsAlreadyAdded(itemID))
                {
                    return 2;
                }
                else if (!this.IsSameCategory(itemID))
                {
                    return 4;

                }
                return 5;
            }
            return 0;
        }

        /// <summary>
        /// To Check Item is Already Exist in cookies or not
        /// </summary>
        /// <param name="ShopStockID">ShopStockID</param>
        /// <returns>bool</returns>
        public bool IsAlreadyAdded(long itemID)
        {
            string cook = HttpContext.Current.Request.Cookies["ProductID"].Value;

            /* To Check wether the item is already Exists or NOT */
            if (cook.Contains(itemID.ToString()) && cook != string.Empty)
            {
                return true;
            }
            return false ;

        }

        /// <summary>
        /// To Check wether the old and new product category is sme it not 
        /// </summary>
        /// <param name="ProdcutID">productID</param>
        /// <returns>bool</returns>
        public bool IsSameCategory(long ProdcutID)
        {
            string cook = HttpContext.Current.Request.Cookies["ProductID"].Value;
            if (cook != string.Empty)
            {
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[0].Split('$');

                Int64 OldCatID = Convert.ToInt64(ItemID_Split[0].ToString());
                long Cate_ID = db.Products.Where(x => x.ID == OldCatID).FirstOrDefault().CategoryID;

                if (db.Products.Where(x => x.ID == ProdcutID).FirstOrDefault().CategoryID == Cate_ID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// To Add ShopStockID in the cookies 
        /// </summary>
        /// <param name="ProductID">Product ID</param>
        /// <returns>bool</returns>
        public int IsAddedSuccessFully(long ProductID, string ImgPath, string ProductName, long ShopStockID)
        {
            try
            {   
              
                if (IsCookiesNULL())
                {
                    HttpContext.Current.Response.Cookies["ProductID"].Value = ProductID.ToString() + "$" + ImgPath.ToString() + "$" + ProductName + "$" + ShopStockID;

                    HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["ProductID"]);

                    HttpContext.Current.Response.Cookies["ProductID"].Expires = System.DateTime.Now.AddDays(30);

                    return 1;
                }
                else
                {

                    HttpContext.Current.Response.Cookies["ProductID"].Value = HttpContext.Current.Request.Cookies["ProductID"].Value + "," + ProductID.ToString() + "$" + ImgPath.ToString() + "$" + ProductName + "$" + ShopStockID;

                    HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["ProductID"]);

                    HttpContext.Current.Response.Cookies["ProductID"].Expires = System.DateTime.Now.AddDays(30);

                    return 1; 

                }
            }
            catch (Exception ex)
            {
                return 5;
            }
        }

        /// <summary>
        /// To check wether the cookies is empty 
        /// </summary>
        /// <returns>bool</returns>
        public bool IsCookiesNULL()
        {
            if (HttpContext.Current.Request.Cookies["CompareCount"] == null && HttpContext.Current.Request.Cookies["ProductID"] == null ||
                HttpContext.Current.Request.Cookies["CompareCount"].Value =="0" && HttpContext.Current.Request.Cookies["ProductID"].Value == string.Empty)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete Record From Cookies on Basis of ShopStockID
        /// </summary>
        /// <param name="itemID">itemID</param>
        /// <returns></returns>
        public int DeleteItemCookie(string itemID)
        {
            try
            {
                string P = HttpContext.Current.Request.Cookies["ProductID"].Value;
                string[] individualItemCookie = P.Split(',');
                HttpCookie ProductID = new HttpCookie("ProductID");
                HttpCookie CompareCount = new HttpCookie("CompareCount");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["ProductID"] != null)
                {
                    ProductID.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(ProductID);

                    CompareCount.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(CompareCount);


                }
                if (ProductID.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("ProductID");
                    HttpContext.Current.Request.Cookies.Remove("CompareCount");
                }

                //Reinitialize cookie
                int CompareCountlst = 0;
                
                //exclude item cookie which want to delete
                foreach (string item in individualItemCookie)
                {
                    if (item != string.Empty)
                    {
                        string[] individualItemDetailsCookie = item.Split('$');
                        if (HttpContext.Current.Request.Cookies["ProductID"] == null && individualItemDetailsCookie[0] != itemID)
                        {
                            HttpContext.Current.Response.Cookies["ProductID"].Value = individualItemDetailsCookie[0].ToString().Trim() + "$"
                                + individualItemDetailsCookie[1].ToString().Trim() + "$" + individualItemDetailsCookie[2].ToString().Trim()
                                + "$" + individualItemDetailsCookie[3].ToString().Trim();
                            HttpContext.Current.Response.Cookies.Add(ProductID);
                            ProductID.Expires = System.DateTime.Now.AddDays(30);
                            ++CompareCountlst;
                        }
                        else if (individualItemDetailsCookie[0] != itemID)
                        {
                            HttpContext.Current.Response.Cookies["ProductID"].Value = HttpContext.Current.Request.Cookies["ProductID"].Value
                                + "," + individualItemDetailsCookie[0].ToString().Trim() + "$" + individualItemDetailsCookie[1].ToString().Trim() + "$"
                                + individualItemDetailsCookie[2].ToString().Trim() + "$" + individualItemDetailsCookie[3].ToString().Trim();
                            HttpContext.Current.Response.AppendCookie(ProductID);
                            ProductID.Expires = System.DateTime.Now.AddDays(30);
                            ++CompareCountlst;
                        }
                        
                    }
                }

                HttpContext.Current.Response.Cookies["CompareCount"].Value = (CompareCountlst).ToString();
                HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["CompareCount"]);
                HttpContext.Current.Response.Cookies["CompareCount"].Expires = System.DateTime.Now.AddDays(30);

                HttpContext.Current.Response.Cookies.Add(ProductID);
                ProductID.Expires = System.DateTime.Now.AddDays(30);

                return 1;
            }
            catch (MyException myEx)
            {
                //throw new BusinessLogicLayer.MyException("[CompareProduct][DeleteItemCookie]", "Can't delete Compare item from cookie!" + Environment.NewLine + myEx.Message);
                return 0;
            }
            catch (Exception ex)
            {
                //throw new BusinessLogicLayer.MyException("[CompareProduct][DeleteItemCookie]", "Can't delete Compare item from cookie!" + Environment.NewLine + ex.Message);
                return 0;
            }
        }


        public static int CookiesCount()
        {
            CompareProduct obj = new CompareProduct();
            if (!obj.IsCookiesNULL())
            {
                string cook = HttpContext.Current.Request.Cookies["ProductID"].Value;
                if (cook != string.Empty)
                {
                    string[] Cookies_Split = cook.Split(',');

                    return Cookies_Split.Count();
                }
            }

            return 0;
        }

        public string getImagePath(long productId, string strColour)
        {
            return ImageDisplay.SetProductThumbPath(productId, strColour, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
        }
        
        public cookiesDataProperties getCookiesDetail(long ProductID)
        {
            cookiesDataProperties obj = new cookiesDataProperties();
            if (!IsCookiesNULL())
            {
                string cook = HttpContext.Current.Request.Cookies["ProductID"].Value;
                if (HttpContext.Current.Request.Cookies["CompareCount"].Value == "1")
                {
                    if (cook != string.Empty)
                    {
                        string[] Cookies_Split = cook.Split(',');
                        string[] ItemID_Split = Cookies_Split[0].Split('$');
                        long itemID = Convert.ToInt64(ItemID_Split[0].ToString());


                        obj = (from pv in db.ProductVarients
                               join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                               join p in db.Products on pv.ProductID equals p.ID
                               join SP in db.ShopProducts on p.ID equals SP.ProductID
                               join S in db.Shops on SP.ShopID equals S.ID
                               join BD in db.BusinessDetails on S.BusinessDetailID equals BD.ID
                               join UL in db.UserLogins on BD.UserLoginID equals UL.ID
                               where pv.IsActive == true
                               where ss.StockStatus == true //&& ss.ID != ShopStockID
                               where ss.IsActive == true
                               where p.IsActive == true
                               //where SP.IsActive == true
                               where S.IsActive == true
                               where BD.IsActive == true
                               where UL.IsLocked == false
                               where p.ID == ProductID && p.ID != itemID
                               select new cookiesDataProperties
                               {
                                   ColourID = pv.ColorID,
                                   ColourName = pv.Color.Name,
                                   ShopStockID = ss.ID,
                                   ProductName = p.Name,
                                   itemID = p.ID
                               }).FirstOrDefault();
                    }
                }
                else if (HttpContext.Current.Request.Cookies["CompareCount"].Value == "2")
                {
                    if (cook != string.Empty)
                    {
                        string[] Cookies_Split = cook.Split(',');
                        string[] ItemID_Split = Cookies_Split[0].Split('$');
                        string[] ItemID_Split1 = Cookies_Split[1].Split('$');
                        long itemID1 = Convert.ToInt64(ItemID_Split[0].ToString());
                        long itemID2 = Convert.ToInt64(ItemID_Split1[0].ToString());


                        obj = (from pv in db.ProductVarients
                               join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                               join p in db.Products on pv.ProductID equals p.ID
                               join SP in db.ShopProducts on p.ID equals SP.ProductID
                               join S in db.Shops on SP.ShopID equals S.ID
                               join BD in db.BusinessDetails on S.BusinessDetailID equals BD.ID
                               join UL in db.UserLogins on BD.UserLoginID equals UL.ID
                               where pv.IsActive == true
                               where ss.StockStatus == true //&& ss.ID != ShopStockID1 && ss.ID != ShopStockID2
                               where ss.IsActive == true
                               where p.IsActive == true
                               //where SP.IsActive == true
                               where S.IsActive == true
                               where BD.IsActive == true
                               where UL.IsLocked == false
                               where p.ID == ProductID && p.ID != itemID1 && p.ID != itemID2
                               select new cookiesDataProperties
                               {
                                   ColourID = pv.ColorID,
                                   ColourName = pv.Color.Name,
                                   ShopStockID = ss.ID,
                                   ProductName = p.Name,
                                   itemID = p.ID
                               }).FirstOrDefault();
                    }
                }
            }
            else
            {
                obj = (from pv in db.ProductVarients
                       join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                       join p in db.Products on pv.ProductID equals p.ID
                       join SP in db.ShopProducts on p.ID equals SP.ProductID
                       join S in db.Shops on SP.ShopID equals S.ID
                       join BD in db.BusinessDetails on S.BusinessDetailID equals BD.ID
                       join UL in db.UserLogins on BD.UserLoginID equals UL.ID
                       where pv.IsActive == true
                       where ss.StockStatus == true //&& ss.ID != ShopStockID
                       where ss.IsActive == true
                       where p.IsActive == true
                       //where SP.IsActive == true
                       where S.IsActive == true
                       where BD.IsActive == true
                       where UL.IsLocked == false
                       where p.ID == ProductID
                       select new cookiesDataProperties
                       {
                           ColourID = pv.ColorID,
                           ColourName = pv.Color.Name,
                           ShopStockID = ss.ID,
                           ProductName = p.Name,
                           itemID = p.ID
                       }).FirstOrDefault();
            }

            return obj;
        }
 	
    }

    /// <summary>
    /// Interface to interact with system
    /// </summary>
    public interface ICheckItem
    {
        /// <summary>
        /// To Check Item is Already Exist in cookies or not
        /// </summary>
        /// <param name="ProdcutID">itemid</param>
        /// <returns>bool</returns>
        bool IsAlreadyAdded(Int64 ProdcutID);

        /// <summary>
        /// To Check wether the old and new product category is sme it not 
        /// </summary>
        /// <param name="ProdcutID">productID</param>
        /// <returns>bool</returns>
        bool IsSameCategory(Int64 ProdcutID);
        /// <summary>
        /// To Add product id in the cookies 
        /// </summary>
        /// <param name="ProductID">Product ID</param>
        /// <returns>bool</returns>
        int IsAddedSuccessFully(Int64 ProductID, string ImgPath, string ProductName, long ShopStockID);

        /// <summary>
        /// To check wether the cookies is empty 
        /// </summary>
        /// <returns>bool</returns>
        bool IsCookiesNULL();

    }

    public interface IGetDetail
    {
        cookiesDataProperties getCookiesDetail(long ProductID);
        string getImagePath(long productId, string strColour);
      
    }

    public class cookiesDataProperties
    {
        public long itemID { get; set; }
        public string ImgPath { get; set; }
        public string ProductName { get; set; }
        public long ShopStockID { get; set; }

        public long ColourID { get; set; }
        public string ColourName { get; set; }
    }
}
