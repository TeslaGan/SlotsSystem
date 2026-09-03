using System;

namespace Core.SlotsSystem
{
    public interface IFlagged<TFlags>
        where TFlags : struct, Enum
    {
        TFlags Flags { get; }
    }
}
