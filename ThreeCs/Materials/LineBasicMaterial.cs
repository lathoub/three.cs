namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    public class LineBasicMaterial : Material
    {
        public Color Color;

        public float linewidth;

        public string linecap;

        public string linejoin;

        public bool fog;

        public Color[] vertexColors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public LineBasicMaterial(Hashtable parameters = null)
        {
            this.Color = Color.White;

            this.linewidth = 1.0f;
            this.linecap = "round";
            this.linejoin = "round";

            this.fog = true;

            this.SetValues( parameters );
        }
    }
}
