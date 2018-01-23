
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace core
{
    [CustomEditor(typeof(TimerControl))]
    public class TimerControlInspector : Editor {
        
        private TimerControl timer;
        private TimerDataArrInspector inspector;
        void OnEnable() {
            timer = target as TimerControl;
            inspector = new TimerDataArrInspector(timer, "timers");
        }
        
        public override void OnInspectorGUI() {
            int timerSize = timer.timers.Length;
            inspector.OnInspectorGUI();
            if (timerSize != timer.timers.Length) {
                timer.ResizeTimer();
            }
        }
    }

    public class TimerDataArrInspector : ArrInspector<TimerData>
    {
        private TimerControl timer;
        public TimerDataArrInspector(TimerControl timer, string varName) : base(timer, varName) {
            this.timer = timer;
            SetTitle(null);
        }
        
        protected override bool OnInspectorGUI(TimerData d, int i)
        {
            //      float width = GetWidth();
            bool changed = false;
            
            EditorGUILayout.BeginVertical();
            changed |= EditorGUIUtil.ObjectField<MethodCall>("Method", ref d.method, true);
            changed |= EditorGUIUtil.FloatField("Duration", ref d.duration);
            changed |= EditorGUIUtil.Toggle("Repeat", ref d.repeat);
            changed |= EditorGUIUtil.Toggle("Enabled", ref d.enabled);
            
            if (Application.isPlaying) {
                float time = timer[i].RemainingTime;
                if (time > 0) {
                    EditorGUILayout.LabelField(time.ToString("n2")+" sec", EditorStyles.boldLabel);
                } else {
                    EditorGUILayout.LabelField("EXPIRED", EditorStyles.boldLabel);
                }
            }
            if (changed) {
                timer[i].data = d;
            }
            EditorGUILayout.EndVertical();
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

