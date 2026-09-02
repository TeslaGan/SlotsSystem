using System;
using System.Collections.Generic;
using SlotsSystem;

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
        public string Name { get; }
        public PassengerType Flags { get; }
        public float Height { get; }

        public Person(string name, PassengerType flags, float height)
        {
            Name = name;
            Flags = flags;
            Height = height;
        }
    }

    public sealed class Car
    {
        public List<Slot<Person>> PassengerSlots { get; }
        public List<Slot<Item>> TrunkSlots { get; }

        public Car(List<Slot<Person>> passengerSlots, List<Slot<Item>> trunkSlots)
        {
            PassengerSlots = passengerSlots;
            TrunkSlots = trunkSlots;
        }
    }

    public static class CarExample
    {
        public static void Run()
        {
            var passengerDefinition = new EnumSlotDefinition<Person, PassengerType>(
                PassengerType.Human | PassengerType.Animal,
                FlagMatchMode.Any);

            var itemDefinition = new EnumSlotDefinition<Item, ItemType>(
                ItemType.Food | ItemType.Weapon | ItemType.Tool,
                FlagMatchMode.Any);

            var passengerSlots = new List<Slot<Person>>
            {
                new(passengerDefinition, person => person.Height < 2f),
                new(passengerDefinition, person => person.Height < 2f)
            };

            var trunkSlots = new List<Slot<Item>>
            {
                new(itemDefinition),
                new(itemDefinition)
            };

            var car = new Car(passengerSlots, trunkSlots);
            var alice = new Person("Alice", PassengerType.Human | PassengerType.Driver, 1.70f);
            var bob = new Person("Bob", PassengerType.Human, 2.10f);
            var sword = new Item("Sword", ItemType.Weapon);
            var apple = new Item("Apple", ItemType.Food);

            car.PassengerSlots[0].TrySet(alice); // true
            car.PassengerSlots[1].TrySet(bob);   // false: ParentMatcher
            car.TrunkSlots[0].TrySet(sword);     // true
            car.TrunkSlots[1].TrySet(apple);     // true
        }
    }
}
