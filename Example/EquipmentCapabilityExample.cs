using System;
using System.Collections.Generic;
using Core.SlotsSystem;

namespace Example
{
    public abstract class EquipmentItem
    {
    }

    public interface IRing
    {
    }

    public interface IManaStorage
    {
        float Mana { get; }
        float Capacity { get; }

        float AddMana(float amount);
        float RemoveMana(float amount);
    }

    public interface IEquippedTickable
    {
        void Tick(IEquipmentContext context, float deltaTime);
    }

    public interface IEquipmentContext
    {
        IEnumerable<EquipmentItem> BackpackItems { get; }
    }

    public sealed class ManaStaff : EquipmentItem, IManaStorage
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

        public float RemoveMana(float amount)
        {
            float removed = MathF.Min(amount, _mana);
            _mana -= removed;

            return removed;
        }
    }

    public sealed class ManaGeneratorRing : EquipmentItem, IRing, IEquippedTickable
    {
        public void Tick(IEquipmentContext context, float deltaTime)
        {
            foreach(EquipmentItem item in context.BackpackItems)
            {
                if(item is IManaStorage storage)
                    storage.AddMana(deltaTime);
            }
        }
    }

    public sealed class ManaBatteryRing : EquipmentItem, IRing, IManaStorage, IEquippedTickable
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

        public float RemoveMana(float amount)
        {
            float removed = MathF.Min(amount, _mana);
            _mana -= removed;

            return removed;
        }

        public void Tick(IEquipmentContext context, float deltaTime)
        {
            foreach(EquipmentItem item in context.BackpackItems)
            {
                if(item is not IManaStorage storage)
                    continue;

                float removed = RemoveMana(deltaTime);

                if(removed <= 0f)
                    return;

                float accepted = storage.AddMana(removed);

                if(accepted < removed)
                    AddMana(removed - accepted);
            }
        }
    }

    public sealed class EquipmentContext : IEquipmentContext
    {
        private readonly IReadOnlySlotContainer<EquipmentItem> _backpack;

        public EquipmentContext(IReadOnlySlotContainer<EquipmentItem> backpack)
        {
            _backpack = backpack ?? throw new ArgumentNullException(nameof(backpack));
        }

        public IEnumerable<EquipmentItem> BackpackItems
        {
            get
            {
                foreach(IReadOnlySlot<EquipmentItem> slot in _backpack.Slots)
                {
                    if(slot.Content != null)
                        yield return slot.Content;
                }
            }
        }
    }

    public sealed class EquipmentLoadout
    {
        private readonly SlotContainer<EquipmentItem> _equipped = new();
        private readonly SlotContainerAccess<EquipmentItem> _ringAccess = new();

        private readonly SlotContainer<EquipmentItem> _backpack = new();
        private readonly SlotContainerAccess<EquipmentItem> _backpackAccess = new();

        public EquipmentLoadout(int backpackCapacity)
        {
            var ringDefinition = new TypeSlotDefinition<EquipmentItem, IRing>();
            var itemDefinition = new SlotDefinition<EquipmentItem>();

            AddRingSlot(new Slot<EquipmentItem>(ringDefinition));
            AddRingSlot(new Slot<EquipmentItem>(ringDefinition));

            for(int i = 0; i < backpackCapacity; i++)
                AddBackpackSlot(new Slot<EquipmentItem>(itemDefinition));
        }

        public IReadOnlySlotContainer<EquipmentItem> Equipped => _equipped;
        public IReadOnlySlotContainer<EquipmentItem> Backpack => _backpack;

        public ISlotContainerAccess<EquipmentItem> RingAccess => _ringAccess;
        public ISlotContainerAccess<EquipmentItem> BackpackAccess => _backpackAccess;

        private void AddRingSlot(ISlot<EquipmentItem> slot)
        {
            _equipped.Add(slot);
            _ringAccess.Add(new SlotAccess<EquipmentItem>(slot));
        }

        private void AddBackpackSlot(ISlot<EquipmentItem> slot)
        {
            _backpack.Add(slot);
            _backpackAccess.Add(new SlotAccess<EquipmentItem>(slot));
        }
    }

    public static class EquipmentRuntime
    {
        public static void TickEquipped(IReadOnlySlotContainer<EquipmentItem> equipped, IEquipmentContext context, float deltaTime)
        {
            foreach(IReadOnlySlot<EquipmentItem> slot in equipped.Slots)
            {
                if(slot.Content is IEquippedTickable tickable)
                    tickable.Tick(context, deltaTime);
            }
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

            battery.AddMana(25f);

            loadout.RingAccess.Slots[0].TryInsert(generator);
            loadout.RingAccess.Slots[1].TryInsert(battery);
            loadout.BackpackAccess.Slots[0].TryInsert(staff);

            var context = new EquipmentContext(loadout.Backpack);

            EquipmentRuntime.TickEquipped(loadout.Equipped, context, 1f);
        }
    }
}
