namespace RxGen.People.Api
{
    /// <summary>
    /// Contains settings used by <see cref="RxGen.People.Api.GenPeopleApiClient" />
    /// </summary>
    public class GenPeopleApiSettings
    {
        /// <summary>
        /// Gets or sets the base URL for connection to the randomuser api.
        /// Default is http://api.randomuser.me
        /// </summary>
        public string ApiBaseUrl { get; set; } = "http://api.randomuser.me";
    }
}