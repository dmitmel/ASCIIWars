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

    public class MerchantSituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController) {
            Player player = gameController.player;
            SituationContainer situations = gameController.situations;
            ItemContainer items = gameController.items;

            Merchant merchant = situations.GetMerchant(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            Dictionary<string, Action> actions = merchant.items.ToDictionary(
                (MerchantItem itemRef) => {
                    int itemPrice = itemRef.price;
                    Item item = items.GetByTypeAndID(itemRef.type, itemRef.id);
                    string title = $"Купить '{item.name}' ({item.description}) - {itemPrice} монет";

                    return Pair(title, Action(() => {
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
                    }));
                }
            );

            MenuDrawer.Select(
                $"Вы провстречали '{merchant.name}' ({merchant.type}). Ваши монеты: {player.coins}.",
                Dictionaries.Merge(
                    actions,
                    MakeDictionary(
                        Pair("Продолжить", Action(() => {
                            resultSituation = situations.RandomSituationByIDs(merchant.nextSituations);
                        })),
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
