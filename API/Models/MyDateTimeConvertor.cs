using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace API.Models
{
    /// <summary>
    /// Modify  Json request and response 
    /// </summary>
    public class MyDateTimeConvertor : DateTimeConverterBase
    {
        //Read Date from Json request
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }
        //Write Date in Json Response
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
           
            writer.WriteValue(((DateTime)value).ToString("MMM, dd yyyy hh:mm tt"));
        }
    }
}