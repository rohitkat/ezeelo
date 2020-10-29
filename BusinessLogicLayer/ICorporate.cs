using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public interface ICorporate
    {
        bool IsCorporate(Int64 ProductID);

        List<CorporateCustomerShippingDeliveryDetail> ShippingAddessDetail(List<ModelLayer.Models.ViewModel.CorporateCustomerShippingAddressViewModel> shippingAddress);

        List<CorporateOrderShippingFacilityDetail> FacilitiesAdd(List<ModelLayer.Models.ViewModel.CorporateFacilityDetailsViewModel> FacilityDetails);

    }
}
