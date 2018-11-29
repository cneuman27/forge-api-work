using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Implementation.Extensions;

namespace ForgeAPI.Implementation.JSONEnumConverters
{
    public class CJSONConverter_TranslationStatus : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enums.E_TranslationStatus);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString().ConvertToTranslationStatus();
            }

            return Enums.E_TranslationStatus.Undefined;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value is List<Enums.E_TranslationStatus>)
            {
                List<Enums.E_TranslationStatus> list;

                list = value as List<Enums.E_TranslationStatus>;

                writer.WriteStartArray();

                foreach (Enums.E_TranslationStatus type in list)
                {
                    writer.WriteValue(type.ConvertToJSON());
                }

                writer.WriteEndArray();

                return;
            }

            if (!(value is Enums.E_TranslationStatus))
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue(((Enums.E_TranslationStatus)value).ConvertToJSON());
        }
    }
}
