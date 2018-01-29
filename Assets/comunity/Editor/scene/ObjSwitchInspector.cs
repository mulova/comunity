using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using drawer.ex;
using Object = UnityEngine.Object;
using commons;

namespace comunity
{
	[CustomEditor(typeof(ObjSwitch))]
	public class ObjSwitchInspector : Editor
	{
		private ObjSwitch objSwitch;
        private ObjSwitchElementInspector elementInspector;
		private ObjSwitchPresetInspector presetInspector;
		internal static bool exclusive = true;

		void OnEnable()
		{
			objSwitch = (ObjSwitch)target;
			elementInspector = new ObjSwitchElementInspector(objSwitch, "switches");
			elementInspector.InitEnumTypeSelector(typeof(Enum), "enumType");
			presetInspector = new ObjSwitchPresetInspector(objSwitch, "preset");
			presetInspector.Numbering = false;
			presetInspector.SetPresetKeys(GetAllKeys());
		}
	
		public override void OnInspectorGUI()
		{
			bool changed = false;
			Type type = null;
			if (elementInspector.DrawEnumTypeSelector(out type)) {
				changed = true;
			}
			EditorGUIUtil.Toggle("Exclusive(Editor Only)", ref exclusive);
			changed |= elementInspector.OnInspectorGUI();
			if (EditorUI.DrawHeader("Preset")) {
				EditorUI.BeginContents();
				changed |= presetInspector.OnInspectorGUI();
				if (changed) {
					presetInspector.SetPresetKeys(GetAllKeys());
                    CompatibilityEditor.SetDirty(objSwitch);
				}
				EditorUI.EndContents();
			}
		}

		private string[] GetAllKeys()
		{
			List<string> keys = new List<string>();
			foreach (ObjSwitchElement e in objSwitch.switches) {
				keys.Add(e.name);
			}
			return keys.ToArray();
		}
	}

	public class ObjSwitchElementInspector : ArrInspector<ObjSwitchElement>
	{
		private ObjSwitch objSwitch;
        private Dictionary<ObjSwitchElement, ArrayDrawer<GameObject>> objInspector;
        private Dictionary<ObjSwitchElement, ArrayDrawer<Transform>> transInspector;
		private HashSet<string> on;

		public ObjSwitchElementInspector(ObjSwitch obj, string varName) : base(obj, varName)
		{ 
			SetTitle(null);
			this.objSwitch = obj;
			objInspector = new Dictionary<ObjSwitchElement, ArrayDrawer<GameObject>>();
            transInspector = new Dictionary<ObjSwitchElement, ArrayDrawer<Transform>>();
			on = new HashSet<string>();
		}
	
		protected override bool OnInspectorGUI(ObjSwitchElement data, int i)
		{
			ArrayDrawer<GameObject> objArr = objInspector.Get(data);
			if (objArr == null) {
				objArr = new ArrayDrawer<GameObject>(objSwitch, data, "objs");
                objArr.title = null;
                objArr.allowSelection = false;
                objInspector[data] = objArr;
            }
            ArrayDrawer<Transform> transArr = transInspector.Get(data);
            if (transArr == null) {
                transArr = new ArrayDrawer<Transform>(objSwitch, data, "trans");
                transArr.title = null;
                transArr.allowSelection = false;
				transInspector[data] = transArr;
			}
			bool changed = false;
			EditorGUILayout.BeginVertical();
//			EditorGUILayout.BeginHorizontal();
			changed |= DrawEnum(ref data.name);
			bool toggle = on.Contains(data.name);
            if (EditorGUIUtil.Toggle(null, ref toggle)) {
                if (ObjSwitchInspector.exclusive) {
                    on.Clear();
                    on.Add(data.name);
                    toggle = true;
                } else {
                    if (toggle) {
                        on.Add(data.name);
                    } else {
                        on.Remove(data.name);
                    }
                }
                List<Object> objList = new List<Object>();
                foreach (var s in objSwitch.switches)
                {
                    objList.AddRange(s.objs);
                }
                Undo.RecordObjects(objList.ToArray(), "switch "+objSwitch.name);
				// set transform's position to stored position.
                objSwitch.Set(new List<string>(on).ToArray());
				changed = true;
			}
//			EditorGUILayout.EndHorizontal();
//            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Objs");
            changed |= objArr.Draw();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Transforms");
            changed |= transArr.Draw();
            EditorGUILayout.EndVertical();
//            EditorGUILayout.EndHorizontal();
			if (toggle)
			{
				// check if position is moved
				if (data.trans.Length != data.pos.Length)
				{
					data.pos = new Vector3[data.trans.Length];
				}
				for (int j=0; j<data.trans.Length; ++j)
				{
					if (data.trans[j] != null && data.trans[j].localPosition != data.pos[j])
					{
						data.pos[j] = data.trans[j].localPosition;
						changed = true;
					}
				}
			}
			EditorGUILayout.EndVertical();
			return changed;
		}

		private void AddAll(ObjSwitchElement e)
		{
			HashSet<GameObject> set = new HashSet<GameObject>();
			set.AddAll(e.objs);
//		Array.ForEach(e.objs, o=>set.Add(o));
			foreach (GameObject sel in Selection.gameObjects) {
				foreach (Transform t in sel.GetComponentsInChildren<Transform>()) {
					if (!set.Contains(t.gameObject)) {
						set.Add(t.gameObject);
						ArrayUtil.Add(ref e.objs, t.gameObject);
					}
				}
			}
		}
	
		protected override bool DrawFooter()
		{
			return false;
		}
	}

	public class ObjSwitchPresetInspector : ArrInspector<ObjSwitchPreset>
	{
		private ObjSwitch objSwitch;
		private List<StrArrInspector> inspectors;

		public ObjSwitchPresetInspector(ObjSwitch obj, string varName) : base(obj, varName)
		{ 
			this.objSwitch = obj;
			SetTitle(null);
			inspectors = new List<StrArrInspector>();
		}

		private string[] keys;

		public void SetPresetKeys(string[] keys)
		{
			this.keys = keys;
			inspectors.ForEach(i => i.SetPreset(keys));
		}

		protected override bool OnInspectorGUI(ObjSwitchPreset data, int i)
		{
			if (inspectors.Count <= i) {
				StrArrInspector ins = new StrArrInspector(data, "keys");
				ins.SetPreset(keys);
				ins.Numbering = false;
				inspectors.Add(ins);
			}
			EditorGUIUtil.TextField(null, ref data.presetName);
			StrArrInspector arr = inspectors[i];
			EditorGUILayout.BeginHorizontal();
			bool changed = arr.OnInspectorGUI();
			if (GUILayout.Button("Set", EditorStyles.miniButton)) {
				objSwitch.Set(data.keys);
			}
			EditorGUILayout.EndHorizontal();
			return changed;
		}
	
		protected override bool DrawFooter()
		{
			return false;
		}
	}

}