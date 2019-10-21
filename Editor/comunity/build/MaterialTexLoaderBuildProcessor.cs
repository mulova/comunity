using mulova.comunity;
using mulova.preprocess;
using UnityEngine;

namespace mulova.build
{
    public class MaterialTexLoaderBuildProcessor : ComponentBuildProcess
    {
        protected override void Verify(Component comp) {}
        
        protected override void Preprocess(Component comp)
        {
            MaterialTexLoaderInspector.ClearMatTexture(comp as MaterialTexLoader);
        }
        
        protected override void Postprocess(Component c)
        {
        }
        
        public override System.Type compType
        {
            get
            {
                return typeof(MaterialTexLoader);
            }
        }
        
    }
}
