using System;
using System.Collections.Generic;

namespace Core.SlotsSystem
{
    public interface ISlotContainerAccess<TEntity> : IReadOnlySlotContainer<TEntity>
    {
        new IReadOnlyList<ISlotAccess<TEntity>> Slots { get; }

        bool TryInsert(TEntity entity);
        bool TryExtract(Func<TEntity, bool> matcher, out TEntity entity);
    }
}
