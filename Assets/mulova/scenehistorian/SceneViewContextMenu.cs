using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace scenehistorian
{
    public static class SceneViewContextMenu
    {
        private static bool drag;
        public delegate void MenuFunc(GenericMenu m);
        public static Func<bool> isAvailable;
        private static readonly List<MenuItem> contextMenuCallback = new List<MenuItem>();

        private class MenuItem : IComparable<MenuItem>
        {
            public readonly MenuFunc func;
            public readonly int order;

            public MenuItem(MenuFunc func, int order)
            {
                this.func = func;
                this.order = order;
            }

            public int CompareTo(MenuItem that)
            {
                return this.order-that.order;
            }
        }

        static SceneViewContextMenu()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        public static void AddContextMenu(MenuFunc func, int order)
        {
            contextMenuCallback.Add(new MenuItem(func, order));
            contextMenuCallback.Sort();
        }

        static void OnSceneGUI(SceneView sceneview)
        {
            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    drag = false;               
                } else if (Event.current.type == EventType.MouseDrag)
                {
                    drag = true;                
                } else if (Event.current.type == EventType.MouseUp && !drag)
                {
                    if (isAvailable == null || isAvailable())
                    {
                        var menu = new GenericMenu();
                        contextMenuCallback.ForEach(f => f.func(menu));
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                }
            }
        }
    }
}

