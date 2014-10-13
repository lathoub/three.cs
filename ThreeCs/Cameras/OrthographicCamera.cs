namespace ThreeCs.Cameras
{
    public class OrthographicCamera : Camera
    {
        #region Fields

        
        public float Bottom;

        
        public float Left;

        
        public float Right;

        
        public float Top;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public OrthographicCamera(float left, float right, float top, float bottom, float near = 0.1f, int far = 2000)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            this.Near = near;
            this.Far = far;

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        protected OrthographicCamera(OrthographicCamera other)
            : base(other)
        {
            this.Left = other.Left;
            this.Right = other.Right;
            this.Top = other.Top;
            this.Bottom = other.Bottom;

            this.Near = other.Near;
            this.Far = other.Far;
        }

        #endregion

        #region Public Properties


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new OrthographicCamera(this);
        }

        /// <summary>
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            this.ProjectionMatrix.MakeOrthographic(this.Left, this.Right, this.Top, this.Bottom, this.Near, this.Far);
        }

        #endregion
    }
}