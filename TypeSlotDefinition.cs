namespace SlotsSystem
{
    public sealed class TypeSlotDefinition<TEntity, TAccepted> : SlotDefinition<TEntity>
    {
        public override bool Match(TEntity entity)
        {
            return entity is TAccepted;
        }
    }
}
