namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Textures;

    public class PointCloudMaterial : Material
    {
        public Color color = Color.White;

        public Texture map;

        public float size = 1;

        public bool sizeAttenuation = true;

        public Color[] vertexColors;

        public bool fog = true;
        
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
