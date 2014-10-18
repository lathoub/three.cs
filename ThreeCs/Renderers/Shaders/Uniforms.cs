namespace ThreeCs.Renderers.Shaders
{
    using System.Collections.Generic;

    public class Uniforms : Dictionary<string, KVP>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
