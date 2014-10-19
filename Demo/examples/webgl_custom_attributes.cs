namespace Demo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using OpenTK.Graphics.OpenGL;

    using ThreeCs.Cameras;
    using ThreeCs.Extras;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Renderers.Shaders;
    using ThreeCs.Scenes;
    using ThreeCs.Textures;

    [Example("webgl_custom_attributes", ExampleCategory.OpenTK, "custom", 0.7f)]
    class webgl_custom_attributes : Example
    {
        private PerspectiveCamera camera;

        private Scene scene;

        private Mesh sphere;

        private Hashtable attributes;

        private Uniforms uniforms;

        private IList<float> noise;

        private const string VertexShader = @"	
		
            uniform float amplitude;

			attribute float displacement;

			varying vec3 vNormal;
			varying vec2 vUv;

			void main() {

				vNormal = normal;
				vUv = ( 0.5 + amplitude ) * uv + vec2( amplitude );

				vec3 newPosition = position + amplitude * normal * vec3( displacement );
				gl_Position = projectionMatrix * modelViewMatrix * vec4( newPosition, 1.0 );

			}";

        private const string FragmentShader = @"
	
            varying vec3 vNormal;
			varying vec2 vUv;

			uniform vec3 color;
			uniform sampler2D texture;

			void main() {

				vec3 light = vec3( 0.5, 0.2, 1.0 );
				light = normalize( light );

				float dProd = dot( vNormal, light ) * 0.5 + 0.5;

				vec4 tcolor = texture2D( texture, vUv );
				vec4 gray = vec4( vec3( tcolor.r * 0.3 + tcolor.g * 0.59 + tcolor.b * 0.11 ), 1.0 );

				gl_FragColor = gray * vec4( vec3( dProd ) * vec3( color ), 1.0 );

			}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

			camera = new PerspectiveCamera( 30, control.Width / (float)control.Height, 1, 10000 );
			this.camera.Position.Z = 300;

			scene = new Scene();

            attributes = new Hashtable
            {
                { "displacement", new Hashtable() { { "type", "f" }, { "value", null } } }
            };

            uniforms = new Uniforms
            {
                { "amplitude", new Uniform() { {"type", "f"},  {"value", 1.0f}} },
                { "color",     new Uniform() { {"type", "c"},  {"value", (Color)colorConvertor.ConvertFromString("#ff2200")}} },
                { "texture",   new Uniform() { {"type", "t"},  {"value", ImageUtils.LoadTexture(@"examples\textures/water.jpg")} }},
            };

            //((Texture)uniforms["texture"].Value).WrapS = ThreeCs.Three.RepeatWrapping;
            //((Texture)uniforms["texture"].Value).WrapT = ThreeCs.Three.RepeatWrapping;

            var shaderMaterial = new ShaderMaterial
            {
	            uniforms = 		 uniforms,
	            attributes =     attributes,
                vertexShader =   VertexShader,
                fragmentShader = FragmentShader,
            };

            const float Radius = 50.0f; const int Segments = 128; const int Rings = 64;
            var geometry = new SphereGeometry(Radius, Segments, Rings);
            geometry.Dynamic = true;

            sphere = new Mesh(geometry, shaderMaterial);

            var vertices = geometry.Vertices;
            var values = new float[vertices.Count];

            noise = new float[vertices.Count];

            for (var v = 0; v < vertices.Count; v++)
            {
                values[v] = 0;
                noise[v] = Mat.Random() * 5;
            }

            ((Hashtable)attributes["displacement"])["f"] = values;


            scene.Add(sphere);

            renderer.SetClearColor((Color)colorConvertor.ConvertFromString("#050505"));
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

            var time = stopWatch.ElapsedMilliseconds * 0.01f;

            this.sphere.Rotation.Y = this.sphere.Rotation.Z = 0.01f * time;

            uniforms["amplitude"]["value"] = 2.5f * Math.Sin(this.sphere.Rotation.Y * 0.125);
            uniforms["color"]["value"] = ((Color)uniforms["color"]["value"]).OffsetHSL(512 * 0.0005f, 0, 0);

            var displacement = attributes["displacement"] as Hashtable;
            var values = (float[])displacement["f"];

            for (var i = 0; i < values.Length; i++)
            {
                values[i] = (float)Math.Sin(0.1 * i + time);

                noise[i] += 0.5f * (0.5f - Mat.Random());
                noise[i] = this.noise[i].Clamp(-5, 5);

                values[i] += noise[i];
            }

            displacement["needsUpdate"] = true;
            
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
