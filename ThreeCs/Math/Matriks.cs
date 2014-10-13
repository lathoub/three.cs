namespace ThreeCs.Math
{
    using System;
    using System.Diagnostics;
    using System.Drawing;



    public static class Mat
    {
        public static double RadToDeg(double rad)
        {
            return (rad * 180.0 / System.Math.PI);
        }

        public static double DegToRad(double deg)
        {
            return (System.Math.PI * deg / 180.0);
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static Color Random(this Color value)
        {
            var random = new Random();

            return Color.FromArgb(255, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Matriks
    {
        /// <summary>
        /// 
        /// </summary>
        public static Vector3 SetFromMatrixPosition(Matrix4 m)
        {
            var v = new Vector3();

            v.X = m.M41;
            v.Y = m.M42;
            v.Z = m.M43;

            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix3 GetInverse(Matrix4 m)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix3 GetNormalMatrix(Matrix4 m)
        {
            var matrix3 = new Matrix3();

            var inverse = GetInverse(m);

            return Transpose(inverse);

        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 Transpose(Matrix4 m)
        {
            var tmp, m = this.elements;

            tmp = m[1]; m[1] = m[3]; m[3] = tmp;
            tmp = m[2]; m[2] = m[6]; m[6] = tmp;
            tmp = m[5]; m[5] = m[7]; m[7] = tmp;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deze"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 SubVectors(Vector3 deze, Vector3 a, Vector3 b)
        {
            deze.X = a.X - b.X;
            deze.Y = a.Y - b.Y;
            deze.Z = a.Z - b.Z;

            return deze;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deze"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 deze, Vector3 v)
        {
            var x = deze.X; var y = deze.Y; var z = deze.Z;

            deze.X = y * v.Z - z * v.Y;
            deze.Y = z * v.X - x * v.Z;
            deze.Z = x * v.Y - y * v.X;

            return deze;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deze"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 CrossVectors(Vector3 deze, Vector3 a, Vector3 b)
        {
            var ax = a.X; var ay = a.Y; var az = a.Z;
            var bx = b.X; var by = b.Y; var bz = b.Z;

            deze.X = ay * bz - az * by;
            deze.Y = az * bx - ax * bz;
            deze.Z = ax * by - ay * bx;

            return deze;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="te"></param>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            var x = Vector3.Zero;
            var y = Vector3.Zero;
            var z = Vector3.Zero;

            z = SubVectors(z, eye, target);
            z.Normalize();

			if ( z.Length == 0 ) {
				z.Z = 1;
			}

            x = CrossVectors(x, up, z);
            x.Normalize();

            if (x.Length == 0)
            {
				z.X += 0.0001f;
			    x = CrossVectors(x, up, z);
                x.Normalize();
			}

            y = CrossVectors(y, z, x);
            y.Normalize();

            var te = Matrix4.Identity;
			te.M11 = x.X; te.M21 = y.X; te.M31 = z.X;
			te.M12 = x.Y; te.M22 = y.Y; te.M32 = z.Y;
			te.M13 = x.Z; te.M23 = y.Z; te.M33 = z.Z;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        public static Matrix4 MakeRotationFromEuler(Euler euler)
        {
            var m = new Matrix4();

            var x = euler.X; var y = euler.Y; var z = euler.Z;
            var a = (float)Math.Cos(x); var b = (float)Math.Sin(x);
            var c = (float)Math.Cos(y); var d = (float)Math.Sin(y);
            var e = (float)Math.Cos(z); var f = (float)Math.Sin(z);
  
            if ( euler.Order == Euler.RotationOrders.XYZ )
            {
			    var ae = a * e; var af = a * f; var be = b * e; var bf = b * f;

			    m.M11 = c * e;
			    m.M21 = - c * f;
			    m.M31 = d;

			    m.M12 = af + be * d;
			    m.M22 = ae - bf * d;
			    m.M32 = - b * c;

			    m.M13 = bf - ae * d;
			    m.M23 = be + af * d;
			    m.M33 = a * c;
		    }
            else if (euler.Order == Euler.RotationOrders.YXZ)
            {
                throw new NotImplementedException();
            }
            else if (euler.Order == Euler.RotationOrders.ZXY)
            {
                throw new NotImplementedException();
            }
            else if (euler.Order == Euler.RotationOrders.ZYX)
            {
                throw new NotImplementedException();
            }
            else if (euler.Order == Euler.RotationOrders.YZX)
            {
                throw new NotImplementedException();
            }
            else if (euler.Order == Euler.RotationOrders.XZY)
            {
                throw new NotImplementedException();
            }

            // last column
            m.M14 = 0;
            m.M24 = 0;
            m.M34 = 0;

            // bottom row
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            m.M44 = 1;

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="order"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static Euler SetFromQuaternion(Quaternion q, Euler.RotationOrders order = Euler.RotationOrders.XYZ, bool update = false)
        {
            var euler = new Euler();

            // q is assumed to be normalized

            // http://www.mathworks.com/matlabcentral/fileexchange/20696-function-to-convert-between-dcm-euler-angles-quaternions-and-euler-vectors/content/SpinCalc.m

            var sqx = q.X * q.X;
            var sqy = q.Y * q.Y;
            var sqz = q.Z * q.Z;
            var sqw = q.W * q.W;

            if (order == Euler.RotationOrders.XYZ)
            {

                euler.X = (float)Math.Atan2(2 * (q.X * q.W - q.Y * q.Z), (sqw - sqx - sqy + sqz));
                euler.Y = (float)Math.Asin((2 * (q.X * q.Z + q.Y * q.W).Clamp(-1, 1)));
                euler.Z = (float)Math.Atan2(2 * (q.Z * q.W - q.X * q.Y), (sqw + sqx - sqy - sqz));

            }
            else if (order == Euler.RotationOrders.YXZ)
            {

                euler.X = (float)Math.Asin((2 * (q.X * q.W - q.Y * q.Z).Clamp(-1, 1)));
                euler.Y = (float)Math.Atan2(2 * (q.X * q.Z + q.Y * q.W), (sqw - sqx - sqy + sqz));
                euler.Z = (float)Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), (sqw - sqx + sqy - sqz));

            }
            else if (order == Euler.RotationOrders.ZXY)
            {

                euler.X = (float)Math.Asin((2 * (q.X * q.W + q.Y * q.Z).Clamp(-1, 1)));
                euler.Y = (float)Math.Atan2(2 * (q.Y * q.W - q.Z * q.X), (sqw - sqx - sqy + sqz));
                euler.Z = (float)Math.Atan2(2 * (q.Z * q.W - q.X * q.Y), (sqw - sqx + sqy - sqz));

            }
            else if (order == Euler.RotationOrders.ZYX)
            {

                euler.X = (float)Math.Atan2(2 * (q.X * q.W + q.Z * q.Y), (sqw - sqx - sqy + sqz));
                euler.Y = (float)Math.Asin((2 * (q.Y * q.W - q.X * q.Z).Clamp(-1, 1)));
                euler.Z = (float)Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), (sqw + sqx - sqy - sqz));

            }
            else if (order == Euler.RotationOrders.YZX)
            {

                euler.X = (float)Math.Atan2(2 * (q.X * q.W - q.Z * q.Y), (sqw - sqx + sqy - sqz));
                euler.Y = (float)Math.Atan2(2 * (q.Y * q.W - q.X * q.Z), (sqw + sqx - sqy - sqz));
                euler.Z = (float)Math.Asin((2 * (q.X * q.Y + q.Z * q.W).Clamp(-1, 1)));

            }
            else if (order == Euler.RotationOrders.XZY)
            {

                euler.X = (float)Math.Atan2(2 * (q.X * q.W + q.Y * q.Z), (sqw - sqx + sqy - sqz));
                euler.Y = (float)Math.Atan2(2 * (q.X * q.Z + q.Y * q.W), (sqw + sqx - sqy - sqz));
                euler.Z = (float)Math.Asin((2 * (q.Z * q.W - q.X * q.Y).Clamp(-1, 1)));

            }

            euler.Order = order;

            //	if ( update !== false ) this.onChangeCallback();

            return euler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Quaternion SetFromRotationMatrix(Matrix4 me)
        {
            var q = new Quaternion();

            // assumes the upper 3x3 of m is a pure rotation matrix (i.e, unscaled)

            var m11 = me.M11; var m12 = me.M21; var m13 = me.M31;
            var m21 = me.M12; var m22 = me.M22; var m23 = me.M32;
            var m31 = me.M13; var m32 = me.M23; var m33 = me.M33;

            var trace = m11 + m22 + m33;

            if (trace > 0)
            {
                var s = 0.5f / (float)Math.Sqrt(trace + 1.0);

                q.W = 0.25f / s;
                q.X = (m32 - m23) * s;
                q.Y = (m13 - m31) * s;
                q.Z = (m21 - m12) * s;

            }
            else if (m11 > m22 && m11 > m33)
            {
                var s = 2.0f * (float)Math.Sqrt(1.0 + m11 - m22 - m33);

                q.W = (m32 - m23) / s;
                q.X = 0.25f * s;
                q.Y = (m12 + m21) / s;
                q.Z = (m13 + m31) / s;

            }
            else if (m22 > m33)
            {
                var s = 2.0f * (float)Math.Sqrt(1.0 + m22 - m11 - m33);

                q.W = (m13 - m31) / s;
                q.X = (m12 + m21) / s;
                q.Y = 0.25f * s;
                q.Z = (m23 + m32) / s;

            }
            else
            {
                var s = 2.0f * (float)Math.Sqrt(1.0 + m33 - m11 - m22);

                q.W = (m21 - m12) / s;
                q.X = (m13 + m31) / s;
                q.Y = (m23 + m32) / s;
                q.Z = 0.25f * s;

            }

            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 Multi(Matrix4 ae, Matrix4 be)
        {
            var te = new Matrix4();

            var a11 = ae.M11; var  a12 = ae.M21; var  a13 = ae.M31; var  a14 = ae.M41;
            var a21 = ae.M12; var  a22 = ae.M22; var  a23 = ae.M32; var  a24 = ae.M42;
            var a31 = ae.M13; var  a32 = ae.M23; var  a33 = ae.M33; var  a34 = ae.M43;
            var a41 = ae.M14; var  a42 = ae.M24; var  a43 = ae.M34; var  a44 = ae.M44;

            var b11 = be.M11; var  b12 = be.M21; var  b13 = be.M31; var  b14 = be.M41;
            var b21 = be.M12; var  b22 = be.M22; var  b23 = be.M32; var  b24 = be.M42;
            var b31 = be.M13; var  b32 = be.M23; var  b33 = be.M33; var  b34 = be.M43;
            var b41 = be.M14; var  b42 = be.M24; var  b43 = be.M34; var  b44 = be.M44;

            te.M11 = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            te.M21 = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            te.M31 = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            te.M41 = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            te.M12 = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            te.M22 = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            te.M32 = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            te.M42 = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            te.M13 = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            te.M23 = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            te.M33 = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            te.M43 = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            te.M14 = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            te.M24 = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            te.M34 = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            te.M44 = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 ApplyProjection(Vector3 v, Matrix4 me)
        {
            var x = v.X; var y = v.Y; var z = v.Z;

            var d = 1 / (me.M14 * x + me.M24 * y + me.M34 * z + me.M44); // perspective divide

            v.X = (me.M11 * x + me.M21 * y + me.M31 * z + me.M41) * d;
            v.Y = (me.M12 * x + me.M22 * y + me.M32 * z + me.M42) * d;
            v.Z = (me.M13 * x + me.M23 * y + me.M33 * z + me.M43) * d;

            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 MakeOrthographic(float left, float right, float top, float bottom, float near, float far)
        {
            var te = new Matrix4();

            var w = right - left;
            var h = top - bottom;
            var p = far - near;

            var x = (right + left) / w;
            var y = (top + bottom) / h;
            var z = (far + near) / p;

            throw new NotImplementedException();

            //te[0] = 2 / w; te[4] = 0; te[8] = 0; te[12] = -x;
            //te[1] = 0; te[5] = 2 / h; te[9] = 0; te[13] = -y;
            //te[2] = 0; te[6] = 0; te[10] = -2 / p; te[14] = -z;
            //te[3] = 0; te[7] = 0; te[11] = 0; te[15] = 1;

            return te;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 MakeFrustum(float left, float right, float bottom, float top, float near, float far)
        {
            var te = new Matrix4();

            var x = 2 * near / (right - left);
            var y = 2 * near / (top - bottom);

            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = -(far + near) / (far - near);
            var d = -2 * far * near / (far - near);

            te.M11 = x; te.M21 = 0; te.M31 = a; te.M41 = 0;
            te.M12 = 0; te.M22 = y; te.M32 = b; te.M42 = 0;
            te.M13 = 0; te.M23 = 0; te.M33 = c; te.M43 = d;
            te.M14 = 0; te.M24 = 0; te.M34 = -1; te.M44 = 0;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 MakePerspective(float fov, float aspect, float near, float far)
        {
            var rad = Mat.DegToRad(fov * 0.5f);

            var ymax = near * (float)System.Math.Tan(rad); // An angle, measured in radians
            var ymin = -ymax;
            var xmin = ymin * aspect;
            var xmax = ymax * aspect;

            return MakeFrustum(xmin, xmax, ymin, ymax, near, far);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="te"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Matrix4 MultiplyScalar(Matrix4 te, float s)
        {
            te.M11 *= s; te.M21 *= s; te.M31 *= s; te.M41 *= s;
            te.M12 *= s; te.M22 *= s; te.M32 *= s; te.M42 *= s;
            te.M13 *= s; te.M23 *= s; te.M33 *= s; te.M43 *= s;
            te.M14 *= s; te.M24 *= s; te.M34 *= s; te.M44 *= s;

            return te;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Matrix3 GetInverse(Matrix3 me)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Matrix4 GetInverse(Matrix4 me)
        {
            // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm

            var n11 = me.M11; var n12 = me.M21; var n13 = me.M31; var n14 = me.M41;
            var n21 = me.M12; var n22 = me.M22; var n23 = me.M32; var n24 = me.M42;
            var n31 = me.M13; var n32 = me.M23; var n33 = me.M33; var n34 = me.M43;
            var n41 = me.M14; var n42 = me.M24; var n43 = me.M34; var n44 = me.M44;

            var te = new Matrix4();
            te.M11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
            te.M21 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
            te.M31 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
            te.M41 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;
            te.M12 = n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44;
            te.M22 = n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44;
            te.M32 = n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44;
            te.M42 = n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34;
            te.M13 = n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44;
            te.M23 = n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44;
            te.M33 = n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44;
            te.M43 = n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34;
            te.M14 = n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43;
            te.M24 = n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43;
            te.M34 = n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43;
            te.M44 = n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33;

            var det = n11 * te.M11 + n21 * te.M21 + n31 * te.M31 + n41 * te.M41;

            if (det == 0)
            {
                Trace.TraceError("Matrix4.getInverse(): can't invert matrix, determinant is 0");
                return Matrix4.Identity;
            }

            return MultiplyScalar(te, 1.0f / det);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 MakeRotationFromQuaternion(Matrix4 te, Quaternion q)
        {
            var x = q.X; var y = q.Y; var z = q.Z; var w = q.W;
            var x2 = x + x; var y2 = y + y; var z2 = z + z;
            var xx = x * x2; var xy = x * y2; var xz = x * z2;
            var yy = y * y2; var yz = y * z2; var zz = z * z2;
            var wx = w * x2; var wy = w * y2; var wz = w * z2;

            te.M11 = 1 - (yy + zz);
            te.M21 = xy - wz;
            te.M31 = xz + wy;
              
            te.M12 = xy + wz;
            te.M22 = 1 - (xx + zz);
            te.M32 = yz - wx;
              
            te.M13 = xz - wy;
            te.M23 = yz + wx;
            te.M33 = 1 - (xx + yy);
              
            //.M last column
            te.M14 = 0;
            te.M24 = 0;
            te.M34 = 0;
             
            //.M bottom row
            te.M41 = 0;
            te.M42 = 0;
            te.M43 = 0;
            te.M44 = 1;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 Scale(Matrix4 te, Vector3 v)
        {
            var x = v.X; var y = v.Y; var z = v.Z;

            te.M11 *= x; te.M21 *= y; te.M31 *= z;
            te.M12 *= x; te.M22 *= y; te.M32 *= z;
            te.M13 *= x; te.M23 *= y; te.M33 *= z;
            te.M14 *= x; te.M24 *= y; te.M34 *= z;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 SetPosition(Matrix4 te, Vector3 v)
        {
            te.M41 = v.X;
            te.M42 = v.Y;
            te.M43 = v.Z;

            return te;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 Compose(Matrix4 value, Vector3 position, Quaternion quaternion, Vector3 scale)
        {
            value = MakeRotationFromQuaternion(value, quaternion);
            value = Scale(value, scale);
            value = SetPosition(value, position);

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 SetValue(this Vector3 vector, string i, float value)
        {
            switch (i)
            {
                case "x":
                    vector.X = value;
                    break;
                case "y":
                    vector.Y = value;
                    break;
                case "z":
                    vector.Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("i can only be x,y or z");
                    break;
            }

            return vector;
        }
    }
}
