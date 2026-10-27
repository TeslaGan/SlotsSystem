# Core.SlotsSystem

Маленькая generic-система слотов для C# и Unity-проектов.

Система отвечает только за хранение сущностей в слотах, проверку допустимого содержимого, контролируемый внешний доступ и группировку слотов. Gameplay-смысл слота — топливо, экипировка, ингредиент, выход производства и т.д. — задаётся кодом проекта поверх Core.

## Slot

`Slot<TEntity>` — авторитетное хранилище одной сущности.

Слот создаётся с definition, который определяет допустимое содержимое:

```csharp
var definition = new TypeSlotDefinition<object, IItem>();
var slot = new Slot<object>(definition);
```

Проверить сущность можно через `CanAccept(...)`.

Изменить содержимое владелец слота может через `TrySet(...)`:

```csharp
slot.TrySet(item);
slot.TrySet(null);
```

`null` является обычной очисткой слота.

Если нужно получить заменённое содержимое:

```csharp
if(slot.TrySet(newItem, out Item previousItem))
{
    // previousItem — прежнее содержимое слота.
}
```

После успешного изменения вызывается `Changed`. Событие содержит `SlotChangeData<TEntity>` с read-only ссылкой на слот, предыдущим и текущим содержимым.

## Slot views and access

Core разделяет авторитетное изменение состояния и внешний доступ:

- `IReadOnlySlot<TEntity>` — чтение содержимого и `Changed`;
- `ISlot<TEntity>` — авторитетный доступ владельца через `TrySet(...)`;
- `ISlotAccess<TEntity>` — внешний доступ через `TryInsert(...)` и `TryExtract(...)`.

`SlotAccess<TEntity>` оборачивает существующий слот и может добавлять отдельные правила вставки и извлечения:

```csharp
var slot = new Slot<Item>(definition);
var access = new SlotAccess<Item>(
    slot,
    insertMatcher: item => CanInsert(item),
    extractMatcher: item => CanExtract(item));
```

Внешний код работает через access:

```csharp
access.TryInsert(item);
access.TryExtract(out Item extracted);
```

При этом владелец всё ещё может менять состояние напрямую:

```csharp
slot.TrySet(replacement);
```

Это позволяет, например, запретить внешнее извлечение топлива во время работы печи, но оставить самой печи возможность заменить уголь на золу.

## Containers

Одиночный слот не требует контейнера.

Если несколько слотов нужно рассматривать как одну группу, используются:

- `SlotContainer<TEntity>`;
- `SlotContainerAccess<TEntity>`;
- `IReadOnlySlotContainer<TEntity>`;
- `ISlotContainer<TEntity>`;
- `ISlotContainerAccess<TEntity>`.

Пример:

```csharp
var inventory = new SlotContainer<Item>();

inventory.Add(new Slot<Item>(definition));
inventory.Add(new Slot<Item>(definition));

inventory.Slots[0].TrySet(item);
```

Один и тот же физический слот можно одновременно включить в авторитетный контейнер и во внешний access-контейнер.

## Gameplay capabilities

`Core.SlotsSystem` не определяет интерфейсы вроде `IFuelConsumer`, `IEquipment`, `IManaStorage` или `IEquipment`.

Их рекомендуется объявлять в игровом слое и строить поверх slot/access API.

Например, предмет экипировки может предоставлять capability:

```csharp
public interface IEquippedTickable
{
    void Tick(IEquipmentContext context, float deltaTime);
}
```

Система экипировки работает только с read-only слотами:

```csharp
foreach(IReadOnlySlot<Item> slot in equipped.Slots)
{
    if(slot.Content is IEquippedTickable tickable)
        tickable.Tick(context, deltaTime);
}
```

А изменение экипировки может быть доступно внешним системам через `ISlotContainerAccess<TEntity>`.

Таким образом Core предоставляет механизм хранения и доступа, а проект самостоятельно определяет:

- какие capabilities есть у содержимого;
- какие группы слотов участвуют в конкретной механике;
- какой context получает активный предмет;
- какие операции доступны внешним системам.

В `EquipmentCapabilityExample.cs` показан пример с двумя кольцами и рюкзаком. Кольца получают tick через gameplay capability, а context предоставляет им только предметы рюкзака. Поэтому генератор и батарея маны работают с предметами рюкзака, но не балансируют ману между самими кольцами.

## Definitions

`SlotDefinition<TEntity>` — базовый definition слота.

Готовые варианты:

- `EnumSlotDefinition<TEntity, TFlags>` — проверка enum-флагов через `IFlagged<TFlags>` и режим `Any` или `All`;
- `TypeSlotDefinition<TEntity, TAccepted>` — проверка совместимости с типом или интерфейсом.

Пример enum definition:

```csharp
var definition = new EnumSlotDefinition<Item, ItemType>(
    ItemType.Food | ItemType.Weapon,
    FlagMatchMode.Any);
```

Пример type definition:

```csharp
var definition = new TypeSlotDefinition<object, IItem>();
```

Для собственных правил можно реализовать `ISlotDefinition<TEntity>`.

## Examples

Примеры находятся в папке `Example`:

- `CharacterEquipmentExample.cs` — независимые типизированные слоты экипировки;
- `BackpackExample.cs` — группа слотов через `SlotContainer<TEntity>`;
- `CarExample.cs` — внешний доступ и дополнительные правила через `SlotAccess<TEntity>`;
- `FurnaceExample.cs` — внешний access может быть заблокирован, пока владелец слота продолжает менять его состояние;
- `EquipmentCapabilityExample.cs` — gameplay capabilities, read-only группы и отдельные access-группы поверх SlotsSystem.
