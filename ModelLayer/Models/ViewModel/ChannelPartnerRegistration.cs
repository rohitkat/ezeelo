using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ModelLayer.Models.ViewModel
{
    public class ChannelPartnerRegistration
    {
        public long ID { get; set; }
        public UserLogin userLogin { get; set; }
        public PersonalDetail personalDetail { get; set; }
        public BusinessDetail businessDetail { get; set; }
        public ChannelPartner channelPartner { get; set; }
        public OwnerBank ownerBank { get; set; }
    }
}
