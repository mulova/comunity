using UnityEngine;
using UnityEditor;

namespace core
{
    public class MaterialTexLoaderBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp) {}
        
        protected override void PreprocessComponent(Component comp)
        {
            ClearMatTexture(comp as MaterialTexLoader);
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
        
        public static void SetMatTexture(MaterialTexLoader l)
        {
            foreach (MaterialTexData d in l.materials)
            {
                d.material.SetTexture(MaterialTexLoader.TEX1, AssetDatabase.LoadAssetAtPath<Texture>(d.tex1.GetEditorPath()));
                d.material.SetTexture(MaterialTexLoader.TEX2, AssetDatabase.LoadAssetAtPath<Texture>(d.tex2.GetEditorPath()));
                CompatibilityEditor.SetDirty(d.material);
            }
        }
        
        public static void ClearMatTexture(MaterialTexLoader l)
        {
            foreach (MaterialTexData d in l.materials)
            {
                d.material.SetTexture(MaterialTexLoader.TEX1, null);
                d.material.SetTexture(MaterialTexLoader.TEX2, null);
                CompatibilityEditor.SetDirty(d.material);
            }
        }
    }
}
