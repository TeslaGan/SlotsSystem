namespace SlotsSystem
{
    public interface ISlotDefinition<TEntity>
    {
        bool Match(TEntity entity);
    }
}
