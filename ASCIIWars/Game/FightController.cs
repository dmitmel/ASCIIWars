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
using System.Collections.Generic;
using ASCIIWars.ConsoleGraphics;

namespace ASCIIWars.Game {
    public static class FightController {
        public static FightResult Fight(Player player, Enemy enemy) {
            FightResult? result = null;

            int enemyHealth = enemy.health;

            while (result == null) {
                string title = $"Игрок ({player.health}/{player.maxHealth} HP, {player.attack} ATK, {player.defense} DEF) " +
                    $"VS {enemy.name} ({enemyHealth}/{enemy.maxHealth} HP, {enemy.attack} ATK, {enemy.defense} DEF)";
                MenuDrawer.Select(title, new Dictionary<string, Action> {
                    { "Атаковать", () => {
                            enemyHealth -= ComputeRealDamage(player.attack, enemy.defense);
                            if (enemyHealth <= 0) {
                                result = FightResult.EnemyDied;
                                player.coins += enemy.coinsReward;
                                MenuDrawer.ShowInfoDialog($"{enemy.name} повержен! Вы получили за это " +
                                                          $"{enemy.coinsReward} монет.");
                            } else {
                                int damageToPlayer = ComputeRealDamage(enemy.attack, player.defense);
                                player.health -= damageToPlayer;
                                if (player.health <= 0) {
                                    result = FightResult.PlayerDied;
                                    MenuDrawer.ShowInfoDialog($"Вас убил {enemy.name}!");
                                } else {
                                    MenuDrawer.ShowInfoDialog($"{enemy.name} нанёс вам {damageToPlayer} урона.");
                                }
                            }
                        } },
                    { "Инвентарь", () => { InventoryController.Start(player); } },
                    { "Убежать", () => { result = FightResult.PlayerRanAway; } }
                });
            }

            return (FightResult) result;
        }

        static int ComputeRealDamage(int takenDamage, int defense) {
            return (int) Math.Ceiling(takenDamage - (defense * 0.5));
        }
    }

    public enum FightResult {
        EnemyDied, PlayerDied, PlayerRanAway
    }
}
