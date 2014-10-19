namespace Demo
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Materials;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;

    [Example("webgl_geometry_cube", ExampleCategory.OpenTK, "Geometry")]
    class webgl_geometry_cube : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Object3D mesh;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            this.camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 1000);
            this.camera.Position.Z = 400;

            this.scene = new Scene();

            var geometry = new BoxGeometry(200, 200, 200);

            var texture = ImageUtils.LoadTexture(@"examples/textures/crate.gif");
            texture.anisotropy = this.renderer.MaxAnisotropy;

            var material = new MeshBasicMaterial { map = texture };

            this.mesh = new Mesh(geometry, material);
            this.scene.Add(mesh);
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
            Debug.Assert(null != this.renderer);
            Debug.Assert(null != this.mesh);

            // Cube sample
            this.mesh.Rotation.X += 0.005f;
            this.mesh.Rotation.Y += 0.01f;

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
