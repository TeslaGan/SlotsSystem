using System;
using System.Collections.Generic;
using SlotsSystem;

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
        public string Name { get; }
        public ItemType Flags { get; }

        public Item(string name, ItemType flags)
        {
            Name = name;
            Flags = flags;
        }
    }

    public static class BackpackExample
    {
        public static void Run()
        {
            var definition = new SlotDefinition<Item, ItemType>(
                ItemType.Food | ItemType.Weapon | ItemType.Tool,
                FlagMatchMode.Any);

            var slots = new List<Slot<Item>>
            {
                new(definition),
                new(definition)
            };

            var sword = new Item("Sword", ItemType.Weapon);
            var apple = new Item("Apple", ItemType.Food);

            slots[0].TrySet(sword);
            slots[1].TrySet(apple);
        }
    }
}
