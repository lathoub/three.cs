namespace ThreeCs.Scenes
{
    using System;
    using System.Drawing;

    public class Fog : ICloneable
    {
        #region Fields

        
        public Color color = Color.White;

        
        public float far;

        
        public string name;

        
        public float near;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public Fog()
        {
        }

        /// <summary>
        /// </summary>
        public Fog(Color color, float near = 1, float far = 2000)
        {
            this.color = color;
            this.near = near;
            this.far = far;
        }

        /// <summary>
        /// </summary>
        protected Fog(Fog other)
        {
            this.color = other.color;
            this.near = other.near;
            this.far = other.far;
        }

        #endregion

        #region Public Events


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Fog(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}