using System;

namespace SlotsSystem
{
    public interface IFlagged<TFlags>
        where TFlags : struct, Enum
    {
        TFlags Flags { get; }
    }
}
