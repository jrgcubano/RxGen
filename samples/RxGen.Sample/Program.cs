using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reactive.Linq;
using RxGen.People.Models;
using RxGen.Core.Handlers;
using RxGen.People;

namespace RxGen.Sample
{
    class Program
    {
        private static void ManualPaginationExample()
        {
            // configure seed
            var generator =
               RxGen.People(new LoggingHandler(new HttpClientHandler()))
                    .Seed("mypeople")
                    .Ammount(2)
                    .Nationality(Nationality.ES, Nationality.US)
                    .Gender(Gender.Female);

            // create an observable for page one and subscribe to it
            generator
               .Page(1)
               .AsObservable()
               .Subscribe(
                   (res) => Console.WriteLine($"Rx Users: {res.Result[0]}"),
                   (ex) => Console.WriteLine($"Rx Error: {ex.Message}")
            );
        }

        private static void GetDataUsingTaskExampleWithFireAndForget()
        {
            Task _ = GetDataUsingTaskExample();
        }

        private static async Task GetDataUsingTaskExample()
        {
            // var data = await new GenPeopleApi().GetUsers();

            // configure seed
            var generator =
                RxGen.People(new LoggingHandler(new HttpClientHandler()))
                     .Seed("mypeople")
                     .Ammount(2)
                     .Nationality(Nationality.ES, Nationality.US)
                     .Gender(Gender.Female);

            // get users using awaitable task
            var users = await generator.AsTask();
        }

        private static void RepeatForeverExample()
        {
            // configure seeder
            var generator = RxGen.People()
                .Ammount(5)
                .Nationality(Nationality.US)
                .Gender(Gender.Male)
                .AsObservable()
                .RepeatForever(TimeSpan.FromSeconds(2))
                .Subscribe(
                    (res) => System.Console.WriteLine($"Rx Page: {res.Info.Page}, Users: {res.Result[0]}"),
                    (ex) => System.Console.WriteLine($"Rx Error: {ex.Message}")
                );
        }


        private static void RepeatWithErrorsExample()
        {
            // configure seeder
            var generator = RxGen.People()
                .Ammount(5)
                .Nationality(Nationality.US)
                .Gender(Gender.Male)
                .AsObservable()
                .RepeatWithRandomError(0.1, TimeSpan.FromSeconds(2))
                .Subscribe(
                    (res) => System.Console.WriteLine($"Rx Page: {res.Info.Page}, Users: {res.Result[0]}"),
                    (ex) => System.Console.WriteLine($"Rx Error: {ex.Message}")
                );
        }

        private static void AutomaticPaginationExample()
        {
            // configure seeder
            var generator = RxGen.People()
                .Seed("mypeople")
                .Ammount(5)
                .Nationality(Nationality.US)
                .Gender(Gender.Male)
                .AutoPagination(5, TimeSpan.FromSeconds(2))
                .Subscribe(
                    (res) => System.Console.WriteLine($"Rx Page: {res.Info.Page}, Users: {res.Result[0]}"),
                    (ex) => System.Console.WriteLine($"Rx Error: {ex.Message}")
                );
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine("+--------- RxGen Samples --------+");
            System.Console.WriteLine("-- People --");
            RepeatForeverExample();
            // RepeatWithErrorsExample();
            // ManualPaginationExample();
            // GetDataUsingTaskExampleWithFireAndForget();
            // AutomaticPaginationExample();
            System.Console.WriteLine("Press ESC to stop");
            do
            {
                while (!System.Console.KeyAvailable)
                {
                    // Do something
                }
            } while (System.Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}