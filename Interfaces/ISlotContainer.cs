using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface ISlotContainer<TEntity>
    {
        IReadOnlyList<ISlot<TEntity>> Slots { get; }
    }
}
