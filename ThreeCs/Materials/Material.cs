namespace ThreeCs.Materials
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using ThreeCs.Renderers.WebGL;
    using ThreeCs.Textures;

    public class Material : ICloneable
    {
        private static int MaterialIdCount;

        public int id = MaterialIdCount++;

        public Guid Uuid = Guid.NewGuid();

        public WebGlShader __webglShader;

        public Hashtable Defines = new Hashtable();

        public WebGlProgram program;

        public string name;

        public string type = "Material";

        public int Side = Three.FrontSide;

        public float Opacity = 1;
        
        public bool Transparent = false;

        public int Blending = Three.NormalBlending;
        
        public int BlendSrc = Three.SrcAlphaFactor;
        
        public int BlendDst = Three.OneMinusSrcAlphaFactor;
        
        public int BlendEquation = Three.AddEquation;
        
        public bool DepthTest = true;
        
        public bool DepthWrite = true;
        
        public bool PolygonOffset = false;
        
        public float PolygonOffsetFactor = 0.0f;
        
        public float PolygonOffsetUnits = 0.0f;
        
        public float AlphaTest = 0.0f;

        public int Overdraw = 0; // Overdrawn pixels (typically between 0 and 1) for fixing antialiasing gaps in CanvasRenderer

        public bool Visible = true;
        
        public bool NeedsUpdate = true;

        public List<UniformLocation> UniformsList;

        public int VertexColors; // set to use "color" attribute stream

        public Texture EnvMap = null; // Hoort hier eigenlijk niet




        /// <summary>
        /// 
        /// </summary>
        public Material()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Material(Material other)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <param name="values"></param>
        protected void SetValues(Hashtable values)
        {
            if (values == null)
                return;

            // This works only in C# through reflection.

            foreach (DictionaryEntry item in values)
            {
                var newValue = item.Value;
                var key = item.Key as string;
                Debug.Assert(null != key);

                var type = this.GetType();
                var propertyInfo = type.GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
                if (null != propertyInfo)
                {
                    propertyInfo.SetValue(this, newValue);
                }
                else
                {
                    var fieldInfo = type.GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (null != fieldInfo)
                    {
                        fieldInfo.SetValue(this, newValue);
                    }
                    else
                    {
                        Trace.TraceWarning("attribute {0} not found", key);
                    }
                }


                //if (newValue == null)
                //{
                //    //Trace.TraceInformation("THREE.Material: '" + key + "' parameter is undefined.");
                //    continue;
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Material(this);
        }

    }
}
