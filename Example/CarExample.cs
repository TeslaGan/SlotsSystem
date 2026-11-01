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
        public Person(string name, PassengerType flags)
        {
            Name = name;
            Flags = flags;
        }

        public string Name { get; }
        public PassengerType Flags { get; }
    }

    public sealed class Car
    {
        private readonly SlotContainerAccess<Item> _trunkAccess = new();

        public Car()
        {
            Driver = new SlotAccess<Person>(
                new Slot<Person>(
                    new EnumSlotDefinition<Person, PassengerType>(
                        PassengerType.Driver,
                        FlagMatchMode.All)));

            FrontPassenger = new SlotAccess<Person>(
                new Slot<Person>(
                    new EnumSlotDefinition<Person, PassengerType>(
                        PassengerType.Human,
                        FlagMatchMode.All)));

            RearLeftPassenger = new SlotAccess<Person>(
                new Slot<Person>(
                    new EnumSlotDefinition<Person, PassengerType>(
                        PassengerType.Human | PassengerType.Animal,
                        FlagMatchMode.Any)));

            RearRightPassenger = new SlotAccess<Person>(
                new Slot<Person>(
                    new EnumSlotDefinition<Person, PassengerType>(
                        PassengerType.Human | PassengerType.Animal,
                        FlagMatchMode.Any)));

            var trunkDefinition = new EnumSlotDefinition<Item, ItemType>(ItemType.Food | ItemType.Weapon | ItemType.Tool, FlagMatchMode.Any);

            _trunkAccess.Add(new SlotAccess<Item>(new Slot<Item>(trunkDefinition)));
            _trunkAccess.Add(new SlotAccess<Item>(new Slot<Item>(trunkDefinition)));
        }

        public ISlotAccess<Person> Driver { get; }
        public ISlotAccess<Person> FrontPassenger { get; }
        public ISlotAccess<Person> RearLeftPassenger { get; }
        public ISlotAccess<Person> RearRightPassenger { get; }

        public ISlotContainerAccess<Item> TrunkAccess => _trunkAccess;
    }

    public static class CarExample
    {
        public static void Run()
        {
            var car = new Car();

            var alice = new Person("Alice", PassengerType.Human | PassengerType.Driver);
            var bob = new Person("Bob", PassengerType.Human);
            var dog = new Person("Dog", PassengerType.Animal);

            var sword = new Item("Sword", ItemType.Weapon);
            var apple = new Item("Apple", ItemType.Food);

            car.Driver.TryInsert(alice);
            car.FrontPassenger.TryInsert(bob);

            car.FrontPassenger.TryInsert(dog);
            car.RearLeftPassenger.TryInsert(dog);

            car.TrunkAccess.TryInsert(sword);
            car.TrunkAccess.TryInsert(apple);
        }
    }
}
