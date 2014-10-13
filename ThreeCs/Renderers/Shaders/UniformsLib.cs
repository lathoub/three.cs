
namespace ThreeCs.Renderers.Shaders
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    using ThreeCs.Math;
    using ThreeCs.Textures;

    public class UniformsLib : Dictionary<string, Uniforms>
    {
        /// <summary>
        /// 
        /// </summary>
        public UniformsLib()
        {
            this.Add("common", this.MakeCommon());
            this.Add("bump", this.MakeBump());
            this.Add("normalmap", this.MakeNormalMap());
            this.Add("fog", this.MakeFog());
            this.Add("lights", this.MakeLights());
            this.Add("particle", this.MakeParticle());
            this.Add("shadowmap", this.MakeShadowMap());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeCommon()
        {
            return new Uniforms
                             {
                                 { "diffuse",               new KVP("c", Color.Chartreuse) },
                                 { "opacity",               new KVP("f", 1.0f) },

                                 { "map",                   new KVP("t", null) },
                                 { "offsetRepeat",          new KVP("v4", new Vector4(0, 0, 1, 1)) },

                                 { "lightmap",              new KVP("t", null) },
                                 { "specularMap",           new KVP("t", null) },
                                 { "alphaMap",              new KVP("t", null) },

                                 { "envMap",                new KVP("t", null) },
                                 { "flipEnvMap",            new KVP("f", -1) },
                                 { "useRefract",            new KVP("i", 0) },
                                 { "reflectivity",          new KVP("f", 1.0f) },
                                 { "refractionRatio",       new KVP("f", 0.98f) },
                                 { "combine",               new KVP("i", 0) },

                                 { "morphTargetInfluences", new KVP("f", 0.0f) }
                             };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeBump()
        {
            return new Uniforms
                            { 
                                { "bumpMap",      new KVP("t", null) },
                                { "bumpScale",    new KVP("f", 1.0f) }
                            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeNormalMap()
        {
            return new Uniforms
                                {
                                    { "normalMap",      new KVP("t", null) },
                                    { "normalScale",    new KVP("v2", new Vector2(1, 1)) }
                                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeFog()
        {
            return new Uniforms
                          {
                              { "fogDensity",   new KVP("f", 0.00025f) },
                              { "fogNear",      new KVP("f", 1) },
                              { "fogFar",       new KVP("f", 2000) },
                              { "fogColor",     new KVP("c", Color.White) }
                          };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeLights()
        {
            return new Uniforms
                             {
                                 { "ambientLightColor",         new KVP("fv", new Hashtable()) },

                                 { "directionalLightDirection", new KVP("fv", new Hashtable()) },
                                 { "directionalLightColor",     new KVP("fv", new Hashtable()) },

                                 { "hemisphereLightDirection",  new KVP("fv", new Hashtable()) },
                                 { "hemisphereLightSkyColor",   new KVP("fv", new Hashtable()) },
                                 { "hemisphereLightGroundColor", new KVP("fv", new Hashtable()) },

                                 { "pointLightColor",           new KVP("fv", new Hashtable()) },
                                 { "pointLightPosition",        new KVP("fv", new Hashtable()) },
                                 { "pointLightDistance",        new KVP("fv", new Hashtable()) },

                                 { "spotLightColor",            new KVP("fv", new Hashtable()) },
                                 { "spotLightPosition",         new KVP("fv", new Hashtable()) },
                                 { "spotLightDirection",        new KVP("fv", new Hashtable()) },
                                 { "spotLightDistance",         new KVP("fv1", new Hashtable()) },
                                 { "spotLightAngleCos",         new KVP("fv1", new Hashtable()) },
                                 { "spotLightExponent",         new KVP("fv1", new Hashtable()) }
                             };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeParticle()
        {
            return new Uniforms
                               {
                                   { "psColor", new KVP("c", Color.White) },
                                   { "opacity", new KVP("f", 1.0) },
                                   { "size", new KVP("f", 1.0) },
                                   { "scale", new KVP("f", 1.0) },
                                   { "map", new KVP("t", null) },

                                   { "fogDensity", new KVP("f", 0.00025) },
                                   { "fogNear", new KVP("f", 1) },
                                   { "fogFar", new KVP("f", 2000) },
                                   { "fogColor", new KVP("c", Color.White) }
                               };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uniforms MakeShadowMap()
        {
            return new Uniforms
                                {
                                    { "shadowMap", new KVP("tv", new List<Texture>()) },
                                    { "shadowMapSize", new KVP("v2v", new List<Size>()) },

                                    { "shadowBias", new KVP("fv1", new List<float>()) },
                                    { "shadowDarkness", new KVP("fv1", new List<float>()) },

                                    { "shadowMatrix", new KVP("m4v", new List<Matrix4>()) }
                                };
        }

    }
}
