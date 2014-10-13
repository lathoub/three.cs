namespace Demo
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Materials;
    using ThreeCs.Objects;
    using ThreeCs.Renderers;
    using ThreeCs.Scenes;

    [Example("webgl_geometry_hierarchy", ExampleCategory.OpenTK, "Geometry", 0.2f)]
    class webgl_geometry_hierarchy : Example
    {
        private PerspectiveCamera camera;
   
        private Object3D group;

        private Scene scene;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(70, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 500;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#FFFFFF"), 1, 10000);

            var geometry = new BoxGeometry(100, 100, 100);
            var material = new MeshNormalMaterial();

            group = new Object3D();

            for (var i = 0; i < 1000; i++)
            {
                var mesh = new Mesh(geometry, material);

                mesh.Position.X = random.Next() * 2000 - 1000;
                mesh.Position.Y = random.Next() * 2000 - 1000;
                mesh.Position.Z = random.Next() * 2000 - 1000;

                mesh.Rotation.X = (float)(random.Next() * 2.0 * Math.PI);
                mesh.Rotation.Y = (float)(random.Next() * 2.0 * Math.PI);

                mesh.MatrixAutoUpdate = false;
                mesh.UpdateMatrix();

                group.Add(mesh);
            }

            scene.Add(group);

            renderer.SetClearColor((Color)colorConvertor.ConvertFromString("#FFFFFF"));
            renderer.SortObjects = false;
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
            Debug.Assert(null != this.group);
            
            var time = stopWatch.ElapsedMilliseconds;
            
            var ftime = time * 0.001f;

            var rx = (float)(Math.Sin(ftime * 0.7) * 0.5);
            var ry = (float)(Math.Sin(ftime * 0.3) * 0.5);
            var rz = (float)(Math.Sin(ftime * 0.2) * 0.5);

            //camera.Position.X += (mouseX - camera.Position.X) * .05f;
            //camera.Position.Y += (-mouseY - camera.Position.Y) * .05f;

            //camera.LookAt(scene.Position);

            this.group.Rotation.X = rx;
            this.group.Rotation.Y = ry;
            this.group.Rotation.Z = rz;

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
