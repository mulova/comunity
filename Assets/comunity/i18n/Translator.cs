//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace comunity {
	public class Translator
	{
		public static SystemLanguage lang;
		private Dictionary<SystemLanguage, string> title = new Dictionary<SystemLanguage, string>();
		public readonly object id;
		
		public Translator(object id) {
			this.id = id;
		}
		
		public string this[SystemLanguage lang] {
			get { return GetString(lang); }
			set { SetString(lang, value); }
		}
		
		public void SetString(SystemLanguage lang, string str) {
			title[lang] = str;
		}
		
		public string GetString(SystemLanguage lang) {
			return title[lang];
		}
		
		public void SetString(string str) {
			title[lang] = str;
		}
		
		public string GetString() {
			string str = string.Empty;
			
			if (!title.TryGetValue(lang, out str)) {
				title.TryGetValue(SystemLanguage.English, out str);
				//			if (Platform.isEditor) {
				//				str = string.Format("[No {0}]{1}", lang, str);
				//			}
			}
			return str;
		}
		
		public override string ToString()
		{
			return GetString();
		}
	}
}
