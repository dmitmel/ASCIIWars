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
using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Game;
using ASCIIWars.Modding;
using ASCIIWars.Util;
#if !DEBUG
using System.Linq;
using System.Threading;
#endif

namespace ASCIIWars {
    public class Application {
        public static AssetContainer Assets;
        public static List<Campaign> Campaigns = new List<Campaign>();
        public static ModLoader ModLoader = new ModLoader();

        public static void Main() {
            try {
                Console.CursorVisible = false;

                PrepareLoading();
                DoLoading();
#if !DEBUG
            FinishLoading();
#endif
                Console.WriteLine(Assets["asciiArts"]["title.txt"].content);

                MenuState state = MenuState.MainMenu;
                while (state != MenuState.ExitGame) {
                    switch (state) {
                        case MenuState.MainMenu:
                            MenuDrawer.Select(new Dictionary<string, Action> {
                                { "Новая Игра", () => { state = MenuState.NewGame; } },
                                { "Загрузить Игру", () => { state = MenuState.LoadGame; } },
                                { "Настройки", () => { state = MenuState.Settings; } },
                                { "Выйти Из Игры", () => { state = MenuState.ExitGame; } }
                            });
                            break;

                        case MenuState.NewGame:
                            Dictionary<string, Action> actions = Campaigns.ToDictionary(campaign => {
                                return new KeyValuePair<string, Action>(campaign.name, () => {
                                    var gameController = new GameController(campaign.situations, campaign.items);
                                    gameController.Start();
                                    // В GameController'е стоит свой game-loop, поэтому, 
                                    // когда он завершится (игрок выйдет из игры) - контроль вернётся сюда
                                    state = MenuState.MainMenu;
                                });
                            });

                            actions["Назад"] = () => { state = MenuState.MainMenu; };
                            MenuDrawer.Select("Новая Игра: Выбор кампании", actions);
                            break;

                        case MenuState.LoadGame:
                            MenuDrawer.Select("Загрузить игру", new Dictionary<string, Action> {
                                { "<Тут нет сохраений. Ты можешь начать новую игру>", () => { state = MenuState.NewGame; } },
                                { "Назад", () => { state = MenuState.MainMenu; } }
                            });
                            break;

                        case MenuState.Settings:
                            MenuDrawer.Select("Настройки", new Dictionary<string, Action> {
                                { "<Пока что, тут ничего нет>", () => { state = MenuState.Settings; } },
                                { "Назад", () => { state = MenuState.MainMenu; } }
                            });
                            break;

                        case MenuState.ExitGame:
                            break;
                    }
                }
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            } finally {
                Console.CursorVisible = true;
                ModLoader.DestroyMods();
            }
        }

        static void PrepareLoading() {
            Console.Clear();
#if !DEBUG
            Thread.Sleep(2500);
#endif
        }

        static void DoLoading() {
            Task[] realTasks = {
                new Task("Загружаем ассеты...", () => {
                    Assets = new AssetContainer("assets");
                }),
                new Task("Загружаем классическую кампанию...", () => {
                    MyConsole.MoveCursorDown(3);
                    Campaigns.Add(Campaign.LoadFrom(Assets, "classic-campaign"));
                    MyConsole.MoveCursorUp(3);
                }),
                new Task("Загружаем моды...", () => {
                    MyConsole.MoveCursorDown(3);
                    ModLoader.LoadMods("mods");
                    MyConsole.MoveCursorUp(3);
                })
            };

#if DEBUG
            LoadingBar.Load(realTasks);
#else
            Task[] funnyTasks = FunnyTasksLoader.Load();
            Task[] allTasks = realTasks.Concat(funnyTasks).ToArray();
            LoadingBar.Load(allTasks);
#endif
        }

#if !DEBUG
        static void FinishLoading() {
            Thread.Sleep(2500);
            Console.Write("Приготовтесь к...");
            Thread.Sleep(1000);
            MyConsole.ClearLine();
            Thread.Sleep(2000);
        }
#endif
    }

    enum MenuState {
        MainMenu, NewGame, LoadGame, Settings, ExitGame
    }
}
