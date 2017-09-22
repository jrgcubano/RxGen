using System;

namespace RxGen.Core.Utils
{
    public class RandomGenerator : IRandomGenerator
    {
        private Random _random;

        public RandomGenerator()
        {
            _random = new Random();
        }

        public double NextDouble() =>
            _random.NextDouble();
    }
}