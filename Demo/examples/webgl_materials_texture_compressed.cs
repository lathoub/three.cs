namespace Demo.examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Extras;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Lights;
    using ThreeCs.Loaders;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Scenes;

    [Example("webgl_materials_texture_compressed", ExampleCategory.OpenTK, "materials", 0.0f)]
    class webgl_materials_texture_compressed : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private BoxGeometry geometry;

        private List<Mesh> meshes = new List<Mesh>();

        private readonly Vector2 mouse = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(50, control.Width / (float)control.Height, 1, 2000);
            camera.Position.Z = 1000;

            scene = new Scene();

            geometry = new BoxGeometry(200, 200, 200);

            /*
            This is how compressed textures are supposed to be used:

            DXT1 - RGB - opaque textures
            DXT3 - RGBA - transparent textures with sharp alpha transitions
            DXT5 - RGBA - transparent textures with full alpha range
            */

            var loader = new DDSLoader();
            loader.Loaded += (o, args) =>
            {
            };

            var map1 = loader.Load("examples/textures/compressed/disturb_dxt1_nomip.dds");
            map1.MinFilter = map1.MagFilter = ThreeCs.Three.LinearFilter;
            map1.Anisotropy = 4;

            var map2 = loader.Load("examples/textures/compressed/disturb_dxt1_mip.dds");
            map2.Anisotropy = 4;

            var map3 = loader.Load("examples/textures/compressed/hepatica_dxt3_mip.dds");
			map3.Anisotropy = 4;

            var map4 = loader.Load("examples/textures/compressed/explosion_dxt5_mip.dds");
			map4.Anisotropy = 4;

            var map5 = loader.Load("examples/textures/compressed/disturb_argb_nomip.dds");
			map5.MinFilter = map5.MagFilter = ThreeCs.Three.LinearFilter;
			map5.Anisotropy = 4;

            var map6 = loader.Load("examples/textures/compressed/disturb_argb_mip.dds");
			map6.Anisotropy = 4;

            var cubemap1 = loader.Load("examples/textures/compressed/Mountains.dds");
            //    texture.magFilter = ThreeCs.Three.LinearFilter
            //    texture.minFilter = ThreeCs.Three.LinearFilter;
            //    texture.mapping = new CubeReflectionMapping();
            //    material1.needsUpdate = true;
            //} );

            var cubemap2 = loader.Load("examples/textures/compressed/Mountains_argb_mip.dds");
            //    texture.magFilter = ThreeCs.Three.LinearFilter;
            //    texture.minFilter = ThreeCs.Three.LinearFilter;
            //    texture.mapping = new CubeReflectionMapping();
            //    material5.needsUpdate = true;
            //} );

            var cubemap3 = loader.Load("examples/textures/compressed/Mountains_argb_nomip.dds");
            //    texture.magFilter = ThreeCs.Three.LinearFilter;
            //    texture.minFilter = ThreeCs.Three.LinearFilter;
            //    texture.mapping = new CubeReflectionMapping();
            //    material6.needsUpdate = true;
            //} );

            var material1 = new MeshBasicMaterial() { Map = map1, EnvMap = cubemap1 };
            var material2 = new MeshBasicMaterial() { Map = map2 };
            var material3 = new MeshBasicMaterial() { Map = map3, alphaTest = 0.5f, side = ThreeCs.Three.DoubleSide };
			var material4 = new MeshBasicMaterial() { Map = map4, side = ThreeCs.Three.DoubleSide, blending = ThreeCs.Three.AdditiveBlending, depthTest = false, transparent = true } ;
            var material5 = new MeshBasicMaterial() { EnvMap = cubemap2 };
            var material6 = new MeshBasicMaterial() { EnvMap = cubemap3 };
            var material7 = new MeshBasicMaterial() { Map = map5 };
            var material8 = new MeshBasicMaterial() { Map = map6 };

			var mesh = new Mesh( new TorusGeometry( 100, 50, 32, 16 ), material1 );
			mesh.Position.X = -600;
			mesh.Position.Y = -200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( geometry, material2 );
			mesh.Position.X = -200;
			mesh.Position.Y = -200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( geometry, material3 );
			mesh.Position.X = -200;
			mesh.Position.Y = 200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( geometry, material4 );
			mesh.Position.X = -600;
			mesh.Position.Y = 200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( new BoxGeometry( 200, 200, 200 ), material5 );
			mesh.Position.X = 200;
			mesh.Position.Y = 200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( new BoxGeometry( 200, 200, 200 ), material6 );
			mesh.Position.X = 200;
			mesh.Position.Y = -200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( geometry, material7 );
			mesh.Position.X = 600;
			mesh.Position.Y = -200;
            scene.Add(mesh);
            meshes.Add(mesh);

			mesh = new Mesh( geometry, material8 );
			mesh.Position.X = 600;
			mesh.Position.Y = 200;
            scene.Add(mesh);
            meshes.Add(mesh);
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            this.mouse.X = (here.X - ((float)clientSize.Width / 2));
            this.mouse.Y = (here.Y - ((float)clientSize.Height / 2));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            var time = stopWatch.ElapsedMilliseconds * 0.001f;

            foreach (var mesh in this.meshes)
            {
                mesh.Rotation.X = time;
                mesh.Rotation.Y = time;
            }

            renderer.Render(scene, camera);
        }
    }
}
