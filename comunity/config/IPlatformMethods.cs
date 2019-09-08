using System;

namespace mulova.comunity
{
	public interface IPlatformMethods
	{
		void SetNoBackupFlag (string path);
		void SetNoBackupFlag (string path, int version);
	}
}
