using System;

namespace Core.SlotsSystem
{
    public readonly struct SlotChangeData<TEntity>
    {
        public SlotChangeData(IReadOnlySlot<TEntity> slot, TEntity previousContent, TEntity content)
        {
            Slot = slot ?? throw new ArgumentNullException(nameof(slot));
            PreviousContent = previousContent;
            Content = content;
        }

        public IReadOnlySlot<TEntity> Slot { get; }
        public TEntity PreviousContent { get; }
        public TEntity Content { get; }
    }
}
