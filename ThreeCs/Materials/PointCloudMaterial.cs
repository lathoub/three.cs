namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Textures;

    public class PointCloudMaterial : Material
    {
        public Color color = Color.White;

        public Texture Map;

        public float Size = 1;

        public bool SizeAttenuation = true;

        public Color[] VertexColors;

        public bool Fog = true;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public PointCloudMaterial(Hashtable parameters = null)
        {
            this.SetValues(parameters);
        }
    }
}
