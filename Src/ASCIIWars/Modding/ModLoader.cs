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
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ASCIIWars.ConsoleGraphics;
using ASCIIWars.Util;
using System.Linq;

namespace ASCIIWars.Modding {
    public class ModLoader {
        public readonly List<ModDescriptor> modDescriptors = new List<ModDescriptor>();

        public void LoadMods(string modsDirectory) {
            List<string> modDirectories = Directory.GetDirectories(modsDirectory).ToList();
            LoadingBar.Load(modDirectories.Map(modDirectory => {
                ModInfo modInfo = null;
                Assembly assembly = null;

                return new Task($"Загружаем моды: мод из папки {modDirectory}", () => {
                    MyConsole.MoveCursorDown(3);
                    LoadingBar.Load(
                        new Task($"Загружаем JSON для мода из папки {modDirectory}...", () => {
                            string modJSON = $"{modDirectory}/mod.json";
                            modInfo = JsonConvert.DeserializeObject<ModInfo>(File.ReadAllText(modJSON));
                            modInfo.modDirectory = modDirectory;
                        }),
                        new Task(() => $"Загружаем код для мода '{modInfo.name}'", () => {
                            assembly = Assembly.LoadFrom($"{modDirectory}/{modInfo.dllName}");
                        }),
                        new Task(() => $"Инициализируем мод '{modInfo.name}...'", () => {
                            Type modDescriptorType = assembly.GetType(modInfo.descriptorClass);
                            ConstructorInfo modDescriptorConstructor = modDescriptorType.GetConstructor(new[] { typeof(ModInfo) });
                            var modDescriptor = (ModDescriptor) modDescriptorConstructor.Invoke(new[] { modInfo });
                            modDescriptors.Add(modDescriptor);

                            MyConsole.MoveCursorDown(3);
                            modDescriptor.OnLoadBy(this);
                            MyConsole.MoveCursorUp(3);
                        })
                    );
                    MyConsole.MoveCursorUp(3);
                });
            }));
        }

        public void DestroyMods() {
            modDescriptors.ForEach(modDescriptor => modDescriptor.OnDestroyBy(this));
        }
    }

    public class ModLoadException : Exception {
        public ModLoadException(string message) : base(message) { }
    }
}
