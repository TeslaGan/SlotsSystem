using System;

namespace SlotsSystem
{
    public readonly struct SlotChange<TEntity>
    {
        public Slot<TEntity> Slot { get; }
        public TEntity PreviousContent { get; }
        public TEntity Content { get; }

        public SlotChange(Slot<TEntity> slot, TEntity previousContent, TEntity content)
        {
            Slot = slot ?? throw new ArgumentNullException(nameof(slot));
            PreviousContent = previousContent;
            Content = content;
        }
    }
}
