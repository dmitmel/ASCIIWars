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

using Newtonsoft.Json;
using ASCIIWars.ConsoleGraphics;

namespace ASCIIWars.Game {
    /**
     * @short Содержит компоненты кампании.
     *
     * В представлении ассетов выглядит так:
     *
     * ```
     * assets/
     *     campaign-folder/      <--- папка, имя которой - ID кампании
     *         campaign.json     <--- файл с данными кампании
     *         items.json        <--- Файл с вещами
     *         situations.json   <--- Файл с ситуациями
     * ```
     *
     * @see SituationController
     * @see ItemContainer
     */
    public class Campaign {
        public string name;

        [JsonIgnore]
        public string id;
        [JsonIgnore]
        public SituationContainer situations;
        [JsonIgnore]
        public ItemContainer items;

        public static Campaign LoadFrom(AssetContainer assets, string id) {
            Campaign campaign = null;

            LoadingBar.Load(
                new Task($"Загружаем кампанию под ID... {id}", () => {
                    campaign = JsonConvert.DeserializeObject<Campaign>(assets[id]["campaign.json"].content);
                    campaign.id = id;
                }),
                new Task(() => $"Загружаем ситуации кампании '{campaign.name}'...", () => {
                    campaign.situations =
                        JsonConvert.DeserializeObject<SituationContainer>(assets[id]["situations.json"].content);
                }),
                new Task(() => $"Загружаем предметы кампании '{campaign.name}'...", () => {
                    campaign.items =
                        JsonConvert.DeserializeObject<ItemContainer>(assets[id]["items.json"].content);
                })
            );

            return campaign;
        }
    }
}
