using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetlogoController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get(int areaid)
        {
            object obj = new object();
            try
            {
                if (areaid == null || areaid <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid AreaId.", data = string.Empty };
                }
                db.Configuration.ProxyCreationEnabled = false;
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                int franchiseId =   db.FranchiseLocations.Where(x => x.AreaID == areaid && x.IsActive).Select(x => x.FranchiseID ?? 0).FirstOrDefault();
                DesignBlockType blocktype = new DesignBlockType();
                blocktype = db.DesignBlockTypes.Where(x => x.Name == "Logo").FirstOrDefault();
                BlockItemsList blockitemlist = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == blocktype.ID && x.FranchiseID == franchiseId).FirstOrDefault();
                if (blockitemlist != null)
                {
                    string imagename = rcKey.HOME_IMAGE_HTTP + blockitemlist.ImageName;
                    //ImageDisplay.SetProductThumbPath((Int64)blockitemlist.ProductID, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    obj = new { Success = 1, Message = "Success.", data = imagename };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
