using Core.SlotsSystem;

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
        public Boots(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class Pants : IPants
    {
        public Pants(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class BodyArmor : IBodyArmor
    {
        public BodyArmor(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class Helmet : IHelmet
    {
        public Helmet(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class Sword : IHandEquipment
    {
        public Sword(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class CharacterEquipment
    {
        public CharacterEquipment()
        {
            Boots = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IBoots>());
            Pants = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IPants>());
            Body = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IBodyArmor>());
            Head = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IHelmet>());
            Hand = new Slot<IEquipment>(new TypeSlotDefinition<IEquipment, IHandEquipment>());
        }

        public Slot<IEquipment> Boots { get; }
        public Slot<IEquipment> Pants { get; }
        public Slot<IEquipment> Body { get; }
        public Slot<IEquipment> Head { get; }
        public Slot<IEquipment> Hand { get; }
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

            equipment.Boots.TrySet(boots);
            equipment.Pants.TrySet(pants);
            equipment.Body.TrySet(armor);
            equipment.Head.TrySet(helmet);
            equipment.Hand.TrySet(sword);

            equipment.Head.TrySet(sword);
            equipment.Boots.TrySet(helmet);
        }
    }
}
