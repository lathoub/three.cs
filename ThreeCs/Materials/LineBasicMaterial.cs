namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    public class LineBasicMaterial : Material
    {
        public Color Color;

        public float Linewidth;

        public string Linecap;

        public string Linejoin;

        public bool Fog;

        public Color[] VertexColors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public LineBasicMaterial(Hashtable parameters = null)
        {
            this.Color = Color.White;

            this.Linewidth = 1.0f;
            this.Linecap = "round";
            this.Linejoin = "round";

            this.Fog = true;

            this.SetValues( parameters );
        }
    }
}
