using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
  public class BulkStockListViewModel
    {

      public List<BulkStockViewModel> listBulkStock { get; set; }      
      //To send images for all product stocks
      public UploadFilesViewModel UploadFiles{ get; set; }
      public HttpPostedFileBase MyFile { get; set; }
     
    }
}
