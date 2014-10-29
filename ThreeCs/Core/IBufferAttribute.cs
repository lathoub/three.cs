namespace ThreeCs.Core
{
    using System;

    public interface IBufferAttribute
    {
        bool needsUpdate { get; set; }

        int buffer { get; set; }

        int length { get; }

        int ItemSize { get; }

        Type Type { get; }
    }
}
