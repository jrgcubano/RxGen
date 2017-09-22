using System.Collections.Generic;
using Newtonsoft.Json;
using RxGen.Core.Api;
using RxGen.People.Models;

namespace RxGen.People.Api
{
    public class GenPeopleResponse : GenResponse
    {
        [JsonProperty("results")]
        public IList<GenUser> Result { get; set; }

        [JsonProperty("info")]
        public GenInfo Info { get; set; }
    }
}