//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;

/**
* Draw Enum Popup UI if the enum class source is set correctly
*/
namespace comunity
{
    public sealed class EnumParser
    {
        private static readonly string REGEX = @".*enum \w*\s*\{\s*(?:(?:(\w*)(?:\s*=\s*\d+)?)\s*\,(?:\s*|//.*|/\*.*\*/)*)*(?:(?:(\w*)(?:\s*=\s*\d+)?))?(?:\s*|//.*|/\*.*\*/)*\}";
        private Regex regex = new Regex(REGEX); 
        private string[] enums;
        public readonly Type enumType;
        
        public EnumParser() {
        }
        
        public EnumParser(MonoScript script) {
            if (script != null) {
                enums = Parse(script);
            }
            this.enumType = null;
        }
        
        public EnumParser(Type enumType) {
            if (enumType != null && enumType.IsEnum) {
                enums = System.Enum.GetNames(enumType);
            }
            this.enumType = enumType;
        }
        
        public string[] Parse(MonoScript script) {
            string src = script.ToString();
            Match m = regex.Match(src);
            List<string> list = new List<string>();
            for (int i=1; i<m.Groups.Count; i++) {
                Group g = m.Groups[i];
                foreach (Capture c in g.Captures) {
                    list.Add(c.Value);
                }
            }
            return list.ToArray();
        }
        
        public string[] GetEnums() {
            return enums;
        }
        
        /**
        * Draw Enum Popup UI if the enum class source is set correctly
        */
        public bool OnInspectorGUI(ref string str, params GUILayoutOption[] options) {
            string val = str;
            bool changed = false;
            if (enums == null || enums.Length == 0) {
                changed |= EditorGUIUtil.TextField(null, ref val, options);
            } else {
                changed |= EditorGUIUtil.PopupNullable<string>(null, ref val, enums, options);
            }
            str = val;
            return changed;
        }
    }
}

