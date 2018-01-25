using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;

namespace core
{
    [CustomEditor(typeof(LangSwitch))]
    public class LangSwitchInspector : Editor
    {
        private LangSwitch langSwitch;
        private LangObjArrInspector inspector;
        
        void OnEnable() {
            langSwitch = target as LangSwitch;
            inspector = new LangObjArrInspector(langSwitch, "obj");
        }
        
        public override void OnInspectorGUI() {
            inspector.OnInspectorGUI();
        }
    }

    public class LangObjArrInspector : ArrInspector<LangObj>
    {
        public LangObjArrInspector(Object obj, string varName) : base(obj, varName) { 
            SetTitle(null);
        }
        
        protected override bool OnInspectorGUI(LangObj data, int i)
        {
            bool changed = false;
            changed |= EditorGUIUtil.PopupEnum<SystemLanguage>(null, ref data.lang);
            changed |= EditorGUIUtil.ObjectField<GameObject>(null, ref data.obj, true);
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}
