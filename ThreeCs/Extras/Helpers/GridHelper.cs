namespace ThreeCs.Extras.Helpers
{
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Core;
    using ThreeCs.Materials;
    using ThreeCs.Objects;

    public class GridHelper : Line
    {
        public Color Color1;

        public Color Color2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="step"></param>
        public GridHelper(float size, float step)
        {
            var geometry = new Geometry();
            var material = new LineBasicMaterial { vertexColors = new Color[Three.VertexColors] };

            this.Color1 = Color.Aquamarine;
            this.Color2 = Color.Fuchsia;

            for ( var i = - size; i <= size; i += step ) 
            {
                geometry.Vertices.Add(new Vector3( - size, 0, i ));
                geometry.Vertices.Add(new Vector3(size, 0, i));
                geometry.Vertices.Add(new Vector3(i, 0, - size)); 
                geometry.Vertices.Add( new Vector3( i, 0, size ));

                var color = (i == 0) ? this.Color1 : this.Color2;
                geometry.Colors.Add(color);
                geometry.Colors.Add(color);
                geometry.Colors.Add(color);
                geometry.Colors.Add(color);
            }

            this.Geometry = geometry;
            this.Material = material;
            this.Type = this.LinePieces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorCenterLine"></param>
        /// <param name="colorGrid"></param>
        public void SetColors(Color colorCenterLine, Color colorGrid)
        {
            this.Color1 = colorCenterLine ;
            this.Color1 = colorGrid;
            this.Geometry.ColorsNeedUpdate = true;
        }

    }
}
