namespace ThreeCs.Cameras
{
    using ThreeCs.Core;
    using ThreeCs.Math;

    
    public class Camera : Object3D
    {
        public Matrix4 MatrixWorldInverse = new Matrix4().Identity();
        public Matrix4 ProjectionMatrix = new Matrix4().Identity();

        
        public float Far = 2000.0f;

        
        public float Near = 0.1f;

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Camera()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected Camera(Camera other)
            : base(other)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        public override void LookAt(Vector3 vector)
        {
            // This routine does not support objects with rotated and/or translated parent(s)
            var m1 = new Matrix4();
            m1.LookAt(this.Position, vector, this.Up);

            this.Quaternion.SetFromRotationMatrix(m1);
        }

        #endregion
    }
}