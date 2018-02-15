using System;

namespace commons
{
    [System.Serializable]
    public class LoggerData : ICloneable
    {
        public string name;
        public LogLevel level = LogLevel.WARN;
        public int fontSize;
        public bool bold;
        public LogColor color;

        public LoggerData()
        {
        }

        public LoggerData(string name, LogLevel level)
        {
            this.name = name;
            this.level = level;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", name, level);
        }
    }
}