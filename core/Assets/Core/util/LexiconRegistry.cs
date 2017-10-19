using System;
using UnityEngine;
using System.Collections.Generic;

namespace core
{
    [CreateAssetMenu(menuName="Scriptable Object/Lexicon Registry", fileName="Lexicon")]
    public class LexiconRegistry : ScriptableObject
    {
        public AssetGuid assetDir; // editor only
        public TextAsset initial;
        public AssetRef[] assets;

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
            SystemLanguage lang = Game.lang;
            if (IsValidLanguage(lang.ToString()))
            {
                return lang;
            } else if (BuildConfig.LANGUAGE.IsNotEmpty())
            {
                // return first language in 'lang' value in platform_data.bytes
                return BuildConfig.LANGUAGE.SplitCSV()[0].ParseEnum<SystemLanguage>(SystemLanguage.English);
            }
            return lang;

        }

        // check if valid language by "lang" in build_config.bytes
        private static HashSet<string> langs;
        public static bool IsValidLanguage(object langId)
        {
            if (langs == null)
            {
                langs = new HashSet<string>();
                string langStr = BuildConfig.LANGUAGE;
                if (langStr.IsNotEmpty())
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

