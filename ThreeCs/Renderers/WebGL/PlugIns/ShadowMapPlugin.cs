namespace ThreeCs.Renderers.WebGL.PlugIns
{
    using System.Collections.Generic;

    using ThreeCs.Cameras;
    using ThreeCs.Core;
    using ThreeCs.Lights;
    using ThreeCs.Scenes;

    class ShadowMapPlugin
    {
        private OpenTKRenderer _renderer;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="lights"></param>
        /// <param name="webglObjects"></param>
        /// <param name="webglObjectsImmediate"></param>
        public ShadowMapPlugin(OpenTKRenderer renderer, LightCollection lights, Dictionary<int, List<WebGlObject>> webglObjects, List<WebGlObject> webglObjectsImmediate)
        {
            this._renderer = renderer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        public void Render(Scene scene, Camera camera)
        {
		    if ( this._renderer.shadowMapEnabled == false ) 
                return;

            this._renderer.ResetGlState();
        }
    }
}
