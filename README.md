# SlotsSystem

Small, reusable slot system for Unity and C# projects.

SlotsSystem provides a compact set of primitives for:

- storing one entity in a slot;
- validating accepted content;
- exposing read-only or controlled external access;
- grouping slots into containers and access routes;
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

`TryInsert(...)` only inserts into an empty slot. It never replaces existing content. Replacement remains an owner-level `ISlot<TEntity>.TrySet(...)` operation.

### Containers

Slots can be grouped as physical storage or as access routes.

`SlotContainer<TEntity>` groups physical slots:

```csharp
var inventory = new SlotContainer<Item>();

inventory.Add(new Slot<Item>(definition));
inventory.Add(new Slot<Item>(definition));
```

`SlotContainerAccess<TEntity>` groups `ISlotAccess<TEntity>` instances and exposes operations over those accesses:

```csharp
var access = new SlotContainerAccess<Item>();

access.Add(new SlotAccess<Item>(firstSlot));
access.Add(new SlotAccess<Item>(secondSlot));
```

`TryInsert(...)` scans accesses in order and inserts into the first slot that accepts the entity:

```csharp
access.TryInsert(item);
```

Occupied or rejected slots are skipped. Existing content is never replaced through access.

`TryExtract(...)` scans accesses in order and extracts the first accessible entity that matches the caller predicate:

```csharp
access.TryExtract(
    item => item is Apple,
    out Item apple);
```

The predicate describes what the caller is looking for. The individual `SlotAccess<TEntity>` still decides whether extraction from that slot is allowed.

The same `SlotAccess<TEntity>` can be included in multiple `SlotContainerAccess<TEntity>` instances. This allows different systems to expose different routes over the same physical slots without duplicating slot state.

For example, one inventory can expose separate equipment and backpack routes over the same underlying accesses.

### Definitions

Built-in definitions:

- `TypeSlotDefinition<TEntity, TAccepted>`
- `EnumSlotDefinition<TEntity, TFlags>`

Custom acceptance rules can be implemented with `ISlotDefinition<TEntity>`.

## Gameplay integration

SlotsSystem only owns slot storage, validation, access, grouping, and route-level insert/extract operations.

Gameplay capabilities should be defined by the consuming project. A furnace can decide when extraction is allowed, an equipment system can tick equipped items, a pickup system can target only backpack accesses, and a UI can work with read-only slot views without adding those concepts to SlotsSystem itself.

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
https://github.com/TeslaGan/SlotsSystem.git#v1.0.1
```

## License

MIT.
