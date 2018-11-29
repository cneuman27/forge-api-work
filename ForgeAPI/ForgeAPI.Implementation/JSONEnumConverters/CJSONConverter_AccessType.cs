using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_AccessType : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_AccessType);
        }

        public override object ReadJson(
            JsonReader reader, 
            Type objectType, 
            object existingValue, 
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToAccessType();
            }

            return Enums.E_AccessType.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer, 
            object value, 
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_AccessType>)
            {
                List<Enums.E_AccessType> list;

                list = value as List<Enums.E_AccessType>;

                writer.WriteStartArray();

                foreach (Enums.E_AccessType type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_AccessType))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_AccessType)value).ConvertToJSON());
        }
    }
}
