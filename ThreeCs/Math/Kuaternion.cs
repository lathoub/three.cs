namespace ThreeCs.Math
{


    public class Kuaternion
    {
        public static Quaternion SetFromEuler(Quaternion q, Euler euler)
        {
		    var c1 = (float)System.Math.Cos( euler.X / 2 );
            var c2 = (float)System.Math.Cos(euler.Y / 2);
            var c3 = (float)System.Math.Cos(euler.Z / 2);
            var s1 = (float)System.Math.Sin(euler.X / 2);
            var s2 = (float)System.Math.Sin(euler.Y / 2);
            var s3 = (float)System.Math.Sin(euler.Z / 2);

		    if ( euler.Order == Euler.RotationOrders.XYZ ) {

			    q.X = s1 * c2 * c3 + c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 - s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 + s1 * s2 * c3;
			    q.W = c1 * c2 * c3 - s1 * s2 * s3;

		    } else if ( euler.Order == Euler.RotationOrders.YXZ  ) {

			    q.X = s1 * c2 * c3 + c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 - s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 - s1 * s2 * c3;
			    q.W = c1 * c2 * c3 + s1 * s2 * s3;

		    } else if ( euler.Order == Euler.RotationOrders.ZXY ) {

			    q.X = s1 * c2 * c3 - c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 + s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 + s1 * s2 * c3;
			    q.W = c1 * c2 * c3 - s1 * s2 * s3;

		    } else if ( euler.Order == Euler.RotationOrders.ZYX ) {

			    q.X = s1 * c2 * c3 - c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 + s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 - s1 * s2 * c3;
			    q.W = c1 * c2 * c3 + s1 * s2 * s3;

		    } else if ( euler.Order == Euler.RotationOrders.YZX ) {

			    q.X = s1 * c2 * c3 + c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 + s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 - s1 * s2 * c3;
			    q.W = c1 * c2 * c3 - s1 * s2 * s3;

		    } else if ( euler.Order == Euler.RotationOrders.XZY ) {

			    q.X = s1 * c2 * c3 - c1 * s2 * s3;
			    q.Y = c1 * s2 * c3 - s1 * c2 * s3;
			    q.Z = c1 * c2 * s3 + s1 * s2 * c3;
			    q.W = c1 * c2 * c3 + s1 * s2 * s3;

		    }

		//    if ( update !== false ) this.onChangeCallback();

		    return q;
        }
    }
}
