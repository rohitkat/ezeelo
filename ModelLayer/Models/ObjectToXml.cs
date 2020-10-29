using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace ModelLayer.Models
{
    public static class ObjectToXml
    {
        public static string GetXMLFromObject(object o)
        {

            XmlSerializer serializer = new XmlSerializer(o.GetType());
            StringWriter sw = new StringWriter();

            try
            {
                XmlTextWriter tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                //throw ex.Message;

            }
            return sw.ToString();
        }
    }
}
