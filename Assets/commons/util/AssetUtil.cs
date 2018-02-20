using System.IO;
using System.Text;
using System;

namespace commons
{
    public static class AssetUtil
    {
        /**
         * @wildcard filtering정보. null이면 모두를 반환한다. 예) *.fbx
         */
        public static FileInfo[] ListFiles(string absolutePath, string wildcard = null)
        {
            if (Directory.Exists(absolutePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(absolutePath);
                
                if (wildcard != null)
                {
                    return dirInfo.GetFiles(wildcard, SearchOption.AllDirectories);
                } else
                {
                    return dirInfo.GetFiles("*", SearchOption.AllDirectories);
                }
            } else
            {
                return new FileInfo[] { new FileInfo(absolutePath) };
            }
        }

        public static string[] GetDirectories(string rootDir)
        {
            if (Directory.Exists(rootDir))
            {
                return Directory.GetDirectories(rootDir);
            } else
            {
                return new string[0];
            }
        }

        public static void DeleteDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            foreach (FileInfo f in dir.GetFiles())
            {
                f.Delete(); 
            }
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                d.Delete(true); 
            }
            Directory.Delete(dirPath);
        }

    }
    
}
