namespace Demo
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using ThreeCs.Renderers;

    abstract class Example : IExample
    {
        protected OpenTKRenderer renderer;

        protected readonly Random random = new Random();

        protected readonly ColorConverter colorConvertor = new ColorConverter();

        protected readonly Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public virtual void Load(Control control)
        {
            Debug.Assert(null != control);

            this.renderer = new OpenTKRenderer(control);

            stopWatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        public virtual void Resize(Size clientSize)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSize"></param>
        /// <param name="here"></param>
        public virtual void MouseMove(Size clientSize, Point here)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// 
        /// </summary>
        public virtual void Unload()
        {
            this.renderer.Dispose();
        }
    }
}
