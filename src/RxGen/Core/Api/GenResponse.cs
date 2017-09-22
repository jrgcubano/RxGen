using Newtonsoft.Json;

namespace RxGen.Core.Api
{
    public abstract class GenResponse : IResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        public bool IsError => !string.IsNullOrEmpty(Error);
    }
}