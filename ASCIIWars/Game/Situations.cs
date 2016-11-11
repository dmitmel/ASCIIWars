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

using System.Collections.Generic;

namespace ASCIIWars.Game {
    public class Situation {
        public string type;
        public string objectID;
    }

    public class NextSituation {
        public string title;
        public List<string> IDs;
    }

    public class Branch {
        public string id;
        public string title;
        public List<NextSituation> nextSituations;
    }

    public class Enemy {
        public string id;
        public string name;
        public int defense;
        public int maxHealth;
        public int health;
        public int attack;
        public int coinsReward;
        public List<object> itemsReward;
        public List<string> situationsOnDefeat;
        public List<string> situationsOnRunAway;
    }

    public class MerchantItem {
        public string id;
        public string type;
        public int price;
    }

    public class Merchant {
        public string id;
        public string name;
        public string type;
        public List<MerchantItem> items;
        public List<string> nextSituations;
    }

    public class CraftingItem {
        public string id;
        public string type;
        public int count;
    }

    public class Craft {
        public List<CraftingItem> ingredients;
        public CraftingItem result;
    }

    public class CraftingPlace {
        public string name;
        public List<Craft> crafts;
        public List<string> nextSituations;
    }

    public class SituationContainer {
        public Dictionary<string, Situation> situations;
        public Dictionary<string, Branch> branches;
        public Dictionary<string, Enemy> enemies;
        public Dictionary<string, Merchant> merchants;
        public Dictionary<string, CraftingPlace> craftingPlaces;

        public Situation GetSituation(string id) {
            return situations[id];
        }

        public Branch GetBranch(string id) {
            return branches[id];
        }

        public Enemy GetEnemy(string id) {
            return enemies[id];
        }

        public Merchant GetMerchant(string id) {
            return merchants[id];
        }

        public CraftingPlace GetCraftingPlace(string id) {
            return craftingPlaces[id]; 
        }
    }
}
