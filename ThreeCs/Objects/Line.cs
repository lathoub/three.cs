namespace ThreeCs.Objects
{
    using System;
    using System.Drawing;

    using ThreeCs.Core;
    using ThreeCs.Materials;
    using ThreeCs.Math;

    public class Line : Object3D
    {
        public int LineStrip = 0;
        public int LinePieces = 1;

        public Vector3 Start = new Vector3();
        public Vector3 End = new Vector3();

        public int Mode;

        /// <summary>
        /// Constructor
        /// </summary>
        public Line(BaseGeometry geometry = null, Material material = null, int? type = null)
        {
            this.type = "Line";

            this.Geometry = geometry ?? new Geometry();
            this.Material = material ?? new LineBasicMaterial { Color = new Color().Random() };

            this.Mode = Three.LineStrip;
            if (null != type) this.Mode = type.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RayCast()
        {
            throw new NotImplementedException();
        }
    }
}
