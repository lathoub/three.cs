namespace Demo.WebGL
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Loaders;
    using ThreeCs.Materials;
    using ThreeCs.Objects;
    using ThreeCs.Renderers;
    using ThreeCs.Scenes;
    using ThreeCs.Lights;
    using ThreeCs.Math;

    [Example("webgl_loader_vtk", ExampleCategory.WebGL, "loader", 0.5f)]
    class webgl_loader_vtk : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

//        private TrackballControls controls;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 0.1f, 1000000.0f);
            this.camera.Position.Z = 0.2f;
/*
            controls = new TrackballControls(camera);

            controls.rotateSpeed = 5.0;
            controls.zoomSpeed = 5;
            controls.panSpeed = 2;

            controls.noZoom = false;
            controls.noPan = false;

            controls.staticMoving = true;
            controls.dynamicDampingFactor = 0.3;
*/
            scene = new Scene();

            scene.Add(camera);
      
 			// light

            var dirLight = new DirectionalLight(Color.White);
            dirLight.Position = new Vector3(200, 200, 1000).Normalize();

            camera.Add(dirLight);
            camera.Add(dirLight.target);

            var material = new MeshLambertMaterial() { color = Color.White, side = ThreeCs.Three.DoubleSide };

            var loader = new VTKLoader();
            loader.Loaded += (o, args) =>
            {
                var mesh = new Mesh(args.Geometry, material);
                mesh.Position.Y = -0.09f;
                scene.Add(mesh);
            };

            loader.Load(@"data\models/vtk/bunny.vtk");
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

            //controls.handleResize();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

           // controls.update();

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
