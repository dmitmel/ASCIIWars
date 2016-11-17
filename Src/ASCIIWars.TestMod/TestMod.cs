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
using ASCIIWars.Game;
using ASCIIWars.Modding;

namespace ASCIIWars.TestMod {
    public class TestMod : ModDescriptor {
        public TestMod(ModInfo modInfo) : base(modInfo) { }

        public override void OnLoadBy(ModLoader modLoader) {
            Campaign campaign = Campaign.LoadFrom(assets, "my-campaign");
            Application.Campaigns.Add(campaign);
        }

        public override void OnDestroyBy(ModLoader modLoader) {
            
        }
    }
}
