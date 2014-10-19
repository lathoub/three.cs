namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Textures;

    public class MeshLambertMaterial : Material, IWireframe
    { 
        public Color Color = Color.White; // diffuse

        public Color Ambient = Color.White;

        public Color Emissive = Color.Black;

        public bool WrapAround = false;

        public Vector3 wrapRGB = new Vector3( 1, 1, 1 );

        public object map = null;

        public Texture LightMap = null;

        public Texture SpecularMap = null;

        public Texture AlphaMap = null;

        public Texture EnvMap = null;

        public int Combine = Three.MultiplyOperation;

        public float Reflectivity = 1;

        public float RefractionRatio = 0.98f;

        public bool Fog = true;

        public int Shading = Three.SmoothShading;

        // IWireFrameable

        public bool Wireframe { get; set; }

        public float WireframeLinewidth { get; set; }

        //

        public string WireframeLinecap = "round";

        public string WireframeLinejoin = "round";

        public Color[] VertexColors;

        public bool Skinning = false;

        public bool MorphTargets = false;

        public bool MorphNormals = false;
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshLambertMaterial(Hashtable parameters = null)
        {
            // IWireFrameable
            this.Wireframe = false;
            this.WireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
