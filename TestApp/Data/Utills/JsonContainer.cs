using Newtonsoft.Json;

namespace TestApp.Data
{
    public class JsonContainer<T>
    {
        public JsonContainer() { }

        [JsonProperty("users")]
        public T Collection { get; set; }
    }
}
