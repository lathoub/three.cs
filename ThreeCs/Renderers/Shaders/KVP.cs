namespace ThreeCs.Renderers.Shaders
{
    using System.Diagnostics;

    [DebuggerDisplay("{Value}", Name = "{Key}")]
    public class KVP
    {
        public string Key;
        public object Value;

        public bool needsUpdate;

        public KVP(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
