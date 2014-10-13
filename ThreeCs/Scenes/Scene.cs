namespace ThreeCs.Scenes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Lights;
    using ThreeCs.Materials;
    using ThreeCs.Objects;

    public class Scene : Object3D
    {
        #region Fields

        
        public bool AutoUpdate;

        
        public Material OverrideMaterial;

        
        public Fog Fog;

        
        public ObservableCollection<Light> __lights;

        public List<Object3D> __objectsAdded = new List<Object3D>();

        public List<Object3D> __objectsRemoved = new List<Object3D>();

        #endregion

        // Dynamic objects

        public Dictionary<int, List<WebGlObject>> __webglObjects;

        public List<WebGlObject> __webglObjectsImmediate;

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor.  Create a new scene object.
        /// </summary>
        public Scene()
        {
            this.__lights = new ObservableCollection<Light>();

            this.AutoUpdate = true; // checked by the renderer
            this.MatrixAutoUpdate = false;
        }

        /// <summary>
        ///     Copy Constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Scene(Scene other)
            : base(other)
        {
            this.__lights = new ObservableCollection<Light>();
            foreach (var light in other.__lights)
                this.__lights.Add((Light)light.Clone());

            this.AutoUpdate = other.AutoUpdate;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        public void __addObject(Object3D object3D)
        {
            if (object3D is Light)
            {
                var light = object3D as Light;

                if (this.__lights.IndexOf(light) == -1)
                {
                    this.__lights.Add(light);
                }

                var directionalLight = object3D as DirectionalLight;
                if ((null != directionalLight) && (directionalLight.target != null) && (directionalLight.target.Parent == null))
                {
                    this.Add(directionalLight.target);
                }

            }
            else if (!(object3D is Camera || object3D is Bone))
            {

                this.__objectsAdded.Add(object3D);

                // check if previously removed

                var i = this.__objectsRemoved.IndexOf(object3D);
                if (i != -1)
                {
                    this.__objectsRemoved.RemoveAt(i);
                }

            }

            //this.dispatchEvent( { type: 'objectAdded', object3D: object3D } );
            //object3D.dispatchEvent( { type: 'addedToScene', scene: this } );

            // Add the kids of the new object as well
            for (var c = 0; c < object3D.Children.Count; c++)
            {
                this.__addObject(object3D.Children[c]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        public void __removeObject(Object3D object3D)
        {
            if (object3D is Light)
            {

                var light = object3D as Light;

                var i = this.__lights.IndexOf(light);
                if (i != -1)
                {
                    this.__lights.RemoveAt(i);
                }

                var directionalLight = object3D as DirectionalLight;

                if ((null != directionalLight) && (null != directionalLight.shadowCascadeArray))
                {
                    for (var x = 0; x < directionalLight.shadowCascadeArray.Count; x++)
                    {
                        this.__removeObject(directionalLight.shadowCascadeArray[x]);
                    }
                }

            }
            else if (!(object3D is Camera))
            {

                this.__objectsRemoved.Add(object3D);

                // check if previously added

                var i = this.__objectsAdded.IndexOf(object3D);
                if (i != -1)
                {
                    this.__objectsAdded.RemoveAt(i);
                }
            }

            //this.dispatchEvent( { type: 'objectRemoved', object3D: object3D } );
            //object3D.dispatchEvent( { type: 'removedFromScene', scene: this } );

            for (var c = 0; c < object3D.Children.Count; c++)
            {
                this.__removeObject(object3D.Children[c]);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Scene(this);
        }

        #endregion
    }
}