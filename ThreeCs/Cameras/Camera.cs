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
            this.type = "Camera";
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
        /// 
        /// </summary>
        /// <param name="optionalTarget"></param>
        /// <returns></returns>
        public override Vector3 GetWorldDirection(Vector3 optionalTarget = null) 
        {
            var quaternion = new Quaternion();

            var result = new Vector3();
            if (optionalTarget != null) 
                result = optionalTarget;

            this.GetWorldQuaternion(quaternion);
            return result = new Vector3(0, 0, - 1).ApplyQuaternion(quaternion);
        }

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