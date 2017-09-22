using System.Net.Http;
using System.Threading.Tasks;
using RxGen.Core.Api;

namespace RxGen.People.Api
{
    /// <summary>
    /// Gen people api client to get random users
    /// </summary>
    public class GenPeopleApiClient : BaseGenApiClient, IGenPeopleApiClient
    {
        /// <summary>
        /// Initializes a new intance of the <see cref="GenPeopleApiClient"/> class
        /// </summary>
        /// <param name="messageHandler">The channel message handler</param>
        public GenPeopleApiClient(string baseUrl, HttpMessageHandler messageHandler = null)
            : base(baseUrl, messageHandler)
        {
        }

        /// <summary>
        /// Get a collection of random users
        /// </summary>
        /// <param name="request">request params</param>
        /// <returns></returns>
        public async Task<GenPeopleResponse> GetPeople(GenPeopleRequest request)
        {
            var response = await GetAsync<GenPeopleResponse>(
                request.AsUrl()).ConfigureAwait(false);
            return response;
        }
    }
}