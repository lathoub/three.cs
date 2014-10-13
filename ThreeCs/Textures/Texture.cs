namespace ThreeCs.Textures
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;

    using ThreeCs.Math;

    using OpenTK.Graphics.OpenGL;

    
    public class Texture : ICloneable
    {
        #region Static Fields

        protected static int TextureIdCount;

        #endregion

        #region Fields

        public bool __webglInit = false;

        public int __webglTexture;

        public int anisotropy = 1;

        public int __oldAnisotropy = -1;

        public bool flipY = true;

        public bool generateMipmaps = true;

        public int id = TextureIdCount++;

        public string name;

        public string sourceFile;

        public bool needsUpdate = false;

        public Bitmap image;

        public Hashtable mipmaps;

        //this.mapping = mapping !== undefined ? mapping : THREE.Texture.DEFAULT_MAPPING;

        //this.wrapS = wrapS !== undefined ? wrapS : THREE.ClampToEdgeWrapping;

        //this.wrapT = wrapT !== undefined ? wrapT : THREE.ClampToEdgeWrapping;

        //this.magFilter = magFilter !== undefined ? magFilter : THREE.LinearFilter;

        //this.minFilter = minFilter !== undefined ? minFilter : THREE.LinearMipMapLinearFilter;

        public PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;//format !== undefined ? format : THREE.RGBAFormat;

        public PixelFormat format = PixelFormat.Bgra;//format !== undefined ? format : THREE.RGBAFormat;

        public PixelType type = PixelType.UnsignedByte;
        
        public Vector2 offset = new Vector2(0, 0);

        public bool premultiplyAlpha = false;

        public Vector2 repeat = new Vector2(1, 1);

        // valid values: 1, 2, 4, 8 (see http://www.khronos.org/opengles/sdk/docs/man/xhtml/glPixelStorei.xml)
        public int unpackAlignment = 4;

        public Guid uuid = Guid.NewGuid();

        #endregion

        //this.onUpdate = null;

        #region Constructors and Destructors

        protected Texture()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Texture(Bitmap image, object mapping = null , bool wrapS = false, bool wrapT = false/* magFilter, minFilter, format, type, int anisotropy */)
        {
            this.image = image;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        protected Texture(Texture other)
        {
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("id: {0}, filename: {1}, size = {2}", this.id, Path.GetFileNameWithoutExtension(this.sourceFile), this.image.Size);
        }

        #region Public Events


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Texture(this);
        }

        #endregion

        #region Methods

        #endregion
    }
}