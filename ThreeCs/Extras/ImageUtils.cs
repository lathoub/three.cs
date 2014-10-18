namespace ThreeCs.Extras
{
    using System.Drawing;

    using ThreeCs.Textures;

    public class ImageUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static Texture LoadTexture(string url, object mapping = null)
        {
            var image = (Bitmap)Image.FromFile(url, true);

            image.RotateFlip(RotateFlipType.Rotate180FlipX);

            return new Texture(image) { NeedsUpdate = true, SourceFile = url };
        }
    }
}

