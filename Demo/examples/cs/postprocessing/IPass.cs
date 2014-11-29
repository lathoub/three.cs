namespace THREE
{
    using ThreeCs.Renderers;

    public interface IPass
    {
        bool Enabled { get; set; }
        bool Clear { get; set; }
        bool NeedsSwap { get; set; }

        void Render(WebGLRenderer renderer, WebGLRenderTarget writeBuffer, WebGLRenderTarget readBuffer, float delta);
    }
}
