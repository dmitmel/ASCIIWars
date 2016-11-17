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

namespace ASCIIWars.Game {
    public static class InventoryController {
        public static void Start(Player player) {
            var state = InventoryState.ShowInventory;
            Item currentItem = null;
            Selectable currentSelectable = null;
            Consumable currentConsumable = null;

            while (state != InventoryState.Exit) {
                switch (state) {
                    case InventoryState.ShowInventory: {
                            var actions = new Dictionary<string, Action>();

                            IEnumerable<Selectable> selectables = player.inventory
                                                                        .Where(item => item is Selectable)
                                                                        .Cast<Selectable>();
                            foreach (var selectable in selectables) {
                                int count = selectables.Count(it => it.Equals(selectable));
                                actions[$"{selectable.name} ({count})"] = () => {
                                    currentSelectable = selectable;
                                    state = InventoryState.ShowSelectable;
                                };
                            }

                            IEnumerable<Consumable> consumables = player.inventory
                                                                        .Where(item => item is Consumable)
                                                                        .Cast<Consumable>();
                            foreach (var consumable in consumables) {
                                int count = consumables.Count(it => it.Equals(consumable));
                                actions[$"{consumable.name} ({count})"] = () => {
                                    currentConsumable = consumable;
                                    state = InventoryState.ShowConsumable;
                                };
                            }

                            IEnumerable<Item> items = player.inventory
                                                            .Where(item => !(item is Consumable || item is Selectable));
                            foreach (var item in items) {
                                int count = items.Count(it => it.Equals(item));
                                actions[$"{item.name} ({count})"] = () => {
                                    currentItem = item;
                                    state = InventoryState.ShowItem;
                                };
                            }

                            string title = $"Инвертарь (здоровье - {player.health}/{player.maxHealth}, " +
                                                      $"атака - {player.attack}, защита - {player.defense}, " +
                                                      $"монеты - {player.coins})";
                            actions["Назад"] = () => { state = InventoryState.Exit; };
                            MenuDrawer.Select(title, actions);
                        }
                        break;

                    case InventoryState.ShowItem: {
                            int count = player.inventory
                                              .Where(item => !(item is Consumable || item is Selectable))
                                              .Count(it => it.Equals(currentItem));
                            string itemTitle = $"{currentItem.name} ({count}) - {currentItem.description}";
                            MenuDrawer.Select(itemTitle, new Dictionary<string, Action> {
                                { "Выбросить", () => {
                                        player.RemoveItemFromInventory(currentItem);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Назад", () => { state = InventoryState.ShowInventory; } }
                            });
                        }
                        break;

                    case InventoryState.ShowSelectable: {
                            int count = player.inventory
                                              .Where(item => item is Selectable)
                                              .Count(it => it.Equals(currentSelectable));
                            string selectableTitle = (currentSelectable.isSelected) ?
                                $"{currentSelectable.name} ({count}) (Выбрано) - {currentSelectable.description}" :
                                $"{currentSelectable.name} ({count}) (Не выбрано) - {currentSelectable.description}";
                            MenuDrawer.Select(selectableTitle, new Dictionary<string, Action> {
                                { "Выбрать", () => {
                                        player.SelectItemInInventory(currentSelectable);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Отключить", () => {
                                        player.DeselectItemInInventory(currentSelectable);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Выбросить", () => {
                                        player.RemoveItemFromInventory(currentSelectable);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Назад", () => { state = InventoryState.ShowInventory; } }
                            });
                        }
                        break;

                    case InventoryState.ShowConsumable: {
                            int count = player.inventory
                                              .Where(item => item is Consumable)
                                              .Count(it => it.Equals(currentConsumable));
                            string consumableTitle = $"{currentConsumable.name} ({count}) - {currentConsumable.description}";
                            MenuDrawer.Select(consumableTitle, new Dictionary<string, Action> {
                                { "Использовать", () => {
                                        player.ConsumeItemFromInventory(currentConsumable);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Выбросить", () => {
                                        player.RemoveItemFromInventory(currentSelectable);
                                        state = InventoryState.ShowInventory;
                                    } },
                                { "Назад", () => { state = InventoryState.ShowInventory; } }
                            });
                        }
                        break;

                    case InventoryState.Exit:
                        break;
                }
            }
        }
    }

    enum InventoryState {
        ShowInventory, ShowItem, ShowSelectable, ShowConsumable, Exit
    }
}
