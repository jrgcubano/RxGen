using FluentAssertions;
using RxGen.People.Api;
using Xunit;

namespace RxGen.Tests
{
    public class PeopleApiClientTests
    {
        [Fact]
        public void Should_person_api_url_as_base_endpoint_uri()
        {
            var apiSettings = new GenPeopleApiSettings();
            var apiClient = new GenPeopleApiClient(apiSettings.ApiBaseUrl);
            var httpClient = apiClient.GetClient();

            httpClient.BaseAddress.Should().Be($"{apiSettings.ApiBaseUrl}/");
        }
    }
}
