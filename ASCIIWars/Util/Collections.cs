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
using System.Linq;
using System.Collections.Generic;

namespace ASCIIWars.Util {
    public static class Collections {
        public static T RandomElement<T>(this List<T> list) {
            int index = GlobalRandom.Next(list.Count);
            return list[index];
        }

        public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T> enumerable,
                                                             Func<T, KeyValuePair<K, V>> action) {
            var result = new Dictionary<K, V>();
            foreach (T item in enumerable)
                result.Add(action.Invoke(item));
            return result;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list) {
            return list.Count() == 0;
        }
    }
}
