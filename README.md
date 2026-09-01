# Core.SlotsSystem

Маленькая generic-система слотов для C# и Unity-проектов.

## Slot

`Slot<TEntity>` — основной элемент системы. Он хранит одну сущность `TEntity`.

Слот создаётся с definition, который описывает, какие сущности в него можно помещать:

```csharp
var definition = new TypeSlotDefinition<object, IItem>();
var slot = new Slot<object>(definition);
```

При необходимости при создании можно передать дополнительное условие владельца через `ParentMatcher`:

```csharp
var slot = new Slot<Item>(definition, item => backpack.HasSpaceFor(item));
```

При добавлении обычной сущности слот сначала проверяет её через `Definition.Match(entity)`, а затем, если задано, проверяет условие владельца через `ParentMatcher(entity)`.

Например, definition может проверить, что яблоко вообще является предметом, который подходит этому типу слота, а рюкзак после этого может проверить, хватает ли в нём места для этого яблока.

Сущность помещается в слот через `TrySet(...)`:

```csharp
var apple = new Item("Apple", ItemType.Food);

if(slot.TrySet(apple))
{
    // apple помещён в слот
}
```

Если сущность не подходит definition или не проходит условие владельца, `TrySet(...)` возвращает `false` и содержимое слота не меняется.

Передача `null` является обычной очисткой слота:

```csharp
slot.TrySet(null);
```

После успешного `TrySet(...)` событие `Changed` вызывается после обновления `Content`. Payload события — `SlotChangeData<TEntity>`, содержащий сам слот, предыдущее и текущее содержимое.

Коллекция слотов как отдельная сущность в репозитории не представлена. При необходимости потребитель сам хранит нужную ему коллекцию `Slot<TEntity>`.

## Definitions

`SlotDefinition<TEntity>` — класс описания типа слота.

В репозитории есть готовые варианты:

- `EnumSlotDefinition<TEntity, TFlags>` — принимает сущности по enum-флагам через `IFlagged<TFlags>` и режим `Any` или `All`.
- `TypeSlotDefinition<TEntity, TAccepted>` — принимает сущности, совместимые с указанным типом или интерфейсом.

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

Можно реализовать собственный `ISlotDefinition<TEntity>`, если нужен другой способ описания допустимого содержимого слота.

## Example

В репозитории есть примеры использования системы. Они помещены в папку `Example`.
