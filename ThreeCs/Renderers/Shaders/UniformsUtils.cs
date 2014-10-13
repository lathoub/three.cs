namespace ThreeCs.Renderers.Shaders
{
    using System.Collections.Generic;

    public class UniformsUtils
    {
        public static new Uniforms Merge(List<Uniforms> uniforms)
        {
            var merged = new Uniforms();

            foreach ( var uniform in uniforms)
            {
                foreach (var kvp in uniform)
                {
                    merged.Add(kvp.Key, kvp.Value);
                }
            }
            return merged;
        }
    }
}
