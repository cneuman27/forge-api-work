using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_Region : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_Region);
        }

        public override object ReadJson(
            JsonReader reader, 
            Type objectType, 
            object existingValue, 
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToRegion();
            }

            return Enums.E_AccessType.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_Region>)
            {
                List<Enums.E_Region> list;

                list = value as List<Enums.E_Region>;

                writer.WriteStartArray();

                foreach (Enums.E_Region type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_Region))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_Region)value).ConvertToJSON());
        }
    }
}
