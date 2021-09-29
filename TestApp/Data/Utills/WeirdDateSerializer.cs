using Newtonsoft.Json;
using System;

namespace TestApp.Data
{
    public class WeirdDateSerializer : JsonConverter
    {
        public static DateTime ReadDate { get; set; }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime time = (DateTime)value;
            writer.WriteValue($"{time.Hour}-{time.Minute}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;
            string[] parts = s.Split('-');
            DateTime value = ReadDate.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));
            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }
    }
}
