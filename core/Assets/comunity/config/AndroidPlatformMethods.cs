#if UNITY_ANDROID
namespace core
{
	public class AndroidPlatformMethods : IPlatformMethods
	{
		public void SetNoBackupFlag(string path)
		{
		}

		public void SetNoBackupFlag(string path, int version)
		{
		}
	}
}
#endif
