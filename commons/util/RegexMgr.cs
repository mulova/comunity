using System.Text.RegularExpressions;

namespace commons
{
    public class RegexMgr
    {
        private Regex regex;
        private string pattern;
        
        public Regex exp
        {
            get
            {
                if (pattern.IsEmpty())
                {
                    return null;
                }
                if (regex == null)
                {
                    regex = new Regex(pattern);
                }
                return regex;
            }
        }
        
        public bool IsMatch(string str)
        {
            return exp == null || exp.IsMatch(str);
        }
        
        public void SetPattern(string regexPattern)
        {
            Reset();
            AddPattern(regexPattern);
        }
        
        public void AddPattern(string regexPattern)
        {
            if (pattern.IsEmpty())
            {
                pattern = regexPattern;
            } else
            {
                pattern = string.Concat(pattern, "|", regexPattern);
            }
            regex = null;
        }
        
        public void Reset()
        {
            regex = null;
            pattern = null;
        }
    }
}
