using System;

namespace Core.SlotsSystem
{
    public sealed class SlotAccess<TEntity> : ISlotAccess<TEntity>
    {
        private readonly ISlot<TEntity> _slot;
        private readonly Func<TEntity, bool> _insertMatcher;
        private readonly Func<TEntity, bool> _extractMatcher;

        public SlotAccess(ISlot<TEntity> slot, Func<TEntity, bool> insertMatcher = null, Func<TEntity, bool> extractMatcher = null)
        {
            _slot = slot ?? throw new ArgumentNullException(nameof(slot));
            _insertMatcher = insertMatcher;
            _extractMatcher = extractMatcher;
        }

        public event Action<SlotChangeData<TEntity>> Changed
        {
            add => _slot.Changed += value;
            remove => _slot.Changed -= value;
        }

        public ISlotDefinition<TEntity> Definition => _slot.Definition;
        public TEntity Content => _slot.Content;

        public bool CanExtract
        {
            get
            {
                if(_slot.Content is null)
                    return false;

                return _extractMatcher == null || _extractMatcher(_slot.Content);
            }
        }

        public bool CanInsert(TEntity entity)
        {
            if(_slot.Content is not null)
                return false;

            if(_slot.CanAccept(entity) == false)
                return false;

            return _insertMatcher == null || _insertMatcher(entity);
        }

        public bool TryInsert(TEntity entity)
        {
            if(CanInsert(entity) == false)
                return false;

            return _slot.TrySet(entity);
        }

        public bool TryExtract(out TEntity entity)
        {
            entity = _slot.Content;

            if(CanExtract == false)
                return false;

            return _slot.TrySet(default, out entity);
        }
    }
}
