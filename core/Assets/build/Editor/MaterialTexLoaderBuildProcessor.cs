using UnityEngine;
using UnityEditor;

namespace core
{
    public class MaterialTexLoaderBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp) {}
        
        protected override void PreprocessComponent(Component comp)
        {
            MaterialTexLoaderInspector.ClearMatTexture(comp as MaterialTexLoader);
        }
        
        protected override void PreprocessOver(Component c)
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
