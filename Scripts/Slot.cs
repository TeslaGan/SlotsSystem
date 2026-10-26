using System;

namespace Core.SlotsSystem
{
    public sealed class Slot<TEntity> : ISlot<TEntity>
    {
        public event Action<SlotChangeData<TEntity>> Changed;

        public Slot(ISlotDefinition<TEntity> definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public ISlotDefinition<TEntity> Definition { get; }
        public TEntity Content { get; private set; }

        public bool CanAccept(TEntity entity)
        {
            if(entity is null)
                return false;

            return Definition.Match(entity);
        }

        public bool TrySet(TEntity entity)
        {
            return TrySet(entity, out _);
        }

        public bool TrySet(TEntity entity, out TEntity previousContent)
        {
            previousContent = Content;

            if(entity is not null && CanAccept(entity) == false)
                return false;

            Content = entity;
            Changed?.Invoke(new SlotChangeData<TEntity>(this, previousContent, Content));

            return true;
        }
    }
}
