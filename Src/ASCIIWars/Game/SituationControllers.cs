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
using System.Collections.Generic;

namespace ASCIIWars.Game {
    /**
     * @short Контроллер для ситуаций. Используется, если тип ситуации
     *        совпадает с ID контроллера.
     * @see #HandleSituation
     * @see SituationControllersRegistry
     */
    public interface SituationController {
        /**
         * @short Используется для обработки какой-то ситуации.
         * @param currentSituation Текущая ситуация. Объект с точными
         *        данными о ситуации можно получить из #SituationController
         *        по #Situation.objectID.
         * @param gameController Ссылка на контроллер игры, использующий
         *        этот метод.
         * @returns Следуйщую ситуацию.
         */
        Situation HandleSituation(Situation currentSituation, GameController gameController);
    }

    /**
     * @short Содержит контроллеры ситуаций по ID'шникам.
     * 
     * По умолчанию добавляются стандартные контроллеры:
     * - `branch` -> #ASCIIWars.Game.BranchSituationController
     * - `enemy` -> #ASCIIWars.Game.EnemySituationController
     * - `merchant` -> #ASCIIWars.Game.MerchantSituationController
     * - `craftingPlace` -> #ASCIIWars.Game.CraftingPlaceSituationController
     * 
     * @see SitutationController
     */
    public static class SituationControllersRegistry {
        public static readonly Dictionary<string, SituationController> Controllers = new Dictionary<string, SituationController>();

        static SituationControllersRegistry() {
            Add("branch", new BranchSituationController());
            Add("enemy", new EnemySituationController());
            Add("merchant", new MerchantSituationController());
            Add("craftingPlace", new CraftingPlaceSituationController());
            Add("quest", new QuestSituationController());
        }

        /// То же самое, что и `Controllers[id] = controller`.
        public static void Add(string id, SituationController controller) {
            Controllers[id] = controller;
        }

        /// То же самое, что и `Controllers[id]`.
        public static SituationController Get(string id) {
            return Controllers[id];
        }
    }

    /**
     * @short Исключение, которое выбрасывается при окончании игры.
     */
    public class GameOverException : Exception { }
}
