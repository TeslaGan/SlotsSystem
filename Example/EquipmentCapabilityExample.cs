using System;
using Core.SlotsSystem;

namespace Example
{
    public abstract class EquipmentItem
    {
    }

    public interface IRing
    {
    }

    public interface IManaReceiver
    {
        float Mana { get; }
        float Capacity { get; }

        float AddMana(float amount);
    }

    public interface IManaTransmitter
    {
    }

    public interface IEquippedTickable
    {
        void Tick(IReadOnlySlotContainer<EquipmentItem> inventory, float deltaTime);
    }

    public sealed class ManaStaff : EquipmentItem, IManaReceiver
    {
        private float _mana;

        public float Mana => _mana;
        public float Capacity => 100f;

        public float AddMana(float amount)
        {
            float accepted = MathF.Min(amount, Capacity - _mana);
            _mana += accepted;

            return accepted;
        }
    }

    public sealed class ManaGeneratorRing : EquipmentItem, IRing, IManaTransmitter, IEquippedTickable
    {
        public void Tick(IReadOnlySlotContainer<EquipmentItem> inventory, float deltaTime)
        {
            foreach(IReadOnlySlot<EquipmentItem> slot in inventory.Slots)
            {
                if(slot.Content is not IManaReceiver receiver)
                    continue;

                if(slot.Content is IManaTransmitter)
                    continue;

                receiver.AddMana(deltaTime);
                return;
            }
        }
    }

    public sealed class ManaBatteryRing : EquipmentItem, IRing, IManaReceiver, IManaTransmitter, IEquippedTickable
    {
        private float _mana;

        public float Mana => _mana;
        public float Capacity => 100f;

        public float AddMana(float amount)
        {
            float accepted = MathF.Min(amount, Capacity - _mana);
            _mana += accepted;

            return accepted;
        }

        public void Tick(IReadOnlySlotContainer<EquipmentItem> inventory, float deltaTime)
        {
            foreach(IReadOnlySlot<EquipmentItem> slot in inventory.Slots)
            {
                if(slot.Content is not IManaReceiver receiver)
                    continue;

                if(slot.Content is IManaTransmitter)
                    continue;

                if(_mana <= 0f)
                    return;

                float mana = MathF.Min(deltaTime, _mana);
                float accepted = receiver.AddMana(mana);

                _mana -= accepted;
                return;
            }
        }
    }

    public sealed class EquipmentLoadout
    {
        private readonly SlotContainerAccess<EquipmentItem> _inventoryAccess = new();
        private readonly SlotContainerAccess<EquipmentItem> _equipmentAccess = new();
        private readonly SlotContainerAccess<EquipmentItem> _backpackAccess = new();

        public EquipmentLoadout(int backpackCapacity)
        {
            var ringDefinition = new TypeSlotDefinition<EquipmentItem, IRing>();
            var itemDefinition = new SlotDefinition<EquipmentItem>();

            AddEquipmentSlot(new Slot<EquipmentItem>(ringDefinition));
            AddEquipmentSlot(new Slot<EquipmentItem>(ringDefinition));

            for(int i = 0; i < backpackCapacity; i++)
                AddBackpackSlot(new Slot<EquipmentItem>(itemDefinition));
        }

        public ISlotContainerAccess<EquipmentItem> InventoryAccess => _inventoryAccess;
        public ISlotContainerAccess<EquipmentItem> EquipmentAccess => _equipmentAccess;
        public ISlotContainerAccess<EquipmentItem> BackpackAccess => _backpackAccess;

        public bool TryPickup(EquipmentItem item)
        {
            return _backpackAccess.TryInsert(item);
        }

        public bool TryQuickMove(EquipmentItem item)
        {
            if(_equipmentAccess.TryInsert(item))
                return true;

            return _backpackAccess.TryInsert(item);
        }

        public void Tick(float deltaTime)
        {
            foreach(IReadOnlySlot<EquipmentItem> slot in _equipmentAccess.Slots)
            {
                if(slot.Content is IEquippedTickable tickable)
                    tickable.Tick(_inventoryAccess, deltaTime);
            }
        }

        private void AddEquipmentSlot(ISlot<EquipmentItem> slot)
        {
            var access = new SlotAccess<EquipmentItem>(slot);

            _inventoryAccess.Add(access);
            _equipmentAccess.Add(access);
        }

        private void AddBackpackSlot(ISlot<EquipmentItem> slot)
        {
            var access = new SlotAccess<EquipmentItem>(slot);

            _inventoryAccess.Add(access);
            _backpackAccess.Add(access);
        }
    }

    public static class EquipmentCapabilityExample
    {
        public static void Run()
        {
            var loadout = new EquipmentLoadout(4);

            var generator = new ManaGeneratorRing();
            var battery = new ManaBatteryRing();
            var staff = new ManaStaff();
            var spareRing = new ManaGeneratorRing();

            battery.AddMana(25f);

            loadout.InventoryAccess.Slots[0].TryInsert(generator);
            loadout.TryQuickMove(battery);
            loadout.TryPickup(staff);
            loadout.TryPickup(spareRing);

            loadout.Tick(1f);
        }
    }
}
