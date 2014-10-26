namespace ThreeCs.Scenes
{
    using System;

    using ThreeCs.Core;
    using ThreeCs.Materials;

    public class Scene : Object3D
    {
        #region Fields

        public bool AutoUpdate;
        
        public Material OverrideMaterial;

        public Fog Fog;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor.  Create a new scene object.
        /// </summary>
        public Scene()
        {
            this.type = "Scene";

            this.Fog = null;
            this.OverrideMaterial = null;

            this.AutoUpdate = true; // checked by the renderer
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
            return null;
        }

        #endregion
    }
}