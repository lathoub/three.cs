namespace ThreeCs.Materials
{
    using System.Collections;

    public class MeshNormalMaterial : Material, IWirerframe
    {
        public int shading = Three.FlatShading;

        // IWireFrameable

        public bool wireframe { get; set; }

        public float wireframeLinewidth { get; set; }

        //

        public bool morphTargets = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshNormalMaterial(Hashtable parameters = null)
        {
            // IWireFrameable
            wireframe = false;
            wireframeLinewidth = 1;
            
            this.SetValues(parameters);
        }
    }
}
