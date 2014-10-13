namespace ThreeCs.Renderers.Renderables
{
    using ThreeCs.Math;

    public class RenderableVertex
    {
        public Vector3 position = new Vector3();
        public Vector3 positionWorld = new Vector3();
        public Vector4 positionScreen = new Vector4();

        public bool visible = true;
    }
}
