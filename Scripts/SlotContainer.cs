using System;
using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public sealed class SlotContainer<TEntity> : ISlotContainer<TEntity>, IReadOnlySlotContainer<TEntity>
    {
        private readonly List<ISlot<TEntity>> _slots = new();
        private readonly List<IReadOnlySlot<TEntity>> _readOnlySlots = new();

        public IReadOnlyList<ISlot<TEntity>> Slots => _slots;

        IReadOnlyList<IReadOnlySlot<TEntity>> IReadOnlySlotContainer<TEntity>.Slots => _readOnlySlots;

        public void Add(ISlot<TEntity> slot)
        {
            if(slot == null)
                throw new ArgumentNullException(nameof(slot));

            if(_slots.Contains(slot))
                throw new ArgumentException("Slot is already contained.", nameof(slot));

            _slots.Add(slot);
            _readOnlySlots.Add(slot);
        }

        public bool Remove(ISlot<TEntity> slot)
        {
            if(slot == null)
                throw new ArgumentNullException(nameof(slot));

            if(_slots.Remove(slot) == false)
                return false;

            _readOnlySlots.Remove(slot);

            return true;
        }
    }
}
