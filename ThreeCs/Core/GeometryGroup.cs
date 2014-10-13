namespace ThreeCs.Core
{
    using System.Collections.Generic;

    using ThreeCs.Math;

    public class GeometryGroup : BaseGeometry
    {
        protected static int GeometryGroupIdCount;

        public int Id = GeometryGroupIdCount++;


        public override void ComputeBoundingSphere()
        {
        }

        public override void ApplyMatrix(Matrix4 matrix)
        {
        }




        public List<int> Faces3;

        public long MaterialIndex;

        public long Vertices;

        public int NumMorphTargets;

        public int NumMorphNormals;
    }
}