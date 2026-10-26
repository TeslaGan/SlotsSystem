using System;

namespace Core.SlotsSystem
{
    public interface IReadOnlySlot<TEntity>
    {
        event Action<SlotChangeData<TEntity>> Changed;

        ISlotDefinition<TEntity> Definition { get; }
        TEntity Content { get; }
    }
}
