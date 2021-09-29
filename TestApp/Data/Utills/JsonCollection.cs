using Newtonsoft.Json;

namespace TestApp.Data
{
    public class JsonCollection
    {
        public JsonCollection() { }

        [JsonProperty("users")]
        public UserData[] Collection { get; set; }
    }
}
