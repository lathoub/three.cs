namespace Demo.WebGL
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Demo.examples.cs.controls;
    using Demo.examples.loaders;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Materials;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;
    using ThreeCs.Lights;
    using ThreeCs.Math;

    [Example("webgl_loader_vtk", ExampleCategory.OpenTK, "loader")]
    class webgl_loader_vtk : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private TrackballControls controls;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(60, control.Width / (float)control.Height, 0.1f, 1000000.0f);
            this.camera.Position.Z = 0.2f;

            controls = new TrackballControls(control, camera);

            controls.RotateSpeed = 5.0f;
            controls.ZoomSpeed = 5;
            controls.PanSpeed = 2;

            controls.NoZoom = false;
            controls.NoPan = false;

            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.3f;

            scene = new Scene();

            scene.Add(camera);
      
 			// light

            var dirLight = new DirectionalLight(Color.White);
            dirLight.Position = new Vector3(200, 200, 1000).Normalize();

            camera.Add(dirLight);
            camera.Add(dirLight.target);

            var material = new MeshLambertMaterial() { Color = Color.White, Side = ThreeCs.Three.DoubleSide };

            // Link in the loader

            var loader = new VTKLoader();
            loader.Loaded += (o, args) =>
            {
                args.BufferGeometry.ComputeVertexNormals();

                var mesh = new Mesh(args.BufferGeometry, material);
                mesh.Position.Y = -0.09f;
                scene.Add(mesh);
            };
            loader.Load(@"examples\models/vtk/bunny.vtk");
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

            this.renderer.Size = clientSize;

            //controls.handleResize();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            Debug.Assert(null != this.renderer);

           controls.Update();

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
