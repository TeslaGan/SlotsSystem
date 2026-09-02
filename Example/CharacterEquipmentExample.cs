using SlotsSystem;

namespace Example
{
    public interface IEquipment
    {
        string Name { get; }
    }

    public interface IBoots : IEquipment
    {
    }

    public interface IPants : IEquipment
    {
    }

    public interface IBodyArmor : IEquipment
    {
    }

    public interface IHelmet : IEquipment
    {
    }

    public interface IHandEquipment : IEquipment
    {
    }

    public sealed class Boots : IBoots
    {
        public string Name { get; }

        public Boots(string name)
        {
            Name = name;
        }
    }

    public sealed class Pants : IPants
    {
        public string Name { get; }

        public Pants(string name)
        {
            Name = name;
        }
    }

    public sealed class BodyArmor : IBodyArmor
    {
        public string Name { get; }

        public BodyArmor(string name)
        {
            Name = name;
        }
    }

    public sealed class Helmet : IHelmet
    {
        public string Name { get; }

        public Helmet(string name)
        {
            Name = name;
        }
    }

    public sealed class Sword : IHandEquipment
    {
        public string Name { get; }

        public Sword(string name)
        {
            Name = name;
        }
    }

    public sealed class CharacterEquipment
    {
        public Slot<IEquipment> Boots { get; }
        public Slot<IEquipment> Pants { get; }
        public Slot<IEquipment> Body { get; }
        public Slot<IEquipment> Head { get; }
        public Slot<IEquipment> Hand { get; }

        public CharacterEquipment()
        {
            Boots = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IBoots>());
            Pants = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IPants>());
            Body = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IBodyArmor>());
            Head = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IHelmet>());
            Hand = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IHandEquipment>());
        }
    }

    public static class CharacterEquipmentExample
    {
        public static void Run()
        {
            var equipment = new CharacterEquipment();

            var boots = new Boots("Leather Boots");
            var pants = new Pants("Traveler Pants");
            var armor = new BodyArmor("Iron Armor");
            var helmet = new Helmet("Iron Helmet");
            var sword = new Sword("Longsword");

            equipment.Boots.TrySet(boots);   // true
            equipment.Pants.TrySet(pants);   // true
            equipment.Body.TrySet(armor);    // true
            equipment.Head.TrySet(helmet);   // true
            equipment.Hand.TrySet(sword);    // true

            equipment.Head.TrySet(sword);    // false: Sword is not IHelmet
            equipment.Boots.TrySet(helmet);  // false: Helmet is not IBoots
        }
    }
}
