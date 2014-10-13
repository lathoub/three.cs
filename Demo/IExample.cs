namespace Demo
{
    using System.Drawing;
    using System.Windows.Forms;

    interface IExample
    {
        void Load(Control control);
        void Resize(Size clientSize);
        void MouseMove(Size clientSize, Point here);
        void Render();
        void Unload();
    }
}
