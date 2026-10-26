namespace Core.SlotsSystem
{
    public interface ISlotAccess<TEntity> : IReadOnlySlot<TEntity>
    {
        bool CanInsert(TEntity entity);
        bool CanExtract { get; }

        bool TryInsert(TEntity entity);
        bool TryExtract(out TEntity entity);
    }
}
