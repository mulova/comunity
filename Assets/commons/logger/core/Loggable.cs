using System;

namespace commons
{
    public class Loggable
    {
        private Loggerx _log;

        public Loggerx log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                    _log.context = this;
                }
                return _log;
            }
            set
            {
                _log = value;
                _log.context = this;
            }
        }

        public Type logType
        {
            set
            {
                log = LogManager.GetLogger(value);
            }
        }

        public string logName
        {
            set
            {
                log = LogManager.GetLogger(value);
            }
        }
    }
}

