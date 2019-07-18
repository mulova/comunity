using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using commons;
using System.Collections.Generic.Ex;
using System.Text.Ex;

namespace comunity
{
    public static class Lexicon
    {
        public static readonly LexPages pages = new LexPages();
        private static readonly Dictionary<string, string> altMap = new Dictionary<string, string>();
        // key: id,  value: lexicon_id
        private static readonly Dictionary<string, string> invAltMap = new Dictionary<string, string>();
        private static readonly HashSet<string> conflict = new HashSet<string>();
        public static readonly Loggerx log = LogManager.GetLogger(typeof(Lexicon));
        public delegate string GetTitleName(SystemLanguage lang);
        public static GetTitleName getTitle = t=> t.ToString();

        public static ICollection<string> GetColumnNames()
        {
            return pages.columnNames;
        }
        
        public static string[] GetAll(params string[] ids)
        {
            string[] texts = new string[ids.Length];
            for (int i=0; i<texts.Length; ++i)
            {
                texts[i] = Get(ids[i]);
            }
            return texts;
        }

        private static string Format(string msg, params object[] formatParam)
        {
            if (formatParam.IsEmpty()) {
                return msg;
            }
            if (msg.IsNotEmpty()) {
                try {
                    return string.Format (msg, formatParam);
                } catch (System.Exception fex) {
                    log.Error(fex, msg);
                    return msg;
                }
            } else {
                return string.Empty;
            }
        }
        
        public static string Get(string id, params object[] param)
        {
            string altId = altMap.Get(id);
            if (altId == null)
            {
                altId = id;
            }
            return Format(pages.GetAny(altId), param);
        }

        public static string GetValue(string id, object columnKey, params object[] param)
        {
            string altId = altMap.Get(id);
            if (altId == null)
            {
                altId = id;
            }
            return Format(pages.Get(altId, columnKey), param);
        }

        public static string[] GetRow(string id)
        {
            string altId = altMap.Get(id);
            if (altId == null)
            {
                altId = id;
            }
            return pages.GetRow(altId);
        }
        
        public static string Translate(string m, params object[] replace)
        {
            return Format(pages.Translate(m), replace);
        }
        
        public static string FindAltKey(string msg)
        {
            string key = pages.FindKey(msg);
            if (key.IsNotEmpty())
            {
                return invAltMap.Get(key, string.Empty);
            }
            return string.Empty;
        }
        
        private static SystemLanguage lang;
        
        public static void SetLanguage(SystemLanguage l)
        {
            lang = l;
            pages.SetLanguage(getTitle(l));
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(l.GetCode());
        }
        
        public static void SetMotherLanguage(SystemLanguage l)
        {
            pages.SetMotherLanguage(getTitle(l));
        }
        
        public static SystemLanguage GetLanguage()
        {
            return lang;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns><c>true</c>, if key or alt key exists, <c>false</c> otherwise.</returns>
        public static bool ContainsKey(string key)
        {
            return pages.ContainsKey(key)||altMap.ContainsKey(key);
        }
        
        public static void Clear()
        {
            pages.Clear();
            altMap.Clear();
            invAltMap.Clear();
            conflict.Clear();
        }
        
        public static void AddLexicon(byte[] data)
        {
            pages.Add(data);
        }
        
        public static void AddAltLexicon(byte[] b)
        {
            SpreadSheet xls = new SpreadSheet(b);
            while (xls.HasNextRow())
            {
                xls.NextRow();
                string key = xls.GetNextCellString();
                string value = xls.GetNextCellString();
                if (key.IsEmpty())
                {
                    continue;
                }
                invAltMap[value] = key;
                if (!conflict.Contains(key))
                {
                    if (altMap.ContainsKey(key))
                    {
                        conflict.Add(key);
                    } else
                    {
                        altMap[key] = value;
                    }
                }
            }
            if (log.IsLoggable(LogLevel.INFO))
            {
                log.Info("Duplicate Message Key: {0}", conflict.Join(","));
            }
        }
        
    }
}

