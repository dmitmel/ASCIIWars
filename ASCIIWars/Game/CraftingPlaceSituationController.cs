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
using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Util;
using static ASCIIWars.Util.Dictionaries;
using static ASCIIWars.Util.Lambdas;

namespace ASCIIWars.Game {

    public class CraftingPlaceSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController) {
            Player player = gameController.player;
            SituationContainer situations = gameController.situations;
            ItemContainer items = gameController.items;

            CraftingPlace craftingPlace = situations.GetCraftingPlace(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            var actions = craftingPlace.crafts.ToDictionary((Craft craft) => {
                List<Tuple<Item, int>> ingredientsAndCounts =
                    craft.ingredients.Map(items.ResolveReferenceAndCount);

                List<string> ingredientsNames = ingredientsAndCounts
                    .Map(TupleFunc((Item ingredient, int count) => $"{count}x {ingredient.name}"));

                Item craftResult = items.ResolveReference(craft.result);
                var craftTitle = $"{ingredientsNames.Join(" + ")} => {craft.result.count}x {craftResult.name}";

                return Pair(craftTitle, Action(() => {
                    List<Tuple<Item, int>> missingIngredients = ingredientsAndCounts
                        .Map(TupleFunc((Item ingredient, int requiredCount) => {
                            int realCount = player.CountOfItemInInventory(ingredient);
                            return Tuple.Create(ingredient, requiredCount - realCount);
                        }))
                        .Filter(TupleFunc((Item ingredient, int missingCount) => missingCount > 0));

                    if (missingIngredients.IsEmpty()) {
                        int ingredientsCount = ingredientsAndCounts
                            .Map(TupleFunc((Item ingredient, int count) => count))
                            .Sum();
                        int inventorySizeAfterCraft = player.inventory.Count - ingredientsCount + craft.result.count;

                        if (inventorySizeAfterCraft < Player.MAX_INVENTORY_SIZE) {
                            ingredientsAndCounts.ForEach(TupleAction<Item, int>(player.RemoveItemFromInventory));
                            player.AddItemToInventory(craftResult, craft.result.count);
                            MenuDrawer.ShowInfoDialog($"Вы скрафтили {craft.result.count}x {craftResult.name}!");
                        } else {
                            MenuDrawer.ShowInfoDialog("Вам нехватает места в инвентаре!");
                        }
                    } else {
                        string missingIngredientsString = missingIngredients
                            .Map(TupleFunc((Item ingredient, int count) => $"{count}x {ingredient.name}"))
                            .Join(", ");
                        MenuDrawer.ShowInfoDialog($"Вам нехватает {missingIngredientsString}!");
                    }
                }));
            });

            MenuDrawer.Select(
                craftingPlace.name,
                Dictionaries.Merge(
                    actions,
                    MakeDictionary(
                        Pair<string, Action>("Продолжить", () => {
                            resultSituation = situations.RandomSituationByIDs(craftingPlace.nextSituations);
                        }),
                        Pair<string, Action>("Инвентарь", () => InventoryController.Start(player)),
                        Pair<string, Action>("Сохраниться", () => { }),
                        Pair<string, Action>("Выйти", () => { throw new GameOverException(); })
                    )
                )
            );

            return resultSituation;
        }
    }
}
