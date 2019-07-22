//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;
using commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;
using System.Ex;

namespace comunity
{
    public class ObjSwitch : MonoBehaviour
    {
        [SerializeField] public ObjSwitchElement[] switches = new ObjSwitchElement[0];
        [SerializeField] public string enumType;
        [SerializeField] public ObjSwitchPreset[] preset;
        public bool overwrite = false;
        private ObjSwitchElement DUMMY = new ObjSwitchElement();
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
            ObjSwitchElement s = switches.Find(e => e.name.EqualsIgnoreCase(k));
            if (s != null)
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

        public void Apply()
        {
            HashSet<GameObject> on = new HashSet<GameObject>();
            HashSet<GameObject> off = new HashSet<GameObject>();
		
            int match = 0;
            foreach (ObjSwitchElement e in switches)
            {
                if (keySet.Contains(Normalize(e.name)))
                {
                    // groups objects to switch on and off
                    on.AddAll<GameObject>(e.objs);
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
                } else
                {
                    off.AddAll<GameObject>(e.objs);
                }
            }

            off.RemoveAll(on);
		
            foreach (GameObject o in on)
            {
                if (o != null)
                {
                    o.SetActive(true);
                }
            }
            foreach (GameObject o in off)
            {
                if (o != null)
                {
                    o.SetActive(false);
                }
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
            ObjSwitchElement slot = GetSwitchSlot(key);
            foreach (GameObject o in slot.objs)
            {
                if (o.name.EqualsIgnoreCase(name))
                {
                    return o;
                }
            }
            return null;
        }

        public T GetComponent<T>(object key) where T: Component
        {
            // Get Switch slot
            ObjSwitchElement slot = GetSwitchSlot(key);
            foreach (GameObject o in slot.objs)
            {
                T c = o.GetComponent<T>();
                if (c != null)
                {
                    return c;
                }
            }
            return null;
        }

        private ObjSwitchElement GetSwitchSlot(object key)
        {
            string id = Normalize(key);
            foreach (ObjSwitchElement e in switches)
            {
                if (e.name.EqualsIgnoreCase(id))
                {
                    return e;
                }
            }
            return DUMMY;
        }
    }

    [System.Serializable, Constructor]
    public class ObjSwitchElement : ICloneable
    {
        public string name = string.Empty;
        public GameObject[] objs = new GameObject[0];
        public Transform[] trans = new Transform[0];
        public Vector3[] pos = new Vector3[0];
        [NonSerialized] public Action action;

        public object Clone()
        {
            ObjSwitchElement e = new ObjSwitchElement();
            e.name = this.name;
            e.objs = (GameObject[])objs.Clone();
            e.trans = (Transform[])trans.Clone();
            e.pos = (Vector3[])pos.Clone();
            return e;
        }

        public override string ToString()
        {
            return name;
        }
    }

    [System.Serializable, ConstructorAttribute]
    public class ObjSwitchPreset : ICloneable
    {
        public string presetName;
        public string[] keys = new string[0];

        public object Clone()
        {
            ObjSwitchPreset p = new ObjSwitchPreset();
            p.keys = ArrayUtil.Clone(keys);
            return p;
        }
    }

    public static class ObjSwitchEx
    {
        public static void SetEx(this ObjSwitch s, params object[] param)
        {
            if (s == null)
            {
                return;
            }
            s.Set(param);
        }
    }
}