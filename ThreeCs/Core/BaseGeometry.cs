namespace ThreeCs.Core
{
    using ThreeCs.Math;

    public interface BaseGeometry
    {
        int id { get; }

        Object3D BoundingBox { get; set; }

        Sphere BoundingSphere { get; set; }

        void ComputeBoundingSphere();

        // Dynamic attributesLocation and object

        bool __webglInit { get; set; }
    }
}
