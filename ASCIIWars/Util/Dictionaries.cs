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
    public static class Dictionaries {
        public static Dictionary<K, V> Merge<K, V>(this Dictionary<K, V> a, Dictionary<K, V> b) {
            var result = new Dictionary<K, V>();
            a.ForEach(pair => result.Add(pair));
            b.ForEach(pair => result.Add(pair));
            return result;
        }

        public static void ForEach<K, V>(this Dictionary<K, V> dictionary,
                                                     Action<KeyValuePair<K, V>> action) {
            foreach (KeyValuePair<K, V> pair in dictionary)
                action.Invoke(pair);
        }

        public static KeyValuePair<K, V> Pair<K, V>(K key, V val) {
            return new KeyValuePair<K, V>(key, val);
        }

        public static Dictionary<K, V> MakeDictionary<K, V>(params KeyValuePair<K, V>[] pairs) {
            return pairs.ToList().ToDictionary(pair => pair);
        }

        public static void Add<K, V>(this Dictionary<K, V> dictionary, KeyValuePair<K, V> pair) {
            dictionary.Add(pair.Key, pair.Value);
        }
    }
}
