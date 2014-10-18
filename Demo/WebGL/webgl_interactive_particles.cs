namespace Demo.WebGL
{
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Examples;

    using Three.Core;

    using ThreeCs.Cameras;
    using ThreeCs.Extras;
    using ThreeCs.Extras.Geometries;
    using ThreeCs.Materials;
    using ThreeCs.Math;
    using ThreeCs.Objects;
    using ThreeCs.Renderers.Shaders;
    using ThreeCs.Scenes;

    [Example("webgl_interactive_particles", ExampleCategory.OpenTK, "Interactive", 0.1f)]
    class webgl_interactive_particles : Example
    {
        private PerspectiveCamera camera;

        private Projector projector;

        private Scene scene;

        private Mesh mesh;

        private Line line;

        private Raycaster raycaster;

        private Uniforms uniforms;

        private PointCloud particles;

        private Vector2 mouse = new Vector2();

        private Hashtable attributes;

        private int intersect;

        private const int PARTICLE_SIZE = 20;

        private const string VertexShader = @"
			attribute float size;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {

				vColor = customColor;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				gl_PointSize = size * ( 300.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}
";

        private const string FragmentShader = @"
			uniform vec3 color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );

				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

				if ( gl_FragColor.a < ALPHATEST ) discard;

			}
";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public override void Load(Control control)
        {
            base.Load(control);

            camera = new PerspectiveCamera(45, control.Width / (float)control.Height, 1, 10000);
            camera.Position.Z = 250;

            scene = new Scene();
            scene.Fog = new Fog((Color)colorConvertor.ConvertFromString("#050505"), 2000, 3500);

            attributes = new Hashtable { { "size",        new Hashtable() { { "f", null } } }, 
                                         { "customColor", new Hashtable() { { "c", null } } } };

            uniforms = new Uniforms { { "color", new KVP("c", Color.White) },
                                      { "texture", new KVP("t", ImageUtils.LoadTexture(@"data\textures/sprites/disc.png")) } };
   
        	var shaderMaterial = new ShaderMaterial() {
                    uniforms = uniforms,
					attributes = attributes,
					vertexShader = VertexShader,
					fragmentShader = FragmentShader,

					alphaTest = 0.9f };

            var geometry = new BoxGeometry(200, 200, 200, 16, 16, 16);

            particles = new PointCloud(geometry, shaderMaterial);

        //    var values_size = ((BufferAttribute<float>)attributes["size"]).array;
        //    var values_color = ((BufferAttribute<float>)attributes["customColor"]).array;

        //    var vertices = particles.Geometry.Vertices;

        //    for (int v = 0, vl = vertices.Count; v < vertices.Count; v++)
        //    {
        //        values_size[v] = PARTICLE_SIZE * 0.5f;
        ////        values_color[v] = new Color().setHSL(0.01f + 0.1f * (v / vl), 1.0f, 0.5f);
        //    }

            scene.Add(particles);

            //

            projector = new Projector();
            raycaster = new Raycaster();
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
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public override void MouseMove(Size clientSize, Point here)
        {
            // Normalize mouse position
            mouse.X = (here.X / (float)clientSize.Width) * 2 - 1;
            mouse.Y = -(here.Y / (float)clientSize.Height) * 2 + 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
 			particles.Rotation.X += 0.0005f;
			particles.Rotation.Y += 0.001f;

			var vector = new Vector3( mouse.X, mouse.Y, 0.5f );

			projector.UnprojectVector( vector, camera );

            raycaster.Ray = new Ray(camera.Position, vector.Sub(camera.Position).Normalize());

			var intersects = raycaster.IntersectObject( particles );

			if ( intersects.Count > 0 ) {

              //  if (intersect != intersects[0].index)
              //  {

              ////      var positions = ((BufferAttribute<float>)bg.attributes["position"]).array;

              //      ((BufferAttribute<float>)attributes["size"]).array[intersect] = PARTICLE_SIZE;

              //      intersect = intersects[0].index;

              //      ((BufferAttribute<float>)attributes["size"]).array[intersect] = PARTICLE_SIZE * 1.25f;
              //      ((BufferAttribute<float>)attributes["size"]).needsUpdate = true;

              //  }

            }
            else if (intersect != null)
            {
                //((BufferAttribute<float>)attributes["size"]).array[intersect] = PARTICLE_SIZE;
                //((BufferAttribute<float>)attributes["size"]).needsUpdate = true;
                //intersect = null;
			}

			renderer.Render( scene, camera );
       }

    }
}
