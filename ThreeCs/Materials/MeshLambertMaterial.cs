namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Textures;

    public class MeshLambertMaterial : Material, IWirerframe
    { 
        public Color color = Color.White; // diffuse

        public Color ambient = Color.White;

        public Color emissive = Color.Black;

        public bool WrapAround = false;

        public Vector3 wrapRGB = new Vector3( 1, 1, 1 );

        public object map = null;

        public Texture lightMap = null;

        public Texture specularMap = null;

        public Texture alphaMap = null;

        public Texture envMap = null;

        public int combine = Three.MultiplyOperation;

        public float reflectivity = 1;

        public float refractionRatio = 0.98f;

        public bool fog = true;

        public int shading = Three.SmoothShading;

        // IWireFrameable

        public bool wireframe { get; set; }

        public float wireframeLinewidth { get; set; }

        //

        public string wireframeLinecap = "round";

        public string wireframeLinejoin = "round";

        public Color[] vertexColors;

        public bool skinning = false;

        public bool morphTargets = false;

        public bool morphNormals = false;
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshLambertMaterial(Hashtable parameters = null)
        {
            // IWireFrameable
            wireframe = false;
            wireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
