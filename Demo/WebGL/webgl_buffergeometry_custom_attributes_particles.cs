﻿namespace Demo.WebGL
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Renderers;
    using ThreeCs.Renderers.Shaders;
    using ThreeCs.Scenes;

    [Example("webgl_buffergeometry_custom_attributes_particles", ExampleCategory.WebGL, "BufferGeometry", 0.2f)]
    class webgl_buffergeometry_custom_attributes_particles : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Uniforms uniforms;

        private PointCloud particleSystem;

        private BufferGeometry geometry;

        private int particles = 100000;

        private string VertexShader = @"			
            attribute float size;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {

				vColor = customColor;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				gl_PointSize = size * ( 300.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}";

        private string FragmentShader = @"
            uniform vec3 color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );

				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

			}";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(40, control.Width / (float)control.Height, 1, 10000);
            this.camera.Position.Z = 300;

            scene = new Scene();

            var attributes = new Hashtable { { "size",        new Hashtable() { { "f", null } } }, 
                                             { "customcolor", new Hashtable() { { "c", null } } } };

            uniforms = new Uniforms { { "color", new KVP("c", Color.White) },
                                      { "texture", new KVP("t", ImageUtils.LoadTexture(@"data\textures/sprites/spark1.png")) } };

			var shaderMaterial = new ShaderMaterial() {
				uniforms =       uniforms,
				attributes =     attributes,
				vertexShader =   this.VertexShader,
				fragmentShader = this.FragmentShader,

				blending =       ThreeCs.Three.AdditiveBlending,
				depthTest =      false,
				transparent =    true
			};

			var radius = 200;

			geometry = new BufferGeometry();

            var positions = new float[particles * 3];
            var values_color = new float[particles * 3];
            var values_size = new float[particles];

            var color = new Color();

            for(var v = 0; v < particles; v++ ) 
            {
				values_size[ v ] = 20;

                positions[v * 3 + 0] = ((float)random.NextDouble() * 2 - 1) * radius;
                positions[v * 3 + 1] = ((float)random.NextDouble() * 2 - 1) * radius;
                positions[v * 3 + 2] = ((float)random.NextDouble() * 2 - 1) * radius;

                color.setHSL(v / (float)particles, 1.0f, 0.5f);

                color = Color.Tomato;

                values_color[v * 3 + 0] = color.R;
                values_color[v * 3 + 1] = color.G;
                values_color[v * 3 + 2] = color.B;
            }

            geometry.AddAttribute("position",    new BufferAttribute<float>(positions, 3));
            geometry.AddAttribute("customColor", new BufferAttribute<float>(values_color, 3));
            geometry.AddAttribute("size",        new BufferAttribute<float>(values_size, 1));

			particleSystem = new PointCloud( geometry, shaderMaterial );

			scene.Add( particleSystem );
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
            Debug.Assert(null != this.particleSystem);

            var time = stopWatch.ElapsedMilliseconds * 0.005f;

            particleSystem.Rotation.Z = 0.01f * time;

            var size = geometry.attributes["size"] as BufferAttribute<float>;
            Debug.Assert(null != size);
            
            for (var i = 0; i < particles; i++)
                size.Array[i] = 10 * (1 + (float)System.Math.Sin(0.1 * i + time));

            size.needsUpdate = true;

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
