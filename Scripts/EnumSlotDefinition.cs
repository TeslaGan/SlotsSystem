using System;

namespace Core.SlotsSystem
{
    public sealed class EnumSlotDefinition<TEntity, TFlags> : SlotDefinition<TEntity>
        where TEntity : IFlagged<TFlags>
        where TFlags : struct, Enum
    {
        public EnumSlotDefinition(TFlags acceptedFlags, FlagMatchMode matchMode)
        {
            AcceptedFlags = acceptedFlags;
            MatchMode = matchMode;
        }

        public TFlags AcceptedFlags { get; }
        public FlagMatchMode MatchMode { get; }

        public override bool Match(TEntity entity)
        {
            return MatchMode switch
            {
                FlagMatchMode.Any => FlagMatch.Any(entity.Flags, AcceptedFlags),
                FlagMatchMode.All => FlagMatch.All(entity.Flags, AcceptedFlags),
                _ => throw new ArgumentOutOfRangeException(nameof(MatchMode), MatchMode, null)
            };
        }
    }
}
