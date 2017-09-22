using System;
using System.Threading.Tasks;

namespace RxGen.People.Api
{
    /// <summary>
    /// Represents a gen people api client to get random users
    /// </summary>
    public interface IGenPeopleApiClient : IDisposable
    {
        /// <summary>
        /// Get random people
        /// </summary>
        /// <param name="request">people request params</param>
        /// <returns></returns>
        Task<GenPeopleResponse> GetPeople(GenPeopleRequest request);
    }
}