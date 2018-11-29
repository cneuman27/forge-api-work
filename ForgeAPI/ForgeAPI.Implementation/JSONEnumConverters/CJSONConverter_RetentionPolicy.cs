using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_RetentionPolicy : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_RetentionPolicy);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToRetentionPolicy();
            }

            return Enums.E_RetentionPolicy.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_RetentionPolicy>)
            {
                List<Enums.E_RetentionPolicy> list;

                list = value as List<Enums.E_RetentionPolicy>;

                writer.WriteStartArray();

                foreach (Enums.E_RetentionPolicy type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_RetentionPolicy))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_RetentionPolicy)value).ConvertToJSON());
        }
    }
}
