
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

using System.Collections.Generic;
using System.IO;

namespace ASCIIWars.Game {
    /**
     * @short Контейнер ассетов. Хранит их по групам. В каждой
     *        группе находятся файлы ассетов.
     * 
     * В представлении файловой системы выглядит так:
     * ```
     * > assetsDirectory/
     * >     assetGroup1/
     * >         asset1.txt
     * >         asset2.json
     * >     assetGroup2/
     * >         asset1.png
     * >         asset2.mp3
     * ```
     * 
     * @see AssetGroup
     * @see Asset
     */
    public class AssetContainer {
        public readonly Dictionary<string, AssetGroup> assetGroups = new Dictionary<string, AssetGroup>();

        /// Инициализирует контейнер с указанем деректории, откуда
        /// грузить группы ассетов.
        public AssetContainer(string assetsDirectory) {
            string[] assetGroupsDirectories = Directory.GetDirectories(assetsDirectory);
            foreach (string assetGroupDirectory in assetGroupsDirectories) {
                string assetGroupName = Path.GetFileName(assetGroupDirectory);

                string[] assetFiles = Directory.GetFiles(assetGroupDirectory);
                var assets = new Dictionary<string, Asset>();

                foreach (string assetFile in assetFiles) {
                    string assetName = Path.GetFileName(assetFile);
                    assets[assetName] = new Asset(assetsDirectory, assetGroupName, assetName);
                }

                assetGroups[assetGroupName] = new AssetGroup(assetsDirectory, assetGroupName, assets);
            }
        }

        public AssetGroup this[string groupName] {
            get { return assetGroups[groupName]; }
            set { assetGroups[groupName] = value; }
        }

        public override string ToString() {
            return $"[AssetContainer: assetGroups={assetGroups}]";
        }
    }

    /**
     * @short Хранит файлы с ассетами.
     * 
     * В представлении файловой системы выглядит так:
     * ```
     * assetsDirectory/
     * >     assetGroup1/
     * >         asset1.txt
     * >         asset2.json
     *     assetGroup2/
     *         asset1.png
     *         asset2.mp3
     * ```
     * 
     * @see AssetContainer
     * @see Asset
     */
    public class AssetGroup {
        public readonly string assetsDirectory;
        public readonly string name;
        public readonly Dictionary<string, Asset> assets;

        public AssetGroup(string assetsDirectory, string name, Dictionary<string, Asset> assets) {
            this.assetsDirectory = assetsDirectory;
            this.name = name;
            this.assets = assets;
        }

        public Asset this[string assetName] {
            get { return assets[assetName]; }
            set { assets[assetName] = value; }
        }

        public override string ToString() {
            return $"[AssetGroup: name={name}, assets={assets}]";
        }
    }

    /**
     * @short Ассет. Хранит текст ассета.
     * 
     * @see AssetContainer
     * @see AssetGroup
     */
    public class Asset {
        public readonly string assetsDirectory;
        public readonly string groupName;
        public readonly string name;

        public readonly string path;
        public readonly string content;
        public readonly string[] lines;

        public Asset(string assetsDirectory, string groupName, string name) {
            this.assetsDirectory = assetsDirectory;
            this.groupName = groupName;
            this.name = name;

            path = $"{assetsDirectory}/{groupName}/{name}";
            content = File.ReadAllText(path);
            lines = File.ReadAllLines(path);
        }

        public override string ToString() {
            return $"[Asset: groupName={groupName}, name={name}]";
        }
    }
}