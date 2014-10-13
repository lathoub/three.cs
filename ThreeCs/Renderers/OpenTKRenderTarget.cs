namespace ThreeCs.Renderers
{
    using System;
    using System.Collections;

    using ThreeCs.Math;

    using OpenTK.Graphics.OpenGL;

    public class OpenTKRenderTarget : ICloneable
    {
        public int Width;
        public int Height;

    //    options = options || {};

        //this.wrapS = options.wrapS !== undefined ? options.wrapS : THREE.ClampToEdgeWrapping;
        //this.wrapT = options.wrapT !== undefined ? options.wrapT : THREE.ClampToEdgeWrapping;

        public TextureMagFilter magFilter = TextureMagFilter.Linear;

        public TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear;

        //this.anisotropy = options.anisotropy !== undefined ? options.anisotropy : 1;

        public Vector2 offset = new Vector2( 0, 0 );
        public Vector2 repeat = new Vector2(1, 1);

        //this.format = options.format !== undefined ? options.format : THREE.RGBAFormat;
        //this.type = options.type !== undefined ? options.type : THREE.UnsignedByteType;

        public bool DepthBuffer = true;
        public bool StencilBuffer = true;

        public bool generateMipmaps = true;

        public int __webglFramebuffer = -1;

        public int __webglTexture = -1;

        //this.shareDepthFrom = null;


        // renderPluginsPost

        // renderPluginsPre

        /// <summary>
        /// 
        /// </summary>
        protected OpenTKRenderTarget()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public OpenTKRenderTarget(int width, int height, Hashtable options)
        {
            this.Width = width;
            this.Height = height;
  
            //depthBuffer = options.depthBuffer !== undefined ? options.depthBuffer : true;
            //stencilBuffer = options.stencilBuffer !== undefined ? options.stencilBuffer : true;
        }

                /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other"></param>
        protected OpenTKRenderTarget(OpenTKRenderer other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new OpenTKRenderTarget();
        }


    }
}
