
namespace ThreeCs.Math
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using ThreeCs.Annotations;
    using ThreeCs.Cameras;

    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}")]
    public class Vector3 : IEquatable<Vector3>, ICloneable, INotifyPropertyChanged
    {
        private float x;

        private float y;

        private float z;

        public object UserData;

        /// <summary>
        /// 
        /// </summary>
        public Vector3()
        {
            this.X = this.Y = this.Z = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Scalar"></param>
        public Vector3(float Scalar)
        {
            this.X = this.Y = this.Z = Scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Defines a unit-length Vector3 that points towards the X-axis.
        /// </summary>
        public static readonly Vector3 UnitX = new Vector3(1, 0, 0);

        /// <summary>
        /// Defines a unit-length Vector3 that points towards the Y-axis.
        /// </summary>
        public static readonly Vector3 UnitY = new Vector3(0, 1, 0);

        /// <summary>
        /// /// Defines a unit-length Vector3 that points towards the Z-axis.
        /// </summary>
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);

        /// <summary>
        /// Defines a zero-length Vector3.
        /// </summary>
   //     public static readonly Vector3 Zero = new Vector3(0, 0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
   //     public static readonly Vector3 One = new Vector3(1, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Vector3 SetLength(float l)
        {
            Length = l;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public float Length
        {
            get 
            { 
                return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
            set
            {
                var OldLength = (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

                if (OldLength != 0 && value != OldLength)
                {
                    this.MultiplyScalar(value / OldLength);
                }
            }
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Vector3(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public Vector3 Set(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector3 Add(Vector3 v, Vector3 w = null)
        {
            if (w != null)
            {
                Trace.TraceWarning("THREE.Vector3: .add() now only accepts one argument. Use .addVectors( a, b ) instead.");
                return this.AddVectors(v, w);
            }

            this.x += v.x;
            this.y += v.y;
            this.z += v.z;

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Vector3 Sub(Vector3 v, Vector3 w = null)
        {
            if (w != null)
            {
                Trace.TraceWarning("THREE.Vector3: .add() now only accepts one argument. Use .addVectors( a, b ) instead.");
                return this.SubVectors(v, w);
            }

            this.x -= v.x;
            this.y -= v.y;
            this.z -= v.z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 One()
        {
            return new Vector3(1, 1, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetComponent(int index, float value)
        {
            switch (index)
            {
                case 0:
                    this.X = value;
                    break;
                case 1:
                    this.Y = value;
                    break;
                case 2:
                    this.Z = value;
                    break;

                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", index));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetComponent(int index)
        {
            switch (index)
            {
                case 0:
                    return this.X;
                case 1:
                    return this.Y;
                case 2:
                    return this.Z;

                default:
                    throw new IndexOutOfRangeException(String.Format("Index {0} is out of bounds", index));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public Vector3 Copy(Vector3 another)
        {
            this.X = another.X;
            this.Y = another.Y;
            this.Z = another.Z;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X + right.X, Y = left.Y + right.Y, Z = left.Z + right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X - right.X, Y = left.Y - right.Y, Z = left.Z - right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X * right.X, Y = left.Y * right.Y, Z = left.Z * right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            return new Vector3 { X = left.X / right.X, Y = left.Y / right.Y, Z = left.Z / right.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Scalar"></param>
        /// <returns></returns>
        public Vector3 AddScalar(float Scalar)
        {
            this.X += Scalar;
            this.Y += Scalar;
            this.Z += Scalar;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 AddVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X + right.X;
            this.Y = left.Y + right.Y;
            this.Z = left.Z + right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 SubVectors(Vector3 left, Vector3 right)
        {
            return SubtractVectors(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 SubtractVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X - right.X;
            this.Y = left.Y - right.Y;
            this.Z = left.Z - right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector3 MultiplyScalar(float scalar)
        {
            this.X *= scalar;
            this.Y *= scalar;
            this.Z *= scalar;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 MultiplyVectors(Vector3 left, Vector3 right)
        {
            this.X = left.X * right.X;
            this.Y = left.Y * right.Y;
            this.Z = left.Z * right.Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ApplyMatrix3(Matrix3 matrix)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var E = matrix.Elements;

            this.X = E[0] * X + E[3] * Y + E[6] * Z;
            this.Y = E[1] * X + E[4] * Y + E[7] * Z;
            this.Z = E[2] * X + E[5] * Y + E[8] * Z;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ApplyMatrix4(Matrix4 matrix)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var E = matrix.Elements;

            this.X = E[0] * X + E[4] * Y + E[8] * Z + E[12];
            this.Y = E[1] * X + E[5] * Y + E[9] * Z + E[13];
            this.Z = E[2] * X + E[6] * Y + E[10] * Z + E[14];
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ApplyProjection(Matrix4 matrix)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            var e = matrix.Elements;
            var d = 1 / (e[3] * X + e[7] * Y + e[11] * Z + e[15]); // perspective divide

            this.X = (e[0] * X + e[4] * Y + e[8] * Z + e[12]) * d;
            this.Y = (e[1] * X + e[5] * Y + e[9] * Z + e[13]) * d;
            this.Z = (e[2] * X + e[6] * Y + e[10] * Z + e[14]) * d;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public Vector3 ApplyQuaternion(Quaternion quaternion)
        {
            var X = this.X;
            var Y = this.Y;
            var Z = this.Z;

            var qX = quaternion.X;
            var qY = quaternion.Y;
            var qZ = quaternion.Z;
            var qw = quaternion.W;

            // calculate quat * vector

            var IX = qw * X + qY * Z - qZ * Y;
            var IY = qw * Y + qZ * X - qX * Z;
            var IZ = qw * Z + qX * Y - qY * X;
            var IW = -qX * X - qY * Y - qZ * Z;

            // calculate result * inverse quat

            this.X = IX * qw + IW * -qX + IY * -qZ - IZ * -qY;
            this.Y = IY * qw + IW * -qY + IZ * -qX - IX * -qZ;
            this.Z = IZ * qw + IW * -qZ + IX * -qY - IY * -qX;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Reflect(Vector3 vector)
        {
            var v = new Vector3();
            v.Copy(this).ProjectOnVector(vector).MultiplyScalar(2);
            return this.SubtractVectors(v, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public Vector3 Unproject(Camera camera)
        {
            var matrix = new Matrix4();

            matrix.MultiplyMatrices(camera.MatrixWorld, matrix.GetInverse(camera.ProjectionMatrix));
            return this.ApplyProjection(matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 TransformDirection(Matrix4 matrix)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            var e = matrix.Elements;

            this.X = e[0] * X + e[4] * Y + e[8] * Z;
            this.Y = e[1] * X + e[5] * Y + e[9] * Z;
            this.Z = e[2] * X + e[6] * Y + e[10] * Z;

            this.Normalize();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Vector3 DivideScalar(float scalar)
        {
            if (scalar == 0)
            {
                this.Set(0f, 0f, 0f);
            }
            else
            {
                var invScalar = 1 / scalar;
                this.X *= invScalar;
                this.Y *= invScalar;
                this.Z *= invScalar;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Min(Vector3 vector)
        {
            if (this.X > vector.X)
            {
                this.X = vector.X;
            }
            if (this.Y > vector.Y)
            {
                this.X = vector.Y;
            }
            if (this.Z > vector.Z)
            {
                this.X = vector.Z;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Max(Vector3 vector)
        {
            if (this.X < vector.X)
            {
                this.X = vector.X;
            }
            if (this.Y < vector.Y)
            {
                this.X = vector.Y;
            }
            if (this.Z < vector.Z)
            {
                this.X = vector.Z;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Vector3 Clamp(Vector3 min, Vector3 max)
        {
            // This function assumes Min < Max, if this assumption isn't true it will not operate correctlY
            if (this.X < min.X)
            {
                this.X = min.X;
            }
            else if (this.X > max.X)
            {
                this.X = max.X;
            }

            if (this.Y < min.Y)
            {
                this.Y = min.Y;
            }
            else if (this.Y > max.Y)
            {
                this.Y = max.Y;
            }

            if (this.Z < min.Z)
            {
                this.Z = min.Z;
            }
            else if (this.Z > max.Z)
            {
                this.Z = max.Z;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Negate()
        {
            return this.MultiplyScalar(-1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float Dot(Vector3 vector)
        {
            return (this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float LengthSq()
        {
            return (this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float LengthManhattan()
        {
            return (Math.Abs(this.X) + Math.Abs(this.Y) + Math.Abs(this.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 Normalize()
        {
            return this.DivideScalar(this.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public Vector3 Lerp(Vector3 vector, float alpha)
        {
            this.X += (vector.X - this.X) * alpha;
            this.Y += (vector.Y - this.Y) * alpha;
            this.Z += (vector.Z - this.Z) * alpha;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Cross(Vector3 vector)
        {
            float X = this.X, Y = this.Y, Z = this.Z;

            this.X = Y * vector.Z - Z * vector.Y;
            this.Y = Z * vector.X - X * vector.Z;
            this.Z = X * vector.Y - Y * vector.X;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Vector3 CrossVectors(Vector3 left, Vector3 right)
        {
            float aX = left.X, aY = left.Y, aZ = left.Z;
            float bX = right.X, bY = right.Y, bZ = right.Z;

            this.X = aY * bZ - aZ * bY;
            this.Y = aZ * bX - aX * bZ;
            this.Z = aX * bY - aY * bX;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float AngleTo(Vector3 vector)
        {
            var theta = this.Dot(vector) / (this.Length * vector.Length);
            return (float)Math.Acos(theta.Clamp(-1, 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceTo(Vector3 vector)
        {
            return (float)Math.Sqrt(this.DistanceToSquared(vector));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DistanceToSquared(Vector3 vector)
        {
            var dX = this.X - vector.X;
            var dY = this.Y - vector.Y;
            var dZ = this.Z - vector.Z;

            return dX * dX + dY * dY + dZ * dZ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 SetFromMatrixPosition(Matrix4 matrix)
        {
            this.X = matrix.Elements[12];
            this.Y = matrix.Elements[13];
            this.Z = matrix.Elements[14];

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 SetValue(string i, float value)
        {
            switch (i)
            {
                case "x":
                    this.X = value;
                    break;
                case "y":
                    this.Y = value;
                    break;
                case "z":
                    this.Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("i can only be x,y or z");
                    break;
            }
            
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 SetFromMatrixScale(Matrix3 matrix)
        {
            var sx = this.Set(matrix.Elements[0], matrix.Elements[1], matrix.Elements[2]).Length;
            var sy = this.Set(matrix.Elements[4], matrix.Elements[5], matrix.Elements[6]).Length;
            var sz = this.Set(matrix.Elements[8], matrix.Elements[9], matrix.Elements[10]).Length;

            this.X = sx;
            this.Y = sy;
            this.Z = sz;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 SetFromMatrixColumn(int index, Matrix4 matrix)
        {
            var Offset = index * 4;

            var MElements = matrix.Elements;

            this.X = MElements[Offset];
            this.Y = MElements[Offset + 1];
            this.Z = MElements[Offset + 2];

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(Vector3 vector)
        {
            return ((vector.X == this.X) && (vector.Y == this.Y) && (vector.Z == this.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 FromArray(float[] source, int offset = 0)
        {
            this.X = source[offset];
            this.Y = source[offset + 1];
            this.Z = source[offset + 2];

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            return new[] { this.X, this.Y, this.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 ProjectOnVector(Vector3 vector)
        {
            var v1 = new Vector3();
            v1.Copy(vector).Normalize();
            var dot = this.Dot(v1);

            return this.Copy(v1).MultiplyScalar(dot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public Vector3 ApplyEuler(Euler euler)
        {
            this.ApplyQuaternion(new Quaternion().SetFromEuler(euler));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3 ApplyAxisAngle(Vector3 axis, float angle)
        {
            var quaternion = new Quaternion();
            this.ApplyQuaternion(quaternion.SetFromAxisAngle(axis, angle));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        public Vector3 ProjectOnPlane(Vector3 planeNormal)
        {
            var vector = new Vector3();
            vector.Copy(this).ProjectOnVector(planeNormal);
            return this - vector;
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