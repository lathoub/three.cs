namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Textures;

    public class MeshBasicMaterial : Material, IWirerframe
    {
        public Texture map;

        public Texture lightMap;

        public Texture specularMap;

        public Texture alphaMap;

        public Texture envMap;

        public Color color;

        public int combine;

        public float reflectivity;

        public float refractionRatio;

        public bool fog;

        public int shading;

        // IWireFrameable

        public bool wireframe { get; set; }

        public float wireframeLinewidth { get; set; }

        //

        public string wireframeLinecap;

        public string wireframeLinejoin;

        public Color[] vertexColors;

        public bool skinning;

        public bool morphTargets;

        public int numSupportedMorphTargets;

        /// <summary>
        /// 
        /// </summary>
        public MeshBasicMaterial(Hashtable parameters = null)
        {
	        this.color = Color.White; // emissive

	        this.map = null;

	        this.lightMap = null;

	        this.specularMap = null;

	        this.alphaMap = null;

	        this.envMap = null;
	        this.combine = Three.MultiplyOperation;
	        this.reflectivity = 1;
	        this.refractionRatio = 0.98f;

	        this.fog = true;

            this.shading = Three.SmoothShading;

            // IWireFrameable
            wireframe = false;
            wireframeLinewidth = 1;
            
            this.wireframeLinecap = "round";
	        this.wireframeLinejoin = "round";

	        this.skinning = false;
	        this.morphTargets = false;

            this.SetValues(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected MeshBasicMaterial(MeshBasicMaterial other) : base(other)
        {
            
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new MeshBasicMaterial(this);
        }
    }
}
