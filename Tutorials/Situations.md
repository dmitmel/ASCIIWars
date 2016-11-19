# Ситуации

Как я уже говорил в
[README.md](https://github.com/fed-of-coders/ASCIIWars#readme),

> Все кампании в игре (да-да, тут есть кампании с разными сюжетами) полностью состоят
> из набора ситуаций, связаных между собой, и их можно представить в виде графа/схемы.

Так что в данном туториале речь пойдёт о ситуациях, их представлении и файле
`situations.json`.

## Значение

**Ситуация** - Какое-то событие или локация, где в данный момент находится игрок. И
игрока есть выбор действий, и каждое действие приводит либо к результату, либо к
случайно выбраной ситуации из возможных вариантов.

## JSON
JSON для ситуаций строго структурирован, так как читается с помощью
[Json.NET](http://www.newtonsoft.com/json). Так что, ситуации описываются так:
```json
{
    "situations": {
        "situationID": {"type": "situationType", "objectID":
            "ID объекта, который описывает ситуацию"}
    }
}
```

Существует несколько типов ситуаций:
- `branch` - сюжетная
- `enemy` - враг
- `merchant` - торговец
- `craftingPlace` - крафтильня
- `quest` - квест

Объекты, которые описывают ситуации, находятся после объявлений ситуаций:
```json
{
    "situations": {},
    "branches": {},
    "enemies": {},
    "merchants": {},
    "craftingPlaces": {},
    "quests": {}
}
```
Все объявления содержатся словарях, где ключи - это ID объектов, а значения - сами
объекты. ID главной ситуации - `onEnter`.

### Тип `branch`
Ситуации этого типа говорят игроку о чём-то и предлагают выбрать действие. К примеру:
```
 Вы видете лес и город. Куда пойти?
 > В лес
   В город
```

Вот пример JSON'а:
```json
{
    "situations": {
        "onEnter": {"type": "branch", "objectID": "onEnter"}
    },
    "branches": {
        "onEnter": {
            "title": "Вы видете лес и город. Куда пойти?",
            "nextSituations": [
                {"title": "В город", "IDs": ["town"]},
                {"title": "В лес", "IDs": ["forest"]}
            ]
        }
    }
}
```

### Тип `enemy`
Ситуации этого типа предлагают игроку сразиться с врагом, или убежать от врага. К
примеру:
```
 Волк встаёт на вашем пути. У него 50/50 здоровья, 0 защиты и 10 атаки.
 > Сразиться
   Убежать
```

Вот пример JSON'а:
```json
{
    "situations": {
        "fightWithWolf": {"type": "enemy", "objectID": "wolf"}
    },
    "enemies": {
        "wolf": {
            "name": "Волк",
            "maxHealth": 50,
            "health": 50,
            "attack": 10,
            "defense": 0,
            "coinsReward": 15,
            "drop": [],
            "situationsOnDefeat": ["walkInForest"],
            "situationsOnRunAway": ["walkInForest"]
        }
    },
}
```

Значение `drop` - это массив [ссылок на предметы](#Ссылки-на-предметы).

### Тип `merchant`
Ситуации этого типа представляют торговца. К примеру:
```
 Вы провстречали 'Кузнец' (Кузнец). Ваши монеты: 10.
 > Купить 'Деревянная броня' (Защищает на 10 едениц. Сделана из дерева. Осторожно с огнём!) - 5 монет
   Продолжить
```

Кнопка `Продолжить` позволяет уйти от торговца.

Вот пример JSON'а:
```json
{
    "situations": {
        "blacksmith": {"type": "merchant", "objectID": "blacksmith"}
    },
    "merchants": {
        "blacksmith": {
            "name": "Кузнец",
            "type": "Кузнец",
            "items": [
                {"id": "woodenArmor", "type": "armor", "price": 5}
            ],
            "nextSituations": ["marketSquare"]
        },
    }
}
```

### Тип `craftingPlace`
Ситуации этого типа представляют крафтильню (место для крафта). К примеру:
```
 Городская мастерская
 > 1x Дерево => 4x Палка
   2x Дерево + 1x Палка => 1x Деревянный меч
   Продолжить
```

Кнопка `Продолжить` позволяет уйти из крафтильни.

Вот пример JSON'а:
```json
{
    "situations": {
        "workshopInTown": {"type": "craftingPlace", "objectID": "workshopInTown"}
    },
    "craftingPlaces": {
        "workshopInTown": {
            "name": "Городская мастерская",
            "crafts": [
                {
                    "ingredients": [
                        {"id": "wood", "type": "material", "count": 1}
                    ],
                     "result": {"id": "stick", "type": "material", "count": 4}
                },
                {
                    "ingredients": [
                        {"id": "wood", "type": "material", "count": 2},
                        {"id": "stick", "type": "material", "count": 1}
                    ],
                    "result": {"id": "woodenSword", "type": "weapon", "count": 1}
                }
            ],
            "nextSituations": ["town"]
        }
    },
}
```

Ингредиенты и результаты крафта - это [ссылки на предметы](#Ссылки-на-предметы).

### Тип `quest`
Ситуации этого типа предлагают выполнить квест. Квесты заключаются в том, чтобы принести какие-то вещи. К примеру:
```
 Вы встретили Cool Quest Holder. Он/она/оно предлагает вам квест.
 > Дать 2x Маленькое зелье лечения
   Уйти
```

Вот пример JSON'а:
```json
{
    "situations": {
        "coolQuest": {"type": "quest", "objectID": "coolQuest"}
    },
    "quests": {
        "myCoolQuest": {
            "holderName": "Cool Quest Holder",
            "questItem": {"type": "healthPotion", "id": "smallHealthPotion", "count": 2},
            "coinsReward": 999,
            "itemsReward": [
                {"type": "armor", "id": "woodenArmor", "count": 1},
                {"type": "weapon", "id": "woodenSword", "count": 1}
            ],
            "situationsOnCancel": ["town"]
        }
    }
}
```

Вещь квеста и награды - это [ссылки на предметы](#Ссылки-на-предметы).

### Ссылки на предметы

Ссылка на предмет - это вот такой объект:
```json
{"id": "myWeapon", "type": "weapon", "count": 123}
```

## Код
Ситуации содержатся в контейнере -
[`SituationContainer`](https://github.com/fed-of-coders/ASCIIWars/blob/master/Src/ASCIIWars/Game/Situations.cs#L97)

Для управления ситуациями существуют
[контроллеры  ситуаций](https://github.com/fed-of-coders/ASCIIWars/blob/master/Src/ASCIIWars/Game/SituationControllers.cs#L26).
Каждый такой контроллер принимает текущею ситуацию, ссылку на
[`GameController`](https://github.com/fed-of-coders/ASCIIWars/blob/master/Src/ASCIIWars/Game/GameController.cs),
И возвращают новую ситуацию.

Такие контроллеры содержатся в регистре -
[`SituationControllersRegistry`](https://github.com/fed-of-coders/ASCIIWars/blob/master/Src/ASCIIWars/Game/SituationControllers.cs#L50).
Если тип ситуации совпадает с ID контроллера - он будет использован.
