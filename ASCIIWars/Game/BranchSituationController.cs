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

using System.Linq;
using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Util;
using static ASCIIWars.Util.Dictionaries;
using static ASCIIWars.Util.Lambdas;

namespace ASCIIWars.Game {
    public class BranchSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController) {
            Player player = gameController.player;
            SituationContainer situations = gameController.situations;

            Branch branch = situations.GetBranch(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            MenuDrawer.Select(
                branch.title,
                Dictionaries.Merge(
                    branch.nextSituations.ToDictionary(
                        (NextSituation nextSituation) => Pair(nextSituation.title, Action(() => {
                            resultSituation = situations.RandomSituationByIDs(nextSituation.IDs);
                        }))
                    ),
                    MakeDictionary(
                        Pair("Инвентарь", Action(() => InventoryController.Start(player))),
                        Pair("Сохраниться", EmptyAction),
                        Pair("Выйти", ThrowAction(new GameOverException()))
                    )
                )
            );

            return resultSituation;
        }
    }
}
