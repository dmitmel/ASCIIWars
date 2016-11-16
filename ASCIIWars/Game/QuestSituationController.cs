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

using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Util;
using static ASCIIWars.Util.Dictionaries;
using static ASCIIWars.Util.Lambdas;

namespace ASCIIWars.Game {
    public class QuestSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController) {
            Player player = gameController.player;
            SituationContainer situations = gameController.situations;
            ItemContainer items = gameController.items;

            Quest quest = situations.GetQuest(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            Item questItem = items.ResolveReference(quest.questItem);

            MenuDrawer.Select(
                $"Вы встретили {quest.holderName}. Он/она/оно предлагает вам квест.",
                MakeDictionary(
                    Pair($"Дать {quest.questItem.count}x {questItem.name}", Action(() => {
                        int requiredCount = quest.questItem.count;
                        int realCount = player.CountOfItemInInventory(questItem);

                        if (realCount < requiredCount) {
                            MenuDrawer.ShowInfoDialog($"Вам нехватает {requiredCount - realCount}x {questItem.name}.");
                        } else {
                            player.RemoveItemFromInventory(questItem, requiredCount);
                            player.coins += quest.coinsReward;

                            foreach (ItemReference rewardItemRef in quest.itemsReward) {
                                Item rewardItem = items.ResolveReference(rewardItemRef);
                                player.AddItemToInventory(rewardItem, rewardItemRef.count);
                            }

                            string rewardItemsString = quest.itemsReward
                                                           .Map(items.ResolveReferenceAndCount)
                                                           .Map(TupleFunc((Item item, int count) => $"{count}x {item.name}"))
                                                           .Join(", ");
                            if (rewardItemsString.IsEmpty()) {
                                MenuDrawer.ShowInfoDialog($"Квест выполнен! Вы получили за квест {quest.coinsReward} монет.");
                            } else {
                                MenuDrawer.ShowInfoDialog($"Квест выполнен! Вы получили за квест {rewardItemsString} и " +
                                                          $"{quest.coinsReward} монет.");
                            }
                        }
                    })),
                    Pair("Уйти", Action(() => {
                        resultSituation = situations.RandomSituationByIDs(quest.situationsOnCancel);
                    })),
                    Pair("Инвентарь", Action(() => InventoryController.Start(player))),
                    Pair("Сохраниться", EmptyAction),
                    Pair("Выйти", ThrowAction(new GameOverException()))
                )
            );

            return resultSituation;
        }
    }
}
