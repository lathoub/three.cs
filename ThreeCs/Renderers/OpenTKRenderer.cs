namespace ThreeCs.Renderers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    using OpenTK.Graphics.OpenGL;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Extras.Helpers;
    using ThreeCs.Extras.Objects;
    using ThreeCs.Lights;
    using ThreeCs.Materials;
    using ThreeCs.Objects;
    using ThreeCs.Renderers.Shaders;
    using ThreeCs.Renderers.WebGL;
    using ThreeCs.Scenes;
    using ThreeCs.Textures;
    using ThreeCs.Math;

    using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;
    using BlendEquationMode = OpenTK.Graphics.OpenGL.BlendEquationMode;
    using BlendingFactorDest = OpenTK.Graphics.OpenGL.BlendingFactorDest;
    using BlendingFactorSrc = OpenTK.Graphics.OpenGL.BlendingFactorSrc;
    using BufferTarget = OpenTK.Graphics.OpenGL.BufferTarget;
    using BufferUsageHint = OpenTK.Graphics.OpenGL.BufferUsageHint;
    using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
    using CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode;
    using DepthFunction = OpenTK.Graphics.OpenGL.DepthFunction;
    using DrawElementsType = OpenTK.Graphics.OpenGL.DrawElementsType;
    using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
    using ExtTextureFilterAnisotropic = OpenTK.Graphics.OpenGL.ExtTextureFilterAnisotropic;
    using FramebufferTarget = OpenTK.Graphics.OpenGL.FramebufferTarget;
    using FrontFaceDirection = OpenTK.Graphics.OpenGL.FrontFaceDirection;
    using GetPName = OpenTK.Graphics.OpenGL.GetPName;
    using GL = OpenTK.Graphics.OpenGL.GL;
    using PixelStoreParameter = OpenTK.Graphics.OpenGL.PixelStoreParameter;
    using PixelType = OpenTK.Graphics.OpenGL.PixelType;
    using StringName = OpenTK.Graphics.OpenGL.StringName;
    using TextureMagFilter = OpenTK.Graphics.OpenGL.TextureMagFilter;
    using TextureMinFilter = OpenTK.Graphics.OpenGL.TextureMinFilter;
    using TextureParameterName = OpenTK.Graphics.OpenGL.TextureParameterName;
    using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
    using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
    using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
    using VertexAttribPointerType = OpenTK.Graphics.OpenGL.VertexAttribPointerType;

    // Based on version 68

    public struct Info
    {
        public struct Memory
        {
            public int Programs;

            public int Geometries;

            public int Textures;
        }

        public struct Render
        {
            public int Calls;

            public int Vertices;

            public int Faces;

            public int Points;
        }

        public Memory memory;

        public Render render;
    }

    public class OpenTKRenderer : IDisposable
    {
        //private var _canvas = parameters.canvas != = undefined ? parameters.canvas : document.createElement('canvas'),
        //            _context = null;

        private bool _disposed;

        #region Fields

        public int MaxAnisotropy;

        public int maxTextureSize;

        public int maxTextures;

        public int maxVertexTextures;

        public Info info;

        public Size size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;

          //      _canvas.width = size.Width;
          //      _canvas.height = size.Height;

                //if ( updateStyle !== false ) {

                //    _canvas.style.width = width + 'px';
                //    _canvas.style.height = height + 'px';

                //}

                this.SetViewport(0, 0, this.size.Width, this.size.Height);
            }
        }

        private int[] _enabledAttributes = new int[16];

        private int[] _newAttributes = new int[16];

        private List<WebGlProgram> _programs = new List<WebGlProgram>();

        private bool autoClear;

        private bool autoClearColor;

        private bool autoClearDepth;

        private bool autoClearStencil;

        private int clearAlpha;

        private int maxMorphNormals = 4;

        private int maxMorphTargets = 8;

        private List<WebGlObject> opaqueObjects = new List<WebGlObject>();

        private List<Object3D> renderPluginsPost = new List<Object3D>();

        private List<Object3D> renderPluginsPre = new List<Object3D>();

        private ShaderLib shaderLib;

        private bool shadowMapCascade;

        private bool shadowMapDebug;

        public bool shadowMapEnabled;

        public int shadowMapType = Three.PCFShadowMap;

        public bool SortObjects;

        // physically based shading

        public bool gammaInput = false;

        public bool gammaOutput = false;

        private readonly bool supportsBoneTextures;

        private readonly bool supportsVertexTextures;

        private readonly List<WebGlObject> transparentObjects = new List<WebGlObject>();

        private bool _alpha = false;

        private bool _antialias = false;

        private int _contextGlobalCompositeOperation;

        private Camera _currentCamera;

        private long _currentGeometryGroupHash;

        private int _currentMaterialId;

        private int _currentProgram = -1;

        private int _currentFramebuffer = -1;

        private bool _depth = true;

        private Matrix4 _frustum = new Matrix4();

        private bool _logarithmicDepthBuffer = false;

        // GL state cache

        private float _oldLineWidth = -1;

        private int _oldDepthTest = -1;
        
        private int _oldDepthWrite = -1;

        private int _oldBlendDst = -1;

        private int _oldBlendEquation = -1;

        private int _oldBlendSrc = -1;

        private int _oldBlending = -1;

        private int _oldDoubleSided = -1;

        private int _oldFlipSided = -1;

        private bool _oldPolygonOffset;

        private float _oldPolygonOffsetFactor = -1;

        private float _oldPolygonOffsetUnits = -1;

        private string _precision = "highp";

        private bool _premultipliedAlpha = true;

        private bool _preserveDrawingBuffer = false;

        private Matrix4 _projScreenMatrix;

        private Matrix4 _projScreenMatrixPS = new Matrix4();

        private bool _stencil = true;

        private int _usedTextureUnits;

        private bool autoScaleCubemaps = true;

        private Color clearColor;

        private int[] compressedTextureFormats;

        private bool depthTest;

        private bool depthWrite;

        private float fragmentShaderPrecisionHighpFloat;

        private float fragmentShaderPrecisionLowpFloat;

        private float fragmentShaderPrecisionMediumpFloat;

        private bool glExtensionCompressedTextureS3TC;

        private bool glExtensionElementIndexUint;

        private bool glExtensionStandardDerivatives;

        private bool glExtensionTextureFilterAnisotropic;

        private bool glExtensionTextureFloat;

        private Int32 maxCubemapSize;

        private bool shadowMapAutoUpdate = true;

        private int shadowMapCullFace = Three.CullFaceFront;

        private float vertexShaderPrecisionHighpFloat;

        private float vertexShaderPrecisionLowpFloat;

        private float vertexShaderPrecisionMediumpFloat;

        private Size _size;

        private int _viewportX;

        private int _viewportY;

        private int _viewportWidth;

        private int _viewportHeight;

        private int _currentWidth;

        private int _currentHeight;

        private int devicePixelRatio = 1;

        // light arrays cache

        private Vector3 _direction = new Vector3();

        private bool _lightsNeedUpdate = true;

        private LightCollection _lights = new LightCollection();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public OpenTKRenderer(Control control)
        {
            //	        Trace.TraceInformation( "THREE.WebGLRenderer {0}", Three.Version );

            this.shaderLib = new ShaderLib();

            this.clearColor = Color.Black;
            this.clearAlpha = 0;

            this.opaqueObjects.Clear();
            this.transparentObjects.Clear();

            this._viewportX = 0;
            this._viewportY = 0;
            this._viewportWidth = control.Width;
            this._viewportHeight = control.Width;

            // clearing

            this.autoClear = true;
            this.autoClearColor = true;
            this.autoClearDepth = true;
            this.autoClearStencil = true;

            // scene graph

            this.SortObjects = true;

            // custom render plugins

            this.renderPluginsPre.Clear();
            this.renderPluginsPost.Clear();

            // shadow map

            this.shadowMapEnabled = false;
            this.shadowMapAutoUpdate = true;
            this.shadowMapType = Three.PCFShadowMap;
            this.shadowMapCullFace = Three.CullFaceFront;
            this.shadowMapDebug = false;
            this.shadowMapCascade = false;

            // morphs

            this.maxMorphTargets = 8;
            this.maxMorphNormals = 4;

            // flags

            this.autoScaleCubemaps = true;

            this.InitGl();

            this.SetDefaultGlState();

            // GPU capabilities

            GL.GetInteger(GetPName.MaxTextureSize, out this.maxTextureSize);
            GL.GetInteger(GetPName.MaxTextureImageUnits, out this.maxTextures);
            GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out this.maxVertexTextures);
            GL.GetInteger(GetPName.MaxCubeMapTextureSize, out this.maxCubemapSize);

            if (this.glExtensionTextureFilterAnisotropic)
                GL.GetInteger((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out this.MaxAnisotropy);

            this.supportsVertexTextures = (this.maxVertexTextures > 0);
            this.supportsBoneTextures = this.supportsVertexTextures && this.glExtensionTextureFloat;

            int compressedTextureFormatCount;
            GL.GetInteger(GetPName.NumCompressedTextureFormats, out compressedTextureFormatCount);
            if (compressedTextureFormatCount > 0)
            {
                this.compressedTextureFormats = new int[compressedTextureFormatCount];
                GL.GetInteger(GetPName.CompressedTextureFormats, this.compressedTextureFormats);
            }

            //vertexShaderPrecisionHighpFloat   = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.HIGH_FLOAT );
            //vertexShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.MEDIUM_FLOAT );
            //vertexShaderPrecisionLowpFloat    = _gl.getShaderPrecisionFormat( _gl.VERTEX_SHADER, _gl.LOW_FLOAT );

            //fragmentShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.HIGH_FLOAT );
            //fragmentShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.MEDIUM_FLOAT );
            //fragmentShaderPrecisionLowpFloat = _gl.getShaderPrecisionFormat( _gl.FRAGMENT_SHADER, _gl.LOW_FLOAT );

        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
	    public void SetClearColor (Color color, int alpha = 255)
        {
            this.clearColor = color;
		    this.clearAlpha = alpha;

            GL.ClearColor(Color.FromArgb(this.clearAlpha, this.clearColor));
	    }

        /// <summary>
        /// 
        /// </summary>
        private void SetViewport(int x, int y, int width, int height)
        {
            this._viewportX = x * this.devicePixelRatio;
            this._viewportY = y * this.devicePixelRatio;

            this._viewportWidth = width * this.devicePixelRatio;
            this._viewportHeight = height * this.devicePixelRatio;

            GL.Viewport(this._viewportX, this._viewportY, this._viewportWidth, this._viewportHeight);
        }

        private void setScissor ( int x, int y, int width, int height )
        {
            GL.Scissor(
                x * this.devicePixelRatio,
                y * this.devicePixelRatio,
                width * this.devicePixelRatio,
                height * this.devicePixelRatio);

        }

	    private void enableScissorTest  ( bool enable )
	    {
	        if (enable)
	            GL.Enable(EnableCap.ScissorTest);
	        else 
                GL.Disable(EnableCap.ScissorTest);
	    }

        /// <summary>
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        public void ClearTarget(OpenTKRenderTarget renderTarget, bool color, bool depth, bool stencil)
        {
            this.SetRenderTarget(renderTarget);

            Clear(color, depth, stencil);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderTarget"></param>
        private void SetRenderTarget(OpenTKRenderTarget renderTarget)
        {
		    var isCube = ( renderTarget is OpenTKRenderTargetCube );

            if ((null != renderTarget) && (renderTarget.__webglFramebuffer >= 0))
            {
                //if ( renderTarget.DepthBuffer == undefined ) renderTarget.DepthBuffer = true;
                //if ( renderTarget.StencilBuffer == undefined ) renderTarget.StencilBuffer = true;

                // event

                // renderTarget.__webglTexture = GL.CreateTexture();

                // Set up FBO

            }

            var framebuffer = -1;
            int width;
            int height;
            int vx; 
            int vy;

            if (null != renderTarget)
            {
      			if ( isCube )
      			{
                    var renderTargetCube = renderTarget as OpenTKRenderTargetCube;
                    //framebuffer = renderTarget.__webglFramebuffer[ renderTargetCube.activeCubeFace ];

      			} else {

				    framebuffer = renderTarget.__webglFramebuffer;

			    }

			    width = renderTarget.Width;
			    height = renderTarget.Height;

			    vx = 0;
			    vy = 0;
            } 
            else
            {
                framebuffer = -1;

                width = this._viewportWidth;
                height = this._viewportHeight;

                vx = this._viewportX;
                vy = this._viewportY;

            }

            if ( framebuffer != this._currentFramebuffer ) {

			    GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer );
			    GL.Viewport( vx, vy, width, height );

			    this._currentFramebuffer = framebuffer;
		    }

		    this._currentWidth = width;
		    this._currentHeight = height;
        }

        private void UpdateRenderTargetMipmap (OpenTKRenderTarget renderTarget ) {

		    if ( renderTarget is OpenTKRenderTargetCube ) {

			    GL.BindTexture(TextureTarget.TextureCubeMap, renderTarget.__webglTexture );
			    GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
			    GL.BindTexture(TextureTarget.TextureCubeMap, 0 );

		    } else {

			    GL.BindTexture(TextureTarget.Texture2D, renderTarget.__webglTexture );
			    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			    GL.BindTexture(TextureTarget.Texture2D, 0 );

		    }

	    }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="renderTarget"></param>
        /// <param name="forceClear"></param>
        public void Render(Scene scene, Camera camera, OpenTKRenderTarget renderTarget = null, bool forceClear = true)
        {            
            Debug.Assert(null != scene, "OpenTKRenderer.Render: scene can not be null");
            Debug.Assert(null != camera, "OpenTKRenderer.Render: camera can not be null");

            IEnumerable<Light> lights = scene.__lights;
            var fog = scene.Fog;

            // reset caching for this frame

            this._currentMaterialId = -1;
            this._currentCamera = null;
            this._lightsNeedUpdate = true;

            // update scene graph

            if (scene.AutoUpdate)
            {
                scene.UpdateMatrixWorld();
            }
            // update camera matrices and frustum
            if (null == camera.Parent)
            {
                camera.UpdateMatrixWorld();
            }

            // update Skeleton objects
            this.UpdateSkeletons(scene);

            camera.MatrixWorldInverse = camera.MatrixWorld.GetInverse();

            this._projScreenMatrix = camera.ProjectionMatrix * camera.MatrixWorldInverse;
        //    this._frustum = new Matrix4().FromMatrix(this._projScreenMatrix); // TODO

            this.InitObjects(scene);

            this.opaqueObjects.Clear();
            this.transparentObjects.Clear();

            this.ProjectObject(scene, scene, camera);

            if (this.SortObjects)
            {
                this.opaqueObjects.Sort(
                    delegate(WebGlObject a, WebGlObject b)
                        {
                            if (a.z != b.z)
                            {
                                return (int)(b.z - a.z);
                            }
                            return (int)(a.id - b.id);
                        });

                this.transparentObjects.Sort(
                    delegate(WebGlObject a, WebGlObject b)
                        {
                            if (a.z != b.z)
                            {
                                return (int)(a.z - b.z);
                            }
                            return (int)(a.id - b.id);
                        });
            }

            // custom render plugins (pre pass)
            this.RenderPlugins(this.renderPluginsPre, scene, camera);

            //
            this.info.render.Calls = 0;
            this.info.render.Vertices = 0;
            this.info.render.Faces = 0;
            this.info.render.Points = 0;

            this.SetRenderTarget(renderTarget);

            if (this.autoClear || forceClear)
            {
                Clear(this.autoClearColor, this.autoClearDepth, this.autoClearStencil);
            }

            // set matrices for regular objects (frustum culled)

            // set matrices for immediate objects
            var renderList = scene.__webglObjectsImmediate;

            foreach (var webglObject in renderList)
            {
                var object3D = webglObject.object3D;
                if (object3D.Visible)
                {
                    SetupMatrices(object3D, camera);
                    //unrollImmediateBufferMaterial( webglObject );
                }
            }

            if (null != scene.OverrideMaterial)
            {
                var material = scene.OverrideMaterial;
                this.SetBlending(material.blending, material.blendEquation, material.blendSrc, material.blendDst);
                this.depthTest = material.depthTest;
                this.depthWrite = material.depthWrite;
                this.SetPolygonOffset(material.polygonOffset, material.polygonOffsetFactor, material.polygonOffsetUnits);
                this.RenderObjects(this.opaqueObjects, camera, lights, fog, true, material);
                this.RenderObjects(this.transparentObjects, camera, lights, fog, true, material);
                this.RenderObjectsImmediate(scene.__webglObjectsImmediate, string.Empty, camera, lights, fog, false, material);
            }
            else
            {
                Material material = null;
                // opaque pass (front-to-back Order)
                this.SetBlending(Three.NoBlending);
                this.RenderObjects(this.opaqueObjects, camera, lights, fog, false, material);
                this.RenderObjectsImmediate(scene.__webglObjectsImmediate, "opaque", camera, lights, fog, false, material);
                // transparent pass (back-to-front Order)
                this.RenderObjects(this.transparentObjects, camera, lights, fog, true, material);
                this.RenderObjectsImmediate(scene.__webglObjectsImmediate, "transparent", camera, lights, fog, true, material);
            }

            // custom render plugins (post pass)
            this.RenderPlugins(this.renderPluginsPost, scene, camera);

            // Generate mipmap if we're using any kind of mipmap filtering
            if ((null != renderTarget) && renderTarget.generateMipmaps 
                                       && renderTarget.minFilter != TextureMinFilter.Nearest 
                                       && renderTarget.minFilter != TextureMinFilter.Linear) 
            {
                this.UpdateRenderTargetMipmap( renderTarget );
            }

            // Ensure depth buffer writing is enabled so it can be cleared on next render
            this.depthTest = true;
            this.depthWrite = true;

            // GL.finish();
        }

        /// <summary>
        /// </summary>
        public void InitGl()
        {
            var extensions = new List<string>((GL.GetString(StringName.Extensions)).Split(' '));

            if (extensions.Contains("OES_texture_float") || extensions.Contains("GL_ARB_texture_float"))
            {
                this.glExtensionTextureFloat = true;
            }

            if (extensions.Contains("OES_standard_derivatives"))
            {
                this.glExtensionStandardDerivatives = true;
            }

            if (extensions.Contains("EXT_texture_filter_anisotropic")
                || extensions.Contains("GL_EXT_texture_filter_anisotropic")
                || extensions.Contains("MOZ_EXT_texture_filter_anisotropic")
                || extensions.Contains("WEBKIT_EXT_texture_filter_anisotropic"))
            {
                this.glExtensionTextureFilterAnisotropic = true;
            }

            if (extensions.Contains("WEBGL_compressed_texture_s3tc")
                || extensions.Contains("MOZ_WEBGL_compressed_texture_s3tc")
                || extensions.Contains("WEBKIT_WEBGL_compressed_texture_s3tc") || extensions.Contains("GL_S3_s3tc"))
            {
                this.glExtensionCompressedTextureS3TC = true;
            }

            this.glExtensionElementIndexUint = true;
        }

        /// <summary>
        /// </summary>
        public void SetDefaultGlState()
        {
            GL.ClearColor(0, 0, 0, 1); 
            GL.ClearDepth(1.0f);
            GL.ClearStencil(0);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Viewport(this._viewportX, this._viewportY, this._viewportWidth, this._viewportHeight);

            GL.ClearColor(Color.FromArgb(this.clearAlpha, this.clearColor));
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        private static void Clear(bool color, bool depth, bool stencil)
        {
            ClearBufferMask bits = 0;

            if (color)
            {
                bits |= ClearBufferMask.ColorBufferBit;
            }
            if (depth)
            {
                bits |= ClearBufferMask.DepthBufferBit;
            }
            if (stencil)
            {
                bits |= ClearBufferMask.StencilBufferBit;
            }

            GL.Clear(bits);
        }

        private static void ClearColor()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static void ClearDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        private static void ClearStencil()
        {
            GL.Clear(ClearBufferMask.StencilBufferBit);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="something"></param>
        /// <param name="object3D"></param>
        private void AddBufferImmediate(object something, Object3D object3D)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="buffer"></param>
        /// <param name="object3D"></param>
        private void AddBuffer(Dictionary<int, List<WebGlObject>> objlist, BaseGeometry buffer, Object3D object3D)
        {
            var id = object3D.id;

            List<WebGlObject> webGlObjects = null;
            if (!objlist.TryGetValue(id, out webGlObjects))
            {
                webGlObjects = new List<WebGlObject>();
                objlist.Add(id, webGlObjects);
            }

            var webGlObject = new WebGlObject { id = id, buffer = buffer, object3D = object3D, material = null, z = 0 };

            webGlObjects.Add(webGlObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private static bool AreCustomAttributesDirty(IAttributes material )
        {
            if (null == material) return false;

            foreach ( DictionaryEntry entry in material.attributes ) {

                var attribute = material.attributes[entry.Key] as Hashtable;
                Debug.Assert(null != attribute, "Failed to cast material.attributes[{0}] to Hashtable", (string)entry.Key);

                var needsUpdate = attribute["needsUpdate"];
                if (null != needsUpdate)
                {
                    if ((bool)needsUpdate)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private static void ClearCustomAttributes(IAttributes material )
        {
            if (null == material) return;

            foreach (DictionaryEntry entry in material.attributes)
            {
                var attribute = material.attributes[entry.Key] as Hashtable;
                Debug.Assert(null != attribute, "Failed to cast material.attributes[{0}] to Hashtable", (string)entry.Key);

                if (null != attribute["needsUpdate"])
                {
                    attribute["needsUpdate"] = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="scene"></param>
        private void RemoveObject( Object3D object3D, Scene scene )
        {
		    if ( object3D is Mesh  ||
			     object3D is PointCloud ||
			     object3D is Line ) 
            {
	//		    removeInstancesWebglObjects( scene.__webglObjects, object3D );
		    } else if ( object3D is ImmediateRenderObject /* || object3D.immediateRenderCallback */ ) {
			    this.RemoveInstances( scene.__webglObjectsImmediate, object3D );
		    }
		    //delete object3D.__webglActive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="object3D"></param>
        private void RemoveInstancesWebglObjects(List<WebGlObject> objlist, Object3D object3D ) {
	//	    delete objlist[ object3D.id ]; 
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objlist"></param>
        /// <param name="object3D"></param>
        private void RemoveInstances(List<WebGlObject> objlist, Object3D object3D)
        {
		    for ( var o = objlist.Count - 1; o >= 0; o -- )
            {
			    if ( objlist[ o ].object3D == object3D ) {

				    objlist.RemoveAt( o );
			    }
		    }
	    }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="scene"></param>
        private void AddObject(Object3D object3D, Scene scene)
        {
            if (object3D.__webglInit == false)
            {
                object3D.__webglInit = true;
                object3D.ModelViewMatrix = new Matrix4().Identity();
                object3D.NormalMatrix = new Matrix3().Identity();
            }

            if (null == object3D.Geometry) //  geometry is er al in
            {
                // ImmediateRenderObject
            }
            else if (object3D.Geometry.__webglInit == false)
            {
                object3D.Geometry.__webglInit = true;

                if (object3D.Geometry is BufferGeometry)
                {
                    InitDirectBuffers(object3D.Geometry as BufferGeometry);
                }
                else if (object3D is Mesh)
                {
                    if (object3D.__webglActive)
                    {
                        this.RemoveObject(object3D, scene);
                    }

                    this.InitGeometryGroups(scene, object3D, object3D.Geometry as Geometry);
                }
                else if (object3D is Line)
                {
                    if (null == object3D.Geometry.__webglVertexBuffer)
                    {
                        var geometry = object3D.Geometry as Geometry;

                        this.CreateLineBuffers(geometry);
                        InitLineBuffers(geometry, object3D);

					    geometry.VerticesNeedUpdate = true;
					    geometry.ColorsNeedUpdate = true;
					    geometry.LineDistancesNeedUpdate = true;

				    }
                }
                else if (object3D is PointCloud)
                {
                    if (null != object3D.Geometry.__webglVertexBuffer)
                    {
                        var geometry = object3D.Geometry as Geometry;

					    this.CreateParticleBuffers( geometry );
                        InitParticleBuffers(geometry, object3D);

					    geometry.VerticesNeedUpdate = true;
					    geometry.ColorsNeedUpdate = true;

				    }
                }
            }

            if (object3D.__webglActive == false)
            {
                if (object3D is Mesh)
                {
                    var mesh = object3D as Mesh;

                    if (object3D.Geometry is BufferGeometry)
                    {
                        this.AddBuffer(scene.__webglObjects, object3D.Geometry as BufferGeometry, object3D);
                    }
                    else if (object3D.Geometry is Geometry)
                    {
                        var geometry = object3D.Geometry as Geometry;
                        foreach (var geometryGroup in geometry.GeometryGroupsList)
                        {
                            this.AddBuffer(scene.__webglObjects, geometryGroup, object3D);
                        }
                    }
                }
                else if (object3D is Line || object3D is PointCloud)
                {
                    this.AddBuffer(scene.__webglObjects, object3D.Geometry, object3D);
                }
                else if (object3D is ImmediateRenderObject /*|| object3D is immediateRenderCallback*/)
                {
                    this.AddBufferImmediate( scene.__webglObjectsImmediate, object3D );
                    throw new NotImplementedException();
                }
            }

            object3D.__webglActive = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateLineBuffers(Geometry geometry)
        {
            GL.GenBuffers(1, out geometry.__webglVertexBuffer);
            GL.GenBuffers(1, out geometry.__webglColorBuffer);
            GL.GenBuffers(1, out geometry.__webglLineDistanceBuffer);

            this.info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitLineBuffers(Geometry geometry, Object3D object3D)
        {
		    var nvertices = geometry.Vertices.Count;

		    geometry.__vertexArray = new float[ nvertices * 3 ];
		    geometry.__colorArray = new float[ nvertices * 3 ];
		    geometry.__lineDistanceArray = new float[ nvertices * 1 ];

		    geometry.__webglLineCount = nvertices;

		    InitCustomAttributes ( geometry, object3D );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateParticleBuffers(Geometry geometry)
        {
            GL.GenBuffers(1, out geometry.__webglVertexBuffer);
            GL.GenBuffers(1, out geometry.__webglColorBuffer);

            this.info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitParticleBuffers(Geometry geometry, Object3D object3D)
        {
		    var nvertices = geometry.Vertices.Count;

		    geometry.__vertexArray = new float[ nvertices * 3 ];
		    geometry.__colorArray = new float[ nvertices * 3 ];

		    geometry.__sortArray = new object();

		    geometry.__webglParticleCount = nvertices;

		    InitCustomAttributes ( geometry, object3D );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private static void InitCustomAttributes(Geometry geometry, Object3D object3D)
        {
		    var nvertices = geometry.Vertices.Count;

		    var material = object3D.Material;
/*
		    if ( material.attributes ) {

			    if ( geometry.__webglCustomAttributesList == null ) {
				    geometry.__webglCustomAttributesList = [];
			    }

			    foreach ( var a in material.attributes )
                {
				    var attribute = material.attributes[ a ];

				    if ( ! attribute.__webglInitialized || attribute.createUniqueBuffers )
                    {
					    attribute.__webglInitialized = true;

					    var size = 1;   // "f" and "i"

					    if ( attribute.type == "v2" ) size = 2;
					    else if ( attribute.type == "v3" ) size = 3;
					    else if ( attribute.type == "v4" ) size = 4;
					    else if ( attribute.type == "c"  ) size = 3;

					    attribute.size = size;

					    attribute.array = new float[ nvertices * size ];

					    attribute.buffer = _gl.createBuffer();
					    attribute.buffer.belongsToAttribute = a;

					    attribute.needsUpdate = true;
				    }

				    geometry.__webglCustomAttributesList.Add( attribute );
			    }
		    }
 * */
        }

        /// <summary>
        /// </summary>
        /// <param name="geometry"></param>
        private static void InitDirectBuffers(BufferGeometry geometry)
        {
            foreach ( DictionaryEntry entry in geometry.attributes ) 
            {
                var bufferType = (entry.Key.Equals("index")) ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

                //// Note: Investigate use of dynamic (supported on Mono MaxOS and Ubuntu?)
                //var type = attribute.array.GetType();
                if (geometry.attributes[entry.Key] is BufferAttribute<ushort>)
                {
                    var attribute = geometry.attributes[entry.Key] as BufferAttribute<ushort>;

                    int buffer = 0;
                    GL.GenBuffers(1, out buffer);
                    attribute.buffer = buffer;

                    GL.BindBuffer(bufferType, attribute.buffer);
                    GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(ushort)), attribute.Array, BufferUsageHint.StaticDraw);

                    Debug.WriteLine("BufferData for attribute.buffer ushort {0}", attribute.buffer);

                }

                if (geometry.attributes[entry.Key] is BufferAttribute<uint>)
                {
                    var attribute = geometry.attributes[entry.Key] as BufferAttribute<uint>;

                    int buffer = 0;
                    GL.GenBuffers(1, out buffer);
                    attribute.buffer = buffer;

                    GL.BindBuffer(bufferType, attribute.buffer);
                    GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(uint)), attribute.Array, BufferUsageHint.StaticDraw);

                    Debug.WriteLine("BufferData for attribute.buffer uint {0}", attribute.buffer);

                }

                if (geometry.attributes[entry.Key] is BufferAttribute<float>)
                {
                    var attribute = geometry.attributes[entry.Key] as BufferAttribute<float>;

                    int buffer = 0;
                    GL.GenBuffers(1, out buffer);
                    attribute.buffer = buffer;

                    GL.BindBuffer(bufferType, attribute.buffer);
                    GL.BufferData(bufferType, (IntPtr)(attribute.length * sizeof(float)), attribute.Array, BufferUsageHint.StaticDraw);

                    Debug.WriteLine("BufferData for attribute.buffer float {0}", attribute.buffer);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="geometry"></param>
        private void InitGeometryGroups(Scene scene, Object3D object3D, Geometry geometry)
        {
            //    var g, geometryGroup, 
            var addBuffers = false;

            var material = object3D.Material;

            if (geometry.GeometryGroups == null || geometry.GroupsNeedUpdate)
            {
     //     	    delete scene.__webglObjects[object3D.id].;

                scene.__webglObjects.Remove(object3D.id);
                geometry.MakeGroups(material is MeshFaceMaterial, this.glExtensionElementIndexUint ? uint.MaxValue : ushort.MaxValue);
                geometry.GroupsNeedUpdate = false;
            }

            // create separate VBOs per geometry chunk

            foreach (var geometryGroup in  geometry.GeometryGroupsList)
            {
                // initialise VBO on the first access

                if (null != geometryGroup.__webglVertexBuffer)
                {
                    this.CreateMeshBuffers(geometryGroup);
                    this.InitMeshBuffers(geometryGroup, object3D);

                    geometry.VerticesNeedUpdate = true;
                    geometry.MorphTargetsNeedUpdate = true;
                    geometry.ElementsNeedUpdate = true;
                    geometry.UvsNeedUpdate = true;
                    geometry.NormalsNeedUpdate = true;
                    geometry.TangentsNeedUpdate = true;
                    geometry.ColorsNeedUpdate = true;

                    addBuffers = true;
                }
                else
                {
                    addBuffers = false;
                }

                if (addBuffers || object3D.__webglActive == false)
                {
                    this.AddBuffer(scene.__webglObjects, geometryGroup, object3D);
                }
            }

            object3D.__webglActive = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        private void InitObjects(Scene scene)
        {
            if (null == scene.__webglObjects)
            {
                scene.__webglObjects = new Dictionary<int, List<WebGlObject>>();
                scene.__webglObjectsImmediate = new List<WebGlObject>();
            }

            while (scene.__objectsAdded.Count > 0)
            {
                this.AddObject(scene.__objectsAdded[0], scene);
                scene.__objectsAdded.RemoveAt(0);
            }

            while (scene.__objectsRemoved.Count > 0)
            {
                this.RemoveObject(scene.__objectsRemoved[0], scene);
                scene.__objectsRemoved.RemoveAt(0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private void UnprojectObject(Scene scene, Object3D object3D, Camera camera)
        {
 		    var projectionMatrixInverse = new Matrix4();

            //return function ( vector, camera ) {

            //    projectionMatrixInverse.getInverse( camera.projectionMatrix );
            //    _projScreenMatrix.multiplyMatrices( camera.matrixWorld, projectionMatrixInverse );

            //    return vector.applyProjection( _projScreenMatrix );
       }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private void ProjectObject(Scene scene, Object3D object3D, Camera camera)
        {
            if (object3D.Visible == false)
            {
                return;
            }

            List<WebGlObject> webglObjects = null;
            scene.__webglObjects.TryGetValue(object3D.id, out webglObjects);

            if (null != webglObjects
                && (object3D.FrustumCulled == false || /*this._frustum.intersectsObject(object3D)*/ true))
            {
                this.updateObject(scene, object3D);

                foreach (var webglObject in webglObjects)
                {
                    this.UnrollBufferMaterial(webglObject);

                    webglObject.render = true;

                    if (this.SortObjects)
                    {
                        if (object3D.RenderDepth > 0)
                        {
                            webglObject.z = object3D.RenderDepth;
                        }
                        else
                        {
                            var vector3 = new Vector3().SetFromMatrixPosition(object3D.MatrixWorld);
                            vector3.ApplyProjection(this._projScreenMatrix);

                            webglObject.z = vector3.Z;
                        }
                    }
                }
            }

            foreach (var o in object3D.Children)
            {
                this.ProjectObject(scene, o, camera);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void RenderBuffer(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, GeometryGroup geometryGroup, Object3D object3D)
        {
            Debug.Assert(null != camera, "");
            Debug.Assert(null != lights, "");
            Debug.Assert(null != material, "");
            Debug.Assert(null != geometryGroup, "");

            if (material.visible == false)
            {
                return;
            }

            var program = this.SetProgram(camera, lights, fog, material, object3D);

            var attributesLocation = program.AttributesLocation;

            var updateBuffers = false;
            
            var wireframeBit = 0;
            var frameable = material as IWirerframe;
            if (frameable != null)
                wireframeBit = frameable.wireframe ? 1 : 0;

            var geometryGroupHash = (geometryGroup.Id * 0xffffff) + (program.Id * 2) + wireframeBit;

            if (geometryGroupHash != this._currentGeometryGroupHash)
            {
                this._currentGeometryGroupHash = geometryGroupHash;
                updateBuffers = true;
            }

            if (updateBuffers)
            {
                this.InitAttributes();
            }

            // vertices

            // TODO var meshBasicMaterial = material as MeshNormalMaterial;
            //Debug.Assert(null != meshBasicMaterial, "meshBasicMaterial is null, wrong cast?");

            var attributeLocation = attributesLocation["position"];
            if (null != material /* && !material.morphTargets */ && null != attributeLocation)
            {
                if (updateBuffers)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglVertexBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                }
            }
            else
            {
            //    if (object3D.morphTargetBase)
            //    {
            //        this.setupMorphTargets(material, geometryGroup, object3D);
            //    }
            }

            if (updateBuffers)
            {
                // custom attributesLocation

                // Use the per-geometryGroup custom attribute arrays which are setup in initMeshBuffers

                if (null != geometryGroup.__webglCustomAttributesList)
                {
                    for (var i = 0; i < geometryGroup.__webglCustomAttributesList.Count; i ++ ) {

                        var attribute = geometryGroup.__webglCustomAttributesList[ i ];

                        var buffer = attribute["buffer"] as Hashtable;
                        var belongsTo = buffer["belongsToAttribute"] as string;

                        if (attributesLocation[belongsTo] != null)
                        {
                            var id = (int)buffer["id"];
                            var location = (int)attributesLocation[belongsTo];
                            
                            //GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                            //enableAttribute(location);
                            //GL.VertexAttribPointer(location, (int)attribute["size"], VertexAttribPointerType.Float, false, 0, 0);
                        }
                    }
                }

                // colors

                var geometry = object3D.Geometry as Geometry;
                Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

                attributeLocation = attributesLocation["color"];
                if (null != attributeLocation)
                {
                    if (geometry.Colors.Count > 0 || geometry.Faces.Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglColorBuffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib3fv(attributesLocation.color, material.defaultAttributeValues.color);
                    //}
                }

                // normals

                attributeLocation = attributesLocation["normal"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglNormalBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                }

                // tangents
                attributeLocation = attributesLocation["tangent"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglTangentBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 4, VertexAttribPointerType.Float, false, 0, 0);
                }

                // uvs
                attributeLocation = attributesLocation["uv"];
                if (null != attributeLocation)
                {
                    if (geometry.FaceVertexUvs[0].Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUVBuffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib2fv((int)attributesLocation["uv"], material.defaultAttributeValues.uv);
                    //}
                }

                attributeLocation = attributesLocation["uv2"];
                if (null != attributeLocation)
                {
                    if (geometry.FaceVertexUvs[1].Count > 0)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUV2Buffer);
                        this.EnableAttribute((int)attributeLocation);
                        GL.VertexAttribPointer((int)attributeLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
                    }
                    //else if (material.defaultAttributeValues)
                    //{
                    //    GL.VertexAttrib2fv((int)attributesLocation["uv2"], material.defaultAttributeValues.uv2);
                    //}
                }

                //if (material.skinning && (int)attributesLocation["skinIndex"] >= 0 && (int)attributesLocation["skinWeight"] >= 0)
                //{
                //    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinIndicesBuffer);
                //    this.enableAttribute((int)attributesLocation["skinIndex"]);
                //    GL.VertexAttribPointer((int)attributesLocation["skinIndex"], 4, VertexAttribPointerType.Float, false, 0, 0);

                //    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinWeightsBuffer);
                //    this.enableAttribute((int)attributesLocation["skinWeight"]);
                //    GL.VertexAttribPointer((int)attributesLocation["skinWeight"], 4, VertexAttribPointerType.Float, false, 0, 0);
                //}

                // line distances

                attributeLocation = attributesLocation["lineDistance"];
                if (null != attributeLocation)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglLineDistanceBuffer);
                    this.EnableAttribute((int)attributeLocation);
                    GL.VertexAttribPointer((int)attributeLocation, 1, VertexAttribPointerType.Float, false, 0, 0);
                }
            }

            this.DisableUnusedAttributes();

            // render mesh

            if (object3D is Mesh)
            {
                var type = geometryGroup.__typeArray == typeof(ushort)
                                            ? DrawElementsType.UnsignedShort
                                            : DrawElementsType.UnsignedInt;

                // wireframe

                if (wireframeBit > 0)
                {
                    var wireFrameMaterial = material as IWirerframe;
                    Debug.Assert(null != wireFrameMaterial, "casting material to IWireFrameable failed");

                    this.SetLineWidth(wireFrameMaterial.wireframeLinewidth);
                    if (updateBuffers)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglLineBuffer);
                    }
                    GL.DrawElements(BeginMode.Lines, geometryGroup.__webglLineCount, type, 0);
                }
                else
                {
                    // triangles

                    if (updateBuffers)
                    {
                        Debug.Assert(geometryGroup.__webglFaceBuffer > 0, "geometryGroup.__webglFaceBuffer has not been created");
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglFaceBuffer);
                    }
                    GL.DrawElements(BeginMode.Triangles, geometryGroup.__webglFaceCount, DrawElementsType.UnsignedShort, 0);

                    this.info.render.Vertices += geometryGroup.__webglFaceCount;
                    this.info.render.Faces += geometryGroup.__webglFaceCount / 3;
                }

                this.info.render.Calls++;

            }
            else if (object3D is Line)
            {
                // render lines

             //   var mode = (object3D.type == LineStrip) ? BeginMode.LineStrip : BeginMode.Lines;

                //this.setLineWidth(material.linewidth);

                //GL.DrawArrays(mode, 0, geometryGroup.__webglLineCount);

                this.info.render.Calls ++;

            }
            else if (object3D is PointCloud)
            {
                // render particles

                GL.DrawArrays(BeginMode.Points, 0, geometryGroup.__webglParticleCount);

                this.info.render.Calls ++;
                this.info.render.Points += geometryGroup.__webglParticleCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="programAttributes"></param>
        /// <param name="geometryAttributes"></param>
        /// <param name="startIndex"></param>
        private void setupVertexAttributes(Material material, IEnumerable programAttributes, IDictionary geometryAttributes, int startIndex )
        {

		    foreach ( DictionaryEntry attribute in programAttributes ) 
            {
			    var attributePointer = attribute.Value;
			    if ( null != attributePointer ) 
                {
                    if (null != geometryAttributes[attribute.Key] as BufferAttribute<float>)
                    {
                        var attributeItem = geometryAttributes[attribute.Key] as BufferAttribute<float>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

					    GL.BindBuffer( BufferTarget.ArrayBuffer, attributeItem.buffer );
					    this.EnableAttribute( (int)attributePointer );
                        GL.VertexAttribPointer((int)attributePointer, attributeSize, VertexAttribPointerType.Float, false, 0, startIndex * attributeSize * sizeof(float));
				    }

                    if (null != geometryAttributes[attribute.Key] as BufferAttribute<uint>)
                    {
                        var attributeItem = geometryAttributes[attribute.Key] as BufferAttribute<uint>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);
                        this.EnableAttribute((int)attributePointer);
                        GL.VertexAttribPointer((int)attributePointer, attributeSize, VertexAttribPointerType.UnsignedInt, false, 0, startIndex * attributeSize * sizeof(uint));
                    }

                    if (null != geometryAttributes[attribute.Key] as BufferAttribute<ushort>)
                    {
                        var attributeItem = geometryAttributes[attribute.Key] as BufferAttribute<ushort>;
                        var attributeSize = attributeItem.ItemSize;

                        Debug.Assert(attributeItem.buffer > 0, "buffer has not been initialized");

                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);
                        this.EnableAttribute((int)attributePointer);
                        GL.VertexAttribPointer((int)attributePointer, attributeSize, VertexAttribPointerType.UnsignedShort, false, 0, startIndex * attributeSize * sizeof(ushort));
                    } 




/*                    else if ( material.defaultAttributeValues ) 
                    {
					    if ( material.defaultAttributeValues[ attributeName ].length == 2 )
                        {
						    GL.VertexAttrib2fv( attributePointer, material.defaultAttributeValues[ attributeName ] );
					    } else if ( material.defaultAttributeValues[ attributeName ].length == 3 ) {
						    GL.VertexAttrib3fv( attributePointer, material.defaultAttributeValues[ attributeName ] );
					    }
				    }
 * */
			    }
            }

		    this.DisableUnusedAttributes();
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="geometry"></param>
        /// <param name="object3D"></param>
        private void RenderBufferDirect(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, BufferGeometry geometry, Object3D object3D)
        {
            if (material.visible == false)
            {
            }
            
		    var program = this.SetProgram( camera, lights, fog, material, object3D );

		    var programAttributes = program.AttributesLocation;

		    var geometryAttributes = geometry.attributes;

            var updateBuffers = false;
            var wireframeBit = 0;//material.wireframe ? 1 : 0;
			var geometryHash = ( geometry.Id * 0xffffff ) + ( program.Id * 2 ) + wireframeBit;

		    if ( geometryHash != this._currentGeometryGroupHash ) 
            {
			    this._currentGeometryGroupHash = geometryHash;
			    updateBuffers = true;
		    }

		    if ( updateBuffers ) 
            {
			    this.InitAttributes();
		    }

		    // render mesh

		    if ( object3D is Mesh )
            {

                var index = geometryAttributes["index"] as IBufferAttribute;

			    if (null != index) 
                {

				    // indexed triangles

                    DrawElementsType type; 
                    var size = 0;

				    if ( index is short[] ) {

                        type = DrawElementsType.UnsignedInt;
					    size = 4;

				    } else {
                        type = DrawElementsType.UnsignedShort;
					    size = 2;
				    }

				    var offsets = geometry.Offsets;

				    if ( offsets.Count == 0 ) {

					    if ( updateBuffers ) {

						    this.setupVertexAttributes( material, programAttributes, geometryAttributes, 0 );
						    GL.BindBuffer( BufferTarget.ElementArrayBuffer, index.buffer );

					    }

                        GL.DrawElements(BeginMode.Triangles, index.length, type, 0);

                        this.info.render.Calls ++;
                        this.info.render.Vertices += index.length; // not really true, here vertices can be shared
                        this.info.render.Faces += index.length / 3;

				    } else {

					    // if there is more than 1 chunk
					    // must set attribute pointers to use new offsets for each chunk
					    // even if geometry and materials didn"t change

					    updateBuffers = true;

					    for ( var i = 0; i <  offsets.Count;  i ++ ) {

						    var startIndex = offsets[ i ].Index;

						    if ( updateBuffers ) {

							    this.setupVertexAttributes( material, programAttributes, geometryAttributes, startIndex );
							    GL.BindBuffer( BufferTarget.ElementArrayBuffer, index.buffer );

						    }

						    // render indexed triangles

						    GL.DrawElements( BeginMode.Triangles, offsets[ i ].Count, type, offsets[ i ].Start * size );

                            this.info.render.Calls ++;
                            this.info.render.Vertices += offsets[ i ].Count; // not really true, here vertices can be shared
                            this.info.render.Faces += offsets[ i ].Count / 3;

					    }

				    }

			    } else {

				    // non-indexed triangles

				    if ( updateBuffers ) {
					    this.setupVertexAttributes( material, programAttributes, geometryAttributes, 0 );
				    }

                    var position = geometryAttributes["position"] as BufferAttribute<float>;

				    // render non-indexed triangles

				    GL.DrawArrays( BeginMode.Triangles, 0, position.length );

                    this.info.render.Calls ++;
                    this.info.render.Vertices += position.length;
                    this.info.render.Faces += position.length / 3;
			    }

		    } 
            else if ( object3D is PointCloud ) 
            {
			    // render particles

			    if ( updateBuffers ) {
				    this.setupVertexAttributes( material, programAttributes, geometryAttributes, 0 );
			    }

                var position = geometryAttributes["position"] as BufferAttribute<float>;

			    // render particles
			    GL.DrawArrays( BeginMode.Points, 0, position.length );

                this.info.render.Calls ++;
                this.info.render.Points += position.length / 3;
		    } 
            else if ( object3D is Line ) 
            {
                var mode = (((Line)object3D).Type == Three.LineStrip) ? BeginMode.LineStrip : BeginMode.Lines;

                this.SetLineWidth( ((LineBasicMaterial)material).linewidth );

			    var index = geometryAttributes["index"];

			    if ( null != index ) {

				    // indexed lines

                    DrawElementsType type; 
                    var size = 0;

				    if ( index is short[] ) 
                    {
                        type = DrawElementsType.UnsignedInt;
					    size = 4;
				    } 
                    else
                    {
                        type = DrawElementsType.UnsignedShort;
					    size = 2;
				    }

				    var offsets = geometry.Offsets;

				    if ( offsets.Count == 0 ) {

					    if ( updateBuffers )
                        {
						    this.setupVertexAttributes( material, programAttributes, geometryAttributes, 0 );

                            if (index is ushort[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<ushort>)index).buffer);
                            if (index is uint[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<uint>)index).buffer);
                            if (index is float[])
                                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<float>)index).buffer);
                        }

				        var length = ((IBufferAttribute)index).length;

                        GL.DrawElements(mode, length, type, 0); // 2 bytes per Uint16Array

                        this.info.render.Calls ++;
                        this.info.render.Vertices += length; // not really true, here vertices can be shared

				    } else {

					    // if there is more than 1 chunk
					    // must set attribute pointers to use new offsets for each chunk
					    // even if geometry and materials didn"t change

					    if ( offsets.Count > 1 ) updateBuffers = true;

					    for ( var i = 0; i < offsets.Count;  i ++ )
                        {

						    var startIndex = offsets[ i ].Index;

						    if ( updateBuffers ) 
                            {
							    this.setupVertexAttributes( material, programAttributes, geometryAttributes, startIndex );

                                if (index is ushort[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<ushort>)index).buffer);
                                if (index is uint[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<uint>)index).buffer);
                                if (index is float[])
                                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((BufferAttribute<float>)index).buffer);
                            }

						    // render indexed lines

						    GL.DrawElements( mode, offsets[ i ].Count, type, offsets[ i ].Start * size ); // 2 bytes per Uint16Array

                            this.info.render.Calls ++;
                            this.info.render.Vertices += offsets[ i ].Count; // not really true, here vertices can be shared
					    }

				    }

			    } else {

				    // non-indexed lines

				    if ( updateBuffers ) {
					    this.setupVertexAttributes( material, programAttributes, geometryAttributes, 0 );
				    }

                    var position = geometryAttributes["position"] as BufferAttribute<float>;

			        var array = position.Array;

                    GL.DrawArrays(mode, 0, array.Length / 3);

			        this.info.render.Calls ++;
                    this.info.render.Points += array.Length / 3;
			    }

		    }
           
        }

        /// <summary>
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="webGlObject"></param>
        private void RenderImmediateObject(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, WebGlObject webGlObject)
        {
  	        var object3D = webGlObject.object3D;

            var program = this.SetProgram( camera, lights, fog, material, object3D );

		    this._currentGeometryGroupHash = - 1;

		    this.SetMaterialFaces( material );

            //if ( webGlObject.immediateRenderCallback ) {
            //    webGlObject.immediateRenderCallback( program, _gl, _frustum );
            //} else {
            //    webGlObject.render( function ( object3D ) { _this.renderBufferImmediate( object3D, program, material ); } );
            //}
        }

        /// <summary>
        /// </summary>
        /// <param name="renderList"></param>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="useBlending"></param>
        /// <param name="overrideMaterial"></param>
        private void RenderObjects(IEnumerable<WebGlObject> renderList, Camera camera, IEnumerable<Light> lights, Fog fog, bool useBlending, Material overrideMaterial)
        {
            foreach (var webglObject in renderList)
            {
                var object3D = webglObject.object3D;

                SetupMatrices(object3D, camera);

                Material material = null;
                if (null != overrideMaterial)
                {
                    material = overrideMaterial;
                }
                else
                {
                    material = webglObject.material;

                    if (null == material)
                    {
                        continue;
                    }

                    if (useBlending)
                    {
                        this.SetBlending(
                            material.blending,
                            material.blendEquation,
                            material.blendSrc,
                            material.blendDst);
                    }

                    this.depthTest = material.depthTest;
                    this.depthWrite = material.depthWrite;
                    this.SetPolygonOffset(
                        material.polygonOffset,
                        material.polygonOffsetFactor,
                        material.polygonOffsetUnits);
                }

                this.SetMaterialFaces(material);

                if (webglObject.buffer is BufferGeometry)
                {
                    this.RenderBufferDirect(camera, lights, fog, material, webglObject.buffer as BufferGeometry, object3D);
                }
                else
                {
                    this.RenderBuffer(camera, lights, fog, material, webglObject.buffer as GeometryGroup, object3D);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="renderList"></param>
        /// <param name="materialType"></param>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="useBlending"></param>
        /// <param name="overrideMaterial"></param>
        private void RenderObjectsImmediate(IEnumerable<WebGlObject> renderList, string materialType, Camera camera, IEnumerable<Light> lights, Fog fog, bool useBlending, Material overrideMaterial)
        {
            Material material = null;

            foreach (var webGlObject in renderList)
            {
                if (webGlObject.object3D.Visible)
                {
                    if (null != overrideMaterial)
                    {
                    }

                    this.RenderImmediateObject(camera, lights, fog, material, webGlObject);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sceneNodes"></param>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        private void RenderPlugins(IEnumerable<Object3D> sceneNodes, Scene scene, Camera camera)
        {
            foreach (var object3D in sceneNodes)
            {
                // reset state for plugin (to start from clean slate)

                this._currentProgram = -1;
                this._currentCamera = null;
                this._oldBlending = -1;
                this._oldDepthTest = -1;
                this._oldDepthWrite = -1;
                this._oldDoubleSided = -1;
                this._oldFlipSided = -1;
                this._currentGeometryGroupHash = -1;
                this._currentMaterialId = -1;
                this._lightsNeedUpdate = true;

                object3D.Render(scene, camera, this._currentWidth, this._currentHeight);

                // reset state after plugin (anything could have changed)

                this._currentProgram = -1;
                this._currentCamera = null;
                this._oldBlending = -1;
                this._oldDepthTest = -1;
                this._oldDepthWrite = -1;
                this._oldDoubleSided = -1;
                this._oldFlipSided = -1;
                this._currentGeometryGroupHash = -1;
                this._currentMaterialId = -1;
                this._lightsNeedUpdate = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <returns></returns>
        private int AllocateBones(Object3D object3D)
        {
            if ( this.supportsBoneTextures && null != object3D && /*null != object3D.skeleton && object3D.skeleton.useVertexTexture*/ false)
            {

            return 1024;

            } else {

                // default for when object is not specified
                // ( for example when prebuilding shader
                //   to be used with multiple objects )
                //
                //  - leave some extra space for other uniformsLocation
                //  - limit here is ANGLE's 254 max uniform vectors
                //    (up to 54 should be safe)

                var nVertexUniforms = 0;
                GL.GetInteger(GetPName.MaxVertexUniformVectors, out nVertexUniforms);
                var nVertexMatrices = Math.Floor((nVertexUniforms - 20) / 4.0);

                var maxBones = (int)nVertexMatrices;

                if (object3D != null && object3D is SkinnedMesh)
                {
                    throw new NotImplementedException();
                    //maxBones = Math.Min(object3D.skeleton.bones.Count, maxBones);

                    //if (maxBones < object3D.skeleton.bones.length)
                    //{

                    //    //Trace.TraceError( 'WebGLRenderer: too many bones - ' + object.skeleton.bones.length + ', this GPU supports just ' + maxBones + ' (try OpenGL instead of ANGLE)' );

                    //}

                }

                return maxBones;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        /// <returns></returns>
        private static LightCountInfo AllocateLights(IEnumerable<Light> lights)
        {
            var dirLights = 0;
            var pointLights = 0;
            var spotLights = 0;
            var hemiLights = 0;

            foreach (var light in lights)
            {
                if ( /*light.onlyShadow ||*/ light.Visible == false)
                {
                    continue;
                }

                if (light is DirectionalLight)
                {
                    dirLights ++;
                }
                if (light is PointLight)
                {
                    pointLights ++;
                }
                if (light is SpotLight)
                {
                    spotLights ++;
                }
                if (light is HemisphereLight)
                {
                    hemiLights ++;
                }
            }

            LightCountInfo lci;
            lci.directional = dirLights;
            lci.point = pointLights;
            lci.spot = spotLights;
            lci.hemi = hemiLights;

            return lci;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        /// <returns></returns>
        private static int AllocateShadows(IEnumerable<Light> lights)
        {
            var maxShadows = 0;

            foreach (var light in lights)
            {
                if (!light.CastShadow)
                {
                    continue;
                }

                if (light is SpotLight)
                {
                    maxShadows ++;
                }
                if (light is DirectionalLight && !((DirectionalLight)light).shadowCascade)
                {
                    maxShadows++;
                }
            }
            return maxShadows;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private static bool materialNeedsSmoothNormals (Material material ) 
        {
            if (null == material) return false;

            var meshNormalMaterial = material as MeshNormalMaterial;
            if (null != meshNormalMaterial)
            {
                if (meshNormalMaterial.Shading == Three.SmoothShading)
                    return true;
            }

            // TODO: do also for other material that carries shading

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private int bufferGuessNormalType(Material material)
        {
            // only MeshBasicMaterial and MeshDepthMaterial don't need normals

            if (material is MeshDepthMaterial)
            {
                return -1;
            }

            if (material is MeshBasicMaterial)
            {
                var m = material as MeshBasicMaterial;
                if (null == m.envMap)
                    return -1;
            }

            if (materialNeedsSmoothNormals(material))
            {
                return Three.SmoothShading;
            }
            else
            {
                return Three.FlatShading;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private bool bufferGuessUVType(Material material)
        {
            // material must use some texture to require uvs

            if (material is MeshBasicMaterial)
            {
                var m = material as MeshBasicMaterial;
                if (null != m.map
                || null != m.specularMap
                || null != m.alphaMap) return true;
            }

            if (material is MeshLambertMaterial)
            {
                var m = material as MeshLambertMaterial;
                if (null != m.map
                || null != m.lightMap
                || null != m.specularMap
                || null != m.alphaMap) return true;
            }

            if (material is MeshPhongMaterial)
            {
                var m = material as MeshPhongMaterial;
                if (null != m.map
                || null != m.lightMap
                || null != m.bumpMap
                || null != m.normalMap
                || null != m.specularMap
                || null != m.alphaMap) return true;
            }

            if (material is ShaderMaterial)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private int bufferGuessVertexColorType(Material material)
        {
            if (material is LineBasicMaterial)
            {
                var m = material as LineBasicMaterial;
                if (null != m.vertexColors)
                    return m.vertexColors.Length;
            }

            if (material is PointCloudMaterial)
            {
                var m = material as PointCloudMaterial;
                if (null != m.vertexColors)
                    return m.vertexColors.Length;
            }

            if (material is MeshPhongMaterial)
            {
                var m = material as MeshPhongMaterial;
                if (null != m.vertexColors)
                    return m.vertexColors.Length;
            }

            if (material is MeshLambertMaterial)
            {
                var m = material as MeshLambertMaterial;
                if (null != m.vertexColors)
                    return m.vertexColors.Length;
            }

            if (material is MeshBasicMaterial)
            {
                var m = material as MeshBasicMaterial;
                if (null != m.vertexColors)
                    return m.vertexColors.Length;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        private void CreateMeshBuffers(GeometryGroup geometryGroup)
        {
            GL.GenBuffers(1, out geometryGroup.__webglVertexBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglNormalBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglTangentBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglColorBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglUVBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglUV2Buffer);

            GL.GenBuffers(1, out geometryGroup.__webglSkinIndicesBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglSkinWeightsBuffer);

            GL.GenBuffers(1, out geometryGroup.__webglFaceBuffer);
            GL.GenBuffers(1, out geometryGroup.__webglLineBuffer);

            if (geometryGroup.NumMorphTargets > 0)
            {
                geometryGroup.__webglMorphTargetsBuffers = null;

                for (var i = 0; i < geometryGroup.NumMorphTargets; i++)
                {
                    throw new NotImplementedException();
                    //   geometryGroup.__webglMorphTargetsBuffers.Add(null);
                }
            }

            if (geometryGroup.NumMorphNormals > 0)
            {
                geometryGroup.__webglMorphNormalsBuffers = null;

                for (var i = 0; i < geometryGroup.NumMorphNormals; i++)
                {
                    throw new NotImplementedException();
                    //   geometryGroup.__webglMorphNormalsBuffers.Add(null);
                }
            }

            this.info.memory.Geometries++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private void deallocateMaterial(Material material)
        {
            //int program = material.program.program;

            //if ( program == null ) return;

            //material.program = null;

            //// only deallocate GL program if this was the last use of shared program
            //// assumed there is only single copy of any program in the _programs list
            //// (that's how it's constructed)

            //var deleteProgram = false;

            //foreach ( var programInfo in _programs) {

            //    if ( programInfo.program == program ) {

            //        programInfo.usedTimes --;

            //        if ( programInfo.usedTimes == 0 ) {

            //            deleteProgram = true;

            //        }

            //        break;

            //    }

            //}

            //if ( deleteProgram == true ) {

            //    // avoid using array.splice, this is costlier than creating new array from scratch

            //    var newPrograms = new List<WebGlProgram>();

            //    foreach ( var programInfo in _programs) {

            //        if ( programInfo.program != program ) {

            //            newPrograms.Add( programInfo );

            //        }

            //    }

            //    _programs = newPrograms;

            //    GL.DeleteProgram( program );

            //  //  this.info.memory.Programs --;

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisableUnusedAttributes()
        {
            for (var i = 0; i < this._enabledAttributes.Length; i ++)
            {
                if (this._enabledAttributes[i] != this._newAttributes[i])
                {
                    GL.DisableVertexAttribArray(i);
                    this._enabledAttributes[i] = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        private void EnableAttribute(int attribute)
        {
            this._newAttributes[attribute] = 1;

            if (this._enabledAttributes[attribute] == 0)
            {
                GL.EnableVertexAttribArray(attribute);
                this._enabledAttributes[attribute] = 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private Material getBufferMaterial(Object3D object3D, Geometry geometry)
        {
            //return object3D.material is MeshFaceMaterial
            //     : object3D.material;

            return object3D.Material;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="geometryGroup"></param>
        /// <returns></returns>
        private Material getBufferMaterial(Object3D object3D, GeometryGroup geometryGroup)
        {
            //return object3D.material is MeshFaceMaterial
            //     ? object3D.material.materials[ geometryGroup.materialIndex ]
            //     : object3D.material;

            return object3D.Material;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private int GetTextureUnit()
        {
            var textureUnit = this._usedTextureUnits;

            if (textureUnit >= this.maxTextures)
            {
                Trace.TraceWarning("WebGLRenderer: trying to use " + textureUnit + " texture units while this GPU supports only " + this.maxTextures);
            }

            this._usedTextureUnits += 1;

            return textureUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitAttributes()
        {
            for (var i = 0; i < this._newAttributes.Length; i ++)
            {
                this._newAttributes[i] = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        private static void SetDirectBuffers(BufferGeometry geometry, BufferUsageHint hint)
        {
		    foreach (DictionaryEntry attribute in geometry.attributes )
		    {
		        var attributeItem = attribute.Value as IBufferAttribute;
                Debug.Assert(null != attributeItem, "casting to IBufferAttribute failed");
		        
                if (attributeItem.needsUpdate ) 
                {
                    if ( (string)attribute.Key == "index" ) 
                    {
                        Debug.Assert(attributeItem.buffer > 0, "attributeItem.buffer has not been created");
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, attributeItem.buffer);

                        if (null != attribute.Value as BufferAttribute<float>)
                            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(attributeItem.length * sizeof(float)), ((BufferAttribute<float>)attribute.Value).Array, hint);
                        if (null != attribute.Value as BufferAttribute<ushort>)
                            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(attributeItem.length * sizeof(ushort)), ((BufferAttribute<ushort>)attribute.Value).Array, hint);
                        if (null != attribute.Value as BufferAttribute<uint>)
                            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(attributeItem.length * sizeof(uint)), ((BufferAttribute<uint>)attribute.Value).Array, hint);
                    } 
                    else 
                    {
                        Debug.Assert(attributeItem.buffer > 0, "attributeItem.buffer has not been created");
                        GL.BindBuffer(BufferTarget.ArrayBuffer, attributeItem.buffer);

                        if (null != attribute.Value as BufferAttribute<float>)
                            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(attributeItem.length * sizeof(float)), ((BufferAttribute<float>)attribute.Value).Array, hint);
                        if (null != attribute.Value as BufferAttribute<ushort>)
                            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(attributeItem.length * sizeof(ushort)), ((BufferAttribute<ushort>)attribute.Value).Array, hint);
                        if (null != attribute.Value as BufferAttribute<uint>)
                            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(attributeItem.length * sizeof(uint)), ((BufferAttribute<uint>)attribute.Value).Array, hint);
                    }

                    attributeItem.needsUpdate = false;
	            }
		    }
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        /// <param name="geometry"></param>
        private void initGeometryGroups(Scene scene, Object3D object3D, Geometry geometry)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="object3D"></param>
        private void InitMaterial(Material material, IEnumerable<Light> lights, Fog fog, Object3D object3D)
        {
            var shaderId = string.Empty;

            if (material is MeshDepthMaterial)
            {
                shaderId = "depth";
            }
            else if (material is MeshNormalMaterial)
            {
                shaderId = "normal";
            }
            else if (material is MeshBasicMaterial)
            {
                shaderId = "basic";
            }
            else if (material is MeshLambertMaterial)
            {
                shaderId = "lambert";
            }
            else if (material is MeshPhongMaterial)
            {
                shaderId = "phong";
            }
            else if (material is LineBasicMaterial)
            {
                shaderId = "basic";
            }
            else if (material is LineDashedMaterial)
            {
                shaderId = "dashed";
            }
            else if (material is PointCloudMaterial)
            {
                shaderId = "particle_basic";
            }

            if (!string.IsNullOrEmpty(shaderId))
            {
                var shader =  (WebGlShader)this.shaderLib[shaderId];

                material.__webglShader  = new WebGlShader();
                
                // TODO: good enough as Clone?
                material.__webglShader.Uniforms = new Uniforms();
                foreach (var e in shader.Uniforms)
                    material.__webglShader.Uniforms.Add(e.Key, e.Value);

                material.__webglShader.VertexShader = shader.VertexShader;
                material.__webglShader.FragmentShader = shader.FragmentShader;
                
                if (null == material.__webglShader)
                {
                    Trace.TraceError("Shader '{0}' could not be found. Check if it was created in UniformsLib", shaderId);
                    return;
                }
            }
            else
            {
                var sm = material as ShaderMaterial;

                material.__webglShader = new WebGlShader
                {
                    Uniforms = sm.uniforms,
                    VertexShader = sm.vertexShader,
                    FragmentShader = sm.fragmentShader
                };
            }

            // heuristics to create shader parameters according to __lights in the scene
            // (not to blow over maxLights budget)

            var maxLightCount = AllocateLights(lights);

            var maxShadows = AllocateShadows(lights);

            var maxBones = this.AllocateBones(object3D);

            var parameters = new Hashtable
                                 {
                                     { "precision", this._precision },
                                     { "supportsVertexTextures", this.supportsVertexTextures },
                                     { "fog", fog },                 
                                     { "logarithmicDepthBuffer", this._logarithmicDepthBuffer },
                                     { "maxBones", maxBones },
                                     //{ "useVertexTexture", (this.supportsBoneTextures) && (null != object3D)
                                     //    /*&& (null != object3D.skeleton) && (object3D.skeleton.useVertexTexture) */
                                     //}, // skinnedMesh
                                     { "maxMorphTargets", this.maxMorphTargets },
                                     { "maxMorphNormals", this.maxMorphNormals },
                                     { "maxDirLights", maxLightCount.directional },
                                     { "maxPointLights", maxLightCount.point },
                                     { "maxSpotLights", maxLightCount.spot },
                                     { "maxHemiLights", maxLightCount.hemi },
                                     { "maxShadows", maxShadows },
                                     { "shadowMapEnabled", this.shadowMapEnabled && object3D.ReceiveShadow && maxShadows > 0 },
                                     { "shadowMapType", this.shadowMapType },
                                     { "shadowMapDebug", this.shadowMapDebug },
                                     { "shadowMapCascade", this.shadowMapCascade },
                                     { "doubleSided", (material.side == Three.DoubleSide) },
                                     { "flipSided", (material.side == Three.BackSide) }
                                 };

            var meshBasicMaterial = material as MeshBasicMaterial;
            if (null != meshBasicMaterial)
            {
                parameters.Add("map", meshBasicMaterial.map != null);
                parameters.Add("useFog", meshBasicMaterial.fog);
                parameters.Add("envMap", meshBasicMaterial.envMap != null);
                parameters.Add("lightMap", meshBasicMaterial.lightMap != null);
                parameters.Add("specularMap", meshBasicMaterial.specularMap != null);
                parameters.Add("alphaMap", meshBasicMaterial.alphaMap != null);
                parameters.Add("vertexColors", meshBasicMaterial.vertexColors);
                parameters.Add("skinning", meshBasicMaterial.skinning);
                parameters.Add("morphTargets", meshBasicMaterial.morphTargets);
                parameters.Add("alphaTest", meshBasicMaterial.alphaTest);
            }

            var lineBasicMaterial = material as LineBasicMaterial;
            if (null != lineBasicMaterial)
            {
                parameters.Add("useFog", lineBasicMaterial.fog);
                parameters.Add("vertexColors", lineBasicMaterial.vertexColors);
            }

            var shaderMaterial = material as ShaderMaterial;
            if (null != shaderMaterial)
            {
                parameters.Add("alphaTest", shaderMaterial.alphaTest);
                parameters.Add("useFog", shaderMaterial.fog);
                parameters.Add("vertexColors", shaderMaterial.vertexColors);
                parameters.Add("skinning", shaderMaterial.skinning);

                parameters.Add("morphTargets", shaderMaterial.morphTargets);
                parameters.Add("morphNormals", shaderMaterial.morphNormals);
            }

            var pointCloudMaterial = material as PointCloudMaterial;
            if (null != pointCloudMaterial)
            {
                parameters.Add("map", pointCloudMaterial.map != null);
                parameters.Add("useFog", pointCloudMaterial.fog);
                parameters.Add("vertexColors", pointCloudMaterial.vertexColors);
                parameters.Add("alphaTest", pointCloudMaterial.alphaTest);
                
                parameters.Add("sizeAttenuation", pointCloudMaterial.sizeAttenuation);
            }

            var meshNormalMaterial = material as MeshNormalMaterial;
            if (null != meshNormalMaterial)
            {
                parameters.Add("alphaTest", meshNormalMaterial.alphaTest);

                parameters.Add("morphTargets", meshNormalMaterial.MorphTargets);
            }

            var meshLambertMaterial = material as MeshLambertMaterial;
            if (null != meshLambertMaterial)
            {
                parameters.Add("map", meshLambertMaterial.map != null);
                parameters.Add("useFog", meshLambertMaterial.fog);
                parameters.Add("envMap", meshLambertMaterial.envMap != null);
                parameters.Add("lightMap", meshLambertMaterial.lightMap != null);
                parameters.Add("specularMap", meshLambertMaterial.specularMap != null);
                parameters.Add("alphaMap", meshLambertMaterial.alphaMap != null);
                parameters.Add("vertexColors", meshLambertMaterial.vertexColors);
                parameters.Add("skinning", meshLambertMaterial.skinning);
                parameters.Add("morphTargets", meshLambertMaterial.morphTargets);
                parameters.Add("alphaTest", meshLambertMaterial.alphaTest);
                
                parameters.Add("wrapAround", meshLambertMaterial.WrapAround);
                parameters.Add("morphNormals", meshLambertMaterial.morphNormals);
            }

            var meshPhongMaterial = material as MeshPhongMaterial;
            if (null != meshPhongMaterial)
            {
                parameters.Add("map", meshPhongMaterial.map != null);
                parameters.Add("useFog", meshPhongMaterial.fog);
                parameters.Add("envMap", meshPhongMaterial.envMap != null);
                parameters.Add("lightMap", meshPhongMaterial.lightMap != null);
                parameters.Add("specularMap", meshPhongMaterial.specularMap != null);
                parameters.Add("alphaMap", meshPhongMaterial.alphaMap != null);
                parameters.Add("vertexColors", meshPhongMaterial.vertexColors);
                parameters.Add("skinning", meshPhongMaterial.skinning);
                parameters.Add("morphTargets", meshPhongMaterial.morphTargets);
                parameters.Add("alphaTest", meshPhongMaterial.alphaTest);
                
                parameters.Add("metal", meshPhongMaterial.Metal);
                parameters.Add("wrapAround", meshPhongMaterial.WrapAround);
                parameters.Add("morphNormals", meshPhongMaterial.morphNormals);
            }

            // Generate code

            var chunks = new List<object>();

            if (!string.IsNullOrEmpty(shaderId))
            {
                chunks.Add(shaderId);
            } 
            else
            {
                if (null != shaderMaterial)
                {
                    chunks.Add(shaderMaterial.fragmentShader);
                    chunks.Add(shaderMaterial.vertexShader);
                }
            }

            // 
            foreach (DictionaryEntry entry in material.defines)
            {
                chunks.Add(entry.Key);
                chunks.Add(entry.Value);
            }

            // 
            foreach (DictionaryEntry entry in parameters)
            {
                chunks.Add(entry.Key);
                chunks.Add(entry.Value);
            }

            // join
            var code = String.Join(",", chunks);

            WebGlProgram program = null;

            // Check if code has been already compiled

            foreach (var programInfo in this._programs)
            {
                if (programInfo.Code == code)
                {
                    program = programInfo;
                    program.UsedTimes ++;

                    Console.WriteLine("Reusing Shader Program {0}", programInfo.Id);

                    break;
                }
            }

            if (program == null)
            {
                program = new WebGlProgram(this, code, material, parameters);
                this._programs.Add(program);

                Console.WriteLine("New Shader Program {0}", program.Id);

                this.info.memory.Programs = this._programs.Count;
            }

            material.program = program;

            var attributes = material.program.AttributesLocation;

            if (null != meshBasicMaterial && meshBasicMaterial.morphTargets) 
            {
                meshBasicMaterial.numSupportedMorphTargets = 0;

                var basis = "morphTarget";

                for (var i = 0; i < this.maxMorphTargets; i ++ ) 
                {
                    var id = basis + i;
                    if ((int)attributes[id] >= 0)
                    {
                        meshBasicMaterial.numSupportedMorphTargets++;
                    }
                }
            }



            //if ( material.morphNormals ) {

            //    material.numSupportedMorphNormals = 0;

            //    var basis = "morphNormal";

            //    for (int i = 0; i < this.maxMorphNormals; i ++ ) {

            //        var id = basis + i;
            //        if ( attributesLocation[ id ] >= 0 ) {
            //            material.numSupportedMorphNormals ++;
            //        }
            //    }
            //}

            material.uniformsList = new Hashtable();

            foreach (var u in material.__webglShader.Uniforms)
            {
                var location = material.program.UniformsLocation[u.Key];
                if (location != null)
                {
                    material.uniformsList.Add(material.__webglShader.Uniforms[u.Key], (int)location);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void InitMeshBuffers(GeometryGroup geometryGroup, Object3D object3D)
        {
            var geometry = object3D.Geometry as Geometry;
            Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

            var faces3 = geometryGroup.Faces3;

            var nvertices = faces3.Count * 3;
            var ntris = faces3.Count * 1;
            var nlines = faces3.Count * 3;

            var material = this.getBufferMaterial(object3D, geometryGroup);

            var uvType = this.bufferGuessUVType(material);
            var normalType = this.bufferGuessNormalType(material);
            var vertexColorType = this.bufferGuessVertexColorType(material);

            geometryGroup.__vertexArray = new float[nvertices * 3];

            if (normalType > 0)
            {
                geometryGroup.__normalArray = new float[nvertices * 3];
            }

            if (geometry.HasTangents)
            {
                geometryGroup.__tangentArray = new float[nvertices * 4];
            }

            if (vertexColorType > 0)
            {
                geometryGroup.__colorArray = new float[nvertices * 3];
            }

            if (uvType)
            {
                if (geometry.FaceVertexUvs.Count > 0)
                {
                    geometryGroup.__uvArray = new float[nvertices * 2];
                }

                if (geometry.FaceVertexUvs.Count > 1)
                {
                    geometryGroup.__uv2Array = new float[nvertices * 2];
                }
            }

            if (geometry.SkinWeights.Count > 0 && geometry.SkinIndices.Count > 0)
            {
                geometryGroup.__skinIndexArray = new float[nvertices * 4];
                geometryGroup.__skinWeightArray = new float[nvertices * 4];
            }

            var UintArray = (this.glExtensionElementIndexUint && ntris > (ushort.MaxValue / 3)) ? typeof(uint) : typeof(ushort);

            geometryGroup.__typeArray = typeof(ushort);
            geometryGroup.__faceArray = new ushort[ntris * 3];
            geometryGroup.__lineArray = new ushort[nlines * 2];

            if (geometryGroup.NumMorphTargets > 0)
            {
                geometryGroup.__morphTargetsArrays = new List<float[]>();
                for (var i = 0; i < geometryGroup.NumMorphTargets; i++)
                {
                    geometryGroup.__morphTargetsArrays.Add(new float[nvertices * 3]);
                }
            }

            if (geometryGroup.NumMorphNormals > 0)
            {
                geometryGroup.__morphNormalsArrays = new List<float[]>();
                for (var i = 0; i < geometryGroup.NumMorphNormals; i++)
                {
                    geometryGroup.__morphNormalsArrays.Add(new float[nvertices * 3]);
                }
            }

            geometryGroup.__webglFaceCount = ntris * 3;
            geometryGroup.__webglLineCount = nlines * 2;

            // custom attributes
            
		    if (material is IAttributes &&  ((IAttributes)material).attributes != null) 
            {
                var attributesMaterial = material as IAttributes;

			    if ( geometryGroup.__webglCustomAttributesList == null ) {
				    geometryGroup.__webglCustomAttributesList = new List<Hashtable>();
			    }

			    foreach ( DictionaryEntry a in attributesMaterial.attributes)
			    {
			        var originalAttribute = a.Value as Hashtable;

				    // Do a shallow copy of the attribute object so different geometryGroup chunks use different
				    // attribute buffers which are correctly indexed in the setMeshBuffers function

				    var attribute = new Hashtable();

                    foreach (DictionaryEntry entry in originalAttribute)
                    {
                        var property = entry.Key;

                        attribute[property] = originalAttribute[property];
                    }

				    if ( null == attribute["__webglInitialized"] || null != attribute["createUniqueBuffers"] ) 
                    {
					    attribute["__webglInitialized"] = true;

					    var size = 1;   // "f" and "i"

                        if ((string)attribute["type"] == "v2") size = 2;
					    else if ( (string)attribute["type"] == "v3" ) size = 3;
					    else if ( (string)attribute["type"] == "v4" ) size = 4;
					    else if ( (string)attribute["type"] == "c"  ) size = 3;

                        attribute["size"] = size;

                        attribute["array"] = new float[nvertices * size];

                        int bufferId = 0;
                        GL.GenBuffers(1, out bufferId);

                        attribute["buffer"] = new Hashtable();

                        ((Hashtable)attribute["buffer"]).Add("id", bufferId);
                        ((Hashtable)attribute["buffer"]).Add("belongsToAttribute", a.Key);

                        originalAttribute["needsUpdate"] = true;
                        attribute["__original"] = originalAttribute;
                    }

				    geometryGroup.__webglCustomAttributesList.Add( attribute );
			    }
		    }
            
            geometryGroup.__inittedArrays = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="uniforms"></param>
        private void LoadUniformsGeneric(Hashtable uniforms)
        {
            var type = string.Empty;
            object value = null;
            var location = 0;

   //         Console.WriteLine("------");

            foreach (DictionaryEntry entry in uniforms)
            {
                try
                {
                    var uniform = entry.Key as KVP;
                    Debug.Assert(null != uniform, "key is null or could not cast to KVP");

                    // needsUpdate property is not added to all uniformsLocation.
           //         if (uniform.needsUpdate == false)
           //             continue;

                    type = uniform.Key;
                    value = uniform.Value;
                    location = (Int32)entry.Value;

            //        Console.WriteLine("loadUniformsGeneric: {0} {1} {2}", location, type, value);

                    switch (type)
                    {
                        case "1i":
                            GL.Uniform1(location, (int)value);
                            break;

                        case "1f":
                            GL.Uniform1(location, (float)value);
                            break;

                        case "2f":
                            var v2f = ((List<float>)value).ToArray();
                            GL.Uniform2(location, v2f[0], v2f[1]);
                            break;

                        case "3f":
                            var v3f = ((List<float>)value).ToArray();
                            GL.Uniform3(location, v3f[0], v3f[1], v3f[2]);
                            break;

                        case "4f":
                            var v4f = ((List<float>)value).ToArray();
                            GL.Uniform4(location, v4f[0], v4f[1], v4f[2], v4f[3]);
                            break;

                        case "1iv":
                            var oneiv = ((List<int>)value).ToArray();
                            GL.Uniform1( location, oneiv.Length, oneiv );
                            break;

                        case "3iv":
                            var threeiv = ((List<int>)value).ToArray();
                            GL.Uniform3( location, threeiv.Length, threeiv );
                            break;

                        case "1fv":
                            var onefv = ((List<float>)value).ToArray();
                            GL.Uniform1( location, onefv.Length, onefv );
                            break;

                        case "2fv":
                            var twofv = ((List<float>)value).ToArray();
                            GL.Uniform2( location, twofv.Length, twofv );
                            break;

                        case "3fv":
                            var threefv = ((List<float>)value).ToArray();
                            GL.Uniform3( location, threefv.Length, threefv );
                            break;

                        case "4fv":
                            var fourfv = ((List<float>)value).ToArray();
                            GL.Uniform4( location, fourfv.Length, fourfv );
                            break;

                        case "Matrix3fv":
                            var matrix3fv = ((List<float>)value).ToArray();
                            GL.UniformMatrix3(location, matrix3fv.Length, false, matrix3fv);
                            break;

                        case "Matrix4fv":
                            var matrix4fv = ((List<float>)value).ToArray();
                            GL.UniformMatrix4(location, matrix4fv.Length, false, matrix4fv);
                            break;

                        case "i":
                            GL.Uniform1(location, (int)value);
                            break;

                        case "f":
                            GL.Uniform1(location, Convert.ToSingle(value));
                            break;

                        case "v2":
                            var v2 = (Vector2)value;
                            GL.Uniform2(location, v2.X, v2.Y);
                            break;

                        case "v3":
                            var v3 = (Vector3)value;
                            GL.Uniform3(location, v3.X, v3.Y, v3.Z);
                            break;

                        case "v4":
                            // single Vector4
                            var v4 = (Vector4)value;
                            GL.Uniform4(location, v4.X, v4.Y, v4.Z, v4.W);
                            break;

                        case "c":
                            var color = (Color)value;
                            GL.Uniform3(location, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
                            break;
                            
                        case "iv1":
                            // flat array of integers (JS or typed array)
                            var iv1 = ((List<int>)value).ToArray();
                            GL.Uniform1(location, iv1.Length, iv1);
                            break;

                        case "iv":
                            // flat array of integers with 3 x N size (JS or typed array)
                            var iv = ((List<int>)value).ToArray();
                            GL.Uniform3(location, iv.Length / 3, iv);

                            break;

                        case "fv1":
                            // flat array of floats (JS or typed array)
                            var fv1 = ((List<float>)value).ToArray();
                            GL.Uniform1(location, fv1.Length, fv1);
                            break;
                                                            
                        case "fv":
                            // flat array of floats with 3 x N size (JS or typed array)
                            var fv = ((List<float>)value).ToArray();
                            GL.Uniform3(location, fv.Length / 3, fv);
                            break;
                            /*
                                                            case "v2v":

                                                                // array of THREE.Vector2

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = new Float32Array( 2 * value.length );

                                                                }

                                                                for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                    offset = i * 2;

                                                                    uniform._array[ offset ]   = value[ i ].x;
                                                                    uniform._array[ offset + 1 ] = value[ i ].y;

                                                                }

                                                                GL.Uniform2fv( location, uniform._array );

                                                                break;

                                                            case "v3v":

                                                                // array of THREE.Vector3

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = new Float32Array( 3 * value.length );

                                                                }

                                                                for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                    offset = i * 3;

                                                                    uniform._array[ offset ]   = value[ i ].x;
                                                                    uniform._array[ offset + 1 ] = value[ i ].y;
                                                                    uniform._array[ offset + 2 ] = value[ i ].z;

                                                                }

                                                                GL.Uniform3fv( location, uniform._array );

                                                                break;

                                                            case "v4v":

                                                                // array of THREE.Vector4

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = new Float32Array( 4 * value.length );

                                                                }

                                                                for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                    offset = i * 4;

                                                                    uniform._array[ offset ]   = value[ i ].x;
                                                                    uniform._array[ offset + 1 ] = value[ i ].y;
                                                                    uniform._array[ offset + 2 ] = value[ i ].z;
                                                                    uniform._array[ offset + 3 ] = value[ i ].w;

                                                                }

                                                                GL.Uniform4fv( location, uniform._array );

                                                                break;

                                                            case "m3":

                                                                // single THREE.Matrix3
                                                                GL.UniformMatrix3fv( location, false, value.elements );

                                                                break;

                                                            case "m3v":

                                                                // array of THREE.Matrix3

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = new Float32Array( 9 * value.length );

                                                                }

                                                                for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                    value[ i ].flattenToArrayOffset( uniform._array, i * 9 );

                                                                }

                                                                GL.UniformMatrix3fv( location, false, uniform._array );

                                                                break;

                                                            case "m4":

                                                                // single THREE.Matrix4
                                                                GL.UniformMatrix4fv( location, false, value.elements );

                                                                break;

                                                            case "m4v":

                                                                // array of THREE.Matrix4

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = new Float32Array( 16 * value.length );

                                                                }

                                                                for ( var i = 0, il = value.length; i < il; i ++ ) {

                                                                    value[ i ].flattenToArrayOffset( uniform._array, i * 16 );

                                                                }

                                                                GL.UniformMatrix4fv( location, false, uniform._array );

                                                                break;
                            */
                        case "t":

                            // single THREE.Texture (2d or cube)

                            var texture = (Texture)value;
                            var textureUnit = this.GetTextureUnit();

                            GL.Uniform1(location, textureUnit);

                            if (null == texture)
                            {
                                continue;
                            }

                            //if ( texture is CubeTexture ||
                            //   ( texture.image is Array && texture.image.length == 6 ) ) { // CompressedTexture can have Array in image :/
                            //    setCubeTexture( texture, textureUnit );
                            //} else if ( texture is OpenTKRenderTargetCube ) {
                            //    setCubeTextureDynamic( texture, textureUnit );
                            //} else {
                            this.SetTexture(texture, textureUnit);
                            //}

                            break;
                            /*
                                                            case "tv":

                                                                // array of THREE.Texture (2d)

                                                                if ( uniform._array == null ) {

                                                                    uniform._array = [];

                                                                }

                                                                for ( var i = 0, il = uniform.value.length; i < il; i ++ ) {

                                                                    uniform._array[ i ] = getTextureUnit();

                                                                }

                                                                GL.Uniform1iv( location, uniform._array );

                                                                for ( var i = 0, il = uniform.value.length; i < il; i ++ ) {

                                                                    texture = uniform.value[ i ];
                                                                    textureUnit = uniform._array[ i ];

                                                                    if ( ! texture ) continue;

                                                                    _this.setTexture( texture, textureUnit );

                                                                }

                                                                break;

                                                            default:

                                                                Trace.TraceWarning( "THREE.WebGLRenderer: Unknown uniform type: " + type );
                                                */
                    }
                }
                catch (OpenTK.GraphicsException e)
                {
                    Trace.TraceError("Setting KVP at location {0}, of type {1}; Message: {2}", location, type, e.Message);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="uniformsLocation"></param>
        /// <param name="object3D"></param>
        private static void LoadUniformsMatrices(Hashtable uniformsLocation, Object3D object3D)
        {
            //Console.WriteLine("object3D.ModelViewMatrix\n{0}", object3D.ModelViewMatrix);

            var location = uniformsLocation["modelViewMatrix"];
            if (null != location)
                GL.UniformMatrix4((int)location, 1, false, object3D.ModelViewMatrix.Elements);

            location = uniformsLocation["normalMatrix"];
            if (location != null)
            {
                GL.UniformMatrix3( (int)location, 1, false, object3D.NormalMatrix.Elements);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private bool MaterialNeedsSmoothNormals(Material material)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsCommon(Uniforms uniforms, Material material)
        {
            Texture uvScaleMap = null;

            Uniforms.SetValue(uniforms, "opacity", material.opacity);

            if (material is MeshBasicMaterial)
            {
                var m = material as MeshBasicMaterial;

                Uniforms.SetValue(uniforms, "diffuse", this.gammaInput ? this.copyGammaToLinear(m.color) : m.color);

                Uniforms.SetValue(uniforms, "map", m.map);
                Uniforms.SetValue(uniforms, "lightMap", m.lightMap);
                Uniforms.SetValue(uniforms, "specularMap", m.specularMap);
                Uniforms.SetValue(uniforms, "alphaMap", m.alphaMap);

                if (null != m.map)
                    uvScaleMap = m.map;
                else if (null != m.specularMap)
                    uvScaleMap = m.specularMap;
                else if (null != m.alphaMap)
                    uvScaleMap = m.alphaMap;

                Uniforms.SetValue(uniforms, "envMap", m.envMap);

                Uniforms.SetValue(uniforms, "flipEnvMap", ( m.envMap is OpenTKRenderTargetCube ) ? 1 : - 1);

                if (this.gammaInput)
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.reflectivity * m.reflectivity);
                }
                else
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.reflectivity);
                }

                Uniforms.SetValue(uniforms, "refractionRatio", m.refractionRatio);

                Uniforms.SetValue(uniforms, "combine", m.combine);
                Uniforms.SetValue(uniforms, "useRefract", false /*m.envMap && (m.envMap.mapping is CubeRefractionMapping) */);
            }

            if (material is MeshPhongMaterial)
            {
                var m = material as MeshPhongMaterial;

                Uniforms.SetValue(uniforms, "diffuse", this.gammaInput ? this.copyGammaToLinear(m.color) : m.color);

                Uniforms.SetValue(uniforms, "map", m.map);
                Uniforms.SetValue(uniforms, "lightMap", m.lightMap);
                Uniforms.SetValue(uniforms, "specularMap", m.specularMap);
                Uniforms.SetValue(uniforms, "alphaMap", m.alphaMap);

                if (null != m.bumpMap)
                {
                    Uniforms.SetValue(uniforms, "bumpMap", m.bumpMap);
                    Uniforms.SetValue(uniforms, "bumpScale", m.bumpScale);
                }

                if (null != m.normalMap) 
                {
                    Uniforms.SetValue(uniforms, "normalMap", m.normalMap);
                    Uniforms.SetValue(uniforms, "normalScale", m.normalScale);
                }

                Uniforms.SetValue(uniforms, "envMap", m.envMap);

                Uniforms.SetValue(uniforms, "flipEnvMap", (m.envMap is OpenTKRenderTargetCube) ? 1 : -1);

                if (this.gammaInput)
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.reflectivity * m.reflectivity);
                }
                else
                {
                    Uniforms.SetValue(uniforms, "reflectivity", m.reflectivity);
                }

                Uniforms.SetValue(uniforms, "refractionRatio", m.refractionRatio);

                Uniforms.SetValue(uniforms, "combine", m.combine);
                Uniforms.SetValue(uniforms, "useRefract", false /*m.envMap && (m.envMap.mapping is CubeRefractionMapping) */);
            }



            // uv repeat and offset setting priorities
            //  1. color map
            //  2. specular map
            //  3. normal map
            //  4. bump map
            //  5. alpha map
/*
            if (null != m.map)
            {
                uvScaleMap = m.map;
            }
            else if (null != m.specularMap)
            {
                uvScaleMap = m.specularMap;

                //} else if ( null != m.normalMap ) {

                //    uvScaleMap = m.normalMap;

                //} else if ( null != m.bumpMap ) {

                //    uvScaleMap = null != m.bumpMap;
            }
            else if (null != m.alphaMap)
            {
                uvScaleMap = m.alphaMap;
            }
*/
            if (uvScaleMap != null)
            {
                var offset = uvScaleMap.offset;
                var repeat = uvScaleMap.repeat;

                uniforms["offsetRepeat"].Value = new Vector4(offset.X, offset.Y, repeat.X, repeat.Y);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetBlending(int value)
        {
            if (this._contextGlobalCompositeOperation != value)
            {
                if (value == Three.NormalBlending)
                {
                    //                   _context.globalCompositeOperation = "source-over";
                }
                else if (value == Three.AdditiveBlending)
                {
                    //                   _context.globalCompositeOperation = "lighter";
                }
                else if (value == Three.SubtractiveBlending)
                {
                    //                  _context.globalCompositeOperation = "darker";
                }

                this._contextGlobalCompositeOperation = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="blending"></param>
        /// <param name="blendEquation"></param>
        /// <param name="blendSrc"></param>
        /// <param name="blendDst"></param>
        private void SetBlending(int blending, int blendEquation, int blendSrc, int blendDst)
        {
            if (blending != this._oldBlending)
            {
                if (blending == Three.NoBlending)
                {
                    GL.Disable(EnableCap.Blend);
                }
                else if (blending == Three.AdditiveBlending)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
                }
                else if (blending == Three.SubtractiveBlending)
                {
                    // TOD: Find blendFuncSeparate() combination
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.OneMinusSrcColor);
                }
                else if (blending == Three.MultiplyBlending)
                {
                    // TOD: Find blendFuncSeparate() combination
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquation(BlendEquationMode.FuncAdd);
                    GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.SrcAlpha);
                }
                else if (blending == Three.CustomBlending)
                {
                    GL.Enable(EnableCap.Blend);
                }
                else
                {
                    GL.Enable(EnableCap.Blend);
                    GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
                    GL.BlendFuncSeparate(
                        BlendingFactorSrc.SrcAlpha,
                        BlendingFactorDest.OneMinusSrcAlpha,
                        BlendingFactorSrc.One,
                        BlendingFactorDest.OneMinusSrcAlpha);
                }
                this._oldBlending = blending;
            }

            if (blending == Three.CustomBlending)
            {
                if (blendEquation != this._oldBlendEquation)
                {
                    // GL.BlendEquation(paramThreeToGL(blendEquation));
                    this._oldBlendEquation = blendEquation;
                }
                if (blendSrc != this._oldBlendSrc || blendDst != this._oldBlendDst)
                {
                    //   GL.BlendFunc(paramThreeToGL(blendSrc), paramThreeToGL(blendDst));
                    this._oldBlendSrc = blendSrc;
                    this._oldBlendDst = blendDst;
                }
            }
            else
            {
                this._oldBlendEquation = -1;
                this._oldBlendSrc = -1;
                this._oldBlendDst = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        private void SetLineWidth(float width)
        {
            if ( width != this._oldLineWidth ) {

            GL.LineWidth( width );

            this._oldLineWidth = width;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        private void SetMaterialFaces(Material material)
        {
            var doubleSided = (material.side == Three.DoubleSide) ? 1 : 0;
            var flipSided = (material.side == Three.BackSide) ? 1 : 0;

            if (this._oldDoubleSided != doubleSided)
            {
                if (doubleSided > 0)
                {
                    GL.Disable(EnableCap.CullFace);
                }
                else
                {
                    GL.Enable(EnableCap.CullFace);
                }

                this._oldDoubleSided = doubleSided;
            }

            if (this._oldFlipSided != flipSided)
            {
                if (flipSided > 0)
                {
                    GL.FrontFace(FrontFaceDirection.Cw);
                }
                else
                {
                    GL.FrontFace(FrontFaceDirection.Ccw);
                }

                this._oldFlipSided = flipSided;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        /// <param name="hint"></param>
        /// <param name="dispose"></param>
        /// <param name="material"></param>
        private void SetMeshBuffers(GeometryGroup geometryGroup, Object3D object3D, BufferUsageHint hint, bool dispose, Material material)
        {
            if (!geometryGroup.__inittedArrays)
            {
                return;
            }

            var normalType = this.bufferGuessNormalType(material);
            var vertexColorType = this.bufferGuessVertexColorType(material);
            var uvType = this.bufferGuessUVType(material);

            var needsSmoothNormals = (normalType > 0) && (normalType == Three.SmoothShading);

            var vertexIndex = 0;

            var offset = 0;
            var offset_uv = 0;
            var offset_uv2 = 0;
            var offset_face = 0;
            var offset_normal = 0;
            var offset_tangent = 0;
            var offset_line = 0;
            var offset_color = 0;
            var offset_skin = 0;
            var offset_morphTarget = 0;
            var offset_custom = 0;
            var offset_customSrc = 0;

            var vertexArray = geometryGroup.__vertexArray;
            var uvArray = geometryGroup.__uvArray;
            var uv2Array = geometryGroup.__uv2Array;
            var normalArray = geometryGroup.__normalArray;
            var tangentArray = geometryGroup.__tangentArray;
            var colorArray = geometryGroup.__colorArray;

            var skinIndexArray = geometryGroup.__skinIndexArray;
            var skinWeightArray = geometryGroup.__skinWeightArray;

            var morphTargetsArrays = geometryGroup.__morphTargetsArrays;
            var morphNormalsArrays = geometryGroup.__morphNormalsArrays;

            var customAttributes = geometryGroup.__webglCustomAttributesList;
            //	    var customAttribute;

            var faceArray = geometryGroup.__faceArray;
            var lineArray = geometryGroup.__lineArray;

            var geometry = object3D.Geometry as Geometry; // this is shared for all chunks
            Debug.Assert(null != geometry, "object3D.Geometry is not Geometry");

            var dirtyVertices = geometry.VerticesNeedUpdate;
            var dirtyElements = geometry.ElementsNeedUpdate;
            var dirtyUvs = geometry.UvsNeedUpdate;
            var dirtyNormals = geometry.NormalsNeedUpdate;
            var dirtyTangents = geometry.TangentsNeedUpdate;
            var dirtyColors = geometry.ColorsNeedUpdate;
            var dirtyMorphTargets = geometry.MorphTargetsNeedUpdate;

            var vertices = geometry.Vertices;
            var chunk_faces3 = geometryGroup.Faces3;
            var obj_faces = geometry.Faces;

            var obj_uvs = geometry.FaceVertexUvs[0];
            List<List<Vector2>> obj_uvs2 = null;
            if (geometry.FaceVertexUvs.Count > 1)
            {
                obj_uvs2 = geometry.FaceVertexUvs[1];
            }

      	    var obj_colors = geometry.Colors;

            var obj_skinIndices = geometry.SkinIndices;
            var obj_skinWeights = geometry.SkinWeights;

            var morphTargets = geometry.MorphTargets;
            var morphNormals = geometry.MorphNormals;

            if (dirtyVertices)
            {
                foreach (var chuck_face in chunk_faces3)
                {
                    var face = obj_faces[chuck_face];

                    var v1 = vertices[face.a];
                    var v2 = vertices[face.b];
                    var v3 = vertices[face.c];

                    vertexArray[offset] = v1.X;
                    vertexArray[offset + 1] = v1.Y;
                    vertexArray[offset + 2] = v1.Z;

                    vertexArray[offset + 3] = v2.X;
                    vertexArray[offset + 4] = v2.Y;
                    vertexArray[offset + 5] = v2.Z;

                    vertexArray[offset + 6] = v3.X;
                    vertexArray[offset + 7] = v3.Y;
                    vertexArray[offset + 8] = v3.Z;

                    offset += 9;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglVertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(float)), vertexArray, hint);

                Debug.WriteLine("BufferData for __webglVertexBuffer float {0}", geometryGroup.__webglVertexBuffer);
            }

            if (dirtyMorphTargets)
            {
                for (var vk = 0; vk < morphTargets.Count; vk++)
                {
                    offset_morphTarget = 0;

                    throw new NotImplementedException();

                    foreach (var chf in chunk_faces3)
                    {
                        var face = obj_faces[chf];
                        /*
                        // morph positions

                        var v1 = morphTargets[vk].vertices[face.a];
                        var v2 = morphTargets[vk].vertices[face.b];
                        var v3 = morphTargets[vk].vertices[face.c];

                        var vka = morphTargetsArrays[vk];

                        vka[offset_morphTarget] = v1.X;
                        vka[offset_morphTarget + 1] = v1.y;
                        vka[offset_morphTarget + 2] = v1.z;

                        vka[offset_morphTarget + 3] = v2.x;
                        vka[offset_morphTarget + 4] = v2.y;
                        vka[offset_morphTarget + 5] = v2.z;

                        vka[offset_morphTarget + 6] = v3.x;
                        vka[offset_morphTarget + 7] = v3.y;
                        vka[offset_morphTarget + 8] = v3.z;

                        // morph normals

                        if (material.morphNormals)
                        {
                            if (needsSmoothNormals)
                            {
                                var faceVertexNormals = morphNormals[vk].vertexNormals[chf];

                                n1 = faceVertexNormals.a;
                                n2 = faceVertexNormals.b;
                                n3 = faceVertexNormals.c;
                            }
                            else
                            {
                                n1 = morphNormals[vk].faceNormals[chf];
                                n2 = n1;
                                n3 = n1;
                            }

                            var nka = morphNormalsArrays[vk];

                            nka[offset_morphTarget] = n1.x;
                            nka[offset_morphTarget + 1] = n1.y;
                            nka[offset_morphTarget + 2] = n1.z;

                            nka[offset_morphTarget + 3] = n2.x;
                            nka[offset_morphTarget + 4] = n2.y;
                            nka[offset_morphTarget + 5] = n2.z;

                            nka[offset_morphTarget + 6] = n3.x;
                            nka[offset_morphTarget + 7] = n3.y;
                            nka[offset_morphTarget + 8] = n3.z;
                        }

                        //
                        offset_morphTarget += 9;
 * */
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglMorphTargetsBuffers[vk]);
                    //          GL.BufferData(BufferTarget.ArrayBuffer, morphTargetsArrays[vk], hint);

                    //                 if (material.morphNormals)
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglMorphNormalsBuffers[vk]);
                        //              GL.BufferData(BufferTarget.ArrayBuffer, morphNormalsArrays[vk], hint);
                    }
                }
            }

            if (obj_skinWeights.Count > 0)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    // weights

                    var sw1 = obj_skinWeights[face.a];
                    var sw2 = obj_skinWeights[face.b];
                    var sw3 = obj_skinWeights[face.c];

                    skinWeightArray[offset_skin] = sw1.X;
                    skinWeightArray[offset_skin + 1] = sw1.Y;
                    skinWeightArray[offset_skin + 2] = sw1.Z;
                    skinWeightArray[offset_skin + 3] = sw1.W;

                    skinWeightArray[offset_skin + 4] = sw2.X;
                    skinWeightArray[offset_skin + 5] = sw2.Y;
                    skinWeightArray[offset_skin + 6] = sw2.Z;
                    skinWeightArray[offset_skin + 7] = sw2.W;

                    skinWeightArray[offset_skin + 8] = sw3.X;
                    skinWeightArray[offset_skin + 9] = sw3.Y;
                    skinWeightArray[offset_skin + 10] = sw3.Z;
                    skinWeightArray[offset_skin + 11] = sw3.W;

                    // indices

                    var si1 = obj_skinIndices[face.a];
                    var si2 = obj_skinIndices[face.b];
                    var si3 = obj_skinIndices[face.c];

                    skinIndexArray[offset_skin] = si1.X;
                    skinIndexArray[offset_skin + 1] = si1.Y;
                    skinIndexArray[offset_skin + 2] = si1.Z;
                    skinIndexArray[offset_skin + 3] = si1.W;

                    skinIndexArray[offset_skin + 4] = si2.X;
                    skinIndexArray[offset_skin + 5] = si2.Y;
                    skinIndexArray[offset_skin + 6] = si2.Z;
                    skinIndexArray[offset_skin + 7] = si2.W;

                    skinIndexArray[offset_skin + 8] = si3.X;
                    skinIndexArray[offset_skin + 9] = si3.Y;
                    skinIndexArray[offset_skin + 10] = si3.Z;
                    skinIndexArray[offset_skin + 11] = si3.W;

                    offset_skin += 12;
                }

                if (offset_skin > 0)
                {
                    throw new NotImplementedException();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinIndicesBuffer);
                    //           GL.BufferData(BufferTarget.ArrayBuffer, skinIndexArray, hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglSkinWeightsBuffer);
                    //           GL.BufferData(BufferTarget.ArrayBuffer, skinWeightArray, hint);
                }
            }

            if (dirtyColors && vertexColorType > 0)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    IList<Color> vertexColors = face.VertexColors;
                    var faceColor = face.color;

                    Color c1, c2, c3;

                    if (vertexColors.Count == 3 && vertexColorType == Three.VertexColors)
                    {
                        c1 = vertexColors[0];
                        c2 = vertexColors[1];
                        c3 = vertexColors[2];
                    }
                    else
                    {
                        c1 = faceColor;
                        c2 = faceColor;
                        c3 = faceColor;
                    }

                    colorArray[offset_color] = c1.R;
                    colorArray[offset_color + 1] = c1.G;
                    colorArray[offset_color + 2] = c1.B;

                    colorArray[offset_color + 3] = c2.R;
                    colorArray[offset_color + 4] = c2.G;
                    colorArray[offset_color + 5] = c2.B;

                    colorArray[offset_color + 6] = c3.R;
                    colorArray[offset_color + 7] = c3.G;
                    colorArray[offset_color + 8] = c3.B;

                    offset_color += 9;
                }

                if (offset_color > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglColorBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length * sizeof(float)), colorArray, hint);

                    Debug.WriteLine("BufferData for __webglColorBuffer float {0}", geometryGroup.__webglColorBuffer);
                }
            }

            if (dirtyTangents && geometry.HasTangents)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexTangents = face.VertexTangents;

                    var t1 = vertexTangents[0];
                    var t2 = vertexTangents[1];
                    var t3 = vertexTangents[2];

                    tangentArray[offset_tangent] = t1.X;
                    tangentArray[offset_tangent + 1] = t1.Y;
                    tangentArray[offset_tangent + 2] = t1.Z;
                    tangentArray[offset_tangent + 3] = t1.W;

                    tangentArray[offset_tangent + 4] = t2.X;
                    tangentArray[offset_tangent + 5] = t2.Y;
                    tangentArray[offset_tangent + 6] = t2.Z;
                    tangentArray[offset_tangent + 7] = t2.W;

                    tangentArray[offset_tangent + 8] = t3.X;
                    tangentArray[offset_tangent + 9] = t3.Y;
                    tangentArray[offset_tangent + 10] = t3.Z;
                    tangentArray[offset_tangent + 11] = t3.W;

                    offset_tangent += 12;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglTangentBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tangentArray.Length * sizeof(float)), tangentArray, hint); // * 3 ??????

                Debug.WriteLine("BufferData for __webglTangentBuffer float {0}", geometryGroup.__webglTangentBuffer);
            }

            if (dirtyNormals && normalType > 0)
            {
                for (var f = 0; f < chunk_faces3.Count; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexNormals = face.VertexNormals;
                    var faceNormal = face.Normal;

                    if (vertexNormals.Count == 3 && needsSmoothNormals)
                    {
                        for (var i = 0; i < 3; i ++)
                        {
                            var vn = vertexNormals[i];

                            normalArray[offset_normal] = vn.X;
                            normalArray[offset_normal + 1] = vn.Y;
                            normalArray[offset_normal + 2] = vn.Z;

                            offset_normal += 3;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 3; i ++)
                        {
                            normalArray[offset_normal] = faceNormal.X;
                            normalArray[offset_normal + 1] = faceNormal.Y;
                            normalArray[offset_normal + 2] = faceNormal.Z;

                            offset_normal += 3;
                        }
                    }
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglNormalBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normalArray.Length * sizeof(float)), normalArray, hint); // * 3 ??????

                Debug.WriteLine("BufferData for __webglNormalBuffer float {0}", geometryGroup.__webglNormalBuffer);
            }

            if (dirtyUvs && obj_uvs.Count > 0 && uvType)
            {
                foreach (var fi in chunk_faces3)
                {
                    var uv = obj_uvs[fi];

                    if (uv == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i ++)
                    {
                        var uvi = uv[i];

                        uvArray[offset_uv] = uvi.X;
                        uvArray[offset_uv + 1] = uvi.Y;

                        offset_uv += 2;
                    }
                }

                if (offset_uv > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUVBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvArray.Length * sizeof(float)), uvArray, hint);  // * 2 ??????

                    Debug.WriteLine("BufferData for __webglUVBuffer float {0}", geometryGroup.__webglUVBuffer);
                }
            }

            if (dirtyUvs && null != obj_uvs2 && obj_uvs2.Count > 0 && uvType)
            {
                foreach (var fi in chunk_faces3)
                {
                    var uv2 = obj_uvs2[fi];

                    if (uv2 == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i ++)
                    {
                        var uv2i = uv2[i];

                        uv2Array[offset_uv2] = uv2i.X;
                        uv2Array[offset_uv2 + 1] = uv2i.Y;

                        offset_uv2 += 2;
                    }
                }

                if (offset_uv2 > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, geometryGroup.__webglUV2Buffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uv2Array.Length * sizeof(float)), uv2Array, hint); // * 3 ??????

                    Debug.WriteLine("BufferData for __webglUV2Buffer float {0}", geometryGroup.__webglUV2Buffer);
                }
            }

            if (dirtyElements)
            {
                foreach (var fi in chunk_faces3)
                {
                    faceArray[offset_face + 0] = (ushort)vertexIndex;
                    faceArray[offset_face + 1] = (ushort)(vertexIndex + 1);
                    faceArray[offset_face + 2] = (ushort)(vertexIndex + 2);

                    offset_face += 3;

                    lineArray[offset_line + 0] = (ushort)vertexIndex;
                    lineArray[offset_line + 1] = (ushort)(vertexIndex + 1);

                    lineArray[offset_line + 2] = (ushort)vertexIndex;
                    lineArray[offset_line + 3] = (ushort)(vertexIndex + 2);
                                                  
                    lineArray[offset_line + 4] = (ushort)(vertexIndex + 1);
                    lineArray[offset_line + 5] = (ushort)(vertexIndex + 2);

                    offset_line += 6;

                    vertexIndex += 3;
                }

                Debug.Assert(geometryGroup.__webglFaceBuffer > 0, "");
                Debug.Assert(geometryGroup.__webglLineBuffer > 0, "");

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglFaceBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(faceArray.Length * sizeof(ushort)), faceArray, hint);

                Debug.WriteLine("BufferData for __webglFaceBuffer ushort {0}", geometryGroup.__webglFaceBuffer);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometryGroup.__webglLineBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(lineArray.Length * sizeof(ushort)), lineArray, hint);

                Debug.WriteLine("BufferData for __webglLineBuffer float {0}", geometryGroup.__webglLineBuffer);
            }

            //if (customAttributes)
            //{
            //}

            if (dispose)
            {
                //delete geometryGroup.__inittedArrays;
                //delete geometryGroup.__colorArray;
                //delete geometryGroup.__normalArray;
                //delete geometryGroup.__tangentArray;
                //delete geometryGroup.__uvArray;
                //delete geometryGroup.__uv2Array;
                //delete geometryGroup.__faceArray;
                //delete geometryGroup.__vertexArray;
                //delete geometryGroup.__lineArray;
                //delete geometryGroup.__skinIndexArray;
                //delete geometryGroup.__skinWeightArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygonoffset"></param>
        /// <param name="factor"></param>
        /// <param name="units"></param>
        private void SetPolygonOffset(bool polygonoffset, float factor, float units)
        {
            if (this._oldPolygonOffset != polygonoffset)
            {
                if (polygonoffset)
                {
                    GL.Enable(EnableCap.PolygonOffsetFill);
                }
                else
                {
                    GL.Disable(EnableCap.PolygonOffsetFill);
                }

                this._oldPolygonOffset = polygonoffset;
            }

            if (polygonoffset && (this._oldPolygonOffsetFactor != factor || this._oldPolygonOffsetUnits != units))
            {
                GL.PolygonOffset(factor, units);

                this._oldPolygonOffsetFactor = factor;
                this._oldPolygonOffsetUnits = units;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="intensitySq"></param>
	    private void setColorGamma(List<float> array, int offset, Color color, float intensitySq ) 
        {
            array.Resize(offset + 1 + 2);

            array[offset]     = (color.R / 255.0f * color.R / 255.0f * intensitySq);
            array[offset + 1] = (color.G / 255.0f * color.G / 255.0f * intensitySq);
            array[offset + 2] = (color.B / 255.0f * color.B / 255.0f * intensitySq);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        private void setColorLinear(List<float> array, int offset, Color color, float intensity)
        {
            array.Resize(offset + 1 + 2);

            array[offset] = (color.R / 255.0f * intensity);
            array[offset + 1] = (color.G / 255.0f * intensity);
            array[offset + 2] = (color.B / 255.0f * intensity);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Color copyGammaToLinear (Color color)
        {
            var value = Color.FromArgb(
		        (int)((color.R / 255.0f * color.R / 255.0f) * 255.0f),
		        (int)((color.G / 255.0f * color.G / 255.0f) * 255.0f),
                (int)((color.B / 255.0f * color.B / 255.0f) * 255.0f) );

		    return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="fog"></param>
        private void RefreshUniformsFog ( Uniforms uniforms, Fog fog ) 
        {

		    uniforms["fogColor"].Value = fog.color;

		    if ( fog is Fog ) {

                uniforms["fogNear"].Value = fog.near;
                uniforms["fogFar"].Value = fog.far;

		    } else if ( fog is FogExp2 ) {

             //   uniforms["fogDensity"].Value = fog.density;

		    }

	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsPhong ( Uniforms uniforms, MeshPhongMaterial material ) 
        {
		    uniforms["shininess"].Value = material.shininess;

		    if ( this.gammaInput ) {
                uniforms["ambient"].Value  = this.copyGammaToLinear(material.ambient);
                uniforms["emissive"].Value = this.copyGammaToLinear(material.emissive);
                uniforms["specular"].Value = this.copyGammaToLinear(material.specular);
		    } else {
			    uniforms["ambient"].Value = material.ambient;
			    uniforms["emissive"].Value = material.emissive;
			    uniforms["specular"].Value = material.specular;
		    }

		    if ( material.WrapAround ) {
			    uniforms["wrapRGB"].Value = material.wrapRGB;
		    }
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lights"></param>
        private void SetupLights ( IEnumerable<Light> lights )
        { 
            var zlights = this._lights;

            var ambiColors = zlights.ambient.colors;

            var dirColors = zlights.directional.colors;
            var dirPositions = zlights.directional.positions;

            var pointColors = zlights.point.colors;
            var pointPositions = zlights.point.positions;
            var pointDistances = zlights.point.distances;

            var spotColors = zlights.spot.colors;
            var spotPositions = zlights.spot.positions;
            var spotDistances = zlights.spot.distances;
            var spotDirections = zlights.spot.directions;
            var spotAnglesCos = zlights.spot.anglesCos;
            var spotExponents = zlights.spot.exponents;

            var hemiSkyColors = zlights.hemi.skyColors;
            var hemiGroundColors = zlights.hemi.groundColors;
            var hemiPositions = zlights.hemi.positions;

            var dirLength = 0;
		    var pointLength = 0;
		    var spotLength = 0;
		    var hemiLength = 0;

            var dirCount = 0;
		    var pointCount = 0;
		    var spotCount = 0;
		    var hemiCount = 0;

            foreach (var light in lights) 
            {
	//		    if ( light.onlyShadow ) continue;

			    var color = light.color;

			    if ( light is AmbientLight )
                {                    
                    if (!light.Visible) continue;

                    ambiColors.Resize(3);

                    float r = 0; float g = 0; float b = 0;

				    if ( this.gammaInput ) {
					    r += (color.R / 255.0f * color.R / 255.0f);
					    g += (color.G / 255.0f * color.G / 255.0f);
					    b += (color.B / 255.0f * color.B / 255.0f);
				    } else {
                        r += (color.R / 255.0f);
                        g += (color.G / 255.0f);
					    b += (color.B / 255.0f);
				    }

                    ambiColors[0] = r;
                    ambiColors[1] = g;
                    ambiColors[2] = b;

			    } 
                else if ( light is DirectionalLight )
                {
                    var directionalLight = light as DirectionalLight;

				    dirCount += 1;

				    if ( ! light.Visible ) continue;

                    this._direction = new Vector3().SetFromMatrixPosition(directionalLight.MatrixWorld);
                    var vector3 = new Vector3().SetFromMatrixPosition(directionalLight.target.MatrixWorld);
				    this._direction-= vector3;
				    this._direction.Normalize();

				    var dirOffset = dirLength * 3;

                    dirPositions.Resize(dirOffset + 1 + 2);

				    dirPositions[ dirOffset ]     = this._direction.X;
				    dirPositions[ dirOffset + 1 ] = this._direction.Y;
				    dirPositions[ dirOffset + 2 ] = this._direction.Z;

				    if ( this.gammaInput ) {
                        this.setColorGamma(dirColors, dirOffset, color, directionalLight.intensity * directionalLight.intensity);
				    } else {
                        this.setColorLinear(dirColors, dirOffset, color, directionalLight.intensity);
				    }

				    dirLength += 1;

			    }
                else if ( light is PointLight )
                {

                    var pointLight = light as PointLight;
              
                    pointCount += 1;

                    if (!light.Visible) continue;

				    var pointOffset = pointLength * 3;

				    if ( this.gammaInput ) {

                        this.setColorGamma(pointColors, pointOffset, color, pointLight.intensity * pointLight.intensity);

				    } else {

                        this.setColorLinear(pointColors, pointOffset, color, pointLight.intensity);

				    }

				    var vector3 = new Vector3().SetFromMatrixPosition( light.MatrixWorld );

                    pointPositions.Resize(pointOffset + 1 + 2);
                    
                    pointPositions[pointOffset] = vector3.X;
				    pointPositions[ pointOffset + 1 ] = vector3.Y;
				    pointPositions[ pointOffset + 2 ] = vector3.Z;

                    pointDistances[pointLength] = pointLight.distance;

				    pointLength += 1;

			    } 
                else if ( light is SpotLight ) 
                {
                    var spotLight = light as SpotLight;
                    
				    spotCount += 1;

                    if (!light.Visible) continue;

				    var spotOffset = spotLength * 3;

				    if ( this.gammaInput ) {

                        this.setColorGamma(spotColors, spotOffset, color, spotLight.intensity * spotLight.intensity);

				    } else {

                        this.setColorLinear(spotColors, spotOffset, color, spotLight.intensity);

				    }

				    var vector3 = new Vector3().SetFromMatrixPosition( light.MatrixWorld );

                    spotPositions.Resize(spotOffset + 1 + 2);
                    spotPositions[spotOffset] = vector3.X;
                    spotPositions[spotOffset + 1] = vector3.Y;
                    spotPositions[spotOffset + 2] = vector3.Z;

                    spotDistances.Resize(spotLength + 1);
                    spotDistances[spotLength] = spotLight.distance;

				    this._direction = vector3;

                    vector3 = new Vector3().SetFromMatrixPosition(spotLight.target.MatrixWorld);
				    this._direction-=vector3;
				    this._direction.Normalize();

                    spotDirections.Resize(spotOffset + 1 + 2);
                    spotDirections[spotOffset] = this._direction.X;
				    spotDirections[ spotOffset + 1 ] = this._direction.Y;
				    spotDirections[ spotOffset + 2 ] = this._direction.Z;

                    spotAnglesCos.Resize(spotLength + 1); 
                    spotAnglesCos[spotLength] = (float)Math.Cos(spotLight.angle);

                    spotExponents.Resize(spotLength + 1);
                    spotExponents[spotLength] = spotLight.exponent;

				    spotLength += 1;

			    } 
                else if ( light is HemisphereLight )
                {
                    var hemisphereLight = light as HemisphereLight;
                    
				    hemiCount += 1;

                    if (!light.Visible) continue;

                    this._direction = new Vector3().SetFromMatrixPosition(light.MatrixWorld).Normalize();

				    var hemiOffset = hemiLength * 3;

                    hemiPositions.Resize(hemiOffset + 1 + 2);
                    hemiPositions[hemiOffset] = this._direction.X;
				    hemiPositions[ hemiOffset + 1 ] = this._direction.Y;
				    hemiPositions[ hemiOffset + 2 ] = this._direction.Z;

				    var skyColor = light.color;
                    var groundColor = hemisphereLight.groundColor;

				    if ( this.gammaInput ) {

                        var intensitySq = hemisphereLight.intensity * hemisphereLight.intensity;

					    this.setColorGamma( hemiSkyColors, hemiOffset, skyColor, intensitySq );
					    this.setColorGamma( hemiGroundColors, hemiOffset, groundColor, intensitySq );

				    } else {

                        this.setColorLinear(hemiSkyColors, hemiOffset, skyColor, hemisphereLight.intensity);
                        this.setColorLinear(hemiGroundColors, hemiOffset, groundColor, hemisphereLight.intensity);

				    }

				    hemiLength += 1;

			    }

		    }

		    // null eventual remains from removed lights
		    // (this is to avoid if in shader)

		    for (var l = dirLength * 3;   l < Math.Max( dirColors.Count, dirCount * 3 );       l++) dirColors[ l ] = 0.0f;
            for (var l = pointLength * 3; l < Math.Max(pointColors.Count, pointCount * 3);     l++) pointColors[l] = 0.0f;
            for (var l = spotLength * 3;  l < Math.Max(spotColors.Count, spotCount * 3);       l++) spotColors[l] = 0.0f;
            for (var l = hemiLength * 3;  l < Math.Max(hemiSkyColors.Count, hemiCount * 3);    l++) hemiSkyColors[l] = 0.0f;
            for (var l = hemiLength * 3;  l < Math.Max(hemiGroundColors.Count, hemiCount * 3); l++) hemiGroundColors[l] = 0.0f;

            zlights.directional.length = dirLength;
            zlights.point.length = pointLength;
            zlights.spot.length = spotLength;
            zlights.hemi.length = hemiLength;
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsLine (Uniforms uniforms, Material material)
        {
            var lineBasicMaterial = material as LineBasicMaterial;
            if (null != lineBasicMaterial)
                uniforms["diffuse"].Value = lineBasicMaterial.Color;
            else
            {
                Debug.Assert(1 == 0, "Other material than LineBasicMaterial???");
            }
/*
            var meshBasicMaterial = material as MeshBasicMaterial;
            if (null != meshBasicMaterial)
                uniforms["diffuse"].Value = meshBasicMaterial.color;

            var meshLambertMaterial = material as MeshLambertMaterial;
            if (null != meshLambertMaterial)
                uniforms["diffuse"].Value = meshLambertMaterial.color;

            var meshPhongMaterial = material as MeshPhongMaterial;
            if (null != meshPhongMaterial)
                uniforms["diffuse"].Value = meshPhongMaterial.color;

            var pointCloudMaterial = material as PointCloudMaterial;
            if (null != pointCloudMaterial)
                uniforms["diffuse"].Value = pointCloudMaterial.color;
     */
            uniforms["opacity"].Value = material.opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsDash(Uniforms uniforms, Material material)
        {
            throw new NotImplementedException();
            //uniforms["dashSize"].Value = material.dashSize;
            //uniforms["totalSize"].Value = material.dashSize + material.gapSize;
            //uniforms["scale"].Value = material.scale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsParticle (Uniforms uniforms, Material material)
        {
            if (material is PointCloudMaterial)
            {
                var m = material as PointCloudMaterial;

                uniforms["psColor"].Value = m.color;
                uniforms["opacity"].Value = m.opacity;
                uniforms["size"].Value = m.size;
                uniforms["scale"].Value = this._currentHeight / 2.0f; // TODO: Cache this.

                uniforms["map"].Value = m.map;

                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="material"></param>
        private void RefreshUniformsLambert(Uniforms uniforms, MeshLambertMaterial material)
        {
            if (this.gammaInput)
            {
                uniforms["ambient"].Value = this.copyGammaToLinear(material.ambient);
                uniforms["emissive"].Value = this.copyGammaToLinear(material.emissive);
            }
            else
            {
                uniforms["ambient"].Value = material.ambient;
                uniforms["emissive"].Value = material.emissive;
            }

            if (material.WrapAround)
            {
                uniforms["wrapRGB"].Value = material.wrapRGB;
            }
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="lights"></param>
        private void refreshUniformsShadow ( Uniforms uniforms, IEnumerable<Light> lights )
        {
            if ( uniforms["shadowMatrix"] != null) 
            {
                foreach ( var light in lights )
                {
                    if (light is ILightShadow)
                    {
                        var lightShadow = light as ILightShadow;

                        if (!light.CastShadow) continue;
                //        if (lightShadow.shadowCascade) continue;

                        if (light is SpotLight || (light is DirectionalLight/* && !lightShadow.shadowCascade*/))
                        {
                            ((List<Texture>)uniforms["shadowMap"].Value).Add(lightShadow.shadowMap);
                            ((List<Size>)uniforms["shadowMapSize"].Value).Add(lightShadow.shadowMapSize);

                            ((List<Matrix4>)uniforms["shadowMatrix"].Value).Add(lightShadow.shadowMatrix);

                            ((List<float>)uniforms["shadowDarkness"].Value).Add(lightShadow.shadowDarkness);
                            ((List<float>)uniforms["shadowBias"].Value).Add(lightShadow.shadowBias);
                        }
                    }
                }
            }
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniforms"></param>
        /// <param name="lights"></param>
        private void RefreshUniformsLights(Uniforms uniforms, LightCollection lights)
        {
            uniforms["ambientLightColor"].Value = this._lights.ambient.colors;

            uniforms["directionalLightColor"].Value = this._lights.directional.colors;
            uniforms["directionalLightDirection"].Value = this._lights.directional.positions;

            uniforms["pointLightColor"].Value = this._lights.point.colors;
            uniforms["pointLightPosition"].Value = this._lights.point.positions;
            uniforms["pointLightDistance"].Value = this._lights.point.distances;

            uniforms["spotLightColor"].Value = this._lights.spot.colors;
            uniforms["spotLightPosition"].Value = this._lights.spot.positions;
            uniforms["spotLightDistance"].Value = this._lights.spot.distances;
            uniforms["spotLightDirection"].Value = this._lights.spot.directions;
            uniforms["spotLightAngleCos"].Value = this._lights.spot.anglesCos;
            uniforms["spotLightExponent"].Value = this._lights.spot.exponents;

            uniforms["hemisphereLightSkyColor"].Value = this._lights.hemi.skyColors;
            uniforms["hemisphereLightGroundColor"].Value = this._lights.hemi.groundColors;
            uniforms["hemisphereLightDirection"].Value = this._lights.hemi.positions;
        }

        /// <summary>
        /// If uniforms are marked as clean, they don't need to be loaded to the GPU.
        /// </summary>
        private void markUniformsLightsNeedsUpdate(Uniforms uniforms, bool value)
        {
            uniforms["ambientLightColor"].needsUpdate = value;

            uniforms["directionalLightColor"].needsUpdate = value;
            uniforms["directionalLightDirection"].needsUpdate = value;

            uniforms["pointLightColor"].needsUpdate = value;
            uniforms["pointLightPosition"].needsUpdate = value;
            uniforms["pointLightDistance"].needsUpdate = value;

            uniforms["spotLightColor"].needsUpdate = value;
            uniforms["spotLightPosition"].needsUpdate = value;
            uniforms["spotLightDistance"].needsUpdate = value;
            uniforms["spotLightDirection"].needsUpdate = value;
            uniforms["spotLightAngleCos"].needsUpdate = value;
            uniforms["spotLightExponent"].needsUpdate = value;

            uniforms["hemisphereLightSkyColor"].needsUpdate = value;
            uniforms["hemisphereLightGroundColor"].needsUpdate = value;
            uniforms["hemisphereLightDirection"].needsUpdate = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lights"></param>
        /// <param name="fog"></param>
        /// <param name="material"></param>
        /// <param name="object3D"></param>
        /// <returns></returns>
        private WebGlProgram SetProgram(Camera camera, IEnumerable<Light> lights, Fog fog, Material material, Object3D object3D)
        {
            this._usedTextureUnits = 0;

            if (material.needsUpdate)
            {
                if (material.program != null)
                {
                    this.deallocateMaterial(material);
                }

                this.InitMaterial(material, lights, fog, object3D);
                material.needsUpdate = false;
            }

            var meshBasicMaterial = material as MeshBasicMaterial;
            if (null != meshBasicMaterial && meshBasicMaterial.morphTargets)
            {
                if (null == object3D.__webglMorphTargetInfluences)
                {
                    object3D.__webglMorphTargetInfluences = new float[this.maxMorphTargets];
                }
            }

            var meshNormalMaterial = material as MeshNormalMaterial;
            if (null != meshNormalMaterial && meshNormalMaterial.MorphTargets)
            {
                if (null == object3D.__webglMorphTargetInfluences)
                {
                    object3D.__webglMorphTargetInfluences = new float[this.maxMorphTargets];
                }
            }

            var refreshProgram = false;
            var refreshMaterial = false;
            var refreshLights = false;

            var program = material.program;
            var uniformsLocation = program.UniformsLocation;
            var m_uniforms = material.__webglShader.Uniforms;

            object uniformLocation = null;

            if (program.Id != this._currentProgram)
            {
                GL.UseProgram(program.Program);
                this._currentProgram = program.Id;

                refreshProgram = true;
                refreshMaterial = true;
                refreshLights = true;
            }

            if (material.id != this._currentMaterialId)
            {
                if (this._currentMaterialId == -1)
                {
                    refreshLights = true;
                }

                this._currentMaterialId = material.id;

                refreshMaterial = true;
            }

            if (refreshProgram || camera != this._currentCamera)
            {
                //Console.WriteLine("camera.ProjectionMatrix \n{0}", camera.ProjectionMatrix);

                uniformLocation = uniformsLocation["projectionMatrix"];
                if (null != uniformLocation)
                    GL.UniformMatrix4((int)uniformLocation, 1, false, camera.ProjectionMatrix.Elements);

                //if ( _logarithmicDepthBuffer ) {

                //   GL.Uniform1( p_uniforms["logDepthBufFC"], 2.0 / ( Math.Log( camera.far + 1.0 ) / Math.LN2 ) );

                //}

                if (camera != this._currentCamera)
                {
                    this._currentCamera = camera;
                }

                // load material specific uniformsLocation
                // (shader material also gets them for the sake of genericity)

                if ( material is ShaderMaterial
                ||   material is MeshPhongMaterial
              /*  ||   material.envMap*/) 
                {
                    uniformLocation = uniformsLocation["cameraPosition"];
                    if (uniformLocation != null)
                    {
                        throw new NotImplementedException();
                  //      _vector3.setFromMatrixPosition( camera.MatrixWorld );
                  //      GL.Uniform3((int)uniformLocation, _vector3.x, _vector3.y, _vector3.z);
                    }
                }


                if (material is MeshPhongMaterial 
                || material is MeshLambertMaterial
                || //               material.skinning ||
                    material is ShaderMaterial)
                {
                    uniformLocation = uniformsLocation["viewMatrix"];
                    if (uniformLocation  != null)
                    {
                        GL.UniformMatrix4((int)uniformLocation, 1, false, camera.MatrixWorldInverse.Elements);
                    }
                }
            }

            // skinning uniformsLocation must be set even if material didn't change
            // auto-setting of texture unit for bone texture must go before other textures
            // not sure why, but otherwise weird things happen

            //if ( material.skinning ) {

            //    if ( object3D.bindMatrix && p_uniforms["bindMatrix"] !== null ) {

            //        GL.uniformMatrix4fv( p_uniforms["bindMatrix"], false, object3D.bindMatrix.elements );

            //    }

            //    if ( object3D.bindMatrixInverse && p_uniforms.bindMatrixInverse !== null ) {

            //        GL.uniformMatrix4fv( p_uniforms["bindMatrixInverse"], false, object3D.bindMatrixInverse.elements );

            //    }

            //    if ( supportsBoneTextures && object3D.skeleton && object3D.skeleton.useVertexTexture ) {

            //        if ( p_uniforms.boneTexture !== null ) {

            //            var textureUnit = getTextureUnit();

            //            GL.uniform1i( p_uniforms["boneTexture"], textureUnit );
            //            _this.setTexture( object3D.skeleton.boneTexture, textureUnit );

            //        }

            //        if ( p_uniforms.boneTextureWidth !== null ) {

            //            GL.uniform1i( p_uniforms["boneTextureWidth"], object3D.skeleton.boneTextureWidth );

            //        }

            //        if ( p_uniforms.boneTextureHeight !== null ) {

            //            GL.uniform1i( p_uniforms["boneTextureHeight"], object3D.skeleton.boneTextureHeight );

            //        }

            //    } else if ( object3D.skeleton && object3D.skeleton.boneMatrices ) {

            //        if ( p_uniforms.boneGlobalMatrices !== null ) {

            //            GL.uniformMatrix4fv( p_uniforms["boneGlobalMatrices"], false, object3D.skeleton.boneMatrices );

            //        }

            //    }

            //}

            if (refreshMaterial)
            {
                // refresh uniformsLocation common to several materials

                if (null != fog && material is MeshPhongMaterial && ((MeshPhongMaterial)material).fog)
                {
                    RefreshUniformsFog(m_uniforms, fog);
                }

                if (material is MeshPhongMaterial ||
                     material is MeshLambertMaterial 
           /*          material.__lights*/)
                {
                        if ( this._lightsNeedUpdate ) {

                            refreshLights = true;
                            this.SetupLights( lights );
                            this._lightsNeedUpdate = false;
                        }

                        if ( refreshLights ) {
                            this.RefreshUniformsLights( m_uniforms, this._lights );
                            this.markUniformsLightsNeedsUpdate( m_uniforms, true );
                        } else {
                            this.markUniformsLightsNeedsUpdate( m_uniforms, false );
                        }
                }

                if (material is MeshBasicMaterial || material is MeshLambertMaterial || material is MeshPhongMaterial)
                {
                    this.RefreshUniformsCommon(m_uniforms, material);
                }

                // refresh single material specific uniformsLocation

                if (material is LineBasicMaterial)
                {
                    RefreshUniformsLine( m_uniforms, material );
                }
                else if (material is LineDashedMaterial)
                {
                    RefreshUniformsLine( m_uniforms, material );
                    RefreshUniformsDash( m_uniforms, material );
                }
                else if (material is PointCloudMaterial)
                {
                    this.RefreshUniformsParticle( m_uniforms, material );
                }
                else if (material is MeshPhongMaterial)
                {
                    this.RefreshUniformsPhong(m_uniforms, material as MeshPhongMaterial);
                }
                else if (material is MeshLambertMaterial)
                {
                    this.RefreshUniformsLambert(m_uniforms, (MeshLambertMaterial)material);
                }
                else if (material is MeshDepthMaterial)
                {
                    m_uniforms["mNear"].Value = camera.Near;
                    m_uniforms["mFar"].Value = camera.Far;
                    m_uniforms["opacity"].Value = material.opacity;
                }
                else if (material is MeshNormalMaterial)
                {
                    m_uniforms["opacity"].Value = material.opacity;
                }

                if ( object3D.ReceiveShadow/* && ! material._shadowPass*/ ) {
                    refreshUniformsShadow( m_uniforms, lights );
                }

                // load common uniformsLocation

                this.LoadUniformsGeneric(material.uniformsList);
            }

            LoadUniformsMatrices(uniformsLocation, object3D);

            uniformLocation = uniformsLocation["modelMatrix"];
            if (uniformLocation != null)
            {
                //Console.WriteLine("object3D.MatrixWorld\n{0}", object3D.MatrixWorld);

                GL.UniformMatrix4((int)uniformLocation, 1, false, object3D.MatrixWorld.Elements);
            }

            return program;
        }

        /// <summary>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="slot"></param>
        private void SetTexture(Texture texture, int slot)
        {
            if (texture.needsUpdate)
            {
                if (! texture.__webglInit)
                {
                    texture.__webglInit = true;

                    //    texture.addEventListener( 'dispose', onTextureDispose );

                    texture.__webglTexture = GL.GenTexture();

                    this.info.memory.Textures++;
                }

                GL.ActiveTexture(TextureUnit.Texture0 + slot);
                GL.BindTexture(TextureTarget.Texture2D, texture.__webglTexture);

                //  GL.PixelStore( PixelStoreParameter.     GL.UNPACK_FLIP_Y_WEBGL, texture.flipY );
                //  GL.PixelStore( PixelStoreParameter.   GL.UNPACK_PREMULTIPLY_ALPHA_WEBGL, texture.premultiplyAlpha );
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, texture.unpackAlignment);

                var bitmap = texture.image;

                var isImagePowerOfTwo = IsPowerOfTwo(bitmap.Width) && IsPowerOfTwo(bitmap.Height);

                this.SetTextureParameters(TextureTarget.Texture2D, texture, isImagePowerOfTwo);

                var mipmaps = texture.mipmaps;

                if (texture is DataTexture)
                {
                    // use manually created mipmaps if available
                    // if there are no manual mipmaps
                    // set 0 level mipmap and then use GL to generate other mipmap levels

                    if (mipmaps.Count > 0 && isImagePowerOfTwo)
                    {
                        for (var i = 0; i < mipmaps.Count; i++)
                        {
                            //var mipmap = mipmaps[i];
                            //GL.TexImage2D(TextureTarget.Texture2D, i, texture.internalFormat, mipmap.width, mipmap.height, 0, texture.format, texture.type, mipmap.data);
                        }
                        texture.generateMipmaps = false;
                    }
                    else
                    {
                        var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, texture.internalFormat, bitmap.Width, bitmap.Height, 0, texture.format, texture.type, data.Scan0);
                        bitmap.UnlockBits(data);
                    }
                }
                else if (texture is CompressedTexture)
                {
                    //for (var i = 0; i < mipmaps.Count; i++)
                    //{
                    //    var mipmap = mipmaps[i];
                    //    if (texture.format != PixelFormat.Rgba)
                    //    {
                    //        GL.CompressedTexImage2D(TextureTarget.Texture2D, i, texture.format, mipmap.width, mipmap.height, 0, mipmap.data);
                    //    } else {
                    //        GL.TexImage2D(TextureTarget.Texture2D, i, texture.internalFormat, mipmap.width, mipmap.height, 0, texture.format, texture.type, mipmap.data);
                    //    }

                    //}
                }
                else
                {
                    // regular Texture (image, video, canvas)

                    // use manually created mipmaps if available
                    // if there are no manual mipmaps
                    // set 0 level mipmap and then use GL to generate other mipmap levels

                    if (null != mipmaps && mipmaps.Count > 0 && isImagePowerOfTwo)
                    {
                        for (var i = 0; i < mipmaps.Count; i ++)
                        {
                            var mipmap = mipmaps[i];
                            // GL.TexImage2D(TextureTarget.Texture2D, i, texture.internalFormat, image.Width, image.Height, 0, texture.format, texture.type, mipmap);
                        }
                        texture.generateMipmaps = false;
                    }
                    else
                    {
                        var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, texture.internalFormat, bitmap.Width, bitmap.Height, 0, texture.format, texture.type, data.Scan0);
                        bitmap.UnlockBits(data);
                    }
                }

                if (texture.generateMipmaps && isImagePowerOfTwo)
                {
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }

                texture.needsUpdate = false;

                //if ( texture.onUpdate ) 
                //    texture.onUpdate();
            }
            else
            {
                GL.ActiveTexture(TextureUnit.Texture0 + slot);
                GL.BindTexture(TextureTarget.Texture2D, texture.__webglTexture);
            }
        }

        /// <summary>
        /// </summary>
        private void SetTextureParameters(TextureTarget textureTarget, Texture texture, bool isImagePowerOfTwo)
        {
            if (isImagePowerOfTwo)
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }
            else
            {
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }

            if ( this.glExtensionTextureFilterAnisotropic && (texture.type != PixelType.Float) ) 
            {
                if ( texture.anisotropy > 1 || texture.__oldAnisotropy > 1 )
                {
                    GL.TexParameter(textureTarget, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, Math.Min(texture.anisotropy, this.MaxAnisotropy));
                    texture.__oldAnisotropy = texture.anisotropy;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="object3D"></param>
        /// <param name="camera"></param>
        private static void SetupMatrices(Object3D object3D, Camera camera)
        {
            object3D.ModelViewMatrix = camera.MatrixWorldInverse * object3D.MatrixWorld;
            object3D.NormalMatrix.GetNormalMatrix(object3D.ModelViewMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="geometryGroup"></param>
        /// <param name="object3D"></param>
        private void SetupMorphTargets(Material material, GeometryGroup geometryGroup, Object3D object3D)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <param name="programAttributes"></param>
        /// <param name="geometryAttributes"></param>
        /// <param name="aa"></param>
        private void SetupVertexAttributes(Material material, int programAttributes, int geometryAttributes, int aa)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object3D"></param>
		private void UpdateSkeletons( Object3D object3D )
        {
            if (object3D is SkinnedMesh)
            {
                //object3D.skeleton.update();
			}

            foreach (var child in object3D.Children)
            {
                this.UpdateSkeletons(child);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webglObject"></param>
        private void UnrollBufferMaterial(WebGlObject webglObject)
        {
            var object3D = webglObject.object3D;
      //      GeometryGroup buffer = webglObject.buffer;

            var geometry = object3D.Geometry;
            var material = object3D.Material;

            if (material is MeshFaceMaterial)
            {
                throw new NotImplementedException();

                var buffer = webglObject.buffer as GeometryGroup;
  
                var materialIndex = geometry is BufferGeometry ? 0 : buffer.MaterialIndex;

                //    material = material.materials[ materialIndex ];

                if (material.transparent)
                {
                    webglObject.material = material;
                    this.transparentObjects.Add(webglObject);
                }
                else
                {
                    webglObject.material = material;
                    this.opaqueObjects.Add(webglObject);
                }
            }
            else
            {
                if (null != material)
                {
                    if (material.transparent)
                    {
                        webglObject.material = material;
                        this.transparentObjects.Add(webglObject);
                    }
                    else
                    {
                        webglObject.material = material;
                        this.opaqueObjects.Add(webglObject);
                    }
                }
            }
        }

        // Buffer setting

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        /// <param name="object3D"></param>
        private void setParticleBuffers(Geometry geometry, BufferUsageHint hint, PointCloud object3D)
        {
		    var vertices = geometry.Vertices;
		    var vl = vertices.Count;
            
            var colors = geometry.Colors;
		    var cl = colors.Count;
            
            var vertexArray = geometry.__vertexArray;
		    var colorArray = geometry.__colorArray;
            
            var sortArray = geometry.__sortArray;
            
            var dirtyVertices = geometry.VerticesNeedUpdate;
		    var dirtyElements = geometry.ElementsNeedUpdate;
		    var dirtyColors = geometry.ColorsNeedUpdate;

		    var customAttributes = geometry.__webglCustomAttributesList;

            var offset = 0;
            
            if ( object3D.sortParticles ) {

                throw new NotImplementedException();

                _projScreenMatrixPS.Copy( _projScreenMatrix );
                _projScreenMatrixPS.Multiply( object3D.MatrixWorld );

                for (int v = 0; v < vl; v ++ ) 
                {

                    var vertex = vertices[ v ];

                    var _vector3 = new Vector3();
                    _vector3.Copy(vertex);
                    _vector3.ApplyProjection(_projScreenMatrixPS);

     //               sortArray[ v ] = [ _vector3.Z, v ];
                }

   //             sortArray.sort(numericalSort);

                for (int v = 0; v < vl; v ++ )
                {

    //                var vertex = vertices[sortArray[v][1]];

                    offset = v * 3;

                    //vertexArray[ offset ]     = vertex.X;
                    //vertexArray[ offset + 1 ] = vertex.Y;
                    //vertexArray[ offset + 2 ] = vertex.Z;

                }

                for (int c = 0; c < cl; c ++ ) {

                    offset = c * 3;

    //                var color = colors[sortArray[c][1]];

                    //colorArray[ offset ]     = color.R;
                    //colorArray[ offset + 1 ] = color.G;
                    //colorArray[ offset + 2 ] = color.B;

                }

                if (null != customAttributes ) {
/*
                    for (int i = 0, il = customAttributes.Count; i < il; i ++ ) {

                        var customAttribute = customAttributes[ i ];

                        if ( ! ( customAttribute.boundTo == null || customAttribute.boundTo == "vertices" ) ) continue;

                        offset = 0;

                        var cal = customAttribute.value.length;

                        if ( customAttribute.size == 1 ) {

                            for (int ca = 0; ca < cal; ca ++ ) {

                                var index = sortArray[ ca ][ 1 ];

                                customAttribute.array[ ca ] = customAttribute.value[ index ];

                            }

                        } else if ( customAttribute.size == 2 ) {

                            for (int ca = 0; ca < cal; ca ++ ) {

                                var index = sortArray[ ca ][ 1 ];

                                var value = customAttribute.value[ index ];

                                customAttribute.array[ offset ]   = value.X;
                                customAttribute.array[ offset + 1 ] = value.Y;

                                offset += 2;

                            }

                        } else if ( customAttribute.size == 3 ) {

                            if ( customAttribute.type == "c" ) {

                                for (int ca = 0; ca < cal; ca ++ ) {

                                    var index = sortArray[ ca ][ 1 ];

                                    var value = customAttribute.value[ index ];

                                    customAttribute.array[ offset ]     = value.R;
                                    customAttribute.array[ offset + 1 ] = value.G;
                                    customAttribute.array[ offset + 2 ] = value.B;

                                    offset += 3;

                                }

                            } else {

                                for (int ca = 0; ca < cal; ca ++ ) {

                                    var index = sortArray[ ca ][ 1 ];

                                    var value = customAttribute.value[ index ];

                                    customAttribute.array[ offset ]   = value.X;
                                    customAttribute.array[ offset + 1 ] = value.Y;
                                    customAttribute.array[ offset + 2 ] = value.Z;

                                    offset += 3;

                                }

                            }

                        } else if ( customAttribute.size == 4 ) {

                            for (int ca = 0; ca < cal; ca ++ ) {

                                var index = sortArray[ ca ][ 1 ];

                                var value = customAttribute.value[ index ];

                                customAttribute.array[ offset ]      = value.X;
                                customAttribute.array[ offset + 1  ] = value.Y;
                                customAttribute.array[ offset + 2  ] = value.Z;
                                customAttribute.array[ offset + 3  ] = value.w;

                                offset += 4;

                            }

                        }

                    }
*/
                }

            } else {

                if ( dirtyVertices )
                {
                    for (int v = 0; v < vl; v ++ ) {

                        var vertex = vertices[ v ];

                        offset = v * 3;

                        vertexArray[ offset ]     = vertex.X;
                        vertexArray[ offset + 1 ] = vertex.Y;
                        vertexArray[ offset + 2 ] = vertex.Z;

                    }

                }

                if ( dirtyColors )
                {
                    for (int c = 0; c < cl; c ++ ) {

                        var color = colors[ c ];

                        offset = c * 3;

                        colorArray[ offset ]     = color.R;
                        colorArray[ offset + 1 ] = color.G;
                        colorArray[ offset + 2 ] = color.B;

                    }

                }

                if (null != customAttributes ) {
/*
                    for (int i = 0, il = customAttributes.Count; i < il; i ++ ) {

                        var customAttribute = customAttributes[ i ];

                        if ( customAttribute.needsUpdate &&
                                ( customAttribute.boundTo == undefined ||
                                    customAttribute.boundTo == "vertices" ) ) {

                            var cal = customAttribute.value.length;

                            offset = 0;

                            if ( customAttribute.size == 1 ) {

                                for (int ca = 0; ca < cal; ca ++ ) {

                                    customAttribute.array[ ca ] = customAttribute.value[ ca ];

                                }

                            } else if ( customAttribute.size == 2 ) {

                                for (int ca = 0; ca < cal; ca ++ ) {

                                    value = customAttribute.value[ ca ];

                                    customAttribute.array[ offset ]   = value.X;
                                    customAttribute.array[ offset + 1 ] = value.Y;

                                    offset += 2;

                                }

                            } else if ( customAttribute.size == 3 ) {

                                if ( customAttribute.type == "c" ) {

                                    for (int ca = 0; ca < cal; ca ++ ) {

                                        value = customAttribute.value[ ca ];

                                        customAttribute.array[ offset ]   = value.R;
                                        customAttribute.array[ offset + 1 ] = value.G;
                                        customAttribute.array[ offset + 2 ] = value.B;

                                        offset += 3;

                                    }

                                } else {

                                    for (int ca = 0; ca < cal; ca ++ ) {

                                        value = customAttribute.value[ ca ];

                                        customAttribute.array[ offset ]   = value.X;
                                        customAttribute.array[ offset + 1 ] = value.Y;
                                        customAttribute.array[ offset + 2 ] = value.Z;

                                        offset += 3;

                                    }

                                }

                            } else if ( customAttribute.size == 4 ) {

                                for (int ca = 0; ca < cal; ca ++ ) {

                                    var value = customAttribute.value[ ca ];

                                    customAttribute.array[ offset ]      = value.X;
                                    customAttribute.array[ offset + 1  ] = value.Y;
                                    customAttribute.array[ offset + 2  ] = value.Z;
                                    customAttribute.array[ offset + 3  ] = value.w;

                                    offset += 4;

                                }

                            }

                        }
                    }
                    */

                }

            }

            if ( dirtyVertices || object3D.sortParticles ) 
            {
                GL.BindBuffer( BufferTarget.ArrayBuffer, geometry.__webglVertexBuffer );
                GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(float)), vertexArray, hint );
            }

            if ( dirtyColors || object3D.sortParticles )
            {
                GL.BindBuffer( BufferTarget.ArrayBuffer, geometry.__webglColorBuffer );
                GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length * sizeof(float)), colorArray, hint );
            }

            if (null != customAttributes ) {
/*
                for (int i = 0, il = customAttributes.Count; i < il; i ++ ) {

                    var customAttribute = customAttributes[ i ];

                    if ( customAttribute.needsUpdate || object3D.sortParticles ) {

                        GL.BindBuffer( BufferTarget.ArrayBuffer, customAttribute.buffer );
                        GL.BufferData( BufferTarget.ArrayBuffer, customAttribute.array, hint );

                    }

                }
*/
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="hint"></param>
        private void setLineBuffers (Geometry geometry, BufferUsageHint hint )
        {
            var vertices = geometry.Vertices;
            var colors = geometry.Colors;
            var lineDistances = geometry.LineDistances;

            var vl = vertices.Count;
            var cl = colors.Count;
            var dl = lineDistances.Count;

            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;
            var lineDistanceArray = geometry.__lineDistanceArray;

            var dirtyVertices = geometry.VerticesNeedUpdate;
            var dirtyColors = geometry.ColorsNeedUpdate;
            var dirtyLineDistances = geometry.LineDistancesNeedUpdate;

            var customAttributes = geometry.__webglCustomAttributesList; // maybe unitialzed

            var offset = 0;

		    if ( dirtyVertices ) {

			    for (int v = 0; v < vl; v ++ ) {

				    var vertex = vertices[ v ];

				    offset = v * 3;

				    vertexArray[ offset ]     = vertex.X;
				    vertexArray[ offset + 1 ] = vertex.Y;
				    vertexArray[ offset + 2 ] = vertex.Z;

			    }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglVertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * 1), vertexArray, hint);

		    }

		    if ( dirtyColors ) {

			    for (int c = 0; c < cl; c ++ ) {

				    var color = colors[ c ];

				    offset = c * 3;

				    colorArray[ offset ]     = color.R;
				    colorArray[ offset + 1 ] = color.G;
				    colorArray[ offset + 2 ] = color.B;

			    }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglColorBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorArray.Length * 1), colorArray, hint);

		    }

		    if ( dirtyLineDistances ) {

			    for (int d = 0; d < dl; d ++ ) {

				    lineDistanceArray[ d ] = lineDistances[ d ];

			    }

                GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.__webglLineDistanceBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(lineDistanceArray.Length * 1), lineDistanceArray, hint);

		    }

		    if (null != customAttributes ) {

			    for (int i = 0, il = customAttributes.Count; i < il; i ++ )
                {
				    var customAttribute = customAttributes[ i ];
/*
				    if ( customAttribute.needsUpdate &&
					     ( customAttribute.boundTo == null ||
						     customAttribute.boundTo == "vertices" ) ) {

					    offset = 0;

					    var cal = customAttribute.value.length;

					    if ( customAttribute.size == 1 ) {

						    for (int ca = 0; ca < cal; ca ++ ) {

							    customAttribute.array[ ca ] = customAttribute.value[ ca ];

						    }

					    } else if ( customAttribute.size == 2 ) {

						    for (int ca = 0; ca < cal; ca ++ ) {

							    var value = customAttribute.value[ ca ];

							    customAttribute.array[ offset ]   = value.X;
							    customAttribute.array[ offset + 1 ] = value.Y;

							    offset += 2;

						    }

					    } else if ( customAttribute.size == 3 ) {

						    if ( customAttribute.type == "c" ) {

							    for (int ca = 0; ca < cal; ca ++ ) {

								    var value = customAttribute.value[ ca ];

								    customAttribute.array[ offset ]   = value.R;
								    customAttribute.array[ offset + 1 ] = value.G;
								    customAttribute.array[ offset + 2 ] = value.B;

								    offset += 3;

							    }

						    } else {

							    for (int ca = 0; ca < cal; ca ++ ) {

								    var value = customAttribute.value[ ca ];

								    customAttribute.array[ offset ]   = value.X;
								    customAttribute.array[ offset + 1 ] = value.Y;
								    customAttribute.array[ offset + 2 ] = value.Z;

								    offset += 3;

							    }

						    }

					    } else if ( customAttribute.size == 4 ) {

						    for (int ca = 0; ca < cal; ca ++ ) {

							    var value = customAttribute.value[ ca ];

							    customAttribute.array[ offset ]    = value.X;
							    customAttribute.array[ offset + 1  ] = value.Y;
							    customAttribute.array[ offset + 2  ] = value.Z;
							    customAttribute.array[ offset + 3  ] = value.w;

							    offset += 4;

						    }

					    }

                        GL.BindBuffer(BufferTarget.ArrayBuffer, customAttribute.buffer);
                        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(customAttribute.array), customAttribute.array, hint);

				    }
                    */
			    }

		    }

	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="object3D"></param>
        private void updateObject(Scene scene, Object3D object3D)
        {
            Material material = null;

            if (object3D.Geometry is BufferGeometry)
            {
                var geometry = object3D.Geometry as BufferGeometry;
                SetDirectBuffers(geometry as BufferGeometry, BufferUsageHint.DynamicDraw);
            }
            else if (object3D is Mesh)
            {
                var geometry = object3D.Geometry as Geometry;

                // check all geometry groups
                if (geometry.BuffersNeedUpdate || geometry.GroupsNeedUpdate)
                {
                    if (geometry is BufferGeometry)
                    {
                     //   InitDirectBuffers(geometry as BufferGeometry);
                    }
                    else if (object3D is Mesh)
                    {
                        this.initGeometryGroups(scene, object3D, geometry);
                    }
                }

                foreach (var geometryGroup in geometry.GeometryGroupsList)
                {
                    material = this.getBufferMaterial(object3D, geometryGroup);

                    if (geometry.BuffersNeedUpdate || geometry.GroupsNeedUpdate)
                    {
                        this.InitMeshBuffers(geometryGroup, object3D);
                    }                 

                    var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).attributes) && AreCustomAttributesDirty( material as IAttributes));

                    if (geometry.VerticesNeedUpdate || geometry.MorphTargetsNeedUpdate || geometry.ElementsNeedUpdate
                    || geometry.UvsNeedUpdate || geometry.NormalsNeedUpdate || geometry.ColorsNeedUpdate
                    || geometry.TangentsNeedUpdate || customAttributesDirty)
                    {
                        this.SetMeshBuffers(geometryGroup, object3D, BufferUsageHint.DynamicDraw, !geometry.Dynamic, material);
                    }
                }

                geometry.VerticesNeedUpdate = false;
                geometry.MorphTargetsNeedUpdate = false;
                geometry.ElementsNeedUpdate = false;
                geometry.UvsNeedUpdate = false;
                geometry.NormalsNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;
                geometry.TangentsNeedUpdate = false;

                geometry.BuffersNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
            else if (object3D is Line)
            {
                var geometry = object3D.Geometry as Geometry;
                
                material = this.getBufferMaterial(object3D, geometry);

                var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).attributes) && AreCustomAttributesDirty(material as IAttributes));

                if (geometry.VerticesNeedUpdate || geometry.ColorsNeedUpdate || geometry.LineDistancesNeedUpdate || customAttributesDirty)
                {
                    setLineBuffers(geometry, BufferUsageHint.DynamicDraw);
                }

                geometry.VerticesNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;
                geometry.LineDistancesNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
            else if (object3D is PointCloud)
            {
                var geometry = object3D.Geometry as Geometry;
                var pointCloud = object3D as PointCloud;

                material = this.getBufferMaterial(object3D, geometry);

                var customAttributesDirty = (material is IAttributes && (null != ((IAttributes)material).attributes) && AreCustomAttributesDirty(material as IAttributes));

                if ( geometry.VerticesNeedUpdate || geometry.ColorsNeedUpdate || pointCloud.sortParticles || customAttributesDirty )
                {
                    setParticleBuffers(geometry, BufferUsageHint.DynamicDraw, pointCloud);
                }

                geometry.VerticesNeedUpdate = false;
                geometry.ColorsNeedUpdate = false;

                if (material is IAttributes && (null != ((IAttributes)material).attributes))
                    ClearCustomAttributes(material as IAttributes);
            }
        }

        #endregion

        private struct LightCountInfo
        {
            #region Fields

            public int directional;

            public int hemi;

            public int point;

            public int spot;

            #endregion
        }

        #region IDisposable Members
        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                try
                {
                    this._disposed = true;

                    // TODO
                }
                finally
                {
                    //base.Dispose(true);           // call any base classes
                }
            }
        }
        #endregion
    }
}