namespace ThreeCs.Renderers.Shaders
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Textures;

    public class UniformsUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <returns></returns>
        public static Uniforms Merge(List<Uniforms> uniforms)
        {
            var merged = new Uniforms();

            foreach ( var uniform in uniforms)
            {
               // var tmp = uniform.clone();

                foreach (var kvp in uniform)
                {
                    merged.Add(kvp.Key, kvp.Value);
                }
            }
            return merged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static object Clone(Uniforms uniforms)
        {
            return new Uniforms().Copy(uniforms);
        }
    }
}
