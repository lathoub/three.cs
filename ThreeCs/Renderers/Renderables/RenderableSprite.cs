namespace ThreeCs.Renderers.Renderables
{
    using ThreeCs.Core;
    using ThreeCs.Materials;
    using ThreeCs.Math;

    public class RenderableSprite
    {
        public int id = 0;

        public Object3D object3D = null;

        public int X = 0;
        public int Y = 0;
        public int Z = 0;

        public int rotation = 0;
        public Vector2 Scale = new Vector2();

        public Material material = null;
    }
}
