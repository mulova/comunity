using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using commons;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.Ex;

namespace scenehistorian
{
    public class SceneHistoryWindow : EditorWindow
    {
        private SceneHistory sceneHistory;
        private const string PATH = "Library/scenehistorian/history";
        private Object currentScene;
        private bool changed;
        private string nameFilter;
        private ReorderList<SceneHistoryItem> listDrawer;
        private const bool SHOW_SIZE = false;
        private GUIContent sortIcon;

        private static readonly Color SORT_COLOR = Color.green;

        private bool valid
        {
            get
            {
                return !BuildPipeline.isBuildingPlayer;
            }
        }

        public static SceneHistoryWindow instance
        {
            get
            {
                return EditorWindow.GetWindow<SceneHistoryWindow>("SceneHistorian");
            }
        }


        void OnEnable()
        {
            sortIcon = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Sort");
            var dir = Path.GetDirectoryName(PATH);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            sceneHistory = SceneHistory.Load(PATH);
            OnSceneOpened(SceneManager.GetActiveScene(), OpenSceneMode.Single);
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += OnSceneObjChange;
#else
            EditorApplication.hierarchyWindowChanged += OnSceneObjChange;
#endif
            if (!BuildPipeline.isBuildingPlayer)
            {
                EditorApplication.pauseStateChanged += OnPauseStateChanged;
                EditorSceneManager.sceneOpening += OnSceneOpening;
                EditorSceneManager.sceneOpened += OnSceneOpened;
                EditorSceneManager.sceneClosing += OnSceneClosing;
                SceneManager.activeSceneChanged += OnActiveScene;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

#if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
#else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
#endif
            var history = sceneHistory.items;
            if (history.Count > 0)
            {
                SceneViewContextMenu.AddContextMenu(menu => {
                    if (history.Count >= 2)
                    {
                        menu.AddItem(new GUIContent("Previous: " + history[1].name), false, GoBack);
                    }
                    for (int i = 2; i < history.Count; ++i)
                    {
                        if (history[i].starred)
                        {
                            menu.AddItem(new GUIContent("scenes/" + history[i].name), false, OnSceneMenu, history[i]);
                        }
                    }
                }, 1);
            }
        }

        void OnDisable()
        {
            // Enter play mode
            if (!Application.isPlaying)
            {
                SaveCam();
                sceneHistory.Save(PATH);
            }
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged -= OnSceneObjChange;
#else
            EditorApplication.hierarchyWindowChanged -= OnSceneObjChange;
#endif
            EditorApplication.pauseStateChanged -= OnPauseStateChanged;
            EditorSceneManager.sceneOpening -= OnSceneOpening;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosing -= OnSceneClosing;
            SceneManager.activeSceneChanged -= OnActiveScene;
            SceneManager.sceneLoaded -= OnSceneLoaded;

#if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
#else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
#endif
        }

        private void OnActiveScene(Scene s1, Scene s2)
        {
            if (Application.isPlaying) {
                return;
            }
            if (sceneHistory.Count == 0)
            {
                return;
            }
            if (!sceneHistory[0].activeScene.path.EqualsIgnoreSeparator(s2.path)) {
                sceneHistory[0].SetActiveScene(s2.path);
                sceneHistory.Save(PATH);
            }
        }

        void OnPauseStateChanged(PauseState state)
        {
            //			if (sceneHistory.Count >= 0)
            //			{
            //				sceneHistory[0].ApplyCam();
            //			}
        }

        private void OnSceneObjChange()
        {
            changed = true;
        }

        private void ChangePlayMode(PlayModeStateChange stateChange)
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                return;
            }
            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (sceneHistory.Count >= 0 && sceneHistory.useCam)
                {
                    sceneHistory[0].ApplyCam();
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //			if (mode != LoadSceneMode.Single)
            //			{
            //				return;
            //			}
            if (BuildPipeline.isBuildingPlayer)
            {
                return;
            }
            if (sceneHistory.useCam)
            {
                int index = sceneHistory.IndexOf(scene.path);
                if (index >= 0)
                {
                    sceneHistory[index].ApplyCam();
                }
            }
        }

        private void SaveCam()
        {
            if (sceneHistory.Count > 0)
            {
                var item = sceneHistory[0];
                item.SaveCam();
            }
        }

        private void OnSceneOpening(string path, OpenSceneMode mode)
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                return;
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            if (mode == OpenSceneMode.Single)
            {
                SaveCam();
            }
            sceneHistory.Save(PATH);
        }

        private void OnSceneOpened(Scene s, OpenSceneMode mode)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            currentScene = AssetDatabase.LoadAssetAtPath<Object>(EditorSceneBridge.currentScene);
            if (currentScene == null)
            {
                return;
            }
            SceneHistoryItem item = null;
            int index = sceneHistory.IndexOf(currentScene);

            if (index >= 0)
            {
                item = sceneHistory[index];
                if (mode == OpenSceneMode.Single)
                {
                    sceneHistory.RemoveAt(index);
                    sceneHistory.Insert(0, item);
                    item.LoadAdditiveScenes();
                    if (sceneHistory.useCam)
                    {
                        item.ApplyCam();
                    }
                } else
                {
                    var sceneObj = AssetDatabase.LoadAssetAtPath<Object>(s.path);
                    if (!item.Contains(sceneObj))
                    {
                        item.AddScene(sceneObj);
                    }
                }
            } else
            {
                item = new SceneHistoryItem(currentScene);
                item.SaveCam();
                sceneHistory.Insert(0, item);
            }
            sceneHistory.Save(PATH);
            changed = false;
        }

        private void OnSceneClosing(Scene s, bool removing)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            if (EditorSceneBridge.currentScene == s.path)
            {
                SaveCam();
            } else
            {
                currentScene = AssetDatabase.LoadAssetAtPath<Object>(EditorSceneBridge.currentScene);
                if (currentScene == null)
                {
                    return;
                }
                int index = sceneHistory.IndexOf(currentScene);
                SceneHistoryItem item = null;
                if (index >= 0)
                {
                    item = sceneHistory[index];
                    var sceneObj = AssetDatabase.LoadAssetAtPath<Object>(s.path);
                    if (item.Contains(sceneObj))
                    {
                        item.RemoveScene(sceneObj);
                    }
                } else
                {
                    item = new SceneHistoryItem(currentScene);
                    sceneHistory.Insert(0, item);
                }
            }
            sceneHistory.Save(PATH);
            changed = false;
        }

        public void GoBack()
        {
            if ((!changed || EditorSceneBridge.SaveCurrentSceneIfUserWantsTo()) && sceneHistory.Count > 1)
            {
                EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
            }
        }

        void OnInspectorUpdate() {
            if (!Application.isPlaying)
            {
                // activeSceneChanged event is not available in Editor mode
                var s = SceneManager.GetActiveScene();
                OnActiveScene(s, s);
            }
        }

        private void OnSceneMenu(object h)
        {
            SceneHistoryItem hist = h as SceneHistoryItem;
            EditorSceneManager.OpenScene(hist.first.path);
        }

        private void ShowMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Sort"), sceneHistory.sort, () => {
                sceneHistory.sort = !sceneHistory.sort;
                sceneHistory.Save(PATH);
            });
            menu.AddItem(new GUIContent("Cam"), sceneHistory.useCam, () => {
                sceneHistory.useCam = !sceneHistory.useCam;
                sceneHistory.Save(PATH);
            });

            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Warning", "Clear history?", "Ok", "Cancel"))
                {
                    sceneHistory.Clear();
                    File.Delete(PATH);
                }
            });
            menu.ShowAsContext();
        }

        private List<UnityObjId> allScenes = new List<UnityObjId>();
        public void OnHeaderGUI()
        {
            //GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (EditorGUIUtil.SearchField("", ref nameFilter))
            {
                allScenes.Clear();
            }
            GUI.enabled = sceneHistory.Count >= 2;
            if (GUILayout.Button("Back", EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20)))
            {
                GoBack();
            }
            var color = GUI.contentColor;
            if (sceneHistory.sort)
            {
                GUI.contentColor = SORT_COLOR;
            }
            if (GUILayout.Button(sortIcon, EditorStyles.toolbarButton, GUILayout.Width(30), GUILayout.Height(20)))
            {
                ShowMenu();
            }
            GUI.contentColor = color;
            GUILayout.EndHorizontal();
        }

        private ReorderList<SceneHistoryItem> CreateDrawer(SceneHistory history)
        {
            GUIContent favoriate = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Save search");
            return new ReorderList<SceneHistoryItem>(history.items)
            {
                displayAdd = false,
                createItem = () => new SceneHistoryItem(Selection.activeObject),
                drawItem = (item, rect, index, active, focus) =>
                {
                    var showCam = history.useCam && item.camProperty.valid;
                    var rightWidth = showCam ? 60 : 20;
                    Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width - rightWidth);
                    Object obj = item.first.reference as Object;
                    item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
                    Rect starredRect = area1[1];
                    if (showCam)
                    {
                        Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 40);
                        if (showCam && GUI.Button(area2[0], "cam", EditorStyles.toolbarButton))
                        {
                            item.camProperty.Apply();
                        }
                        starredRect = area2[1];
                    }
                    bool starred = item.starred;
                    Color cc = GUI.contentColor;
                    GUI.contentColor = starred ? Color.cyan : Color.black;

                    if (GUI.Button(starredRect, favoriate, EditorStyles.toolbarButton))
                    {
                        item.starred = !item.starred;
                    }
                    GUI.contentColor = cc;
                    return starred != item.starred || obj != item.first.reference;
                }
                //UnityObjIdDrawer.DrawItem(item.first, rect, false)
            };
        }

        private Vector3 scrollPos;
        void OnGUI()
        {
            //listDrawer = new SceneHistoryDrawer(sceneHistory);
            //listDrawer.allowSceneObject = false;
            listDrawer = CreateDrawer(sceneHistory);
            OnHeaderGUI();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (nameFilter.IsNotEmpty())
            {
                string[] filters = nameFilter.SplitEx(' ');
                Predicate<SceneHistoryItem> filter = h =>
                {
                    if (h.first == null || h.first.path.IsEmpty())
                    {
                        return false;
                    }
                    string itemName = h.name;
                    foreach (var n in filters)
                    {
                        if (itemName.IndexOfIgnoreCase(n) < 0)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                listDrawer.Filter(filter);
            }
            try
            {
                try
                {
                    listDrawer.Draw();
                } catch (Exception ex)
                {
                    Debug.LogException(ex);
                    sceneHistory.Clear();
                }
                if (listDrawer.changed)
                {
                    sceneHistory.Save(PATH);
                    changed = false;
                }
                if (allScenes.IsEmpty())
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene "+nameFilter);
                    foreach (var id in guids)
                    {
                        allScenes.Add(new UnityObjId(id));
                    }
                }

                if (nameFilter.IsNotEmpty())
                {
					EditorGUILayout.LabelField("Not in history", EditorStyles.miniBoldLabel);
                    string[] filters = nameFilter.SplitEx(' ');
                    var filteredScenes = new SceneHistory();
                    foreach (var s in allScenes)
                    {
                        string filename = Path.GetFileNameWithoutExtension(s.path);
                        bool match = true;
                        foreach (var f in filters)
                        {
                            if (filename.IndexOfIgnoreCase(f) < 0)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match && !sceneHistory.Contains(s.path))
                        {
                            filteredScenes.Add(s.reference);
                        }
                    }
                    var drawer = CreateDrawer(filteredScenes);
                    drawer.Draw();
                }
            } catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            EditorGUILayout.EndScrollView();
            OnFooterGUI();
        }

        void OnFooterGUI()
        {
            GUI.enabled = true;
            if (SHOW_SIZE)
            {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUIUtil.IntField("Size", ref sceneHistory.maxSize))
                {
                    if (sceneHistory.maxSize < 2)
                    {
                        sceneHistory.maxSize = 2;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
