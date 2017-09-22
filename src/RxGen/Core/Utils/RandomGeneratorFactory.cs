using System;

namespace RxGen.Core.Utils
{
    public static class RandomGeneratorFactory
    {
        private static Func<IRandomGenerator> _innerFactory;

        public static void SetFactory(Func<IRandomGenerator> randomGeneratorFactory) =>
            _innerFactory = randomGeneratorFactory;

        public static IRandomGenerator CreateRandomGenerator() =>
            _innerFactory != null
                ? _innerFactory()
                : new RandomGenerator();
    }
}