using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RxGen.People.Models
{
    public class GenUser
    {
        [JsonProperty("gender", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }

        [JsonProperty("name")]
        public GenName Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("login")]
        public GenLogin Login { get; set; }

        [JsonProperty("id")]
        public GenId Id { get; set; }

        [JsonProperty("dob")]
        public DateTime? Dob { get; set; }

        [JsonProperty("registered")]
        public DateTime? Registered { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("cell")]
        public string Cell { get; set; }

        [JsonProperty("nat")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Nationality? Nationality { get; set; }

        [JsonProperty("location")]
        public GenLocation Location { get; set; }

        [JsonProperty("picture")]
        public GenPicture Picture { get; set; }

        public override string ToString() => $"{this.Name?.First}, {this.Name?.Last}";
    }
}