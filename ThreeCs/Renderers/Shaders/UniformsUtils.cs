namespace ThreeCs.Renderers.Shaders
{
    using System;
    using System.Collections.Generic;

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
        public object Clone(List<Uniforms> uniforms)
        {
            throw new NotImplementedException();
            return null;
        }

    }
}
