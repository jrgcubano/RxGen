using System.Net.Http;
using RxGen.Core.Utils;
using RxGen.People;
using RxGen.People.Api;

namespace RxGen
{
    /// <summary>
    /// Rx generator provider of random data builders
    /// </summary>
    public class RxGen
    {
        /// <summary>
        /// Default implementation of rx people generator using the gen people api
        /// </summary>
        /// <param name="messageHandler">configurable channel message handler</param>
        /// <returns></returns>
        public static IRxPeople People(HttpMessageHandler messageHandler = null) =>
            new RxPeople (
                new GenPeopleApiClient(new GenPeopleApiSettings().ApiBaseUrl, messageHandler));

        /// <summary>
        /// Default implementation of rx people generator using the gen people api with custom settings
        /// </summary>
        /// <param name="settings">people settings</param>
        /// <param name="messageHandler">configurable channel message handler</param>
        /// <returns></returns>
        public static IRxPeople People(GenPeopleApiSettings settings, HttpMessageHandler messageHandler = null)
        {
            Guard.NotNull(settings, nameof(settings));

            return new RxPeople (
                new GenPeopleApiClient(settings.ApiBaseUrl, messageHandler));
        }
    }
}