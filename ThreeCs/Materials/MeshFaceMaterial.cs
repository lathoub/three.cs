namespace ThreeCs.Materials
{
    using System.Collections.Generic;

    public class MeshFaceMaterial : Material
    {
        public List<Material> Materials;

        /// <summary>
        /// Constructor
        /// </summary>
        public MeshFaceMaterial()
        {
            this.type = "MeshFaceMaterial";
        }
    }
}
