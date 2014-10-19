namespace ThreeCs.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;

    using ThreeCs.Math;

    public class Geometry : BaseGeometry, ICloneable
    {
        protected static int GeometryIdCount;





        public Guid uuid = Guid.NewGuid();
        






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
            Id = GeometryIdCount++;

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
        public override void ApplyMatrix(Matrix4 matrix)
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

                face.Normal.Copy(cb);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaWeighted"></param>
        public void ComputeVertexNormals(bool areaWeighted = false)
        {
            var vertices = this.Vertices.ToArray();

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
                face.VertexNormals.Add((Vector3)vertices[face.a].Clone());
                face.VertexNormals.Add((Vector3)vertices[face.b].Clone());
                face.VertexNormals.Add((Vector3)vertices[face.c].Clone());
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
        public override void ComputeBoundingSphere()
        {

		    if ( this.BoundingSphere == null )
		    {
		        this.BoundingSphere = new Sphere();
		    }

            this.BoundingSphere.SetFromPoints(this.Vertices);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Merge(Geometry geometry, Matrix4 matrix = null)
        {
            Merge(geometry, matrix, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Merge(Geometry geometry, Matrix4 matrix, int materialIndexOffset)
        {
		    if (geometry == null)
		    {
		        Trace.TraceError("THREE.Geometry.merge(): geometry not an instance of THREE.Geometry.");
			    return;
		    }

            var vertexOffset = this.Vertices.Count;
            var vertices1 = this.Vertices;
            var vertices2 = geometry.Vertices;
            var faces1 = this.Faces;
            var faces2 = geometry.Faces;
            var uvs1 = this.FaceVertexUvs[0];
            var uvs2 = geometry.FaceVertexUvs[0];

            Matrix3 normalMatrix = null;

            if ( matrix != null ) {
			    normalMatrix = new Matrix3().GetNormalMatrix( matrix );
		    }

		    // vertices

		    for ( var i = 0; i < vertices2.Count; i ++ )
		    {
		        var vertex = vertices2[i];

		        var vertexCopy = (Vector3)vertex.Clone();

		        if (matrix != null) vertexCopy.ApplyMatrix4(matrix);

		        vertices1.Add(vertexCopy);
		    }

		    // faces

		    for (int i = 0; i < faces2.Count; i ++ )
		    {

		        var face = faces2[i];//, faceCopy, normal, color;
		        var faceVertexNormals = face.VertexNormals;
			    var faceVertexColors = face.VertexColors;

		        var faceCopy = new Face3(face.a + vertexOffset, face.b + vertexOffset, face.c + vertexOffset);
		        faceCopy.Normal.Copy(face.Normal);

			    if ( normalMatrix != null ) {
				    faceCopy.Normal.ApplyMatrix3( normalMatrix ).Normalize();
			    }

			    for ( var j = 0; j < faceVertexNormals.Count; j ++ ) {

				    var normal = (Vector3)faceVertexNormals[ j ].Clone();

				    if ( normalMatrix != null ) {
					    normal.ApplyMatrix3( normalMatrix ).Normalize();
				    }
				    faceCopy.VertexNormals.Add( normal );
			    }

                //faceCopy.color.Copy(face.color);
                faceCopy.color = face.color;

			    for ( var j = 0; j < faceVertexColors.Length; j ++ )
			    {
			        var color = faceVertexColors[j];
                    faceCopy.VertexColors[j] = color;
                }

			    faceCopy.MaterialIndex = face.MaterialIndex + materialIndexOffset;

			    faces1.Add( faceCopy );
		    }


		    // uvs

		    for (int i = 0; i < uvs2.Count; i++ )
		    {

		        var uv = uvs2[i]; var uvCopy = new List<Vector2>();

			    if ( uv == null ) continue;

			    for ( var j = 0; j < uv.Count; j ++ )
			    {
			        uvCopy.Add(new Vector2(uv[j].X, uv[j].Y));
			    }

		        uvs1.Add(uvCopy);

		    }
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
                    geometryGroup = new GeometryGroup { Faces3 = new List<int>(), MaterialIndex = materialIndex, Vertices = 0, NumMorphTargets = numMorphTargets, NumMorphNormals = numMorphNormals };
                    this.GeometryGroups.Add(groupHash, geometryGroup);
                    this.GeometryGroupsList.Add(geometryGroup);
                }

                if (this.GeometryGroups[groupHash].Vertices + 3 > maxVerticesInGroup)
                {
                }

                geometryGroup.Faces3.Add(i);
                geometryGroup.Vertices = geometryGroup.Vertices + 3;
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
