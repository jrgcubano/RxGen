using System;
using FluentAssertions;
using RxGen.Core.Utils;
using RxGen.People.Models;
using Xunit;
using Moq;
using RxGen.People;
using RxGen.People.Api;
using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RxGen.Tests
{
    public class RxPeopleTests
    {
        private static readonly TimeSpan RepeatDelay = TimeSpan.FromMilliseconds(1000);
        private Mock<IRandomGenerator> _randomGenerator;
        private Mock<IGenPeopleApiClient> _peopleApiClient;

        private GenPeopleResponse CreateReponse(GenPeopleRequest request) =>
            new GenPeopleResponse
            {
                Info = new GenInfo { Page = request.Page, Seed = request.Seed },
                Result = new List<GenUser> { new GenUser { Email = "j@j.com" } }
            };

        public RxPeopleTests()
        {
            _randomGenerator = new Mock<IRandomGenerator>();
            var errorProbability = 0.6;
            _randomGenerator.Setup(x => x.NextDouble()).Returns(errorProbability);
            RandomGeneratorFactory.SetFactory(() => _randomGenerator.Object);

            _peopleApiClient = new Mock<IGenPeopleApiClient>();
            _peopleApiClient.Setup(x =>
                x.GetPeople(
                    It.IsAny<GenPeopleRequest>()))
                        .Returns((GenPeopleRequest req) =>
                            Task.FromResult(CreateReponse(req)));

        }

        [Fact]
        public void Should_throw_exception_when_configured_with_invalid_results()
        {
            var numberOfUsers = -5;

            // configure request
            Action action = () => RxGen.People()
                .Ammount(numberOfUsers);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_throw_exception_when_configured_with_invalid_page()
        {
            var numberOfUsers = 5;
            var page = -1;

            // configure request
            Action action = () => RxGen.People()
                .Ammount(numberOfUsers)
                .Page(page);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_throw_exception_when_configured_to_include_and_exclude_fields_at_the_same_time()
        {
            var numberOfUsers = 5;
            var includeFieldName = Field.Login;
            var excludeFieldName = Field.Nationality;

            // configure request
            Action incExcAction = () => RxGen.People()
                .Ammount(numberOfUsers)
                .IncludeField(includeFieldName)
                .ExcludeField(excludeFieldName);

            Action excIncAction = () => RxGen.People()
                .Ammount(numberOfUsers)
                .ExcludeField(excludeFieldName)
                .IncludeField(includeFieldName);

            incExcAction.ShouldThrow<ArgumentException>();
            excIncAction.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_build_valid_url_with_from_request()
        {
            var numberOfUsers = 5;
            var gender = Gender.Female;
            var genderAsString = gender.ToString().ToLower();
            var nationalities = new Nationality[] { Nationality.ES, Nationality.US };
            var nationalitiesAsString = string.Join(",", nationalities);
            var includeFields = new Field[] { Field.Nationality, Field.Email };
            var includeFieldsAsString = string.Join(",", includeFields);
            var seedName = "myseed";
            var page = 5;
            var expectedUrl = $"?page={page}&results={numberOfUsers}&seed={seedName}&gender={genderAsString}&inc={includeFieldsAsString}&nat={nationalitiesAsString}";

            // configure request
            var request = RxGen.People()
                .Ammount(numberOfUsers)
                .Gender(gender)
                .Nationality(nationalities[0], nationalities[1])
                .IncludeField(includeFields[0], includeFields[1])
                .Seed(seedName)
                .Page(page)
                .AsRequest();

            var url = request.AsUrl();

            url.Should().Be(expectedUrl);
        }

        [Fact]
        public async Task Should_paginate_using_seed_to_given_page()
        {
            var seedName = "mypeople";
            var generator = new RxPeople(_peopleApiClient.Object)
                .Seed(seedName)
                .Ammount(5);

            var pagesToNavigate = new int[] { 1, 2, 3, 2, 1, 5 };

            foreach(var page in pagesToNavigate)
            {
                var source = generator
                    .Page(page)
                    .AsObservable();

                var response = await source;
                response.Info.Seed.Should().Be(seedName);
                response.Info.Page.Should().Be(page);
            }
        }

        [Fact]
        public void Should_auto_paginate_with_paging_delay()
        {
            var generator = new RxPeople(_peopleApiClient.Object)
                .Seed("mypeople")
                .Ammount(5);
            var maxPages = 5;
            var currentPage = 0;

            var scheduler = new TestScheduler();
            var source = generator
                .AutoPagination(maxPages, RepeatDelay, scheduler)
                .Subscribe(
                    (res) => currentPage = res.Info.Page
                );

            currentPage.Should().Be(1);
            scheduler.AdvanceBy(RepeatDelay.Ticks);
            currentPage.Should().Be(2);
            // advance to last page
            scheduler.AdvanceBy(RepeatDelay.Ticks * (maxPages - 2));
            currentPage.Should().Be(maxPages);
            // should restart the cycle and decrease
            scheduler.AdvanceBy(RepeatDelay.Ticks);

            currentPage.Should().Be(maxPages - 1);
        }

        [Fact]
        public void Should_repeat_forever_getting_data()
        {
            var generator = new RxPeople(_peopleApiClient.Object)
                .Ammount(5);

            var scheduler = new TestScheduler();
            var counter = 0;
            var source = generator
                .AsObservable()
                .RepeatForever(RepeatDelay, scheduler)
                .Subscribe(
                    (res) => counter++,
                    (ex) => {}
                );

            counter.Should().Be(1);
            scheduler.AdvanceBy(RepeatDelay.Ticks);
            counter.Should().Be(2);
            scheduler.AdvanceBy(RepeatDelay.Ticks);
            counter.Should().Be(3);
        }

        [Fact]
        public void Should_not_throw_when_given_error_probability_greater_than_random()
        {
            var errorProbability = 0.4; // > 0.6
            var exceptionThrown = false;

            var scheduler = new TestScheduler();
            var generator = new RxPeople(_peopleApiClient.Object)
                .Ammount(5);

            var source = generator
                .AsObservable()
                .RepeatWithRandomError(errorProbability, RepeatDelay, scheduler)
                .Subscribe(
                    (res) => {},
                    (ex) => exceptionThrown = true
                );

            exceptionThrown.Should().BeFalse();
        }

        [Fact]
        public void Should_throw_when_given_error_probability_less_than_random()
        {
            var errorProbability = 0.7; // < 0.6
            var exceptionThrown = false;

            var scheduler = new TestScheduler();
            var generator = new RxPeople(_peopleApiClient.Object)
                .Ammount(5);

            var source = generator
                .AsObservable()
                .RepeatWithRandomError(errorProbability, RepeatDelay, scheduler)
                .Subscribe(
                    (res) => {},
                    (ex) => exceptionThrown = true
                );

            exceptionThrown.Should().BeTrue();
        }

    }
}
