using System;

namespace SlotsSystem
{
    public sealed class SlotDefinition<TEntity, TFlags> : ISlotDefinition<TEntity>
        where TEntity : IFlagged<TFlags>
        where TFlags : struct, Enum
    {
        public TFlags AcceptedFlags { get; }
        public FlagMatchMode MatchMode { get; }

        public SlotDefinition(TFlags acceptedFlags, FlagMatchMode matchMode)
        {
            AcceptedFlags = acceptedFlags;
            MatchMode = matchMode;
        }

        public bool Match(TEntity entity)
        {
            return MatchMode switch
            {
                FlagMatchMode.Any => FlagMatch.Any(entity.Flags, AcceptedFlags),
                FlagMatchMode.All => FlagMatch.All(entity.Flags, AcceptedFlags),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
