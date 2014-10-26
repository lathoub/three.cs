namespace ThreeCs.Math
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}, Order = {Order}")]
    public class Euler : INotifyPropertyChanged
    {
        public enum RotationOrder 
        {
            XYZ, YZX, ZXY, XZY, YXZ, ZYX
        }

        private float x;

        private float y;

        private float z;

        public RotationOrder Order = RotationOrder.XYZ;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        public Euler()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="order"></param>
        public Euler(float x, float y, float z, RotationOrder order = RotationOrder.XYZ)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Order = order;
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
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            return value.Clamp(min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Euler SetFromQuaternion(Quaternion q)
        {
            return SetFromQuaternion(q, this.Order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Euler SetFromQuaternion(Quaternion q, RotationOrder order)
        {
            // http://www.mathworks.com/matlabcentral/fileexchange/20696-function-to-convert-between-dcm-euler-angles-quaternions-and-euler-vectors/content/SpinCalc.m

		    var sqx = q.X * q.X;
		    var sqy = q.Y * q.Y;
		    var sqz = q.Z * q.Z;
		    var sqw = q.W * q.W;

            this.Order = order;

		    if ( this.Order == RotationOrder.XYZ ) {

			    this.x = (float)System.Math.Atan2( 2 * ( q.X * q.W - q.Y * q.Z ), ( sqw - sqx - sqy + sqz ) );
			    this.y = (float)System.Math.Asin(  Clamp( 2 * ( q.X * q.Z + q.Y * q.W ), - 1, 1 ) );
			    this.z = (float)System.Math.Atan2( 2 * ( q.Z * q.W - q.X * q.Y ), ( sqw + sqx - sqy - sqz ) );

		    } else if ( this.Order == RotationOrder.YXZ ) {

			    this.x = (float)System.Math.Asin(  Clamp( 2 * ( q.X * q.W - q.Y * q.Z ), - 1, 1 ) );
			    this.y = (float)System.Math.Atan2( 2 * ( q.X * q.Z + q.Y * q.W ), ( sqw - sqx - sqy + sqz ) );
			    this.z = (float)System.Math.Atan2( 2 * ( q.X * q.Y + q.Z * q.W ), ( sqw - sqx + sqy - sqz ) );

		    } else if ( this.Order == RotationOrder.ZXY ) {

			    this.x = (float)System.Math.Asin(  Clamp( 2 * ( q.X * q.W + q.Y * q.Z ), - 1, 1 ) );
			    this.y = (float)System.Math.Atan2( 2 * ( q.Y * q.W - q.Z * q.X ), ( sqw - sqx - sqy + sqz ) );
			    this.z = (float)System.Math.Atan2( 2 * ( q.Z * q.W - q.X * q.Y ), ( sqw - sqx + sqy - sqz ) );

		    } else if ( this.Order == RotationOrder.ZYX ) {

			    this.x = (float)System.Math.Atan2( 2 * ( q.X * q.W + q.Z * q.Y ), ( sqw - sqx - sqy + sqz ) );
			    this.y = (float)System.Math.Asin(  Clamp( 2 * ( q.Y * q.W - q.X * q.Z ), - 1, 1 ) );
			    this.z = (float)System.Math.Atan2( 2 * ( q.X * q.Y + q.Z * q.W ), ( sqw + sqx - sqy - sqz ) );

		    } else if ( this.Order == RotationOrder.YZX ) {

			    this.x = (float)System.Math.Atan2( 2 * ( q.X * q.W - q.Z * q.Y ), ( sqw - sqx + sqy - sqz ) );
			    this.y = (float)System.Math.Atan2( 2 * ( q.Y * q.W - q.X * q.Z ), ( sqw + sqx - sqy - sqz ) );
			    this.z = (float)System.Math.Asin(  Clamp( 2 * ( q.X * q.Y + q.Z * q.W ), - 1, 1 ) );

		    } else if ( this.Order == RotationOrder.XZY ) {

			    this.x = (float)System.Math.Atan2( 2 * ( q.X * q.W + q.Y * q.Z ), ( sqw - sqx + sqy - sqz ) );
			    this.y = (float)System.Math.Atan2( 2 * ( q.X * q.Z + q.Y * q.W ), ( sqw + sqx - sqy - sqz ) );
			    this.z = (float)System.Math.Asin(  Clamp( 2 * ( q.Z * q.W - q.X * q.Y ), - 1, 1 ) );

		    }

            this.OnPropertyChanged();

		    return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reorder(RotationOrder newOrder)
        {
		    // WARNING: this discards revolution information -bhouston

            var q = new Quaternion().SetFromEuler(this);
            this.Order = newOrder;
            this.SetFromQuaternion(q);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
