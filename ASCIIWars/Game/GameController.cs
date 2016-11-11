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

namespace ASCIIWars.Game {
    public class GameController {
        const string MAIN_SITUATION_NAME = "onEnter";

        public SituationContainer situations;
        public ItemContainer items;

        Player player = new Player();
        Situation currentSituation;
        bool gameIsRunning = true;

        public GameController(SituationContainer situations, ItemContainer items) {
            this.situations = situations;
            this.items = items;
            currentSituation = situations.GetSituation(MAIN_SITUATION_NAME);
        }

        public void Start() {
            try {
                while (gameIsRunning) {
                    Update();
                }
            } catch (GameOverException e) { }
        }

        void Update() {
            SituationController controller = SituationControllersRegistry.Get(currentSituation.type);
            currentSituation = controller.HandleSituation(currentSituation, this, player);
        }
    }

    public interface SituationController {
        Situation HandleSituation(Situation currentSituation, GameController gameController, Player player);
    }

    public static class SituationControllersRegistry {
        public static readonly Dictionary<string, SituationController> Controllers = new Dictionary<string, SituationController>();

        static SituationControllersRegistry() {
            Add("branch", new BranchSituationController());
            Add("enemy", new EnemySituationController());
            Add("merchant", new MerchantSituationController());
            Add("craftingPlace", new CraftingPlaceSituationController());
        }

        public static void Add(string id, SituationController controller) {
            Controllers[id] = controller;
        }

        public static SituationController Get(string id) {
            return Controllers[id];
        }
    }

    public class GameOverException : Exception { }

    public class BranchSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController, Player player) {
            Branch branch = gameController.situations.GetBranch(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            MenuDrawer.Select(
                branch.title,
                Dictionaries.Merge<string, Action>(
                    branch.nextSituations.ToDictionary(
                        (NextSituation nextSituation) => new KeyValuePair<string, Action>(nextSituation.title, () => {
                            string randomID = nextSituation.IDs.RandomElement();
                            resultSituation = gameController.situations.GetSituation(randomID);
                        })
                    ),
                    MakeDictionary(
                        Pair<string, Action>("Инвентарь", () => InventoryController.Start(player)),
                        Pair<string, Action>("Сохраниться", () => { }),
                        Pair<string, Action>("Выйти", () => { throw new GameOverException(); })
                    )
                )
            );

            return resultSituation;
        }
    }

    public class EnemySituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController, Player player) {
            Enemy enemy = gameController.situations.GetEnemy(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            MenuDrawer.Select(
                $"{enemy.name} встаёт на вашем пути. У него {enemy.health}/{enemy.maxHealth} здоровья, " +
                                                          $"{enemy.defense} защиты и {enemy.attack} атаки.",
                MakeDictionary<string, Action>(
                    Pair<string, Action>("Сразиться", () => {
                        FightResult result = FightController.Fight(player, enemy);
                        string randomID = "";

                        switch (result) {
                            case FightResult.PlayerRanAway:
                                randomID = enemy.situationsOnRunAway.RandomElement();
                                break;

                            case FightResult.PlayerDied:
                                throw new GameOverException();

                            case FightResult.EnemyDied:
                                randomID = enemy.situationsOnRunAway.RandomElement();
                                break;
                        }

                        resultSituation = gameController.situations.GetSituation(randomID);
                    }),
                    Pair<string, Action>("Убежать", () => {
                        string randomID = enemy.situationsOnRunAway.RandomElement();
                        resultSituation = gameController.situations.GetSituation(randomID);
                    }),
                    Pair<string, Action>("Инвентарь", () => InventoryController.Start(player)),
                    Pair<string, Action>("Сохраниться", () => { }),
                    Pair<string, Action>("Выйти", () => { throw new GameOverException(); })
                )
            );

            return resultSituation;
        }
    }

    public class MerchantSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController, Player player) {
            Merchant merchant = gameController.situations.GetMerchant(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            MenuDrawer.Select(
                $"Вы провстречали {merchant.name} ({merchant.type}). Ваши монеты: {player.coins}.",
                // Превращаем вещи торговца (List<MerchantItem>) в список действий меню (Dictionary<string, Action>)
                Dictionaries.Merge(
                    merchant.items.ToDictionary(
                        (MerchantItem itemRef) => {
                            int itemPrice = itemRef.price;
                            Item item = gameController.items.GetByTypeAndID(itemRef.type, itemRef.id);
                            string title = $"Купить '{item.name}' - {item.description} ({itemPrice} монет)";

                            return Pair<string, Action>(title, () => {
                                if (player.coins >= itemPrice) {
                                    if (player.inventory.Count < Player.MAX_INVENTORY_SIZE) {
                                        player.AddItemToInventory(item);
                                        player.coins -= itemPrice;
                                        MenuDrawer.ShowInfoDialog($"Вы купили '{item.name}'!");
                                    } else {
                                        MenuDrawer.ShowInfoDialog("Вам нехватает места в инвентаре!");
                                    }
                                } else {
                                    MenuDrawer.ShowInfoDialog($"Вам нехватает {itemPrice - player.coins} монет!");
                                }
                            });
                        }
                    ),
                    MakeDictionary<string, Action>(
                        Pair<string, Action>("Продолжить", () => {
                            string randomID = merchant.nextSituations.RandomElement();
                            resultSituation = gameController.situations.GetSituation(randomID);
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

    public class CraftingPlaceSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController, Player player) {
            CraftingPlace craftingPlace = gameController.situations.GetCraftingPlace(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            var actions = new Dictionary<string, Action>();

            foreach (Craft craft in craftingPlace.crafts) {
                IEnumerable<Tuple<Item, int>> ingredientsAndCounts =
                    craft.ingredients
                         .Select((CraftingItem it) => gameController.items.GetByTypeAndID(it.type, it.id))
                         .Zip(craft.ingredients, (Item ingredient, CraftingItem ingredientInfo) =>
                              Tuple.Create(ingredient, ingredientInfo.count));

                IEnumerable<string> ingredientsNames = ingredientsAndCounts
                    .Select((Tuple<Item, int> ingredientAndCount) =>
                            $"{ingredientAndCount.Item2}x {ingredientAndCount.Item1.name}");

                Item craftResult = gameController.items.GetByTypeAndID(craft.result.type, craft.result.id);
                var craftTitle = $"{string.Join(" + ", ingredientsNames)} => {craft.result.count}x {craftResult.name}";

                actions[craftTitle] = () => {
                    var missingIngredients = new List<Tuple<Item, int>>();

                    foreach (Tuple<Item, int> ingredientAndCount in ingredientsAndCounts) {
                        Item ingredient = ingredientAndCount.Item1;
                        int requiredCount = ingredientAndCount.Item2;
                        int realCount = player.CountOfItemInInventory(ingredient);

                        if (realCount < requiredCount)
                            missingIngredients.Add(Tuple.Create(ingredient, requiredCount - realCount));
                    }

                    if (missingIngredients.IsEmpty()) {
                        int ingredientsCount = ingredientsAndCounts
                            .Select((Tuple<Item, int> ingredientAndCount) => ingredientAndCount.Item2)
                            .Sum();
                        int inventorySizeAfterCraft = player.inventory.Count - ingredientsCount + craft.result.count;

                        if (inventorySizeAfterCraft < Player.MAX_INVENTORY_SIZE) {
                            foreach (Tuple<Item, int> ingredientAndCount in ingredientsAndCounts) {
                                Item ingredient = ingredientAndCount.Item1;
                                int count = ingredientAndCount.Item2;

                                for (int i = 0; i < count; i++)
                                    player.RemoveItemFromInventory(ingredient);
                            }

                            for (int i = 0; i < craft.result.count; i++)
                                player.AddItemToInventory(craftResult);

                            MenuDrawer.ShowInfoDialog($"Вы скрафтили {craft.result.count}x {craftResult.name}!");
                        } else {
                            MenuDrawer.ShowInfoDialog("Вам нехватает места в инвентаре!");
                        }
                    } else {
                        IEnumerable<string> missingIngredientsNames = missingIngredients
                            .Select(ingredientAndCount => $"{ingredientAndCount.Item2}x {ingredientAndCount.Item1.name}");
                        string missingIngredientsString = string.Join(", ", missingIngredientsNames);
                        MenuDrawer.ShowInfoDialog($"Вам нехватает {missingIngredientsString}!");
                    }
                };
            }

            MenuDrawer.Select(
                craftingPlace.name,
                Dictionaries.Merge<string, Action>(
                    actions,
                    MakeDictionary(
                        Pair<string, Action>("Продолжить", () => {
                            string randomID = craftingPlace.nextSituations.RandomElement();
                            resultSituation = gameController.situations.GetSituation(randomID);
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
