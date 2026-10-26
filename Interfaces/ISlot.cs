namespace Core.SlotsSystem
{
    public interface ISlot<TEntity> : IReadOnlySlot<TEntity>
    {
        bool CanAccept(TEntity entity);
        bool TrySet(TEntity entity);
        bool TrySet(TEntity entity, out TEntity previousContent);
    }
}
