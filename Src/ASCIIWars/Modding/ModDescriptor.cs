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

using ASCIIWars.Game;
using Newtonsoft.Json;

namespace ASCIIWars.Modding {
    public abstract class ModDescriptor {
        public readonly ModInfo modInfo;

        AssetContainer _assets;
        public AssetContainer assets {
            get {
                if (_assets == null)
                    _assets = new AssetContainer($"{modInfo.modDirectory}/assets");
                return _assets;
            }
        }

        public ModDescriptor(ModInfo modInfo) {
            this.modInfo = modInfo;
        }

        public abstract void OnLoadBy(ModLoader modLoader);
        public abstract void OnDestroyBy(ModLoader modLoader);
    }

    public class ModInfo {
        public string name;
        public string description;
        public string id;
        public string modVersion;
        public string descriptorClass;
        public string dllName;
        public string projectURL;

        [JsonIgnore]
        public string modDirectory;
    }
}
