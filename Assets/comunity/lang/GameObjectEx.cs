using System.Collections.Generic;
using System;
using commons;

namespace UnityEngine.Ex {
	public static class GameObjectEx {
		public static GameObject FindPath(string path) {
			string[] paths = path.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
			return FindPath (paths);
		}

		public static GameObject FindPath(string[] paths) {
			GameObject o = GameObject.Find(paths[0]);
			if (o != null && paths.Length > 1) {
				Transform t = o.transform;
				for (int i=1; i<paths.Length && t!=null; ++i) {
					t = t.Find(paths[i]);
				}
				return (t != null) ? t.gameObject : null;
			}
			return o;
		}

		public static GameObject CreateParent(this GameObject child, string name) {
			GameObject p = CreateSibling(child, name);
			child.transform.parent = p.transform;
			return p;
		}

		public static void SetLayer(this GameObject o, int layer) {
			o.transform.DepthFirstTraversal(t=>{
				t.gameObject.layer = layer;
				return true;
			});
		}

		public static void SetLayer(this GameObject o, string layerName) {
			int layer = LayerMask.NameToLayer(layerName);
			o.SetLayer(layer);
		}

		public static GameObject CreateSibling(this GameObject s, string name) {
			GameObject go = new GameObject(name);
			go.layer = s.layer;
			Transform strans = s.transform;
			Transform trans = go.transform;
			trans.parent = strans.parent;
			trans.SetLocal(strans);
			trans.SetSiblingIndex(strans.GetSiblingIndex()+1);
			return go;
		}

		public static GameObject CreateChild(this GameObject p, string name) {
			GameObject go = new GameObject(name);
			go.layer = p.layer;
			Transform ptrans = p.transform;
			Transform ctrans = go.transform;
			ctrans.SetParent(ptrans, false);
			return go;
		}
		/**
		 * 상위 GameObject가 inactive이면 active로 바꾼다.
		 */
		public static void Activate(this GameObject obj) {
			obj.SetActive(true);
			Transform parent = obj.transform.parent;
			if (!obj.activeInHierarchy) {
				GameObject go = parent.gameObject;
				if (!go.activeSelf) {
					go.SetActive(true);
				}
				parent = parent.parent;
			}
		}

		public static void SetActiveEx(this GameObject obj, bool active) {
			if (obj != null) {
				obj.SetActive(active);
			}
		}
		
		/// <summary>
		/// Add Component if not exists.
		/// </summary>
		/// <returns>The component</returns>
		/// <param name="o">O.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T FindComponent<T>(this GameObject o) where T:Component {
			T comp = o.GetComponent<T>();
			if (comp == null) {
				comp = o.AddComponent<T>();
			}
			if (comp == null) {
				Debug.LogError(o.name);
			}
			return comp;
		}

		public static Component FindComponent(this GameObject o, Type type) {
			Component comp = o.GetComponent(type);
			if (comp == null) {
				comp = o.AddComponent(type);
			}
			if (comp == null) {
				Debug.LogError(o.name);
			}
			return comp;
		}

		public static void RemoveComponent<T>(this GameObject o) where T:Component {
			T comp = o.GetComponent<T>();
			if (comp != null) {
				comp.DestroyEx();
			}
		}

		/// <summary>
		/// Similar to the GetComponentInParent but Includes disabled GameObject
		/// </summary>
		/// <returns>The component in parent ex.</returns>
		/// <param name="o">O.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInParentEx<T>(this GameObject o) where T:Component {
			Transform t = o.transform;
			T comp = null;
			while (t != null && comp == null) {
				comp = t.GetComponent<T>();
				t = t.parent;
			}
			return comp;
		}

		/**
		 * Component를 enable/disable시킨다.
		 * enable일 경우 component 가 없을 경우 추가한다.
		 */
		public static T SetEnabled<T>(this GameObject o, bool enable) where T:Behaviour {
			T comp = o.GetComponent<T>();
			if (comp == null) {
				if (enable) {
					comp = o.AddComponent<T> ();
				}
			} else {
				comp.enabled = enable;
			}
			return comp;
		}

		public static Component[] GetComponentsWithAttribute<T>(this GameObject obj) where T:Attribute {
			Component[] comps = obj.GetComponentsInChildren<Component>(true);
			List<Component> list = new List<Component>();
			foreach (Component c in comps) {
				if (ReflectionUtil.GetAttribute<T>(c.GetType()) != null) {
					list.Add(c);
				}
			}
			return list.ToArray();
		}


		public static T GetInterface<T>(this GameObject obj, bool includeDisabled) where T:class {
			T[] interfaces = GetInterfaces<T>(obj, includeDisabled);
			if (interfaces.IsNotEmpty()) {
				return default(T);
			}
			return interfaces[0];
		}
		
		public static T[] GetInterfaces<T>(this GameObject obj, bool includeInactive) where T:class {
			Behaviour[] comps = obj.GetComponents<Behaviour>();
			List<T> list = new List<T>();
			foreach (Behaviour c in comps) {
				if (c is T) {
					if (includeInactive || c.enabled) {
						list.Add(c as T);
					}
				}
			}
			return list.ToArray();
		}
		
		public static T GetInterfaceInChildren<T>(this GameObject obj, bool includeDisabled) where T:class {
			return GetInterfacesInChildren<T>(obj, includeDisabled)[0];
		}
		
		public static T[] GetInterfacesInChildren<T>(this GameObject obj, bool includeInactive) where T:class {
			Behaviour[] comps = obj.GetComponentsInChildren<Behaviour>(includeInactive);
			List<T> list = new List<T>();
			foreach (Behaviour c in comps) {
				if (c is T) {
					list.Add(c as T);
				}
			}
			return list.ToArray();
		}
		
		public static T GetInterfaceInAncestors<T>(this GameObject obj, bool includeDisabled) where T:class {
			T t = GetInterface<T>(obj, includeDisabled);
			if (t != null) {
				return t;
			}
			GameObject parent = obj.transform.parent.gameObject;
			if (parent == null) {
				return null;
			}
			return GetInterfaceInAncestors<T>(parent, includeDisabled);
		}

		/// <summary>
		/// Include inactive
		/// </summary>
		/// <returns>The component in children.</returns>
		/// <param name="c">C.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInChildrenEx<T>(this GameObject go) where T:Component {
			Transform found = go.transform.BreadthFirstSearch(t=>t.GetComponent<T>() != null);
			if (found != null) {
				return found.GetComponent<T>();
			}
			return null;
		}
		
        private class ComponentSearchPredicate<T> : IPredicate<Transform> where T: Component{
			public bool Accept(Transform t)
			{
				return t.GetComponent<T>() != null;
			}
			
			public string Name {
				get {
					return typeof(T).FullName;
				}
			}
		}
		
		private class ComponentSearcher<T> : Apply<Transform> where T: Component{
			public List<T> store = new List<T>();
			public void Apply(Transform t)
			{
				T c = t.GetComponent<T>();
				if (c != null) {
					store.Add(c);
				}
			}
		}
	}
}


