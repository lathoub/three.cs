namespace ThreeCs.Materials
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using ThreeCs.Renderers.WebGL;

    public class Material : ICloneable
    {
        private static int MaterialIdCount;

        public int id = MaterialIdCount++;

        public Guid uuid = Guid.NewGuid();

        public WebGlShader __webglShader;

        public Hashtable defines = new Hashtable();

        public WebGlProgram program;

        public string name;

        public int side = Three.FrontSide;

        public float opacity = 1;
        
        public bool transparent = false;

        public int blending = Three.NormalBlending;
        
        public int blendSrc = Three.SrcAlphaFactor;
        
        public int blendDst = Three.OneMinusSrcAlphaFactor;
        
        public int blendEquation = Three.AddEquation;
        
        public bool depthTest = true;
        
        public bool depthWrite = true;
        
        public bool polygonOffset = false;
        
        public float polygonOffsetFactor = 0.0f;
        
        public float polygonOffsetUnits = 0.0f;
        
        public float alphaTest = 0.0f;

        public int overdraw = 0; // Overdrawn pixels (typically between 0 and 1) for fixing antialiasing gaps in CanvasRenderer

        public bool visible = true;
        
        public bool needsUpdate = true;

        public List<UniformLocation> uniformsList; 

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
