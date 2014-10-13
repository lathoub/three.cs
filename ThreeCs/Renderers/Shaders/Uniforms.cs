namespace ThreeCs.Renderers.Shaders
{
    using System.Collections.Generic;

    public class Uniforms : Dictionary<string, KVP>
    {
        public static void SetValue(Uniforms uniforms, string key, object value)
        {
            KVP kvp = null;
            if (uniforms.TryGetValue(key, out kvp))
            {
                kvp.Value = value;
            }
        }

    }
}
