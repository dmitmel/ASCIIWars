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
using System.Linq;
using System.Collections.Generic;
using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Util;
using static ASCIIWars.Util.Dictionaries;
using static ASCIIWars.Util.Lambdas;

namespace ASCIIWars.Game {
    public class EnemySituationController : SituationController {
        public Situation HandleSituation(Situation currentSituation, GameController gameController) {
            Player player = gameController.player;
            SituationContainer situations = gameController.situations;
            ItemContainer items = gameController.items;

            Enemy enemy = situations.GetEnemy(currentSituation.objectID);
            Situation resultSituation = currentSituation;

            MenuDrawer.Select(
                $"{enemy.name} встаёт на вашем пути. У него {enemy.health}/{enemy.maxHealth} здоровья, " +
                                                          $"{enemy.defense} защиты и {enemy.attack} атаки.",
                MakeDictionary(
                    Pair("Сразиться", Action(() => {
                        int enemyHealth = enemy.health;

                        string resultSituationID = "";

                        while (resultSituationID.IsEmpty()) {
                            MenuDrawer.Select(
                                $"Игрок ({player.health}/{player.maxHealth} HP, {player.attack} ATK, {player.defense} DEF) " +
                      $"VS {enemy.name} ({enemyHealth}/{enemy.maxHealth} HP, {enemy.attack} ATK, {enemy.defense} DEF)",

                                MakeDictionary<string, Action>(
                                    Pair<string, Action>("Атаковать", () => {
                                        enemyHealth -= ComputeRealDamage(player.attack, enemy.defense);
                                        if (enemyHealth <= 0) {
                                            resultSituationID = enemy.situationsOnDefeat.RandomElement();
                                            player.coins += enemy.coinsReward;

                                            foreach (ItemReference dropRef in enemy.drop) {
                                                Item item = items.ResolveReference(dropRef);
                                                for (int i = 0; i < dropRef.count; i++)
                                                    player.AddItemToInventory(item);
                                            }

                                            if (enemy.drop.IsEmpty()) {
                                                MenuDrawer.ShowInfoDialog($"{enemy.name} повержен! Вы получили за это " +
                                                                          $"{enemy.coinsReward} монет.");
                                            } else {
                                                IEnumerable<string> dropNames = enemy.drop
                                                                                     .Select((ItemReference itemRef) => {
                                                                                         Item item = items.GetByTypeAndID(itemRef.type, itemRef.id);
                                                                                         return $"{itemRef.count}x {item.name}";
                                                                                     });
                                                string dropString = string.Join(", ", dropNames);
                                                MenuDrawer.ShowInfoDialog($"{enemy.name} повержен! Вы получили за это " +
                                                                          $"{dropString} и {enemy.coinsReward} монет.");
                                            }
                                        } else {
                                            int damageToPlayer = ComputeRealDamage(enemy.attack, player.defense);
                                            player.health -= damageToPlayer;
                                            if (player.health <= 0) {
                                                MenuDrawer.ShowInfoDialog($"Вас убил {enemy.name}!");
                                                throw new GameOverException();
                                            }

                                            MenuDrawer.ShowInfoDialog($"{enemy.name} нанёс вам {damageToPlayer} урона.");
                                        }
                                    }),
                                    Pair<string, Action>("Инвентарь", () => InventoryController.Start(player)),
                                    Pair<string, Action>("Убежать", () => {
                                        resultSituationID = enemy.situationsOnRunAway.RandomElement();
                                    })
                                )
                            );
                        }

                        resultSituation = situations.GetSituation(resultSituationID);
                    })),
                    Pair<string, Action>("Убежать", () => {
                        resultSituation = situations.RandomSituationByIDs(enemy.situationsOnRunAway);
                    }),
                    Pair<string, Action>("Инвентарь", () => InventoryController.Start(player)),
                    Pair<string, Action>("Сохраниться", () => { }),
                    Pair<string, Action>("Выйти", () => { throw new GameOverException(); })
                )
            );

            return resultSituation;
        }

        static int ComputeRealDamage(int takenDamage, int defense) {
            return (int) Math.Ceiling(takenDamage - (defense * 0.5));
        }
    }
}
