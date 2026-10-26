using System;
using Core.SlotsSystem;

namespace Example
{
    [Flags]
    public enum PassengerType
    {
        None = 0,
        Human = 1 << 0,
        Animal = 1 << 1,
        Driver = 1 << 2
    }

    public sealed class Person : IFlagged<PassengerType>
    {
        public Person(string name, PassengerType flags, float height)
        {
            Name = name;
            Flags = flags;
            Height = height;
        }

        public string Name { get; }
        public PassengerType Flags { get; }
        public float Height { get; }
    }

    public static class CarExample
    {
        public static void Run()
        {
            var passengerDefinition = new EnumSlotDefinition<Person, PassengerType>(PassengerType.Human | PassengerType.Animal, FlagMatchMode.Any);
            var itemDefinition = new EnumSlotDefinition<Item, ItemType>(ItemType.Food | ItemType.Weapon | ItemType.Tool, FlagMatchMode.Any);

            var firstPassengerSlot = new Slot<Person>(passengerDefinition);
            var secondPassengerSlot = new Slot<Person>(passengerDefinition);
            var firstTrunkSlot = new Slot<Item>(itemDefinition);
            var secondTrunkSlot = new Slot<Item>(itemDefinition);

            var passengers = new SlotContainerAccess<Person>();
            passengers.Add(new SlotAccess<Person>(firstPassengerSlot, person => person.Height < 2f));
            passengers.Add(new SlotAccess<Person>(secondPassengerSlot, person => person.Height < 2f));

            var trunk = new SlotContainerAccess<Item>();
            trunk.Add(new SlotAccess<Item>(firstTrunkSlot));
            trunk.Add(new SlotAccess<Item>(secondTrunkSlot));

            var alice = new Person("Alice", PassengerType.Human | PassengerType.Driver, 1.70f);
            var bob = new Person("Bob", PassengerType.Human, 2.10f);
            var sword = new Item("Sword", ItemType.Weapon);
            var apple = new Item("Apple", ItemType.Food);

            passengers.Slots[0].TryInsert(alice);
            passengers.Slots[1].TryInsert(bob);
            trunk.Slots[0].TryInsert(sword);
            trunk.Slots[1].TryInsert(apple);
        }
    }
}
