using Newtonsoft.Json;

namespace RxGen.People.Models
{
    public class GenId
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}