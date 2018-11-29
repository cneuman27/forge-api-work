using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_OutputFormatType : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_OutputFormatType);
        }

        public override object ReadJson(
            JsonReader reader, 
            Type objectType, 
            object existingValue, 
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToOrderFormatType();
            }

            return Enums.E_OutputFormatType.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_OutputFormatType>)
            {
                List<Enums.E_OutputFormatType> list;

                list = value as List<Enums.E_OutputFormatType>;

                writer.WriteStartArray();

                foreach (Enums.E_OutputFormatType type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_OutputFormatType))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_OutputFormatType)value).ConvertToJSON());
        }
    }
}
