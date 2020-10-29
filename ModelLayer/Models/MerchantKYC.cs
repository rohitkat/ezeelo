using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class MerchantKYC
    {
        public long ID { get; set; }
        public long MerchantID { get; set; }
       
        public string ShopEstablishmentCertificateImageUrl { get; set; }
        public string PanImageUrl { get; set; }
        public string GSTRegistrationImageUrl { get; set; }
        public string AddressProofUrl { get; set; }
        public string VisingCardImageUrl { get; set; }
        public string CancelledblankChequeImageUrl { get; set; }
        public string PhotoImageUrl { get; set; }
        public string BannerImageUrl { get; set; }

        public bool IsVerified { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreateBy { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

    }
}
