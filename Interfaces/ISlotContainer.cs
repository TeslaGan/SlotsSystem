using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface ISlotContainer<TEntity> : IReadOnlySlotContainer<TEntity>
    {
        new IReadOnlyList<ISlot<TEntity>> Slots { get; }
    }
}
