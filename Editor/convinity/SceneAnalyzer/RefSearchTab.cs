using System.Collections.Generic;
using System.Ex;
using System.Reflection;
using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace convinity
{
    class RefSearchTab : SearchTab<Object>
    {
        private Object searchObj;
        private UnityMemberInfoRegistry registry = new UnityMemberInfoRegistry();
        private bool isScene;

        public RefSearchTab(TabbedEditorWindow window) : base("Ref Search", window)
        {
            registry.ExcludeField("", "");
        }

        public override void OnHeaderGUI(List<Object> found)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayoutUtil.ObjectField<Object>("Ref", ref searchObj, true);
            GUI.enabled = searchObj != null;
            if (GUILayout.Button("Search from Root"))
            {
                isScene = false;
                registry.BeginSearch();
                Search();
            }
            if (GUILayout.Button("Search Scenes"))
            {
                registry.BeginSearch();
                isScene = true;
                Search();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private Object rhsObj;

        public override void OnFooterGUI(List<Object> found)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayoutUtil.ObjectField<Object>("Value", ref rhsObj, true);
            GUI.enabled = searchObj != null&&rhsObj != null&&searchObj.GetType() == rhsObj.GetType()&&allocInfo.Count > 0;
            if (GUILayout.Button("Allocate"))
            {
                Allocate();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void Allocate()
        {
            foreach (FieldRef a in allocInfo)
            {
                a.SetValue(rhsObj);
            }
        }

        private void SearchTransforms(Object search, IEnumerable<Transform> roots, List<Object> store)
        {
            GameObject searchGO = search is Component? (search as Component).gameObject : null;
            foreach (Transform r in roots)
            {
                if (r.gameObject == search)
                {
                    store.Add(r.gameObject);
                    simpleRef.Add(r.gameObject);
                } else
                {
                    foreach (Component c in r.GetComponentsInChildren<Component>(true))
                    {
                        if (c == null||c is Transform||search == c||search == c.gameObject||searchGO == c.gameObject)
                        {
                            continue;
                        }
                        if (SearchMemberRefs(c, search))
                        {
                            store.Add(c);
                        }
                    }
                }
            }
        }

        private List<FieldRef> allocInfo = new List<FieldRef>();
        private List<Object> simpleRef = new List<Object>();

        protected override List<Object> SearchResource()
        {
            List<Object> store = new List<Object>();
            allocInfo.Clear();
            simpleRef.Clear();
            if (!isScene)
            {
				foreach (var root in roots)
				{
					if (AssetDatabase.GetAssetPath(root).IsEmpty())
					{
						IEnumerable<Transform> roots = null;
						if (root != null)
						{
							roots = new Transform[] { (root as GameObject).transform };
						} else
						{
							roots = EditorSceneManager.GetActiveScene().GetRootGameObjects().ConvertAll(o => o.transform);
						}
						
						SearchTransforms(searchObj, roots, store);
						if (searchObj is GameObject)
						{
							foreach (Component c in (searchObj as GameObject).GetComponents<Component>())
							{
								SearchTransforms(c, roots, store);
							}
						}
					} else
					{
						foreach (var o in SearchAssets(typeof(Object), FileType.All))
						{
							if (EditorUtility.DisplayCancelableProgressBar("Assets", o.name, 0.5f))
							{
								break;
							}
							if (o is GameObject)
							{
								foreach (Component c in (o as GameObject).GetComponentsInChildren<Component>(true))
								{
									if (c != null&&searchObj != c&&searchObj != c.gameObject)
									{
										if (SearchMemberRefs(c, searchObj))
										{
											store.Add(c);
										}
									}
								}
							} else
							{
								if (SearchMemberRefs(o, searchObj))
								{
									store.Add(o);
								}
							}
						}
						EditorUtility.ClearProgressBar();
					}
				}
            } else
            {
                EditorTraversal.ForEachScene(scene => {
                    var r = scene.GetRootGameObjects().ConvertAll(o => o.transform);
                    SearchTransforms(searchObj, r, store);
                    if (searchObj is GameObject)
                    {
                        foreach (Component c in (searchObj as GameObject).GetComponents<Component>())
                        {
                            SearchTransforms(c, r, store);
                        }
                    }
                    return null;
                });
            }
            return store;
        }

        private bool SearchMemberRefs(Object o, Object search)
        {
            bool ret = false;
            if (o is GameObject)
            {
                foreach (var c in (o as GameObject).GetComponentsInChildren<Component>())
                {
                    ret |= SearchMemberRefs0(c, search);
                }
            }
            ret |= SearchMemberRefs0(o, search);
            return ret;
        }
        private bool SearchMemberRefs0(Object o, Object search)
        {
            if (o == null)
            {
                return false;
            }
            if (o == search)
            {
                simpleRef.Add(o);
                return true;
            } else
            {
                FieldInfo f = registry.GetFieldForValue(o, search);
                if (f != null)
                {
                    var alloc = new FieldRef(o, f);
                    if (!allocInfo.Contains(alloc))
                    {
                        allocInfo.Add(alloc);
                    }
                    return true;
                } else
                {
                    PropertyInfo p = registry.GetPropertyForValue(o, search);
                    if (p != null)
                    {
                        var alloc = new FieldRef(o, p);
                        if (!allocInfo.Contains(alloc))
                        {
                            allocInfo.Add(alloc);
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        protected override void OnInspectorGUI(List<Object> found)
        {
            GUI.enabled = true;
            if (allocInfo.Count > 0)
            {
				ListDrawer<FieldRef> drawer = new ListDrawer<FieldRef>(allocInfo, new FieldRefDrawer());
                drawer.Draw();
            }
            if (simpleRef.Count > 0)
            {
				var drawer = new ListDrawer<Object>(simpleRef, new ObjListItemDrawer<Object>());
				drawer.Draw();
			}
			if (allocInfo.Count == 0 && simpleRef.Count == 0)
			{
           		EditorGUILayout.HelpBox("No reference found", MessageType.Info);
			}
        }

        public override void OnChangePlayMode(PlayModeStateChange stateChange)
        {
        }

        public override void OnChangeScene(string sceneName)
        {
        }

        public override void OnFocus(bool focus)
        {
        }

        public override void OnSelected(bool sel)
        {
        }
    }
}

