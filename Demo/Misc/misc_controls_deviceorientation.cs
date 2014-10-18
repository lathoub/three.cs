
namespace Demo.Misc
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs;
    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;

    [Example("misc_controls_deviceorientation", ExampleCategory.Misc, "controls", 0.4f)]
    class misc_controls_deviceorientation : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private DeviceOrientationControls controls;

        private Geometry geometry;

        private Object3D mesh;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(75, control.Width / (float)control.Height, 1, 1100);

            controls = new DeviceOrientationControls(camera);

            scene = new Scene();

            geometry = new SphereGeometry(500, 16, 8);
            geometry.ApplyMatrix(new Matrix4().MakeScale(-1, 1, 1));

            var material = new MeshBasicMaterial() { map = ImageUtils.LoadTexture( @"data\textures/2294472375_24a3b8ef46_o.jpg" ) };

            mesh = new Mesh(geometry, material);
            scene.Add(mesh);

            var geometry2 = new BoxGeometry( 100, 100, 100, 4, 4, 4 );
            var material2 = new MeshBasicMaterial() { color = Color.Purple, side = Three.BackSide, wireframe = true };
            var mesh2 = new Mesh( geometry2, material2 );
            scene.Add(mesh2);
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
