# SlotsSystem

Small, reusable slot system for Unity and C# projects.

SlotsSystem provides a compact set of primitives for:

- storing one entity in a slot;
- validating accepted content;
- exposing read-only or controlled external access;
- grouping slots into containers;
- reacting to content changes.

The package intentionally does not define gameplay concepts such as inventories, equipment, fuel, crafting, or item behavior. Those semantics belong to the consuming project.

## Core API

### Slot

`Slot<TEntity>` is the authoritative storage for one entity.

```csharp
var definition = new TypeSlotDefinition<object, IItem>();
var slot = new Slot<object>(definition);

slot.TrySet(item);
slot.TrySet(null);
```

Use `CanAccept(...)` to validate content before changing the slot.

To retrieve replaced content:

```csharp
if(slot.TrySet(newItem, out object previousItem))
{
    // previousItem contains the old value.
}
```

Successful changes raise `Changed` with the previous and current content.

### Access

SlotsSystem separates authoritative mutation from external access:

- `IReadOnlySlot<TEntity>` — read content and observe changes;
- `ISlot<TEntity>` — owner-level mutation through `TrySet(...)`;
- `ISlotAccess<TEntity>` — controlled insertion and extraction.

`SlotAccess<TEntity>` can apply independent insert and extract rules without restricting the slot owner.

```csharp
var access = new SlotAccess<Item>(
    slot,
    insertMatcher: item => CanInsert(item),
    extractMatcher: item => CanExtract(item));

access.TryInsert(item);
access.TryExtract(out Item extracted);
```

### Containers

Slots can be used independently or grouped with:

- `SlotContainer<TEntity>`
- `SlotContainerAccess<TEntity>`
- `IReadOnlySlotContainer<TEntity>`
- `ISlotContainer<TEntity>`
- `ISlotContainerAccess<TEntity>`

```csharp
var inventory = new SlotContainer<Item>();

inventory.Add(new Slot<Item>(definition));
inventory.Add(new Slot<Item>(definition));
```

### Definitions

Built-in definitions:

- `TypeSlotDefinition<TEntity, TAccepted>`
- `EnumSlotDefinition<TEntity, TFlags>`

Custom acceptance rules can be implemented with `ISlotDefinition<TEntity>`.

## Gameplay integration

SlotsSystem only owns slot storage, validation, access, and grouping.

Gameplay capabilities should be defined by the consuming project. A furnace can decide when extraction is allowed, an equipment system can tick equipped items, and a UI can work only with read-only slot views without adding those concepts to SlotsSystem itself.

The `Example` folder contains small integration examples.

## Unity installation

Add the repository through Unity Package Manager:

```text
https://github.com/TeslaGan/SlotsSystem.git
```

Or add it to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.teslagan.slots-system": "https://github.com/TeslaGan/SlotsSystem.git"
  }
}
```

For reproducible builds, pin a tag or commit:

```text
https://github.com/TeslaGan/SlotsSystem.git#v0.1.0
```

## License

MIT.
