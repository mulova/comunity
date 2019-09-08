using UnityEngine;
using System.Collections.Generic;
using mulova.commons;
using System.Text.Ex;

namespace mulova.comunity
{
    [CreateAssetMenu(menuName="Scriptable Object/Lexicon Registry", fileName="Lexicon")]
    public class LexiconRegistry : ScriptableObject
    {
        public AssetGuid assetDir; // editor only
        public TextAsset initial;
        public AssetRef[] assets;

        private static readonly GamePref _pref = new GamePref("_lexreg", true);
        private const string LANG = "LANG";
        public static SystemLanguage lang
        { 
            get 
            {
                return _pref.GetEnum<SystemLanguage>(LANG, SystemLanguage.English);
            }
            set
            {
                _pref.SetEnum(LANG, value);
            }
        }

        public void LoadLexicons()
        {
            Lexicon.Clear();
            Lexicon.AddLexicon(initial.bytes);

            foreach (var a in assets)
            {
                Cdn.cache.GetBytes(a.path, bytes =>
                {
                    if (bytes != null)
                    {
                        Lexicon.AddLexicon(bytes);
                    }
                });
            }
            Lexicon.SetLanguage(GetLanguage());
        }

        // 1. previous language used is returned
        // 2. if it's the first run, system langauge is returned
        //    if system language is not listed in BuildConfig.LANGUAGE, use the first one in the BuildConfig.LANGUAGE
        public static SystemLanguage GetLanguage()
        {
            // 1. Stored language takes precedence.
            SystemLanguage l = LexiconRegistry.lang;
            if (IsValidLanguage(l.ToString()))
            {
                return l;
            } else if (!BuildConfig.LANGUAGE.IsEmpty())
            {
                // return first language in 'lang' value in platform_data.bytes
                return BuildConfig.LANGUAGE.SplitCSV()[0].ParseEnum<SystemLanguage>(SystemLanguage.English);
            }
            return l;

        }

        // check if valid language by "lang" in build_config.bytes
        private static HashSet<string> langs;
        public static bool IsValidLanguage(object langId)
        {
            if (langs == null)
            {
                langs = new HashSet<string>();
                string langStr = BuildConfig.LANGUAGE;
                if (!langStr.IsEmpty())
                {
                    langs.AddAll(langStr.SplitCSV());
                }
            }
            if (langs.IsEmpty())
            {
                return true;
            }
            return langs.Contains(langId.ToString());
        }
    }
}

