namespace ThreeCs.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Materials;

    public class WebGlObject
    {
        public long id;

 //       public GeometryGroup buffer;
        public object buffer;

        public Object3D object3D;

        public Material material;

        public float z;

        public bool render;
    }

    public class GeometryGroup
    {
        public long id;

        public List<int> faces3;

        public long materialIndex;

        public long vertices;

        public int numMorphTargets;

        public int numMorphNormals;

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

    }

    
    public class Geometry : ICloneable
    {
        protected static int GeometryIdCount;

        public int id = GeometryIdCount++;

        public Guid uuid = Guid.NewGuid();
        
        public string name;

        // Dynamic attributesLocation and object
        public bool __webglInit = false;

            public int __webglVertexBuffer;

            public int __webglColorBuffer;

            public int __webglLineDistanceBuffer;

            public float[] __vertexArray;

            public float[] __colorArray;

            public float[] __lineDistanceArray;

            public int __webglLineCount;

            public int __webglParticleCount;

            public object __sortArray;

            public List<Hashtable> __webglCustomAttributesList;

        public Dictionary<string, GeometryGroup> GeometryGroups;

        public List<GeometryGroup> GeometryGroupsList;

        public List<Vector3> Vertices = new List<Vector3>();

        public List<Color> Colors = new List<Color>(); // one-to-one vertex colors, used in Points and Line

        public List<Face3> Faces = new List<Face3>();

        public List<List<List<Vector2>>> FaceVertexUvs = new List<List<List<Vector2>>>();

        public List<GeometryGroup> MorphTargets = new List<GeometryGroup>();

        public List<Color> MorphColors = new List<Color>();

        public List<Vector3> MorphNormals = new List<Vector3>();

        public List<Vector4> SkinWeights = new List<Vector4>();

        public List<Vector4> SkinIndices = new List<Vector4>();

        public List<float> LineDistances = new List<float>();

        public Object3D BoundingBox = null;

        public Sphere BoundingSphere = null;

        public bool HasTangents = false;

        public bool Dynamic = true; // the intermediate typed arrays will be deleted when set to false

        public bool MorphTargetsNeedUpdate;

        // update flags
        
        public bool VerticesNeedUpdate = false;

        public bool ElementsNeedUpdate = false;

        public bool UvsNeedUpdate = false;

        public bool NormalsNeedUpdate = false;

        public bool TangentsNeedUpdate = false;

        public bool ColorsNeedUpdate = false;

        public bool LineDistancesNeedUpdate = false;

        public bool BuffersNeedUpdate = false;

        public bool GroupsNeedUpdate = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Geometry()
        {
            this.FaceVertexUvs.Add(new List<List<Vector2>>());
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Geometry(Geometry other)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public virtual void ApplyMatrix(Matrix4 matrix)
        {
            var normalMatrix = new Matrix3().GetNormalMatrix(matrix);

            foreach (var vertex in this.Vertices)
            {
                vertex.ApplyMatrix4(matrix);
            }

            foreach (var face in this.Faces)
            {
                face.Normal.ApplyMatrix3(normalMatrix).Normalize();

                foreach (Vector3 vertexNormal in face.VertexNormals)
                {
                    vertexNormal.ApplyMatrix3(normalMatrix).Normalize();
                }
            }

            if (this.BoundingBox is Box3)
            {
                this.ComputeBoundingBox();
            }

            if (this.BoundingSphere is Sphere)
            {
                this.ComputeBoundingSphere();
            }
        }

        public Vector3 Center()
        {
            this.ComputeBoundingBox();

            var offset = new Vector3();

            //offset.AddVectors(this.BoundingBox.Min, this.BoundingBox.max);
            //offset.MultiplyScalar(-0.5f);

            //this.ApplyMatrix(new Matrix4().MakeTranslation(offset.X, offset.Y, offset.Z));
            //this.ComputeBoundingBox();

            return offset;
        }

        public void ComputeFaceNormals()
        {
            var cb = new Vector3(); var ab = new Vector3();

            foreach (var face in this.Faces)
            {
                var vA = this.Vertices[face.a];
                var vB = this.Vertices[face.b];
                var vC = this.Vertices[face.c];

                cb.SubVectors(vC, vB);
                ab.SubVectors(vA, vB);
                cb.Cross(ab);

                cb.Normalize();

                face.Normal = cb;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaWeighted"></param>
        public void ComputeVertexNormals(bool areaWeighted = false)
        {
            IList<Vector3> vertices = new Vector3[this.Vertices.Count];

            for (int v = 0; v < this.Vertices.Count; v++)
            {
                vertices[v] = new Vector3();
            }

            if (areaWeighted)
            {

                // vertex normals weighted by triangle areas
                // http://www.iquilezles.org/www/articles/normals/normals.htm

                var cb = new Vector3();
                var ab = new Vector3();
                var db = new Vector3();
                var dc = new Vector3();
                var bc = new Vector3();

                foreach (var face in this.Faces)
                {
                    var vA = this.Vertices[face.a];
                    var vB = this.Vertices[face.b];
                    var vC = this.Vertices[face.c];

                    cb.SubVectors(vC, vB);
                    ab.SubVectors(vA, vB);
                    cb.Cross(ab);

                    vertices[face.a] = cb;
                    vertices[face.b] = cb;
                    vertices[face.c] = cb;
                }
            }
            else
            {
                foreach (var face in this.Faces)
                {
                    vertices[face.a] = face.Normal;
                    vertices[face.b] = face.Normal;
                    vertices[face.c] = face.Normal;

                }
            }

            for (int v = 0; v < this.Vertices.Count; v ++)
            {
                vertices[v].Normalize();
            }

            foreach (var face in this.Faces)
            {
                face.VertexNormals.Add(vertices[face.a]);
                face.VertexNormals.Add(vertices[face.b]);
                face.VertexNormals.Add(vertices[face.c]);
            }
        }

        public void ComputeMorphNormals()
        {
            throw new NotImplementedException();
        }

        public void ComputeTangents()
        {
            throw new NotImplementedException();
        }

        public void ComputeLineDistances()
        {
            throw new NotImplementedException();
        }

        public void ComputeBoundingBox()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ComputeBoundingSphere()
        {

		    if ( this.BoundingSphere == null ) {

                this.BoundingSphere = new Sphere();

		    }

            this.BoundingSphere.SetFromPoints(this.Vertices);
        }

        public void Merge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int MergeVertices()
        {
            var verticesMap = new Dictionary<string, int>(); // Hashmap for looking up vertice by position coordinates (and making sure they are unique)
            var unique = new List<Vector3>();
            var changes = new List<int>();

            var precisionPoints = 4; // number of decimal points, eg. 4 for epsilon of 0.0001
            var precision = Math.Pow( 10, precisionPoints );

	        for (var i = 0; i < this.Vertices.Count; i++)
	        {
	            var v = this.Vertices[i];

                var key = Math.Round( v.X * precision ) + "_" + Math.Round( v.Y * precision ) + "_" + Math.Round( v.Z * precision );

	            var value = 0;
                if (!verticesMap.TryGetValue(key, out value))
                {
                    verticesMap[key] = i;
                    unique.Add(v);
                    changes.Add(unique.Count - 1);
                } 
                else
                {
                    var idx = verticesMap[key];
                    changes.Add(changes[idx]);
                }
            }

            // if faces are completely degenerate after merging vertices, we
            // have to remove them from the geometry.

            var faceIndicesToRemove = new List<int>();

            for (var i = 0; i < this.Faces.Count; i++)
            {
                var face = this.Faces[i];

                face.a = changes[ face.a ];
                face.b = changes[ face.b ];
                face.c = changes[ face.c ];

                var indices = new[]{face.a, face.b, face.c };

                var dupIndex = - 1;

                // if any duplicate vertices are found in a Face3
                // we have to remove the face as nothing can be saved
                for ( var n = 0; n < 3; n ++ ) 
                {
                    if ( indices[n] == indices[ ( n + 1 ) % 3 ] ) 
                    {
                        dupIndex = n;
                        faceIndicesToRemove.Add(i);
                        break;
                    }
                }
            }

            foreach (var idx in faceIndicesToRemove)
            {
                this.Faces.RemoveAt(idx);

                for (var j = 0; j < this.FaceVertexUvs.Count; j++)
                {
                    this.FaceVertexUvs[j].RemoveAt(idx);
                }
            }

            // Use unique set of vertices
            var diff = this.Vertices.Count - unique.Count;
            this.Vertices = unique;

            return diff;
        }

        private struct Hash
        {
            public int hash;
            public int counter;
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeGroups(bool usesFaceMaterial, long maxVerticesInGroup)
        {
            var geometryGroupCounter = 0;

            var hash_map = new Dictionary<int, Hash>();

            var numMorphTargets = this.MorphTargets.Count;
            var numMorphNormals = this.MorphNormals.Count;

            this.GeometryGroups = new Dictionary<string, GeometryGroup>();
            this.GeometryGroupsList = new List<GeometryGroup>();

            for (var i = 0; i < this.Faces.Count; i++)
            {
                var face = this.Faces[0];

                var materialIndex = (usesFaceMaterial) ? face.MaterialIndex : 0;

                Hash aa;
                if (!hash_map.TryGetValue(materialIndex, out aa))
                {
                    hash_map[ materialIndex ] = new Hash { hash = materialIndex, counter = 0 };
                }

                var groupHash = hash_map[materialIndex].hash + "_" + hash_map[materialIndex].counter;

                GeometryGroup geometryGroup = null;
                if (!this.GeometryGroups.TryGetValue(groupHash, out geometryGroup))
                {
                    geometryGroup = new GeometryGroup { id = geometryGroupCounter++, faces3 = new List<int>(), materialIndex = materialIndex, vertices = 0, numMorphTargets = numMorphTargets, numMorphNormals = numMorphNormals };
                    this.GeometryGroups.Add(groupHash, geometryGroup);
                    this.GeometryGroupsList.Add(geometryGroup);
                }

                if (this.GeometryGroups[groupHash].vertices + 3 > maxVerticesInGroup)
                {
                }

                geometryGroup.faces3.Add(i);
                geometryGroup.vertices = geometryGroup.vertices + 3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Geometry(this);
        }

    }
}
