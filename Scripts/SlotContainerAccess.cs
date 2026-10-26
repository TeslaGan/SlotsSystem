using System;
using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public sealed class SlotContainerAccess<TEntity> : ISlotContainerAccess<TEntity>, IReadOnlySlotContainer<TEntity>
    {
        private readonly List<ISlotAccess<TEntity>> _slots = new();
        private readonly List<IReadOnlySlot<TEntity>> _readOnlySlots = new();

        public IReadOnlyList<ISlotAccess<TEntity>> Slots => _slots;

        IReadOnlyList<IReadOnlySlot<TEntity>> IReadOnlySlotContainer<TEntity>.Slots => _readOnlySlots;

        public void Add(ISlotAccess<TEntity> slot)
        {
            if(slot == null)
                throw new ArgumentNullException(nameof(slot));

            if(_slots.Contains(slot))
                throw new ArgumentException("Slot access is already contained.", nameof(slot));

            _slots.Add(slot);
            _readOnlySlots.Add(slot);
        }

        public bool Remove(ISlotAccess<TEntity> slot)
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
