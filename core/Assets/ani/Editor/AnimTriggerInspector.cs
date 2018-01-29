using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEditorInternal;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using comunity;

namespace ani
{
    [CustomEditor(typeof(AnimTrigger))]
    public class AnimTriggerInspector : Editor
    {
        private AnimTrigger trigger;
        private StrStrArrInspector<AnimTriggerElement> arrInspector;
        private SerializedInspector varInspector;
        
        void OnEnable() {
            trigger = target as AnimTrigger;
            varInspector = new SerializedInspector(new SerializedObject(trigger), "animator");
            arrInspector = new StrStrArrInspector<AnimTriggerElement>(trigger, "map", "key", "val");
            
            string[] param = EditorAnimUtil.GetAnimatorParameters(trigger.animator, AnimatorControllerParameterType.Bool);
            arrInspector.SetValuePreset(param);
        }
        
        public override void OnInspectorGUI() {
            varInspector.OnInspectorGUI();
            if (varInspector["animator"].objectReferenceValue != null) {
                arrInspector.OnInspectorGUI();
            }
        }
    }
}

