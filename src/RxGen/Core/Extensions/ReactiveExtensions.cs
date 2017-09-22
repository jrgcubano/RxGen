using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using RxGen.Core.Exceptions;
using RxGen.Core.Utils;
using static RxGen.Core.Utils.RandomGeneratorFactory;

namespace System
{
    public static class ReactiveExtensions
    {
        public static readonly int DefaultDelayInMiliseconds = 1000;

        public static IObservable<T> InnerDelaySubscription<T>(this IObservable<T> @this, TimeSpan dueTime, IScheduler scheduler = null) =>
            scheduler == null ? @this.DelaySubscription(dueTime) : @this.DelaySubscription(dueTime, scheduler);

        /// <summary>
        /// Process generator as observable with delay between calls
        /// </summary>
        /// <param name="source">configured generator</param>
        /// <param name="pagingDelay">paging delay</param>
        /// <returns>observable of people response</returns>
        public static IObservable<T> RepeatForever<T>(this IObservable<T> @this, TimeSpan? repeatDelay = null, IScheduler scheduler = null)
        {
            var obs = @this
                .Concat(
                    Observable.Empty<T>()
                        .InnerDelaySubscription(
                            repeatDelay.HasValue
                                ? repeatDelay.Value
                                : TimeSpan.FromMilliseconds(DefaultDelayInMiliseconds),
                            scheduler))
                .Repeat();
            return obs;
        }

        /// <summary>
        /// Process generator as observable with random response errors between delay calls to api
        /// </summary>
        /// <param name="source">configured generator</param>
        /// <param name="errorProbability">error probability between 0 and 1</param>
        /// <param name="pagingDelay">paging delay</param>
        /// <returns>observable of people response</returns>
        public static IObservable<T> RepeatWithRandomError<T>(this IObservable<T> @this, double errorProbability = 0.1, TimeSpan? repeatDelay = null, IScheduler scheduler = null)
        {
            Guard.Between(errorProbability, 0, 1, nameof(errorProbability));

            var random = CreateRandomGenerator();
            var obs = @this
                .Do((res) =>
                {
                    var localRandom = random;
                    var genProbability = localRandom.NextDouble();
                    if (genProbability < errorProbability)
                        throw new RxGenApiException("FAKE random data api exception");
                })
                .Concat(
                    Observable.Empty<T>()
                        .InnerDelaySubscription(
                            repeatDelay.HasValue
                                ? repeatDelay.Value
                                : TimeSpan.FromMilliseconds(DefaultDelayInMiliseconds),
                            scheduler))
                .Repeat();
            return obs;
        }
    }
}