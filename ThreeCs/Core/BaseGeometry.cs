namespace ThreeCs.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ThreeCs.Math;

    public abstract class BaseGeometry
    {
        public Guid Uuid = Guid.NewGuid();

        public string Name;

        public Object3D BoundingBox = null;

        public Sphere BoundingSphere = null;


        public abstract void ComputeBoundingSphere();

        public abstract void ApplyMatrix(Matrix4 matrix);

        // Dynamic attributesLocation and object

        public bool __webglInit { get; set; }

        public int __webglLineDistanceBuffer;

        public int __webglVertexBuffer;
        public int __webglNormalBuffer;
        public int __webglTangentBuffer;
        public int __webglColorBuffer;
        public int __webglUVBuffer;
        public int __webglUV2Buffer;

        public int __webglSkinIndicesBuffer;
        public int __webglSkinWeightsBuffer;

        public int __webglFaceBuffer;
        public int __webglLineBuffer;

        public List<int> __webglMorphTargetsBuffers;
        public List<int> __webglMorphNormalsBuffers;

        public object __sortArray;

        public float[] __vertexArray;
        public float[] __normalArray;
        public float[] __tangentArray;
        public float[] __colorArray;
        public float[] __uvArray;
        public float[] __uv2Array;
        public float[] __skinIndexArray;
        public float[] __skinWeightArray;
        public Type __typeArray;
        public ushort[] __faceArray;
        public ushort[] __lineArray;

        public List<float[]> __morphTargetsArrays;
        public List<float[]> __morphNormalsArrays;

        public int __webglFaceCount;
        public int __webglLineCount;
        public int __webglParticleCount;

        public List<Hashtable> __webglCustomAttributesList;

        public bool __inittedArrays;

        public float[] __lineDistanceArray;
    }
}
