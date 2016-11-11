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
using System.Linq;
using System.Collections.Generic;

namespace ASCIIWars.Game {
    /**
     * \short Представляет игрока.
     * 
     * У игрока есть такие свойства:
     * - здоровье (#health)
     *   - Ограничено максимальным значением (#maxHealth), которое может
     *     увеличиваться (с помощью #ASCIIWars.Game.HealthBooster)
     * - атака (#attack)
     * - защита (#defense)
     * - инвентарь (#inventory)
     *   - Размер ограничнен максимальным значением (#MAX_INVENTORY_SIZE),
     *     которое не может увеличиваться
     * - монеты (#coins)
     *   - В начале игры даётся определённое количиство монет
     *     (#STARTER_COINS)
     */
    public class Player {
        /// \short Максимальное значение здоровья по умолчанию.
        /// \see maxHealth
        const int DEFAULT_MAX_HEALTH = 100;
        /// \short Максимальный размер инвентаря.
        /// \see inventory
        public const int MAX_INVENTORY_SIZE = 20;
        /// \short Количество монет в начале игры.
        /// \see coins
        const int STARTER_COINS = 10;

        /// \short Максимальное здоровье. Может увеличиваться в течении
        ///        игры с помощью #ASCIIWars.Game.HealthBooster.
        /// \see health
        public int maxHealth = DEFAULT_MAX_HEALTH;
        /// \short Уровень здоровья. Может регенерироваться в
        ///        течении игры с помощью #ASCIIWars.Game.HealthPotion.
        public int health = DEFAULT_MAX_HEALTH;
        /// \short Уровень атаки. Может увеличиваться в течении игры
        ///        с помощью #ASCIIWars.Game.Weapon.
        public int attack;
        /// \short Уровень защиты. Может увеличиваться в течении игры
        ///        с помощью #ASCIIWars.Game.Armor.
        public int defense;
        /// Количество монет у игрока.
        public int coins = STARTER_COINS;
        /// \short Инвентарь. Имеет ограничение по размеру (#MAX_INVENTORY_SIZE).
        public List<Item> inventory = new List<Item>();

        /**
         * \short Добавляет предмет в инвентарь, с учётом максимального размера.
         * \see inventory
         * \see Item
         * \see Item.OnAddToPlayerInventory(Player)
         * \see RemoveItemFromInventory(Item)
         */
        public void AddItemToInventory(Item item) {
            if (inventory.Count < MAX_INVENTORY_SIZE) {
                inventory.Add(item);
                item.OnAddToPlayerInventory(this);
            } else {
                throw new FullInventoryException();
            }
        }

        /**
         * \short Удаляет предмет из инвентаря.
         * \see inventory
         * \see Item
         * \see Item.OnRemoveFromPlayerInventory(Player)
         * \see AddItemToInventory(Item)
         */
        public void RemoveItemFromInventory(Item item) {
            inventory.Remove(item);
            item.OnRemoveFromPlayerInventory(this);
        }

        /**
         * \short Выбирает выбираемый предмет в инвентаре, и отключает
         *        уже выбраный предмет этого типа.
         * \see inventory
         * \see Selectable
         * \see Selectable.OnSelectByPlayer(Player)
         * \see DeselectItemInInventory(Selectable)
         */
        public void SelectItemInInventory(Selectable selectable) {
            Selectable prevSelectedOfThisType = inventory.Where(it => it.GetType() == selectable.GetType())
                                                         .Cast<Selectable>()
                                                         .FirstOrDefault(it => it.isSelected);
            if (prevSelectedOfThisType != null) {
                prevSelectedOfThisType.isSelected = false;
                prevSelectedOfThisType.OnDeselectByPlayer(this);
            }

            selectable.isSelected = true;
            selectable.OnSelectByPlayer(this);
        }

        /**
         * \short Отключает выбираемый предмет в инвентаре.
         * \see inventory
         * \see Selectable
         * \see Selectable.OnDeselectByPlayer(Player)
         * \see SelectItemInInventory(Selectable)
         */
        public void DeselectItemInInventory(Selectable selectable) {
            selectable.isSelected = false;
            selectable.OnDeselectByPlayer(this);
        }

        /**
         * \short Использует и удаляет используемый предмет в инвентаре.
         * \see inventory
         * \see Consumable
         * \see Consumable.OnConsumeByPlayer(Player)
         */
        public void ConsumeItemFromInventory(Consumable consumable) {
            consumable.OnConsumeByPlayer(this);
            RemoveItemFromInventory(consumable);
        }

        /**
         * \short Возвращает число предметов определённого типа в инвентаре.
         * \see inventory
         */
        public int CountOfItemInInventory(Item item) {
            return inventory.Count(it => item.Equals(it));
        }
    }

    /**
     * Исключение, которое выбрасывается когда в инвентаре нехватает места.
     */
    public class FullInventoryException : Exception { }

    public abstract class Item {
        public readonly string id;
        public readonly string name;
        public readonly string description;

        public Item(string id, string name, string description) {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public abstract void OnAddToPlayerInventory(Player player);
        public abstract void OnRemoveFromPlayerInventory(Player player);

        public override int GetHashCode() {
            return id.GetHashCode() ^ name.GetHashCode() ^ description.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (!(obj is Item))
                return false;
            var other = obj as Item;
            return (id == other.id && name == other.name && description == other.description);
        }
    }

    public abstract class Consumable : Item {
        public Consumable(string id, string name, string description) : base(id, name, description) { }

        public abstract void OnConsumeByPlayer(Player player);
    }

    public abstract class Selectable : Item {
        public bool isSelected;

        public Selectable(string id, string name, string description) : base(id, name, description) { }

        public abstract void OnSelectByPlayer(Player player);
        public abstract void OnDeselectByPlayer(Player player);
    }

    public class Weapon : Selectable {
        public readonly int attack;

        public Weapon(string id, string name, string description, int attack) : base(id, name, $"Наносит {attack} урона. {description}") {
            this.attack = attack;
        }

        public override void OnAddToPlayerInventory(Player player) { }

        public override void OnSelectByPlayer(Player player) {
            player.attack = attack;
        }

        public override void OnDeselectByPlayer(Player player) {
            player.attack = 0;
        }

        public override void OnRemoveFromPlayerInventory(Player player) {
            if (isSelected)
                player.attack = 0;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ attack;
        }

        public override bool Equals(object obj) {
            if (!(obj is Weapon))
                return false;
            var other = obj as Weapon;
            return (id == other.id && name == other.name && description == other.description && attack == other.attack);
        }
    }

    public class Armor : Selectable {
        public readonly int defense;

        public Armor(string id, string name, string description, int defense) : base(id, name, $"Защищает на {defense} едениц. {description}") {
            this.defense = defense;
        }

        public override void OnAddToPlayerInventory(Player player) { }

        public override void OnSelectByPlayer(Player player) {
            player.defense = defense;
        }

        public override void OnDeselectByPlayer(Player player) {
            player.defense = 0;
        }

        public override void OnRemoveFromPlayerInventory(Player player) {
            if (isSelected)
                player.defense = 0;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ defense;
        }

        public override bool Equals(object obj) {
            if (!(obj is Armor))
                return false;
            var other = obj as Armor;
            return (id == other.id && name == other.name && description == other.description && defense == other.defense);
        }
    }

    public class HealthPotion : Consumable {
        public readonly int healthRestorage;

        public HealthPotion(string id, string name, int healthRestorage) : base(id, name, $"Регенерирует {healthRestorage} здоровья") {
            this.healthRestorage = healthRestorage;
        }

        public override void OnAddToPlayerInventory(Player player) { }

        public override void OnConsumeByPlayer(Player player) {
            player.health += healthRestorage;
            if (player.health > player.maxHealth)
                player.health = player.maxHealth;
        }

        public override void OnRemoveFromPlayerInventory(Player player) { }

        public override int GetHashCode() {
            return base.GetHashCode() ^ healthRestorage;
        }

        public override bool Equals(object obj) {
            if (!(obj is HealthPotion))
                return false;
            var other = obj as HealthPotion;
            return (id == other.id && name == other.name && description == other.description && 
                    healthRestorage == other.healthRestorage);
        }
    }

    public class HealthBooster : Consumable {
        public readonly int healthBoost;

        public HealthBooster(string id, string name, int healthBoost) : base(id, name, $"Добавляет {healthBoost} здоровья") {
            this.healthBoost = healthBoost;
        }

        public override void OnAddToPlayerInventory(Player player) { }

        public override void OnConsumeByPlayer(Player player) {
            player.maxHealth += healthBoost;
        }

        public override void OnRemoveFromPlayerInventory(Player player) { }

        public override int GetHashCode() {
            return base.GetHashCode() ^ healthBoost;
        }

        public override bool Equals(object obj) {
            if (!(obj is HealthBooster))
                return false;
            var other = obj as HealthBooster;
            return (id == other.id && name == other.name && description == other.description &&
                    healthBoost == other.healthBoost);
        }
    }

    public class Material : Item {
        public Material(string id, string name, string description) : base(id, name, description) { }

        public override void OnAddToPlayerInventory(Player player) { }

        public override void OnRemoveFromPlayerInventory(Player player) { }
    }
}
