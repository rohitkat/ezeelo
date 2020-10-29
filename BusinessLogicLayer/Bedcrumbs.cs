using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{

    public class Bedcrumbs
    {
        protected EzeeloDBContext db = new EzeeloDBContext();
        public string GetCategoryHierarchy(int catID)
        {
            //catID = 27;
            string bedCrumbString = string.Empty;
            /* var selectedCatName = from c in db.Categories where c.ID == catID select new { c.Name };
             
             bedCrumbString = catID +"$"+ selectedCatName.FirstOrDefault().Name;
             var firstLevelCatId = from Category in db.Categories
                                   where Category.ID ==
                            ((from Category0 in db.Categories
                              where
                                Category0.ID == catID
                              select new
                              {
                                  Category0.ParentCategoryID
                              }).FirstOrDefault().ParentCategoryID)
                                   select new
                                   {
                                       Category.Name,
                                       Category.ID
                                   };
            
             if (firstLevelCatId != null)
             {
                
                 foreach (var item in firstLevelCatId)
                 {
                     bedCrumbString = bedCrumbString + "$" + item.ID + "$" + item.Name;
                     var secondLevelCatId = from Category in db.Categories
                                           where Category.ID ==
                                    ((from Category0 in db.Categories
                                      where
                                        Category0.ID == item.ID
                                      select new
                                      {
                                          Category0.ParentCategoryID
                                      }).FirstOrDefault().ParentCategoryID)
                                           select new
                                           {
                                               Category.Name,
                                               Category.ID
                                           };
                     foreach (var item1 in secondLevelCatId)
                     {
                         bedCrumbString = bedCrumbString + "$" + item1.ID + "$" + item1.Name;
                     }
                 }
                 //var secondLevelCatId = from catDetails in db.Categories where catDetails.ID == firstLevelCatId select new { catDetails.ParentCategoryID };
             }*/

            //return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bedCrumbString.ToLower());

            ThreeCategories obj = new ThreeCategories();
            obj = (from n in db.Categories
                   join m in db.Categories on n.ID equals m.ParentCategoryID
                   join p in db.Categories on m.ID equals p.ParentCategoryID
                   where p.ID == catID
                   select new ThreeCategories
                   {
                       LevelOne = n.ID,
                       LevelOneName = n.Name,

                       LevelTwo = m.ID,
                       LevelTwoName = m.Name,

                       LevelThree = p.ID,
                       LevelThreeName = p.Name
                   }).FirstOrDefault();
            bedCrumbString = catID + "$" + obj.LevelThreeName + "$" + obj.LevelTwo + "$" + obj.LevelTwoName + "$" + obj.LevelOne + "$" + obj.LevelOneName;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(bedCrumbString.ToLower());
        }


        public class ThreeCategories
        {
            public Int32 LevelOne { get; set; }
            public Int32 LevelTwo { get; set; }
            public Int32 LevelThree { get; set; }

            public string LevelOneName { get; set; }
            public string LevelTwoName { get; set; }
            public string LevelThreeName { get; set; }

        }

    }
}
