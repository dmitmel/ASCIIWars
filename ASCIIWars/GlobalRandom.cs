//
//  Copyright (c) 2016  Drimachine.org
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

namespace ASCIIWars {
    /**
     * \short Глобальный контейнер для генератора рандамных чисел.
     * 
     * Базирован на функциями из стандартной библиотеки Haskell'я:
     * - `getStdGen` - http://hackage.haskell.org/package/random-1.1/docs/System-Random.html#v:getStdGen
     * - `newStdGen` - http://hackage.haskell.org/package/random-1.1/docs/System-Random.html#v:newStdGen
     */
    public static class GlobalRandom {
        public static Random RandomGenerator { get; private set; }

        static GlobalRandom() {
            RandomGenerator = new Random();
        }

        /// Создаёт новый генератор и возвращает его.
        public static Random NewRandomGenerator() {
            RandomGenerator = new Random();
            return RandomGenerator;
        }

        /// Создаёт новый генератор с сидом и возвращает его.
        public static Random NewRandomGenerator(int seed) {
            RandomGenerator = new Random(seed);
            return RandomGenerator;
        }

        /// То же самое, что и `GlobalRandom.RandomGenerator.Next(max)`.
        public static int Next(int maxValue) {
            return RandomGenerator.Next(maxValue);
        }

        /// То же самое, что и `GlobalRandom.RandomGenerator.Next(min, max)`.
        public static int Next(int minValue, int maxValue) {
            return RandomGenerator.Next(minValue, maxValue);
        }
    }
}
