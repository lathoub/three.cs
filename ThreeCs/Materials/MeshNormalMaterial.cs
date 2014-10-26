namespace ThreeCs.Materials
{
    using System.Collections;

    public class MeshNormalMaterial : Material, IWireframe
    {
        public int Shading = Three.FlatShading;

        public bool Wireframe { get; set; }

        public float WireframeLinewidth { get; set; }

        public bool MorphTargets = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshNormalMaterial(Hashtable parameters = null)
        {
            Shading = Three.FlatShading;

            this.type = "MeshNormalMaterial";

            this.Wireframe = false;
            this.WireframeLinewidth = 1;

            this.MorphTargets = false;

            this.SetValues(parameters);
        }
    }
}
