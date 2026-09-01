namespace Core.SlotsSystem
{
    public class SlotDefinition<TEntity> : ISlotDefinition<TEntity>
    {
        public virtual bool Match(TEntity entity)
        {
            return true;
        }
    }
}
