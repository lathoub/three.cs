namespace ThreeCs.Loaders
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security.Policy;
    using System.Text.RegularExpressions;

    using ThreeCs.Core;
    using ThreeCs.Math;

    public class LoaderEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderEventArgs"/> class.
        /// </summary>
        /// <param name="geometry">
        /// The channel carrier.
        /// </param>
        public LoaderEventArgs(Geometry geometry)
        {
            this.Geometry = geometry;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the channel carrier.
        /// </summary>
        public Geometry Geometry { get; private set; }

        #endregion
    }

    public class VTKLoader
    {
        public event EventHandler<LoaderEventArgs> Loaded;

        protected virtual void RaiseLoaded(Geometry geometry)
        {
            var handler = this.Loaded;
            if (handler != null)
            {
                handler(this, new LoaderEventArgs(geometry));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Load(Url url)
        {

            var geometry = this.Parse("");

            RaiseLoaded(geometry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            var data = File.ReadAllText(filename);

            var geometry = this.Parse(data);

            RaiseLoaded(geometry);
        }

        /// <summary>
        /// 
        /// </summary>
        private Geometry Parse(string data)
        {
            var geometry = new Geometry();

		    // float float float

            {
                string pattern = @"([\+|\-]?[\d]+[\.]*[\d|\-|e]*)[ ]+([\+|\-]?[\d]+[\.]*[\d|\-|e]*)[ ]+([\+|\-]?[\d]+[\.]*[\d|\-|e]*)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["1.0 2.0 3.0", "1.0", "2.0", "3.0"]

                    var x = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    geometry.Vertices.Add(new Vector3(x, y, z));
                }
            }

            // 3 int int int

            {
                string pattern = @"3[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["3 1 2 3", "1", "2", "3"]
                    try
                    {
                        var a = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                        var b = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                        var c = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                        geometry.Faces.Add(new Face3(a, b, c));
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message);
                    }
                }
            }

            // 4 int int int int

            {
                string pattern = @"4[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)[ ]+([\d]+)";

                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(data);

                foreach (Match match in matches)
                {
                    // ["4 1 2 3 4", "1", "2", "3", "4"]

                    var a = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var b = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var c = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    var d = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    geometry.Faces.Add(new Face3(a, b, d));
                    geometry.Faces.Add(new Face3(b, c, d));
                }
            }

            geometry.ComputeFaceNormals();
            geometry.ComputeVertexNormals();
            geometry.ComputeBoundingSphere();

            return geometry;
        }

    }
}
