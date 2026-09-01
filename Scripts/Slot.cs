using System;

namespace Core.SlotsSystem
{
    public sealed class Slot<TEntity>
    {
        public event Action<SlotChangeData<TEntity>> Changed;

        public Slot(ISlotDefinition<TEntity> definition, Func<TEntity, bool> parentMatcher = null)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ParentMatcher = parentMatcher ?? (_ => true);
        }

        public ISlotDefinition<TEntity> Definition { get; }
        public Func<TEntity, bool> ParentMatcher { get; }
        public TEntity Content { get; private set; }

        public bool CanAccept(TEntity entity)
        {
            if(entity is null)
                return false;

            return Definition.Match(entity) && ParentMatcher(entity);
        }

        public bool TrySet(TEntity entity)
        {
            if(entity is not null && CanAccept(entity) == false)
                return false;

            TEntity previousContent = Content;
            Content = entity;
            Changed?.Invoke(new SlotChangeData<TEntity>(this, previousContent, Content));

            return true;
        }
    }
}
