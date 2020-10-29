using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ContactUsController : ApiController
    {
        public object Get(int franchiseid)
        {
            object obj = new object();
            try
            {
                if (franchiseid == null || franchiseid <= 0)
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                HelpLine helpline = new HelpLine();
                helpline = HelpLineDesk.GetHelpLineNumber(franchiseid).FirstOrDefault();
                string HelpLineNo = helpline.HelpLineNumber;
                string EmailId = "support@ezeelo.com";
                string Address = "eZeelo Consumer Services Private Limited, Block 9, 4th floor, J B wing, N.M.C Complex, Mangawalri Bazar Road, Koradi Colony, Nagpur, Maharashtra, 440001.";
                string Img = rcKey.LOCALIMG_PATH + "Content/img/" + "contact.jpg";
                obj = new { Success = 1, Message = "Successfull.", data = new { HelpLineNo = HelpLineNo, EmailId = EmailId, Address = Address, Img = Img } };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
