using System;
using Object = UnityEngine.Object;
using commons;

namespace core
{
    public class FileTypeFilter
    {
        private FileType fileType;
        
        public FileTypeFilter(FileType type) {
            SetFileType(type);
        }
        
        public void SetFileType(FileType type) {
            this.fileType = type;
        }
        
        public bool Filter(object o) {
            string str = ObjToString.ScenePathToString(o);
            return str.Is(fileType);
        }
        
        public bool Filter<T>(T o) {
            string str = ObjToString.ScenePathToString(o);
            return str.Is(fileType);
        }
    }
}

