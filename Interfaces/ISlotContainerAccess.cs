using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface ISlotContainerAccess<TEntity>
    {
        IReadOnlyList<ISlotAccess<TEntity>> Slots { get; }
    }
}
