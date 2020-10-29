using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models
{
    [Table("PartnerRequest")]
    public class PartnerRequest
    {
        [Key]
        public long ID { get; set; }
        public string PartnerCode { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public long StateID { get; set; }
        public string CityName { get; set; }
        public int InvestmentCapacity { get; set; }
        public string Space { get; set; }
        public string ExistingBusiness { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        [NotMapped]
        public SelectList InvestmentCapacityList { get; set; }
        [NotMapped]
        public SelectList StateList { get; set; }
        [NotMapped]
        public SelectList CityList { get; set; }
        [NotMapped]
        public SelectList AreaList { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string Area { get; set; }
        [NotMapped]
        public string Investment_Capacity { get; set; }
        [NotMapped]
        public string DisplayDate { get; set; }
        private string GenrateCode()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            DateTime CurDate = DateTime.Now.Date;
            long count = db.PartnerRequests.Where(p => p.RegistrationDateTime.Year == CurDate.Year && p.RegistrationDateTime.Month==CurDate.Month && p.RegistrationDateTime.Day == CurDate.Day).Count();
            count = (count == 0) ? 1 : (count + 1);
            string Counter = "000" + count.ToString();
            Counter = Counter.Substring(Counter.Length - 3, 3);
            return "PR" + CurDate.ToString("yy") + CurDate.ToString("MM") + CurDate.ToString("dd") + Counter;
        }

        public long SaveData(PartnerRequest obj)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            long IsSaved = -1;
            try
            {
                PartnerRequest objPartnerRequest = new PartnerRequest();
                objPartnerRequest.CityName = obj.CityName;
                objPartnerRequest.ContactNo = obj.ContactNo;
                objPartnerRequest.DeviceID = "X";
                objPartnerRequest.DeviceType = "X";
                objPartnerRequest.EmailId = obj.EmailId;
                objPartnerRequest.ExistingBusiness = obj.ExistingBusiness;
                objPartnerRequest.InvestmentCapacity = obj.InvestmentCapacity;
                objPartnerRequest.Name = obj.Name;
                objPartnerRequest.NetworkIP = obj.NetworkIP;
                objPartnerRequest.PartnerCode = GenrateCode();
                objPartnerRequest.RegistrationDateTime = DateTime.Now;
                objPartnerRequest.Space = obj.Space;
                objPartnerRequest.StateID = obj.StateID;
                db.PartnerRequests.Add(objPartnerRequest);
                db.SaveChanges();
                IsSaved = objPartnerRequest.ID;
            }
            catch
            {
                IsSaved = -1;
            }
            return IsSaved;
        }

        public List<PartnerRequest> GetPartnerRequestList()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            return db.PartnerRequests.OrderByDescending(p => p.RegistrationDateTime).ThenByDescending(p => p.ID).ToList();
        }

    }
    [Table("PaymentData")]
    public class PaymentData
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string TxnId { get; set; }
        public string City { get; set; }
        public int FranchiseId { get; set; }
        public long CartId { get; set; }
    }
}
