namespace ThreeCs.Core
{
    using System;
    using System.Collections.Generic;

    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Renderers.Shaders;

    public struct Offset
    {
        public int Start;
        public int Index;
        public int Count;
    }

    public class BufferGeometry : BaseGeometry, IAttributes // Note: in three.js, BufferGeometry is not Geometry
    {
        protected static int BufferGeometryIdCount;

        // IAttributes

        public Attributes Attributes { get; set; }

        //

        public IList<Offset> Drawcalls = new List<Offset>();

        public IList<Offset> Offsets; // backwards compatibility

        /// <summary>
        /// 
        /// </summary>
        public BufferGeometry()
        {
            Id = BufferGeometryIdCount++;

            // IAttributes
            this.Attributes = new Attributes();

            this.Offsets = this.Drawcalls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        public void AddAttribute(string name, ThreeCs.Renderers.Shaders.Attribute attribute)
        {
            this.Attributes[name] = attribute;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ComputeBoundingSphere()
        {
            var box = new Box3();

            if (this.BoundingSphere == null)
            {
                this.BoundingSphere = new Sphere();
            }

            var bufferAttribute = this.Attributes["position"] as BufferAttribute<float>;
            var positions = bufferAttribute.Array as float[];

            if (null  != positions)
            {
     	        box.MakeEmpty();

                var center = this.BoundingSphere.Center;

                for ( var i = 0; i < positions.Length; i += 3 )
                {
                    var vector = new Vector3(positions[i], positions[i + 1], positions[i + 2]);
                    box.ExpandByPoint(vector);
                }

                box.Center(center);
   
            	// hoping to find a boundingSphere with a radius smaller than the
                // boundingSphere of the boundingBox: sqrt(3) smaller in the best case

                var maxRadiusSq = float.NegativeInfinity;

                for ( var i = 0; i < positions.Length; i += 3 )
                {
                    var vector = new Vector3(positions[i], positions[i + 1], positions[i + 2]);
                    maxRadiusSq = Math.Max(maxRadiusSq, center.DistanceToSquared(vector));
                }

                this.BoundingSphere.Radius = (float)Math.Sqrt(maxRadiusSq);

                //if ()
                //{
                //    Trace.TraceError( "BufferGeometry.computeBoundingSphere(): Computed radius is NaN. The 'position' attribute is likely to have NaN values." );            
                //}
            }
        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public override void ApplyMatrix(Matrix4 matrix)
        {

		    if (this.Attributes.ContainsKey("position"))
		    {
                var position = (BufferAttribute<float>)this.Attributes["position"];

                matrix.ApplyToVector3Array(position.Array);
			    position.needsUpdate = true;
		    }

		    if (this.Attributes.ContainsKey("normal"))
		    {
                var normal = (BufferAttribute<float>)this.Attributes["normal"];

                var normalMatrix = new Matrix3().GetNormalMatrix(matrix);

		        normalMatrix.ApplyToVector3Array(normal.Array);
			    normal.needsUpdate = true;
		    }
	    }
    }
}
