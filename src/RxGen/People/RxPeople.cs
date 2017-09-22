using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RxGen.Core.Utils;
using RxGen.People.Api;
using RxGen.People.Models;

namespace RxGen.People
{
    /// <summary>
    /// Default rx people implementation relying on gen people api
    /// </summary>
    public class RxPeople : IRxPeople, IDisposable
    {
        private readonly IGenPeopleApiClient _peopleApiClient;
        private GenPeopleRequest _request;

        /// <summary>
        /// Initializes a new instance of <see cref="RxPeople"/> class
        /// </summary>
        /// <param name="peopleApi">The gen people api</param>
        public RxPeople(IGenPeopleApiClient peopleApiClient)
        {
            Guard.NotNull(peopleApiClient, nameof(peopleApiClient));

            _peopleApiClient = peopleApiClient;
            _request = new GenPeopleRequest();
        }

        public IRxPeople Ammount(int ammount)
        {
            _request.SetResults(ammount);
            return this;
        }

        public IRxPeople Gender(Gender gender)
        {
            _request.SetGender(gender);
            return this;
        }

        public IRxPeople Seed(string seed)
        {
            _request.SetSeed(seed);
            return this;
        }

        public IRxPeople Nationality(Nationality first, params Nationality[] nationalities)
        {
            _request.AddNationality(first);
            foreach (var nationality in nationalities)
                _request.AddNationality(nationality);
            return this;
        }

        public IRxPeople Page(int page)
        {
            _request.SetPage(page);
            return this;
        }

        public IRxPeople IncludeField(Field first, params Field[] fields)
        {
            _request.IncludeField(first);
            foreach (var field in fields)
                _request.IncludeField(field);
            return this;
        }

        public IRxPeople ExcludeField(Field first, params Field[] fields)
        {
            _request.ExcludeField(first);
            foreach (var field in fields)
                _request.ExcludeField(field);
            return this;
        }

        public GenPeopleRequest AsRequest() => _request;

        public async Task<GenPeopleResponse> AsTask() =>
            await _peopleApiClient.GetPeople(_request);

        public IObservable<GenPeopleResponse> AsObservable() =>
            Observable.FromAsync(AsTask);

        public IObservable<GenPeopleResponse> AsObservable(IScheduler scheduler)
        {
            Guard.NotNull(scheduler, nameof(scheduler));

            return Observable.FromAsync(AsTask, scheduler);
        }

        public void Dispose()
        {
            _peopleApiClient?.Dispose();
            _request = null;
        }
    }
}