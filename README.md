# SlotsSystem

Простая generic-система слотов для C#.

Один `Slot<TEntity>` хранит одну сущность. Сам слот не знает, какие enum-флаги используются для фильтрации: это знает `SlotDefinition<TEntity, TFlags>`.

Проверка состоит из двух частей:

```csharp
return Definition.Match(entity) && ParentMatcher(entity);
```

- `Definition.Match(entity)` — базовая проверка по enum-флагам.
- `ParentMatcher(entity)` — дополнительное runtime-условие владельца слота.

## Рюкзак

```csharp
var definition = new SlotDefinition<Item, ItemType>(
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
var definition = new SlotDefinition<Person, PassengerType>(
    PassengerType.Human | PassengerType.Animal,
    FlagMatchMode.Any);

var passengerSlot = new Slot<Person>(
    definition,
    person => person.Height < 2f);
```

`SlotDefinition` проверяет тип пассажира, а `ParentMatcher` — дополнительное правило конкретного владельца.

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

Один объект может одновременно иметь разные группы слотов с разными `TEntity` и разными enum-флагами.

## Точка расширения

`Slot<TEntity>` зависит только от:

```csharp
ISlotDefinition<TEntity>
```

Поэтому позже можно добавить другой способ matching-а без изменения самого `Slot<TEntity>`. Например type-based definition для `IWeapon` или `IItem`.
