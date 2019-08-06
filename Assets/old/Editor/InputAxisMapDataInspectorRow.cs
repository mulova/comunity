#if OLD_INPUT
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using mulova.commons;

namespace comunity
{
    public class InputAxisMapDataInspectorRow : ArrayInspectorRow
    {
        public const int COL_AXIS = 0;
        public const int COL_EVENT = 1;
        public const int COL_TRIGGER = 2;
        public EnumWrapperRefArray<InputAxis> axis;
        public EnumWrapperRefArray<InputAxisState> trigger;
        public EnumWrapperRefArray<InputEvent> evt;
        
        public InputAxisMapDataInspectorRow(InputAxisMapData obj)
        {
            axis = new EnumWrapperRefArray<InputAxis>(obj, "axis");
            trigger = new EnumWrapperRefArray<InputAxisState>(obj, "trigger");
            evt = new EnumWrapperRefArray<InputEvent>(obj, "evt");
            
            //      axis.layout = GUILayout.MinWidth(100);
            //      trigger.layout = GUILayout.MinWidth(100);
            //      evt.layout = GUILayout.MinWidth(100);
        }
        
        public int MinLength {
            get { return axis.MinLength; }
            set {
                axis.MinLength = value;
                evt.MinLength = value;
                trigger.MinLength = value;
            }
        }
        
        public int Length {
            get {
                return axis.Length;
            }
            set {
                axis.Length = value;
                evt.Length = value;
                trigger.Length = value;
            }
        }
        
        public object Get(int col, int row)
        {
            if (col == COL_AXIS) {
                return axis[row].Enum;
            } else if (col == COL_EVENT) {
                return evt[row].Enum;
            } else if (col == COL_TRIGGER) {
                return trigger[row].Enum;
            }
            return null;
        }
        
        public void AddRow(params object[] row) {
            Length = Length+1;
            
            EnumWrapper a = new EnumWrapper(typeof(InputAxis));
            a.Enum = (InputAxis)row[COL_AXIS];
            axis[Length-1] = a;
            EnumWrapper e = new EnumWrapper(typeof(InputEvent));
            e.Enum = (InputEvent)row[COL_EVENT];
            evt[Length-1] = e;
            EnumWrapper t = new EnumWrapper(typeof(InputAxisState));
            t.Enum = (InputAxisState)row[COL_TRIGGER];
            trigger[Length-1] = t;
        }
        
        public object[] GetRow(int row)
        {
            return new object[] { axis[row].Enum, evt[row].Enum, trigger[row].Enum };
        }
        
        public object[] GetDefault()
        {
            return new object[] { InputAxis.Fire1, InputEvent.Null, InputAxisState.Null };
        }
        
        public void Set(List<object[]> rows)
        {
            Length = 0;
            foreach (object[] r in rows) {
                EnumWrapper a = new EnumWrapper(typeof(InputAxis));
                a.Enum = (InputAxis)r[COL_AXIS];
                axis.Add(a);
                EnumWrapper e = new EnumWrapper(typeof(InputEvent));
                e.Enum = (InputEvent)r[COL_EVENT];
                evt.Add(e);
                EnumWrapper t = new EnumWrapper(typeof(InputAxisState));
                t.Enum = (InputAxisState)r[COL_TRIGGER];
                trigger.Add(t);
            }
        }
        
        public void SetPreset(params object[] preset) {
            if (preset[COL_AXIS] != null) {
                axis.preset = (InputAxis[])preset[COL_AXIS];
            }
            if (preset[COL_EVENT] != null) {
                evt.preset = (InputEvent[])preset[COL_EVENT];
            }
            if (preset[COL_TRIGGER] != null) {
                trigger.preset = (InputAxisState[])preset[COL_TRIGGER];
            }
        }
        
        public bool OnInspectorGUI(int i)
        {
            bool changed = axis.DrawValue(i, axis[i]);
            changed |= trigger.DrawValue(i, trigger[i]);
            changed |= evt.DrawValue(i, evt[i]);
            return changed;
        }
    }
}


#endif