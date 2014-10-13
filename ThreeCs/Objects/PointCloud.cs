namespace ThreeCs.Objects
{
    using ThreeCs.Core;
    using ThreeCs.Materials;

    public class PointCloud : Object3D
    {
 	    public bool sortParticles = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        public PointCloud(BaseGeometry geometry = null, Material material = null)
        {
            this.Geometry = geometry ?? new Geometry();
            this.Material = material ?? new PointCloudMaterial(); 
        }

        /// <summary>
        /// 
        /// </summary>
        public void Raycast()
        {
            
        }
    }
}
