namespace ThreeCs.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using global::Three.Core;

    using ThreeCs.Annotations;
    using ThreeCs.Cameras;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;

    public class Object3D : ICloneable, IDisposable
    {
        #region Static Fields

        protected static int Object3DIdCount;

        #endregion

        // Dynamic attributesLocation and object

        private bool _disposed;

        public bool __webglInit = false;

        public bool __webglActive = false;

        public float[] __webglMorphTargetInfluences;

        public Matrix4 ModelViewMatrix;

        public Matrix3 NormalMatrix;

        public Vector3 DefaultUp = new Vector3( 0, 1, 0 );

        public Vector3 Up;

        //var scope = this;

        public Vector3 Position = new Vector3();

        public Euler Rotation = new Euler();

        public Quaternion Quaternion = new Quaternion().Identity();

        public Vector3 Scale = new Vector3(1, 1, 1);

        public int RenderDepth = -1;

        public bool RotationAutoUpdate = true;

        public Matrix4 Matrix = new Matrix4().Identity();

        public Matrix4 MatrixWorld = new Matrix4().Identity();

        public bool MatrixWorldNeedsUpdate = false;

        public bool Visible = true;

        public bool CastShadow = false;

        public bool ReceiveShadow = false;

        public bool FrustumCulled = true;

        public BaseGeometry Geometry;

        public Material Material;

        #region Fields
        
        public IList<Object3D> Children = new List<Object3D>();
        
        public int id = Object3DIdCount++;

        public bool MatrixAutoUpdate = true;

        [CanBeNull]
        public string Name;

        public Object3D Parent;

        public string Tag;

        public object UserData;
        
        public Guid Uuid = Guid.NewGuid();

        private readonly PreventCircularUpdate preventCircularUpdate = new PreventCircularUpdate();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Object3D()
        {
            this.Up = (Vector3)this.DefaultUp.Clone();

            // When the Rotation Euler is changed, the Quaternion is changed automaticaly
            this.Rotation.PropertyChanged += (o, args) => this.preventCircularUpdate.Do(() => this.Quaternion.SetFromEuler(this.Rotation));

            // When the Quaternion is changed, the Rotation euler is changed automaticaly
            this.Quaternion.PropertyChanged += (o, args) => this.preventCircularUpdate.Do(() => this.Rotation.SetFromQuaternion(this.Quaternion));
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Object3D(Object3D other)
        {
            this.id = other.id;
            // ...
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
            return new Object3D(this);
        }

        #endregion

        #region Methods

        [Obsolete]
        private bool eulerOrder
        {
            get
            {
                Trace.TraceWarning("THREE.Object3D: .eulerOrder has been moved to .rotation.Order.");
                return false; // this.rotation.Order;
            }
            set
            {
                Trace.TraceWarning("THREE.Object3D: .eulerOrder has been moved to .rotation.Order.");
                //this.rotation.Order = value;
            }
        }

        [Obsolete]
        private bool useQuaternion
        {
            get
            {
                Trace.TraceWarning("THREE.Object3D: .useQuaternion has been removed. The library now uses quaternions by default.");
                throw new NotImplementedException();
            }
            set
            {
                Trace.TraceWarning("THREE.Object3D: .useQuaternion has been removed. The library now uses quaternions by default.");
            }
        }

        public void ApplyMatrix(Matrix4 matrix)
        {
            throw new NotImplementedException();
            //this.matrix.ultiplyMatrices(matrix, this.matrix);

            //this.matrix.decompose(this.position, this.quaternion, this.scale);
        }

        public void SetRotationFromAxisAngle()
        {
            throw new NotImplementedException();
            // assumes axis is normalized

            //this.quaternion.setFromAxisAngle(axis, angle);
        }

        public void SetRotationFromEuler()
        {
            throw new NotImplementedException();
            //this.quaternion.setFromEuler(euler, true);
        }

        public void SetRotationFromMatrix()
        {
            throw new NotImplementedException();
            //// assumes the upper 3x3 of m is a pure rotation matrix (i.e, unscaled)

            //this.quaternion.setFromRotationMatrix(m);
        }

        public void SetRotationFromQuaternion()
        {
            throw new NotImplementedException();
            //// assumes q is normalized

            //this.quaternion.copy(q);
        }

        public void RotateOnAxis()
        {
            throw new NotImplementedException();
            //// rotate object on axis in object space
            //// axis is assumed to be normalized

            //var q1 = new Quaternion();

            //return function ( axis, angle ) {

            //    q1.setFromAxisAngle( axis, angle );

            //    this.quaternion.multiply( q1 );

            //    return this;

            //}
       }


        public void RotateX()
        {
            throw new NotImplementedException();
            //var v1 = new Vector3( 1, 0, 0 );

            //return function ( angle ) {

            //    return this.rotateOnAxis( v1, angle );

            //}
       }


        public void RotateY()
        {
            throw new NotImplementedException();
        }


        public void RotateZ()
        {
            throw new NotImplementedException();
        }


        public void TranslateOnAxis()
        {
            throw new NotImplementedException();
        }


        public void Translate()
        {
            throw new NotImplementedException();
        }


        public void TranslateX()
        {
            throw new NotImplementedException();
        }

        public void translateY()
        {
            throw new NotImplementedException();
        }

        public void translateZ()
        {
            throw new NotImplementedException();
        }

        public void localToWorld()
        {
            throw new NotImplementedException();
        }

        public void worldToLocal()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public virtual void LookAt(Vector3 vector) 
        {
            // This routine does not support objects with rotated and/or translated parent(s)
            var m1 = new Matrix4().LookAt(vector, this.Position, this.Up);

            this.Quaternion.SetFromRotationMatrix(m1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public void Render(Scene scene, Camera camera, int width, int height)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycaster"></param>
        /// <param name="intersects"></param>
        public virtual void Raycast(Raycaster raycaster, ref List<Intersect> intersects)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        public void Add(Object3D object3D)
        {
            //if ( arguments.length > 1 ) {
            //    for ( var i = 0; i < arguments.length; i++ ) {
            //        this.add( arguments[ i ] );
            //    }
            //    return this;
            //};

		    if ( object3D == this ) 
            {

			    Trace.TraceError( "THREE.Object3D.add:", object3D, "can't be added as a child of itself." );

		    }

		    if ( object3D is Object3D )
            {

			    if ( object3D.Parent != null ) 
                {

				    object3D.Parent.Remove( object3D );

			    }

			    object3D.Parent = this;
			//    object3D.dispatchEvent( { type: 'added' } );

			    this.Children.Add( object3D );

			    // add to scene

			    var scene = this;

			    while ( scene.Parent != null ) 
                {

				    scene = scene.Parent;

			    }

			    if ( scene != null && scene is Scene ) 
                {

				    ((Scene)scene).__addObject( object3D );

			    }

		    } 
            else
            {

                Trace.TraceError("THREE.Object3D.add: {0} is not an instance of THREE.Object3D.", object3D);
		
		    }
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        public void Remove(Object3D object3D)
        {
            //if ( arguments.length > 1 ) {
            //    for ( var i = 0; i < arguments.length; i++ ) {
            //        this.remove( arguments[ i ] );
            //    }
            //};

		    var index = this.Children.IndexOf( object3D );

		    if ( index != - 1 ) {

			    object3D.Parent = null;
			//    object3D.dispatchEvent( { type: 'removed' } );

			    this.Children.RemoveAt( index );

			    // remove from scene

			    var scene = this;

			    while ( scene.Parent != null ) {

				    scene = scene.Parent;

			    }

			    if ( (scene != null) && scene is Scene ) {

				    ((Scene)scene).__removeObject( object3D );

			    }

		    }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMatrix()
        {
            this.Matrix.Compose(this.Position, this.Quaternion, this.Scale);

            this.MatrixWorldNeedsUpdate = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void UpdateMatrixWorld(bool force = false)
        {
            if (this.MatrixAutoUpdate) 
                this.UpdateMatrix();

            if (this.MatrixWorldNeedsUpdate || force)
            {

                if (this.Parent == null)
                {
                    this.MatrixWorld.Copy(this.Matrix);
                }
                else
                {
                    this.MatrixWorld = this.Parent.MatrixWorld * this.Matrix;
                }

                this.MatrixWorldNeedsUpdate = false;

                force = true;
            }

            // update children

            for (var i = 0; i < this.Children.Count; i ++)
            {
                this.Children[i].UpdateMatrixWorld(force);
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    // TODO
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion
    }

        #endregion
}