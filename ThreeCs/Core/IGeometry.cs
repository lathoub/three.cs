namespace ThreeCs.Core
{
    using ThreeCs.Math;

    public interface IGeometry
    {
        Object3D BoundingBox { get; set; }

        Sphere BoundingSphere { get; set; }

        void ComputeBoundingSphere();
    }
}
