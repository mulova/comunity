//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;

namespace comunity
{
	
    public static class EncryptedPref
    {
        private static string privateKey = "9ETrEsWaFRach3gexaDr";
        // Add some values to this array before using EncryptedPlayerPrefs
        public static string[] keys;

        public static readonly GamePref pref = new GamePref("_gmprf", true);

        public static void SaveEncryption(string key, string type, string value)
        {
            int keyIndex = (int)Mathf.Floor(Random.value * keys.Length);
            string secretKey = keys[keyIndex];
            string check = MD5.Digest(type+"_"+privateKey+"_"+secretKey+"_"+value);
            pref.SetString(key+"_encryption_check", check);
            pref.SetInt(key+"_used_key", keyIndex);
        }

        public static bool CheckEncryption(string key, string type, string value)
        {
            int keyIndex = pref.GetInt(key+"_used_key", -1);
            if (keyIndex < 0)
            {
                return false;
            }
            string secretKey = keys[keyIndex];
            string check = MD5.Digest(type+"_"+privateKey+"_"+secretKey+"_"+value);
            if (!pref.HasKey(key+"_encryption_check"))
                return false;
            string storedCheck = pref.GetString(key+"_encryption_check", "");
            return storedCheck == check;
        }

        private const string LANG = "LANG";
        public static SystemLanguage lang
        { 
            get 
            {
                return pref.GetEnum<SystemLanguage>(LANG, SystemLanguage.English);
            }
            set
            {
                pref.SetEnum(LANG, value);
            }
        }
    }
}
