using System;
using Core.SlotsSystem;

namespace Example
{
    [Flags]
    public enum ItemType
    {
        None = 0,
        Food = 1 << 0,
        Weapon = 1 << 1,
        Tool = 1 << 2
    }

    public sealed class Item : IFlagged<ItemType>
    {
        public Item(string name, ItemType flags)
        {
            Name = name;
            Flags = flags;
        }

        public string Name { get; }
        public ItemType Flags { get; }
    }

    public static class BackpackExample
    {
        public static void Run()
        {
            var definition = new EnumSlotDefinition<Item, ItemType>(ItemType.Food | ItemType.Weapon | ItemType.Tool, FlagMatchMode.Any);
            var backpack = new SlotContainer<Item>();
            var firstSlot = new Slot<Item>(definition);
            var secondSlot = new Slot<Item>(definition);

            backpack.Add(firstSlot);
            backpack.Add(secondSlot);

            var sword = new Item("Sword", ItemType.Weapon);
            var apple = new Item("Apple", ItemType.Food);

            backpack.Slots[0].TrySet(sword);
            backpack.Slots[1].TrySet(apple);
        }
    }
}
