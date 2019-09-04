//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;
using mulova.commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;
using System.Ex;
using System.Linq;

namespace comunity
{
    public class UISwitch : MonoBehaviour
    {
        [SerializeField, EnumType] private string enumType;
        [SerializeField, HideInInspector] public List<GameObject> objs = new List<GameObject>();
        [SerializeField] public List<UISwitchSect> switches = new List<UISwitchSect>();
        [SerializeField] public UISwitchPreset[] preset;

        public bool overwrite = false;
        private UISwitchSect DUMMY = new UISwitchSect();
        private HashSet<string> keySet = new HashSet<string>();

        private ILogger log
        {
            get
            {
                return Debug.unityLogger;
            }
        }


        public void ResetSwitch()
        {
            keySet.Clear();
        }

        public bool Contains(params object[] list)
        {
            foreach (object o in list)
            {
                if (!keySet.Contains(Normalize(o)))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddSwitch(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Add(Normalize(o));
            }
        }

        public void RemoveSwitch(params object[] list)
        {
            foreach (object o in list)
            {
                keySet.Remove(Normalize(o));
            }
        }

        public bool Remove(GameObject o)
        {
            int i = objs.IndexOf(o);
            if (i >= 0)
            {
                foreach (var s in switches)
                {
                    s.RemoveAt(i);
                }

                objs.RemoveAt(i);
                return true;
            } else
            {
                return false;
            }
        }

        public void ToggleSwitch(object key)
        {
            string k = Normalize(key);
            if (keySet.Contains(k))
            {
                keySet.Remove(k);
            } else
            {
                keySet.Add(k);
            }
        }

        public void SetAction(object key, Action action)
        {
            string k = Normalize(key);
            UISwitchSect s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s.isValid)
            {
                s.action = action;
            } else
            {
                Assert.Fail(this, "{0} not found", k);
            }
        }

        public void Set(params object[] param)
        {
            if (!overwrite&&Contains(param))
            {
                if (log.IsLoggable(LogType.Log))
                {
                    log.Debug("ObjSwitch {0}: Duplicate ignored ( {1} )", name, param.Join(","));
                }
                return;
            }
            ResetSwitch();
            AddSwitch(param);
            Apply();
        }

        public bool Is(object o)
        {
            return keySet.Contains(Normalize(o));
        }

        private string Normalize(object o)
        {
            return o.ToString().ToLower();
        }

        public List<string> GetAllKeys()
        {
            return switches.ConvertAll(s => s.name);
        }

        public void Apply()
        {
            var visible = new bool[objs.Count];
            int match = 0;
            foreach (UISwitchSect e in switches)
            {
                if (keySet.Contains(Normalize(e.name)))
                {
                    // groups objects to switch on and off
                    for (int i=0; i<e.visibility.Count; ++i)
                    {
                        visible[i] |= e.visibility[i];
                    }
                    match++;
                    // set positions
                    for (int i = 0; i < e.trans.Length; ++i)
                    {
                        e.trans[i].localPosition = e.pos[i];
                    }
                    try
                    {
                        e.action.Call();
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }

            for (int i=0; i<objs.Count; ++i)
            {
                objs[i].SetActive(visible[i]);
            }

            if (log.IsLoggable(LogType.Log))
            {
                log.Debug("ObjSwitch {0}: {1}", name, keySet.Join(","));
            }
            if (match != keySet.Count)
            {
                Assert.Fail(this, "Invalid param {0}", keySet.Join(","));
            }
        }

        public GameObject GetObject(object key, string name)
        {
            // Get Switch slot
            UISwitchSect slot = GetSwitchSlot(key);
            return slot.FindObject(objs, o => o.name.EqualsIgnoreCase(name));
        }

        public T GetComponent<T>(object key) where T: Component
        {
            // Get Switch slot
            UISwitchSect slot = GetSwitchSlot(key);
            for (int i=0; i<slot.visibility.Count; ++i)
            {
                if (slot.visibility[i])
                {
                    T c = objs[i].GetComponent<T>();
                    if (c != null)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        private UISwitchSect GetSwitchSlot(object key)
        {
            string id = Normalize(key);
            foreach (UISwitchSect e in switches)
            {
                if (e.name.EqualsIgnoreCase(id))
                {
                    return e;
                }
            }
            return DUMMY;
        }
    }

    [System.Serializable]
    public struct UISwitchSect : ICloneable
    {
        [EnumPopup("enumType")]public string name;
        public List<bool> visibility;
        public Transform[] trans;
        public Vector3[] pos;
        [NonSerialized] public Action action;

        public bool isValid
        {
            get
            {
                return !name.IsEmpty() && visibility != null;
            }
        }

        public object Clone()
        {
            UISwitchSect e = new UISwitchSect();
            e.name = this.name;
            e.visibility = new List<bool>(visibility);
            e.trans = (Transform[])trans.Clone();
            e.pos = (Vector3[])pos.Clone();
            return e;
        }

        public override string ToString()
        {
            return name;
        }

        internal GameObject FindObject(List<GameObject> objs, Func<GameObject, bool> predicate)
        {
            for (int i=0; i<visibility.Count; ++i)
            {
                if (visibility[i] && predicate(objs[i]))
                {
                    return objs[i];
                }
            }
            return null;
        }

        internal void ForEach(List<GameObject> objs, Action<GameObject> func)
        {
            for (int i = 0; i < visibility.Count; ++i)
            {
                if (visibility[i])
                {
                    func(objs[i]);
                }
            }
        }

        internal void RemoveAt(int i)
        {
            visibility.RemoveAt(i);
        }
    }

    [System.Serializable]
    public class UISwitchPreset : ICloneable
    {
        public string presetName;
        public string[] keys = new string[0];

        public object Clone()
        {
            UISwitchPreset p = new UISwitchPreset();
            p.keys = ArrayUtil.Clone(keys);
            return p;
        }
    }

    public static class ObjSwitchEx
    {
        public static void SetEx(this UISwitch s, params object[] param)
        {
            if (s == null)
            {
                return;
            }
            s.Set(param);
        }
    }
}