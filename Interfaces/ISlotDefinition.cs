namespace Core.SlotsSystem
{
    public interface ISlotDefinition<TEntity>
    {
        bool Match(TEntity entity);
    }
}
