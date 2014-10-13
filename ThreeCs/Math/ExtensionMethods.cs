
namespace ThreeCs.Math
{
    using System;
    using System.Drawing;

    public static class Mat
    {
        public const double PI2 = (2 * 3.14159265358979323846); 

        public static double RadToDeg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }

        public static double DegToRad(double deg)
        {
            return (Math.PI * deg / 180.0);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static float hue2rgb ( float p, float q, float t )
        {
		    if ( t < 0.0f ) t += 1;
		    if ( t > 1.0f ) t -= 1;
		    if ( t < 1.0f / 6.0f ) return p + ( q - p ) * 6 * t;
		    if ( t < 1.0f / 2.0f ) return q;
		    if ( t < 2.0f / 3.0f ) return p + ( q - p ) * 6 * ( 2 / 3.0f - t );

		    return p;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Color setHSL(this Color value, float h, float s, float l)
        {
	    	// h,s,l ranges are in 0.0 - 1.0

            Color color;

		    if ( s == 0 ) {

                return Color.FromArgb(255, (int)l * 255, (int)l * 255, (int)l * 255);

		    } else {

			    var p = l <= 0.5f ? l * ( 1 + s ) : l + s - ( l * s );
			    var q = ( 2.0f * l ) - p;

			    var r = hue2rgb( q, p, h + 1 / 3.0f );
                var g = hue2rgb(q, p, h);
                var b = hue2rgb(q, p, h - 1 / 3.0f);

                value = Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));

		        return value;

		    }

            return color;
        }
    }
}
