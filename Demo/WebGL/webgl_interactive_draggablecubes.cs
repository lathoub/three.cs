using System;
using System.Collections.Generic;

namespace Demo.WebGL
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using Three.Core;

    using ThreeCs;
    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Lights;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;

    [Example("webgl_interactive_draggablecubes", ExampleCategory.OpenTK, "Interactive", 0.2f)]
    class webgl_interactive_draggablecubes : Example
    {
        private PerspectiveCamera camera;

        private Object3D group;

        private Mesh plane;

        private IList<Object3D> object3Ds = new List<Object3D>();

        private Projector projector;

        private Scene scene;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);
            
            camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 10000);
			camera.Position.Z = 1000;

            //controls = new TrackballControls( camera );
            //controls.rotateSpeed = 1.0;
            //controls.zoomSpeed = 1.2;
            //controls.panSpeed = 0.8;
            //controls.noZoom = false;
            //controls.noPan = false;
            //controls.staticMoving = true;
            //controls.dynamicDampingFactor = 0.3;

			scene = new Scene();

			scene.Add( new AmbientLight( (Color)colorConvertor.ConvertFromString("#505050") ) );

			var light = new SpotLight( (Color)colorConvertor.ConvertFromString("#FFFFFF"), 1.5f );
			light.Position = new Vector3( 0, 500, 2000 );
			light.CastShadow = true;

			light.shadowCameraNear = 200;
			light.shadowCameraFar = camera.Far;
			light.shadowCameraFov = 50;
                  
			light.shadowBias = -0.00022f;
			light.shadowDarkness = 0.5f;
                  
			light.shadowMapWidth = 2048;
			light.shadowMapHeight = 2048;

			scene.Add( light );

			var geometry = new BoxGeometry( 40, 40, 40 );

			for ( var i = 0; i < 200; i ++ ) {

				var object3D = new Mesh( geometry, new MeshLambertMaterial() { color = Color.Violet } );

				((MeshLambertMaterial)object3D.Material).ambient = ((MeshLambertMaterial)object3D.Material).color;

				object3D.Position.X = random.Next() * 1000 - 500;
				object3D.Position.Y = random.Next() * 600 - 300;
				object3D.Position.Z = random.Next() * 800 - 400;

				object3D.Rotation.X = random.Next() * 2 * (float)Math.PI;
				object3D.Rotation.Y = random.Next() * 2 * (float)Math.PI;
				object3D.Rotation.Z = random.Next() * 2 * (float)Math.PI;

				object3D.Scale.X = random.Next() * 2 + 1;
				object3D.Scale.Y = random.Next() * 2 + 1;
				object3D.Scale.Z = random.Next() * 2 + 1;

				object3D.CastShadow = true;
				object3D.ReceiveShadow = true;

			    scene.Add(object3D);

			    object3Ds.Add(object3D);
			}

			plane = new Mesh( new PlaneGeometry( 2000, 2000, 8, 8 ), new MeshBasicMaterial() { color = Color.Black, opacity = 0.25f, transparent = true, wireframe = true } );
			plane.Visible = false;
            scene.Add(plane);

			projector = new Projector();

			renderer.SetClearColor( (Color)colorConvertor.ConvertFromString("#F0F0F0") );
			renderer.size = new Size( control.Width, control.Height );
			renderer.SortObjects = false;

			renderer.shadowMapEnabled = true;
			renderer.shadowMapType = Three.PCFShadowMap;
          }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public override void Resize(Size clientSize)
        {
            Debug.Assert(null != this.camera);
            Debug.Assert(null != this.renderer);

            this.camera.Aspect = clientSize.Width / (float)clientSize.Height;
            this.camera.UpdateProjectionMatrix();

            this.renderer.size = clientSize;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            renderer.Render(scene, camera);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Unload()
        {
            this.scene.Dispose();
            this.camera.Dispose();

            base.Unload();
        }
    }
}
