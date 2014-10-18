namespace ThreeCs.Math
{
    using System;
    using System.Collections.Generic;

    public class Box3
    {
        public Vector3 Min = new Vector3();
        public Vector3 Max = new Vector3();

        /// <summary>
        /// 
        /// </summary>
        public Box3()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Box3(Vector3 min, Vector3 max)
        {
            this.Min.Copy(min);
            this.Max.Copy(max);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MakeEmpty()
        {
            this.Min.X = this.Min.Y = this.Min.Z = float.PositiveInfinity;
            this.Max.X = this.Max.Y = this.Max.Z = float.NegativeInfinity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Vector"></param>
        public void AddPoint(Vector3 Vector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Box3	ExpandByPoint (Vector3 point) 
        {
            this.Min.Min(point);
            this.Max.Max(point);

		    return this;
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public Box3 SetFromPoints(List<Vector3> points)
        {
            this.MakeEmpty();

            foreach (Vector3 t in points)
            {
                this.ExpandByPoint(t);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        public void Center(Vector3 center)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public void ApplyMatrix4(Matrix3 matrix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public void ApplyMatrix4(Matrix4 matrix)
        {
            throw new NotImplementedException();
        }
    }
}
