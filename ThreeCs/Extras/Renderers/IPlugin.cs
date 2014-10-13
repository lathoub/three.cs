namespace ThreeCs.Extras.Renderers
{
    using ThreeCs.Cameras;
    using ThreeCs.Scenes;

    public interface IPlugin
    {
        void init();

        void render(Scene scene, Camera camera, int viewportWidth, int viewportHeight );
    }
}
