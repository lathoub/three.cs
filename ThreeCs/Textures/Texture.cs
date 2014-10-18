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

        public int Id = TextureIdCount++;

        public Guid Uuid = Guid.NewGuid();

        public string Name;

        public string SourceFile;

        public bool NeedsUpdate = false;

        public Bitmap Image;

        public Hashtable Mipmaps;

        //this.mapping = mapping !== undefined ? mapping : Three.Texture.DEFAULT_MAPPING;

        public int WrapS = Three.ClampToEdgeWrapping;

        public int WrapT = Three.ClampToEdgeWrapping;

        public int MagFilter = Three.LinearFilter;

        public int MinFilter = Three.LinearMipMapLinearFilter;

        public PixelInternalFormat InternalFormat = PixelInternalFormat.Rgba;

        public PixelFormat Format = PixelFormat.Bgra;

        public PixelType Type = PixelType.UnsignedByte;
        
        public Vector2 Offset = new Vector2(0, 0);

        public bool PremultiplyAlpha = false;

        public Vector2 Repeat = new Vector2(1, 1);

        // valid values: 1, 2, 4, 8 (see http://www.khronos.org/opengles/sdk/docs/man/xhtml/glPixelStorei.xml)
        public int UnpackAlignment = 4;

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
            this.Image = image;
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
            return string.Format("id: {0}, filename: {1}, size = {2}", this.Id, Path.GetFileNameWithoutExtension(this.SourceFile), this.Image.Size);
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