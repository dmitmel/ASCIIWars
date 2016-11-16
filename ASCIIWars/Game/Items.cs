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

namespace ASCIIWars.Game {
    public class JSONArmor {
        public string name { get; set; }
        public string description { get; set; }
        public int defense { get; set; }
    }

    public class JSONWeapon {
        public string name { get; set; }
        public string description { get; set; }
        public int damage { get; set; }
    }

    public class JSONHealthPotion {
        public string name { get; set; }
        public int healthRestorage { get; set; }
    }

    public class JSONHealthBooster {
        public string name { get; set; }
        public int healthBoost { get; set; }
    }

    public class JSONMaterial {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ItemContainer {
        public Dictionary<string, JSONArmor> armors { get; set; }
        public Dictionary<string, JSONWeapon> weapons { get; set; }
        public Dictionary<string, JSONHealthPotion> healthPotions { get; set; }
        public Dictionary<string, JSONHealthBooster> healthBoosters { get; set; }
        public Dictionary<string, JSONMaterial> materials { get; set; }

        public Armor GetArmor(string id) {
            JSONArmor armorDefinition = armors[id];
            return new Armor(id, armorDefinition.name, armorDefinition.description, armorDefinition.defense);
        }

        public Weapon GetWeapon(string id) {
            JSONWeapon weaponDefinition = weapons[id];
            return new Weapon(id, weaponDefinition.name, weaponDefinition.description, weaponDefinition.damage);
        }

        public HealthPotion GetHealthPotion(string id) {
            JSONHealthPotion healthPotionDefinition = healthPotions[id];
            return new HealthPotion(id, healthPotionDefinition.name, healthPotionDefinition.healthRestorage);
        }

        public HealthBooster GetHealthBooster(string id) {
            JSONHealthBooster healthBoosterDefinition = healthBoosters[id];
            return new HealthBooster(id, healthBoosterDefinition.name, healthBoosterDefinition.healthBoost);
        }

        public Material GetMaterial(string id) {
            JSONMaterial materialDefinition = materials[id];
            return new Material(id, materialDefinition.name, materialDefinition.description);
        }

        public Tuple<Item, int> ResolveReferenceAndCount(ItemReference reference) {
            return Tuple.Create(ResolveReference(reference), reference.count);
        }

        public Item ResolveReference(ItemReference reference) {
            return reference.ReferencedItemIn(this);
        }

        public Item GetByTypeAndID(string itemType, string itemID) {
            switch (itemType) {
                case "armor":
                    return GetArmor(itemID);
                case "weapon":
                    return GetWeapon(itemID);
                case "healthPotion":
                    return GetHealthPotion(itemID);
                case "healthBooster":
                    return GetHealthBooster(itemID);
                case "material":
                    return GetMaterial(itemID);
                default:
                    throw new ArgumentException($"Unknown item type '{itemType}'");
            }
        }
    }
}
