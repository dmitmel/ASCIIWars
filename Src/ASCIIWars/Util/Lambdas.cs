//
//  Copyright (c) 2016  FederationOfCoders.org
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

namespace ASCIIWars.Util {
    public static class Lambdas {
        public static readonly Action EmptyAction = () => { };

        public static Action Action(Action action) {
            return action;
        }

        public static Action ThrowAction(Exception exception) {
            return () => { throw exception; };
        }

        public static Func<Tuple<T1, T2>, O> TupleFunc<T1, T2, O>(Func<T1, T2, O> func) {
            return (tuple) => func(tuple.Item1, tuple.Item2);
        }

        public static Action<Tuple<T1, T2>> TupleAction<T1, T2>(Action<T1, T2> action) {
            return (tuple) => action(tuple.Item1, tuple.Item2);
        }
    }
}
