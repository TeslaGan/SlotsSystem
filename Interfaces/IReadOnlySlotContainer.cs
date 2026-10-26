using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface IReadOnlySlotContainer<TEntity>
    {
        IReadOnlyList<IReadOnlySlot<TEntity>> Slots { get; }
    }
}
