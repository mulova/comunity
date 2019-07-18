using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Generic.Ex;

namespace commons
{
    public class TextReplacer {
        private Regex regex;
        
        private IDictionary<string,string> map = new Dictionary<string,string>();
        private List<string> sources = new List<string>();
        public static readonly Loggerx log = LogManager.GetLogger(typeof(TextReplacer));
        
        /// <summary>
        /// Adds the replace token.
        /// </summary>
        /// <param name="exp">source regular expression. \\ is not supported</param>
        /// <param name="dst">replaced string</param>
        public void AddReplaceToken(string exp, string dst) {
            regex = null;
            sources.Add(exp);
            map.Add(exp.Replace(@"\", ""), dst);
        }
        
        public void AddReplaceString(string src, string dst) {
            regex = null;
            sources.Add(src.Replace(@"\", @"\\"));
            map.Add(src, dst);
        }
        
        /// <summary>
        /// Adds the replace exp.
        /// </summary>
        /// <param name="exp">regular expression</param>
        /// <param name="src">Source string</param>
        /// <param name="dst">Replaced string</param>
        public void AddReplaceExp(string exp, string src, string dst) {
            regex = null;
            sources.Add(exp);
            map.Add(src, dst);
        }
        
        public virtual string Replace(string str) {
            if (regex == null) {
                regex = new Regex(string.Join("|", sources.ToArray()));
            }
            return regex.Replace(str, MatchExp);
        }
        
        /// <summary>
        /// Replace token
        /// </summary>
        /// <param name="m">M.</param>
        protected virtual string MatchExp(Match m) {
            string replace = map.Get(m.Value);
            if (replace == null)
            {
                log.Error("No symbol '{0}'", m.Value);
                return m.Value;
            }
            return replace;
        }
    }
}