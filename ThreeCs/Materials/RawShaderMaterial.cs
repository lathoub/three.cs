namespace ThreeCs.Materials
{
    using System.Collections;

    public class RawShaderMaterial : ShaderMaterial
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public RawShaderMaterial(Hashtable parameters = null)
        {
            this.type = "RawShaderMaterial";

            this.SetValues(parameters);
        }
    }
}
