using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Linq;
using Xunit;
using RxGen.People.Models;
using RxGen.AcceptanceTests.Core.Extensions;

namespace RxGen.AcceptanceTests
{
    public class RxPeopleTests
    {
        [Fact]
        public async Task Should_generate_data_with_basic_info_as_observable()
        {
            var numberOfUsers = 5;
            var gender = Gender.Female;
            var nationalities = new Nationality[] { Nationality.ES, Nationality.US };

            // configure request
            var generator = RxGen.People()
                .Ammount(numberOfUsers)
                .Gender(gender)
                .Nationality(nationalities[0], nationalities[1]);

            // observable
            var obs = generator.AsObservable();

            // wait when testing (a real scenario just need a subscribe)
            var response = await obs;

            response.Result.Count().Should().Be(numberOfUsers);
            response.Result.ForEach((user) =>
            {
                user.Gender.Should().Be(gender);
                nationalities.Should().Contain(user.Nationality.Value);
            });
        }

        [Fact]
        public async Task Should_generate_data_with_basic_info_as_task()
        {
            var numberOfUsers = 5;
            var gender = Gender.Female;
            var nationalities = new Nationality[] { Nationality.ES, Nationality.US };

            // configure request
            var generator = RxGen.People()
                .Ammount(numberOfUsers)
                .Gender(gender)
                .Nationality(nationalities[0], nationalities[1]);

            // task
            var task = generator.AsTask();

            var response = await task;

            response.Result.Count().Should().Be(numberOfUsers);
            response.Result.ForEach((user) =>
            {
                user.Gender.Should().Be(gender);
                nationalities.Should().Contain(user.Nationality.Value);
            });
        }

        [Theory]
        [InlineData("myseed")]
        [InlineData("yourseed")]
        [InlineData("ourseed")]
        // TODO (AutoFixture.AutoMoq (with random seed names))
        public async Task Should_paginate_and_retain_page_info_when_using_seed(string seedName)
        {
            var numberOfUsers = 5;
            var gender = Gender.Female;
            var nationalities = new Nationality[] { Nationality.ES, Nationality.US };

            // configure request
            var generator = RxGen.People()
                .Seed(seedName)
                .Ammount(numberOfUsers)
                .Gender(gender)
                .Nationality(nationalities[0], nationalities[1]);

            var pagesToNavigate = new int[] { 1, 2, 3, 2, 1, 5 };

            foreach(var page in pagesToNavigate)
            {
                // create observable for each page
                var source = generator
                    .Page(page)
                    .AsObservable();

                // wait when testing (a real scenario just need a subscribe)
                var response = await source;

                response.Info.Seed.Should().Be(seedName);
                response.Info.Page.Should().Be(page);
                response.Result.Count().Should().Be(numberOfUsers);
            }
        }
    }
}