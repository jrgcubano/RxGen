using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using RxGen.People.Api;
using static System.ReactiveExtensions;

namespace RxGen.People
{
    public static class RxPeopleExtensions
    {
        public const int DefaultMaxPages = 5;

        /// <summary>
        /// Process generator as observable with automatic pagination extension using observable repeat with paging delay
        ///     Cycle: start on page 1 and up until max page -> then down until 1 -> repeat...
        /// </summary>
        /// <param name="source">configured generator</param>
        /// <param name="maxPage">max page to repeat the pagination cycle up and down</param>
        /// <param name="pagingDelay">paging delay</param>
        /// <returns>observable of people response</returns>
        public static IObservable<GenPeopleResponse> AutoPagination(this IRxPeople @this, int maxPage = DefaultMaxPages, TimeSpan? pagingDelay = null, IScheduler scheduler = null)
        {
            int page = 1;
            bool isUp = true;
            var obs = @this
                .Page(page)
                .AsObservable()
                .Do((res) =>
                {
                    page = isUp ? page + 1 : page - 1;
                    if (page == maxPage || page == 1)
                        isUp = !isUp;
                    @this.Page(page);
                })
                .Concat(
                    Observable.Empty<GenPeopleResponse>()
                        .InnerDelaySubscription(
                            pagingDelay.HasValue
                                ? pagingDelay.Value
                                : TimeSpan.FromMilliseconds(DefaultDelayInMiliseconds), scheduler))
                .Repeat();
            return obs;
        }
    }
}