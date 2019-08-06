#if OLD_INPUT
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using mulova.commons;

namespace comunity
{
    [CustomEditor(typeof(InputEventSystem))]
    public class InputEventSystemInspector : Editor
    {
        private InputEventSystem sys;
        private InputAxisMapInspector inspector;
        
        void OnEnable() {
            sys = (InputEventSystem)target;
            inspector = new InputAxisMapInspector(sys, ref sys.inputStateData);
            inspector.indent = 1;
            inspector.Exclude(InputState.Null);
        }
        
        private InputState srcState;
        private InputState dstState;
        private bool showCopy;
        public override void OnInspectorGUI() {
            inspector.OnInspectorGUI();
            DrawCopyState();
        }
        
        private void DrawCopyState() {
            EditorGUILayout.Separator();
            if (EditorUI.DrawHeader("Copy State")) {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUIUtil.Popup("From", ref srcState, EnumUtil.Values<InputState>());
                EditorGUIUtil.Popup("To", ref dstState, EnumUtil.Values<InputState>());
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Add", GUILayout.Height(30))) {
                    InputAxisMapData srcData = sys.inputStateData[(int)srcState];
                    InputAxisMapData dstData = sys.inputStateData[(int)dstState];
                    dstData.Add(srcData);
                    EditorUtil.SetDirty(target);
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.EndContents();
            }
        }
    }

    class InputAxisMapInspector : EnumArrayInspector<InputState, InputAxisMapData> {
        private InputState[] STATES;
        private ArrayInspector[] inspector;
        private InputEventSystem sys;
        
        public InputAxisMapInspector(InputEventSystem sys, ref InputAxisMapData[] array) : base(ref array, sys) {
            this.sys = sys;
            this.STATES = EnumUtil.Values<InputState>();
            this.inspector = new ArrayInspector[STATES.Length];
            for (int i=0; i<inspector.Length; i++) {
                this.inspector[i] = new ArrayInspector(sys, null, new InputAxisMapDataInspectorRow(array[i]));
            }
        }
        
        protected override bool OnHeaderGUI(InputState evt, int i, InputAxisMapData entry) {
            bool change = base.OnHeaderGUI(evt, i, entry);
            if (Application.isPlaying) {
                if (GUILayout.Button("Set State")) {
                    sys.SetState(evt);
                }
            }
            return change;
        }
        
        
        protected override bool OnInspectorGUI(InputState evt, InputAxisMapData data)
        {
            bool changed = false;
            changed |= inspector[(int)evt].OnInspectorGUI();
            if (changed) {
                EditorUtil.SetDirty(target);
            }
            return changed;
            
        }
        
        protected override bool IsExpanded(InputAxisMapData data)
        {
            return data.axis != null && data.axis.Length > 0;
        }
        
    }
}
#endif