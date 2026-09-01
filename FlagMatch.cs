using System;

namespace SlotsSystem
{
    public static class FlagMatch
    {
        public static bool Any<TFlags>(TFlags value, TFlags required)
            where TFlags : struct, Enum
        {
            ulong valueBits = Convert.ToUInt64(value);
            ulong requiredBits = Convert.ToUInt64(required);

            return (valueBits & requiredBits) != 0UL;
        }

        public static bool All<TFlags>(TFlags value, TFlags required)
            where TFlags : struct, Enum
        {
            ulong valueBits = Convert.ToUInt64(value);
            ulong requiredBits = Convert.ToUInt64(required);

            return (valueBits & requiredBits) == requiredBits;
        }
    }
}
