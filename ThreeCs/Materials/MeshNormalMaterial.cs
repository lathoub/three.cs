namespace ThreeCs.Materials
{
    using System.Collections;

    public class MeshNormalMaterial : Material, IWirerframe
    {
        public int Shading = Three.FlatShading;

        public bool wireframe { get; set; }

        public float wireframeLinewidth { get; set; }

        public bool MorphTargets = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public MeshNormalMaterial(Hashtable parameters = null)
        {
            Shading = Three.FlatShading;

            wireframe = false;
            wireframeLinewidth = 1;

            this.MorphTargets = false;

            this.SetValues(parameters);
        }
    }
}
