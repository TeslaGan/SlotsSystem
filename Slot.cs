using System;

namespace SlotsSystem
{
    public sealed class Slot<TEntity>
    {
        public event Action<SlotChange<TEntity>> Changed;

        public ISlotDefinition<TEntity> Definition { get; }
        public Func<TEntity, bool> ParentMatcher { get; }
        public TEntity Content { get; private set; }

        public Slot(ISlotDefinition<TEntity> definition, Func<TEntity, bool> parentMatcher = null)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ParentMatcher = parentMatcher ?? (_ => true);
        }

        public bool CanAccept(TEntity entity)
        {
            return Definition.Match(entity) && ParentMatcher(entity);
        }

        public bool TrySet(TEntity entity)
        {
            if(CanAccept(entity) == false)
                return false;

            var previousContent = Content;
            Content = entity;
            Changed?.Invoke(new SlotChange<TEntity>(this, previousContent, Content));

            return true;
        }
    }
}
