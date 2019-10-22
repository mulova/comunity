//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Ex;
using System.Reflection;
using System.Text.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/**
* - title이 null이면 folding을 하지 않는다.
*/
namespace mulova.comunity
{
    public abstract class ArrInspector<T> {
        
        public object obj { get; private set; }
        private bool showArray = true;
        private string typeName;
        private string fieldName;
        private int minLength;
        private int maxLength = int.MaxValue;
        private string title;
        private int indent = 1;
        private float width = 100;
        private bool confirmDelete = true;
        
        private TypeSelector enumTypeSelector;
        private EnumParser enumParser = new EnumParser();
        
        protected event Action<T, int> removeCallback;
        protected event Action<T, int> addCallback;
        protected event Action<T, int> changeCallback;
        
        public int MinLength {
            get { return minLength; }
            set { minLength = value; }
        }
        
        public int MaxLength {
            get { return maxLength; }
            set { maxLength = value; }
        }
        
        /// <summary>
        /// </summary>
        /// <value><c>true</c> to expose ordering button; otherwise, <c>false</c>.</value>
        public bool Ordering { get; set; }
        
        public bool Numbering {
            get;
            set;
        }
        
        public bool ConfirmDelete {
            get { return confirmDelete; }
            set { confirmDelete = value; }
        }
        
        public int FixedLength {
            set { MinLength = value; MaxLength = value; Length = value; }
        }
        
        public int Length {
            get { return GetArray().Length; }
            set {
                T[] rows = GetArray();
                Array.Resize(ref rows, value);
                Set(rows);
            }
        }
        
        public T this[int i] {
            get { return GetArray()[i]; }
            set {
                GetArray()[i] = value;
            }
        }
        
        
        /**
        * variable 이 null이면 안된다.
        * @param make
        */
        public ArrInspector(object obj, string variableName)
        {
            this.obj = obj;
            this.fieldName = variableName;
            this.title = variableName;
            Numbering = true;
            FieldInfo field = ReflectionUtil.GetField(obj, variableName);
            if (field.DeclaringType.IsValueType && !field.DeclaringType.IsSerializable) {
                //      if (!ReflectionUtil.IsAttributeDefined<System.SerializableAttribute>(obj, variableName)) {
                Debug.LogError(obj.GetType().FullName+"."+variableName+" is not Serializable");
            }
        }
        
        protected abstract bool OnInspectorGUI(T t, int i);
        
        public void AddItemRemoveCallback(Action<T, int> callback) {
            this.removeCallback += callback;
        }
        
        public void RemoveItemRemoveCallback(Action<T, int> callback) {
            this.removeCallback -= callback;
        }
        
        public void AddItemAddCallback(Action<T, int> callback) {
            this.addCallback += callback;
        }
        
        public void RemoveItemAddCallback(Action<T, int> callback) {
            this.addCallback -= callback;
        }
        
        public void AddItemChangeCallback(Action<T, int> callback) {
            this.changeCallback += callback;
        }
        
        public void RemoveItemChangeCallback(Action<T, int> callback) {
            this.changeCallback -= callback;
        }
        
        public void Unfold(bool unfold) {
            this.showArray = unfold;
        }
        
        public void SetTitle(string title) {
            this.title = title;
        }
        
        public void SetIndent(int indent) {
            EditorGUI.indentLevel = indent;
        }
        
        /**
        * @param enumClass ToString() should have the enum class source code
        */
        public void SetEnum(MonoScript enumClass) {
            enumParser = new EnumParser(enumClass);
        }
        
        public void SetEnum(Type enumType) {
            enumParser = new EnumParser(enumType);
        }
        
        private string enumTypeFieldName;
        public void InitEnumTypeSelector(Type baseType, string enumTypeVar) {
            this.enumTypeFieldName = enumTypeVar;
            if (enumTypeSelector == null) {
                enumTypeSelector = new TypeSelector(baseType);
            } else {
                enumTypeSelector.SetBaseType(baseType);
            }
            string enumTypeName = ReflectionUtil.GetFieldValue<string>(obj, enumTypeVar);
            enumTypeSelector.SetSelected(ReflectionUtil.GetType(enumTypeName));
        }
        
        public TypeSelector GetTypeSelector() {
            return enumTypeSelector;
        }
        
        /// <summary>
        /// Draws the enum type selector for DrawEnum()
        /// </summary>
        /// <returns><c>true</c>, if enum type is changed, <c>false</c> otherwise.</returns>
        public bool DrawEnumTypeSelector() {
            Type type = null;
            return DrawEnumTypeSelector(out type);
        }
        
        /// <summary>
        /// Draws the enum type selector.
        /// </summary>
        /// <returns><c>true</c>, if enum type is changed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool DrawEnumTypeSelector(out Type type) {
            if (Length == 0) {
                type = enumTypeSelector.type;
                return false;
            }
            if (enumTypeSelector == null) {
                type = null;
                Assert.Fail(null, "Call InitTypeSelector() first");
                return false;
            } else {
                bool changed = enumTypeSelector.DrawSelector(ref typeName);
                type = enumTypeSelector.type;
                if (changed && enumTypeFieldName != null) {
                    if (type != null) {
                        ReflectionUtil.SetFieldValue<string>(obj, enumTypeFieldName, type.FullName);
                    } else {
                        ReflectionUtil.SetFieldValue<string>(obj, enumTypeFieldName, string.Empty);
                    }
                    SetDirty();
                }
                return false;
            }
        }
        
        /**
        * Draw Enum UI if SetEnum() is called, else Draw TextField
        */
        public bool DrawEnum(ref string str, params GUILayoutOption[] options) {
            string str2 = str;
            // Create Another EnumParser if new type is selected
            if (enumTypeSelector != null && enumTypeSelector.type != enumParser.enumType) {
                SetEnum(enumTypeSelector.type);
            }
            if (enumParser.OnInspectorGUI(ref str2, options)) {
                str = str2;
                SetDirty();
                return true;
            }
            return false;
        }
        
        private T[] GetArray() {
            T[] rows = ReflectionUtil.GetFieldValue<T[]>(obj, fieldName);
            if (rows == null) {
                rows = new T[0];
            }
            return rows;
        }
        
        public void Add(T row) {
            T[] rows = GetArray();
            ArrayUtil.Add(ref rows, row);
            Set(rows);
        }
        
        public void Set(T[] rows) {
            if (obj is Object)
            {
                Object o = obj as Object;
                Undo.RecordObject(o, o.name);
            }
            ReflectionUtil.SetFieldValue<T[]>(obj, fieldName, rows);
            SetDirty();
        }
        
        protected void SetDirty() {
            if (obj is UnityEngine.Object) {
                EditorUtil.SetDirty(obj as UnityEngine.Object);
            }
        }
        
        protected float GetWidth() {
            return width;
        }
        
        private void UpdateWidth() {
            float buttonWidth = 90; // number label, +, - button's width
            this.width = GUILayoutUtility.GetLastRect().width-buttonWidth;
        }
        
        protected virtual bool DrawHeader() {
            showArray = EditorGUILayout.Foldout(showArray, title);
            return false;
        }
        
        protected virtual bool DrawFooter() { 
            return false;
        }
        
        private T GetDefault() {
            if (typeof(T).GetAttribute<ConstructorAttribute>() != null) {
                return (T)typeof(T).GetConstructor(new Type[0]).Invoke(null);
            } else {
                return default(T);
            }
        }
        
        private List<T> list = new List<T>();
        public virtual bool OnInspectorGUI() {
            //      Undo.SetSnapshotTarget(obj, "Array Change");
            bool changed = false;
            list.Clear();
            
            EditorGUILayout.BeginVertical(); // +1
            EditorGUILayout.BeginHorizontal(); // +2
            if (!string.IsNullOrEmpty(title)) {
                changed |= DrawHeader();
            } else {
                showArray = true;
            }
            if (Length == 0) {
                if (GUILayout.Button("Add "+(!title.IsEmpty()?title:fieldName), EditorStyles.toolbarButton)) {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) {
                        list.Add(GetDefault());
                    } else {
                        list.Add(ReflectionUtil.NewInstance<T>());
                    }
                    changed = true;
                    showArray = true;
                    addCallback.Call(list[0], 0);
                }
            }
            EditorGUILayout.EndHorizontal(); //-2
            T[] rows = GetArray();
            bool nullWarning = false;
            if (showArray) {
                if (!string.IsNullOrEmpty(title)) {
                    EditorGUI.indentLevel += indent;
                }
                int swapIndex = -1;
                for (int i=0; i< rows.Length; i++) {
                    EditorGUILayout.BeginHorizontal(); // +3
                    if (Numbering) {
                        EditorGUILayout.LabelField((i+1).ToString(), EditorStyles.boldLabel, GUILayout.Width(40));
                    }
                    if (typeof(T).IsClass && Object.Equals(rows[i], GetDefault())) {
                        nullWarning = true;
                    }
                    if (OnInspectorGUI(rows[i], i)) {
                        changeCallback.Call(rows[i], i);
                        changed = true;
                    }
                    bool add = false;
                    if (Length < MaxLength && GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20))) {
                        add = true;
                        changed = true;
                        addCallback.Call(rows[i], i);
                    }
                    if (Length > MinLength && GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20))) {
                        if (!ArrInspectorMenu.globalConfirmDelete || !confirmDelete) {
                            changed = true;
                        } else {
                            string itemName = rows[i]!=null? rows[i].ToString(): "-";
                            int sel = EditorUtility.DisplayDialogComplex("Warning", "Remove "+itemName, "Delete", "Cancel", "No Confirm");
                            if (sel == 0) {
                                changed = true;
                            } else if (sel == 1) {
                                list.Add(rows[i]);
                            } else {
                                changed = true;
                                confirmDelete = false;
                            }
                        }
                        if (changed) {
                            removeCallback.Call(rows[i], i);
                        }
                    } else {
                        list.Add(rows[i]);
                    }
                    if (Ordering) {
                        GUI.enabled = i != 0;
                        if (GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(15))) {
                            swapIndex = i-1;
                        }
                        GUI.enabled = i != rows.Length-1;
                        if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(15))) {
                            swapIndex = i;
                        }
                        GUI.enabled = true;
                    }
                    if (add) {
                        if (rows[i] is ICloneable) {
                            list.Add((T)(rows[i] as ICloneable).Clone());
                        } else {
                            list.Add(GetDefault());
                        }
                    }
                    EditorGUILayout.EndHorizontal(); // -3
                }
                if (swapIndex >= 0) {
                    list.Swap(swapIndex, swapIndex+1);
                    changed = true;
                }
                if (!string.IsNullOrEmpty(title)) {
                    EditorGUI.indentLevel -= indent;
                }
            }
            if (changed) {
                Set(list.ToArray());
                SetDirty();
            }
            if (DrawFooter()) {
                SetDirty();
            }
            EditorGUILayout.EndVertical(); // -1
            
            if(Event.current.type == EventType.Repaint) {
                UpdateWidth();
            }
            
            if (nullWarning) {
                EditorGUILayout.HelpBox(string.Format("Implement IClonable for {0} if non-null values are required", typeof(T).FullName), MessageType.Warning);
            }
            
            return changed;
        }
        
        public void Sort(IComparer<T> comparer) {
            T[] src = GetArray();
            T[] arr = (T[])src.Clone();
            Array.Sort(arr, comparer);
            if (!ArrayUtil.Equals(src, arr)) {
                Set(arr);
            }
        }
        
        public void Sort() {
            T[] arr = GetArray();
            Array.Sort(arr);
            Set(arr);
        }
    }
}


public static class ArrInspectorMenu {
	public static bool globalConfirmDelete = true;
	[MenuItem("Tools/unilova/Inspector/Confirm Delete for array")]
	public static void SetGlobalConfirmDelete() {
		globalConfirmDelete = !globalConfirmDelete;
	}
}