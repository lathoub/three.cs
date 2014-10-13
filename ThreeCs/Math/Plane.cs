namespace ThreeCs.Math
{
    using ThreeCs.Objects;

    public class Plane 
    {

        public Vector3 Normal = new Vector3(1, 0, 0);
        public float Constant = 0;

        public Plane Set(Vector3 Normal, float Constant)
        {
            this.Normal.Copy(Normal);
            this.Constant = Constant;
            return this;
        }

        public Plane SetComponents(float X, float Y, float Z, float W)
        {
            this.Normal.Set(X, Y, Z);
            this.Constant = W;
            return this;
        }

        public Plane SetFromNormalAndCoplanarPoint(Vector3 Normal, Vector3 Point)
        {
            this.Normal.Copy(Normal);
            this.Constant -= Point.Dot(this.Normal);
            return this;
        }

        public Plane SetFromCoplanarPoints(Vector3 V1, Vector3 V2, Vector3 V3)
        {
            var V1B = new Vector3();
            var V2B = new Vector3();

            var Normal = V1B.SubtractVectors(V3, V2).Cross(V2B.SubtractVectors(V1, V2)).Normalize();
            this.SetFromNormalAndCoplanarPoint(Normal, V1);
            return this;
        }

        public Plane Copy(Plane Plane)
        {
            this.Normal.Copy(Plane.Normal);
            this.Constant = Plane.Constant;
            return this;
        }

        public Plane Normalize()
        {
            var InverseNormalLength = 1.0f / this.Normal.Length;
            this.Normal.MultiplyScalar(InverseNormalLength);
            this.Constant *= InverseNormalLength;
            return this;
        }

        public Plane Negate()
        {
            this.Constant *= -1;
            this.Normal.Negate();
            return this;
        }

        public float DistanceToPoint(Vector3 Point)
        {
            return this.Normal.Dot(Point) + this.Constant;
        }

        public float DistanceToSphere(Sphere Sphere)
        {
            return this.DistanceToPoint(Sphere.Center) - Sphere.Radius;
        }

        public Vector3 ProjectPoint(Vector3 Point, Vector3 Target = null)
        {
            return (this.OrthoPoint(Point, Target) - Point).Negate();
        }

        public Vector3 OrthoPoint(Vector3 Point, Vector3 Target = null)
        {
            var PerpendicularMagnitude = this.DistanceToPoint(Point);

            var Result = Target ?? new Vector3();
            return Result.Copy(this.Normal).MultiplyScalar(PerpendicularMagnitude);
        }

        public bool IsIntersectionLine(Line Line)
        {
            var StartSign = this.DistanceToPoint(Line.Start);
            var EndSign = this.DistanceToPoint(Line.End);

            return (StartSign < 0 && EndSign > 0) || (EndSign < 0 && StartSign > 0);
        }

        public Vector3 IntersectLine(Line Line, Vector3 Target = null)
        {
            // Vector3 V1 = new Vector3();
            // Vector3 Result = Target ?? new Vector3();
            //TODO: Where the fuck is Line.Delta
            // Vector3 Direction = Line.Delta(V1);
            // float Denominator = this.Normal.Dot(Direction);

            // if (Denominator == 0) {
            //     if (this.DistanceToPoint(Line.Start) == 0) {
            //         return Result.Copy(Line.Start);
            //     }
            //     Trace.TraceWarning("Uhm wat");
            //     return null;
            // }

            // float T = (Line.Start.Dot(this.Normal) + this.Constant) / Denominator;
            // if (T < 0 || T > 1) {
            //     return null;
            // }
            // return Result.Copy(Direction).MultiplyScalar(T) + Line.Start;
            return new Vector3();
        }

        public Vector3 CoplanarPoint(Vector3 Target = null)
        {
            var Result = Target ?? new Vector3();
            return Result.Copy(this.Normal).MultiplyScalar(-this.Constant);
        }

        public Plane ApplyMatrix4(Matrix4 Matrix, Matrix3 NormalMatrix = null)
        {
            var V1 = new Vector3();
            var V2 = new Vector3();
            var M1 = new Matrix3();

            var NormalM = NormalMatrix ?? M1.GetNormalMatrix(Matrix);
            var NewNormal = V1.Copy(this.Normal).ApplyMatrix3(NormalM);
            var NewCoplanarPoint = this.CoplanarPoint(V2);
            NewCoplanarPoint.ApplyMatrix4(Matrix);

            this.SetFromNormalAndCoplanarPoint(NewNormal, NewCoplanarPoint);
            return this;
        }

        public Plane Translate(Vector3 Offset)
        {
            this.Constant = this.Constant - Offset.Dot(this.Normal);
            return this;
        }

        public bool Equals(Plane Plane)
        {
            return Plane.Normal.Equals(this.Normal) && (Plane.Constant == this.Constant);
        }

        public Plane Clone()
        {
            return new Plane().Copy(this);
        }

    }

}
