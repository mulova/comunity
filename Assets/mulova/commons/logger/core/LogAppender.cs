namespace commons
{
    public interface LogAppender
    {
        void Write(Loggerx logger, LogLevel level, object message);

        void Cleanup();
    }
}
