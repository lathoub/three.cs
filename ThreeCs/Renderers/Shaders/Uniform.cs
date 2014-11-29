namespace ThreeCs.Renderers.Shaders
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Count = {this.Count}")]
    public class Uniform : Dictionary<string, object>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Uniform Copy(Uniform original)
        {
            var destination = new Uniform();

            foreach (var entry in original)
            {
                destination.Add(entry.Key, entry.Value);
            }

            return destination;
        }
    }
}