namespace ThreeCs.Extras.Objects
{
    using System;

    using ThreeCs.Core;

    public class ImmediateRenderObject : Object3D
    {
        public object immediateRenderCallback;

        public delegate void render();
    }
}
