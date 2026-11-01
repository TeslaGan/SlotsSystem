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
        private readonly SlotContainerAccess<IEquipment> _equipmentAccess = new();

        public CharacterEquipment()
        {
            Boots = AddSlot(new TypeSlotDefinition<IEquipment, IBoots>());
            Pants = AddSlot(new TypeSlotDefinition<IEquipment, IPants>());
            Body = AddSlot(new TypeSlotDefinition<IEquipment, IBodyArmor>());
            Head = AddSlot(new TypeSlotDefinition<IEquipment, IHelmet>());
            Hand = AddSlot(new TypeSlotDefinition<IEquipment, IHandEquipment>());
        }

        public ISlotAccess<IEquipment> Boots { get; }
        public ISlotAccess<IEquipment> Pants { get; }
        public ISlotAccess<IEquipment> Body { get; }
        public ISlotAccess<IEquipment> Head { get; }
        public ISlotAccess<IEquipment> Hand { get; }

        public ISlotContainerAccess<IEquipment> EquipmentAccess => _equipmentAccess;

        private ISlotAccess<IEquipment> AddSlot(ISlotDefinition<IEquipment> definition)
        {
            var access = new SlotAccess<IEquipment>(new Slot<IEquipment>(definition));
            _equipmentAccess.Add(access);

            return access;
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

            equipment.EquipmentAccess.TryInsert(boots);
            equipment.EquipmentAccess.TryInsert(pants);
            equipment.EquipmentAccess.TryInsert(armor);
            equipment.EquipmentAccess.TryInsert(helmet);
            equipment.EquipmentAccess.TryInsert(sword);
        }
    }
}
