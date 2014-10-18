using System;

namespace ThreeCs.Math
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using ThreeCs.Annotations;

    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}, W = {W}")]
    public class Quaternion : INotifyPropertyChanged
    {
        private float x;

        private float y;

        private float z;

        private float w;

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;

                OnPropertyChanged();
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;

                OnPropertyChanged();
            }
        }

        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;

                OnPropertyChanged();
            }
        }

        public float W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;

                OnPropertyChanged();
            }
        }

        internal void _UpdateEuler()
        {
            throw new NotImplementedException();
        }

        public Quaternion SetFromAxisAngle(Vector3 axis, float angle)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Quaternion()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.W = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Quaternion Identity()
        {
            return new Quaternion(0, 0, 0, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public Quaternion SetFromEuler(Euler euler)
        {
            var q = this;

            var c1 = (float)System.Math.Cos(euler.X / 2);
            var c2 = (float)System.Math.Cos(euler.Y / 2);
            var c3 = (float)System.Math.Cos(euler.Z / 2);
            var s1 = (float)System.Math.Sin(euler.X / 2);
            var s2 = (float)System.Math.Sin(euler.Y / 2);
            var s3 = (float)System.Math.Sin(euler.Z / 2);

            if (euler.Order == Euler.RotationOrder.XYZ)
            {

                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (euler.Order == Euler.RotationOrder.YXZ)
            {

                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;

            }
            else if (euler.Order == Euler.RotationOrder.ZXY)
            {

                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (euler.Order == Euler.RotationOrder.ZYX)
            {

                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;

            }
            else if (euler.Order == Euler.RotationOrder.YZX)
            {

                q.x = s1 * c2 * c3 + c1 * s2 * s3;
                q.y = c1 * s2 * c3 + s1 * c2 * s3;
                q.z = c1 * c2 * s3 - s1 * s2 * c3;
                q.w = c1 * c2 * c3 - s1 * s2 * s3;

            }
            else if (euler.Order == Euler.RotationOrder.XZY)
            {

                q.x = s1 * c2 * c3 - c1 * s2 * s3;
                q.y = c1 * s2 * c3 - s1 * c2 * s3;
                q.z = c1 * c2 * s3 + s1 * s2 * c3;
                q.w = c1 * c2 * c3 + s1 * s2 * s3;

            }

            this.OnPropertyChanged();

            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        public void SetFromRotationMatrix(Matrix4 m1)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

            // assumes the upper 3x3 of m is left pure rotation matrix (i.e, unscaled)

            var te = m1.elements;

            var m11 = te[0]; var m12 = te[4]; var m13 = te[8]; 
            var m21 = te[1]; var m22 = te[5]; var m23 = te[9]; 
            var m31 = te[2]; var m32 = te[6]; var m33 = te[10];

            var trace = m11 + m22 + m33;

            if (trace > 0)
            {
                var s = 0.5f / (float)System.Math.Sqrt(trace + 1.0);

                this.W = 0.25f / s;
                this.X = (m32 - m23) * s;
                this.Y = (m13 - m31) * s;
                this.Z = (m21 - m12) * s;
            }
            else if (m11 > m22 && m11 > m33)
            {
                var s = 2.0f * (float)System.Math.Sqrt(1.0 + m11 - m22 - m33);

                this.W = (m32 - m23) / s;
                this.X = 0.25f * s;
                this.Y = (m12 + m21) / s;
                this.Z = (m13 + m31) / s;
            }
            else if (m22 > m33)
            {
                var s = 2.0f * (float)System.Math.Sqrt(1.0 + m22 - m11 - m33);

                this.W = (m13 - m31) / s;
                this.X = (m12 + m21) / s;
                this.Y = 0.25f * s;
                this.Z = (m23 + m32) / s;
            }
            else
            {
                var s = 2.0f * (float)System.Math.Sqrt(1.0 + m33 - m11 - m22);

                this.W = (m21 - m12) / s;
                this.X = (m13 + m31) / s;
                this.Y = (m23 + m32) / s;
                this.Z = 0.25f * s;
            }

            this.OnPropertyChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        public void Copy(Quaternion quaternion)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="p"></param>
        public Quaternion Multiply(Quaternion q, Quaternion p = null)
        {
		    if ( p != null )
            {
			    Trace.TraceWarning( "THREE.Quaternion: .multiply() now only accepts one argument. Use .multiplyQuaternions( left, right ) instead." );
			    return this.MultiplyQuaternions( q, p );

		    }

		    return this.MultiplyQuaternions( this, q );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Quaternion MultiplyQuaternions ( Quaternion left, Quaternion right )
        {
		    // from http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/code/index.htm

		    var qax = left.x; var qay = left.y; var qaz = left.z; var qaw = left.w;
		    var qbx = right.x; var qby = right.y; var qbz = right.z; var qbw = right.w;

		    this.x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
		    this.y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
		    this.z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
		    this.w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

		    //this.onChangeCallback();

		    return this;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
