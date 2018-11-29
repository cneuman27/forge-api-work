using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_SVFViewType : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_SVFViewType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToSVFViewType();
            }
            
            return Enums.E_SVFViewType.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_SVFViewType>)
            {
                List<Enums.E_SVFViewType> list;

                list = value as List<Enums.E_SVFViewType>;

                writer.WriteStartArray();

                foreach (Enums.E_SVFViewType type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_SVFViewType))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_SVFViewType)value).ConvertToJSON());
        }
    }
}
