using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using mulova.commons;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

namespace convinity
{
//    public class RefSearcher
//    {
//        private Object searchObj;
//        private MemberInfoRegistry registry = new MemberInfoRegistry(MemberInfoRegistryEx.ObjectRefFilter);
//        private bool isScene;
//
//        public RefSearcher()
//        {
//        }
//
//        public override void OnHeaderGUI(List<Object> found)
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUIUtil.ObjectField<Object>("Ref", ref searchObj, true);
//            GUI.enabled = searchObj != null;
//            if (GUILayout.Button("Search from Root"))
//            {
//                isScene = false;
//                Search();
//            }
//            if (GUILayout.Button("Search Scenes"))
//            {
//                isScene = true;
//                Search();
//            }
//            EditorGUILayout.EndHorizontal();
//            GUI.enabled = true;
//        }
//
//        private Object rhsObj;
//
//        public override void OnFooterGUI(List<Object> found)
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUIUtil.ObjectField<Object>("Value", ref rhsObj, true);
//            GUI.enabled = searchObj != null&&rhsObj != null&&searchObj.GetType() == rhsObj.GetType()&&allocInfo.Count > 0;
//            if (GUILayout.Button("Allocate"))
//            {
//                Allocate();
//            }
//            EditorGUILayout.EndHorizontal();
//            GUI.enabled = true;
//        }
//
//        private void Allocate()
//        {
//            foreach (FieldRefDrawer a in allocInfo)
//            {
//                a.SetValue(rhsObj);
//            }
//        }
//
//        private void SearchTransforms(Object search, IEnumerable<Transform> roots, List<Object> store)
//        {
//            GameObject searchGO = search is Component? (search as Component).gameObject : null;
//            foreach (Transform r in roots)
//            {
//                if (r.gameObject == search)
//                {
//                    store.Add(r.gameObject);
//                    simpleRef.Add(r.gameObject);
//                } else
//                {
//                    foreach (Component c in r.GetComponentsInChildren<Component>(true))
//                    {
//                        if (c == null||c is Transform||search == c||search == c.gameObject||searchGO == c.gameObject)
//                        {
//                            continue;
//                        }
//                        if (SearchMemberRefs(c, search))
//                        {
//                            store.Add(c);
//                        }
//                    }
//                }
//            }
//        }
//
//        private List<FieldRefDrawer> allocInfo = new List<FieldRefDrawer>();
//        private List<Object> simpleRef = new List<Object>();
//
//        protected override List<Object> SearchResource(Object root)
//        {
//            List<Object> store = new List<Object>();
//            allocInfo.Clear();
//            simpleRef.Clear();
//            if (!isScene)
//            {
//                if (AssetDatabase.GetAssetPath(root).IsEmpty())
//                {
//                    IEnumerable<Transform> roots = null;
//                    if (root != null)
//                    {
//                        roots = new Transform[] { (root as GameObject).transform };
//                    } else
//                    {
//                        roots = EditorSceneManager.GetActiveScene().GetRootGameObjects().Convert(o => o.transform);
//                    }
//
//                    SearchTransforms(searchObj, roots, store);
//                    if (searchObj is GameObject)
//                    {
//                        foreach (Component c in (searchObj as GameObject).GetComponents<Component>())
//                        {
//                            SearchTransforms(c, roots, store);
//                        }
//                    }
//                } else
//                {
//                    List<Object> assets = SearchAssets(typeof(Object), FileType.All);
//                    for (int i=0; i<assets.Count; ++i)
//                    {
//                        Object o = assets[i];
//                        if (EditorUtility.DisplayCancelableProgressBar("Assets", o.name, i/(float)assets.Count) == false)
//                        {
//                            break;
//                        }
//                        if (o is GameObject)
//                        {
//                            foreach (Component c in (o as GameObject).GetComponentsInChildren<Component>(true))
//                            {
//                                if (c != null&&searchObj != c&&searchObj != c.gameObject)
//                                {
//                                    if (SearchMemberRefs(c, searchObj))
//                                    {
//                                        store.Add(c);
//                                    }
//                                }
//                            }
//                        } else
//                        {
//                            if (SearchMemberRefs(o, searchObj))
//                            {
//                                store.Add(o);
//                            }
//                        }
//                    }
//                    EditorUtility.ClearProgressBar();
//                }
//            } else
//            {
//                BuildScript.ForEachScene(roots => {
//                    SearchTransforms(searchObj, roots, store);
//                    if (searchObj is GameObject)
//                    {
//                        foreach (Component c in (searchObj as GameObject).GetComponents<Component>())
//                        {
//                            SearchTransforms(c, roots, store);
//                        }
//                    }
//                    return null;
//                });
//            }
//            return store;
//        }
//
//        private bool SearchMemberRefs(Object o, Object search)
//        {
//            bool ret = false;
//            if (o is GameObject)
//            {
//                foreach (var c in (o as GameObject).GetComponentsInChildren<Component>())
//                {
//                    ret |= SearchMemberRefs0(c, search);
//                }
//            }
//            ret |= SearchMemberRefs0(o, search);
//            return ret;
//        }
//        private bool SearchMemberRefs0(Object o, Object search)
//        {
//            if (o == null)
//            {
//                return false;
//            }
//            if (o == search)
//            {
//                simpleRef.Add(o);
//                return true;
//            } else
//            {
//                FieldInfo f = registry.GetFieldForValue(o, search);
//                if (f != null)
//                {
//                    var alloc = new FieldRefDrawer(o, f);
//                    if (!allocInfo.Contains(alloc))
//                    {
//                        allocInfo.Add(alloc);
//                    }
//                    return true;
//                } else
//                {
//                    PropertyInfo p = registry.GetPropertyForValue(o, search);
//                    if (p != null)
//                    {
//                        var alloc = new FieldRefDrawer(o, p);
//                        if (!allocInfo.Contains(alloc))
//                        {
//                            allocInfo.Add(alloc);
//                        }
//                        return true;
//                    }
//                }
//                return false;
//            }
//        }
//
//        protected override void OnInspectorGUI(List<Object> found)
//        {
//            GUI.enabled = true;
//            if (allocInfo.Count > 0)
//            {
//                ListDrawer<FieldRefDrawer> drawer = new ListDrawer<FieldRefDrawer>(allocInfo, t => (IItemDrawer<FieldRefDrawer>)t);
//                drawer.Draw(Rotorz.ReorderableList.ReorderableListFlags.ShowIndices);
//            }
//            if (simpleRef.Count > 0)
//            {
//                ListDrawer<Object> drawer = new ListDrawer<Object>(simpleRef);
//                drawer.Draw(Rotorz.ReorderableList.ReorderableListFlags.ShowIndices);
//            } 
//            //                EditorGUILayout.HelpBox("No reference found", MessageType.Info);
//        }
//    }
}

