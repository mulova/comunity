//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using mulova.commons;

namespace comunity
{
    public class LangSwitch : MonoBehaviour {
        private static SystemLanguage lang;
        public LangObj[] obj;
        public static SystemLanguage defaultLang = SystemLanguage.English;
        
        public static void SetLanguage(SystemLanguage lang) {
            LangSwitch.lang = lang;
        }
        
        void Start() {
            if (obj == null || obj.Length == 0) {
                return;
            }
            GameObject chosen = null;
            GameObject def = null;
            foreach (LangObj o in obj) {
                if (o.lang == lang) {
                    Assert.IsNull(chosen);
                    chosen = o.obj;
                } else {
                    o.obj.SetActive(false);
                }
                if (o.lang == defaultLang) {
                    def = o.obj;
                }
            }
            if (def == null) {
                def = obj[0].obj;
            }
            // set active later because of the duplicate handling
            if (chosen != null) {
                chosen.SetActive(true);
            } else {
                def.SetActive(true);
            }
        }
    }
}

			
[System.Serializable]
public class LangObj : ICloneable {
	public SystemLanguage lang;
	public GameObject obj;
	
	public LangObj() {}
	
	public LangObj(SystemLanguage lang, GameObject obj) {
		this.lang = lang;
		this.obj = obj;
	}
	
	public object Clone()
	{
		return new LangObj(lang, obj);
	}
}