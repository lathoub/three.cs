namespace ThreeCs.Materials
{
    using System.Collections;
    using System.Drawing;

    using ThreeCs.Renderers.Shaders;

    public class ShaderMaterial : Material, IWirerframe, IAttributes
    {
        public Hashtable defines = new Hashtable();

        public Uniforms uniforms = new Uniforms();

        // IAttributes

        public Hashtable attributes { get; set; }

        public string vertexShader = "void main() {\n\tgl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );\n}";

        public string fragmentShader = "void main() {\n\tgl_FragColor = vec4( 1.0, 0.0, 0.0, 1.0 );\n}";

        public int shading = Three.SmoothShading;

        public float linewidth = 1;

        // IWireFrameable

        public bool wireframe { get; set; }

        public float wireframeLinewidth { get; set; }

        //

        public bool fog = false; // set to use scene fog

        public bool lights = false; // set to use scene lights

        public Color[] vertexColors; // set to use "color" attribute stream

        public bool skinning = false; // set to use skinning attribute streams

        public bool morphTargets = false; // set to use morph targets

        public bool morphNormals = false; // set to use morph normals

        // When rendered geometry doesn"t include these attributes but the material does,
        // use these default values in WebGL. This avoids errors when buffer data is missing.
        //public object defaultAttributeValues = {
        //"color": [ 1, 1, 1 ],
        //"uv": [ 0, 0 ],
        //"uv2": [ 0, 0 ]
        //};

        public object index0AttributeName = null;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public ShaderMaterial(Hashtable parameters = null)
        {
            // IAttributes
            attributes = new Hashtable();

            // IWireFrameable
            wireframe = false;
            wireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
