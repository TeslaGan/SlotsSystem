using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface ISlotContainerAccess<TEntity> : IReadOnlySlotContainer<TEntity>
    {
        new IReadOnlyList<ISlotAccess<TEntity>> Slots { get; }
    }
}
