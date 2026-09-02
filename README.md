# SlotsSystem

Простая generic-система слотов для C#.

Один `Slot<TEntity>` хранит одну сущность. Слот зависит только от контракта `ISlotDefinition<TEntity>` и поэтому не знает, как именно выполняется matching.

Проверка состоит из двух частей:

```csharp
return Definition.Match(entity) && ParentMatcher(entity);
```

- `Definition.Match(entity)` — базовая проверка definition.
- `ParentMatcher(entity)` — дополнительное runtime-условие владельца слота.

## Definitions

Базовый `SlotDefinition<TEntity>` принимает любую сущность:

```csharp
var anyDefinition = new SlotDefinition<Item>();
```

Enum-вариант фильтрует по флагам:

```csharp
var itemDefinition = new EnumSlotDefinition<Item, ItemType>(
    ItemType.Food | ItemType.Weapon,
    FlagMatchMode.Any);
```

Type-вариант фильтрует по типу или интерфейсу:

```csharp
var weaponDefinition = new TypeSlotDefinition<object, IWeapon>();
var itemDefinition = new TypeSlotDefinition<object, IItem>();
```

При этом `ISlotDefinition<TEntity>` остаётся общим контрактом для всех реализаций.

## Рюкзак

```csharp
var definition = new EnumSlotDefinition<Item, ItemType>(
    ItemType.Food | ItemType.Weapon,
    FlagMatchMode.Any);

var slots = new List<Slot<Item>>
{
    new(definition),
    new(definition)
};

slots[0].TrySet(new Item("Sword", ItemType.Weapon));
slots[1].TrySet(new Item("Apple", ItemType.Food));
```

```text
Backpack
├── Slot<Item> → Sword
└── Slot<Item> → Apple
```

## Автомобиль

У пассажиров свой enum, никак не связанный с `ItemType`:

```csharp
var definition = new EnumSlotDefinition<Person, PassengerType>(
    PassengerType.Human | PassengerType.Animal,
    FlagMatchMode.Any);

var passengerSlot = new Slot<Person>(
    definition,
    person => person.Height < 2f);
```

`EnumSlotDefinition` проверяет тип пассажира, а `ParentMatcher` — дополнительное правило конкретного владельца.

```csharp
passengerSlot.CanAccept(alice); // true
passengerSlot.CanAccept(bob);   // false
```

Полные примеры находятся в папке [`Example`](./Example).

## Несколько мест

Один `Slot` — одно место. Несколько мест — обычная коллекция:

```csharp
List<Slot<Item>> itemSlots;
List<Slot<Person>> passengerSlots;
```

Один объект может одновременно иметь разные группы слотов с разными `TEntity` и разными способами matching-а.
