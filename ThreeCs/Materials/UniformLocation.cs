namespace ThreeCs.Materials
{
    using System.Diagnostics;

    using ThreeCs.Renderers.Shaders;

    [DebuggerDisplay("Location = {Location}, Uniform = {Uniform}")]
    public struct UniformLocation
    {
        public Uniform Uniform;
        public int Location;
    }
}