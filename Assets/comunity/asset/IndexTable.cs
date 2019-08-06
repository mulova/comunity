using System.Collections.Generic;
using System.Collections;
using System;
using mulova.commons;
using System.Collections.Generic.Ex;

namespace comunity
{
    public abstract class IndexTable<R> : Loggable, IEnumerable<KeyValuePair<string, R>> where R:class, new()
    {
        public delegate void LoadDelegate(string path, Action<byte[]> callback);

        public string path { get; private set; }
        protected abstract void ProcessRow(int rowNo, R r);
        protected abstract string GetKey(R row);
        public LoadDelegate loader;

        public R this[string key]
        {
            get
            {
                return GetRow(key);
            }
        }

        private Dictionary<string, R> _indexer;

        protected Dictionary<string, R> indexer
        {
            get
            {
                Load();
                return _indexer;
            }
        }

        private List<R> _rows;

        public List<R> rows
        {
            get
            {
                Load();
                return _rows;
            }
        }

        public bool trimSpace;
        public bool allowNullRow;

        protected bool initialized { get { return _rows != null; } }

        public IndexTable(string path)
        {
            this.path = path;
            this.loader = LoadBytes;
        }

        private void Load()
        {
            if (_rows != null)
            {
                return;
            }
            loader(path, b =>
            {
                if (b != null)
                {
                    SpreadSheet ss = new SpreadSheet(b);
                    ss.allowNullRow = allowNullRow;
                    ss.trimSpace = trimSpace;
                    if (_rows == null)
                    {
                        _rows = ss.GetRows<R>(1);
                    } else
                    {
                        _rows.AddRange(ss.GetRows<R>(1));
                    }
                    _indexer = _rows.ToDictionary(GetKey);
                    for (int i = 0; i < _rows.Count; ++i)
                    {
                        ProcessRow(i+1, _rows[i]);
                    }
                } else
                {
                    log.Warn("Can't access {0}", path);
                }
            });
        }

        public void Reload()
        {
            _indexer = null;
            _rows = null;
            Load();
        }

        protected virtual void LoadBytes(string path, Action<byte[]> callback)
        {
            Cdn.cache.GetBytes(path, callback);
        }

        private Dictionary<string, R> icIndexer;

        public R GetRowIgnoreCase(string id)
        {
            if (icIndexer == null)
            {
                icIndexer = new Dictionary<string, R>();
                foreach (KeyValuePair<string, R> pair in indexer)
                {
                    icIndexer[pair.Key.ToLower()] = pair.Value;
                }
            }
            return icIndexer.Get(id.ToLower());
        }

        public R GetRow(string id)
        {
            if (indexer != null)
            {
                return indexer.Get(id);
            } else
            {
                return null;
            }
        }

        public IEnumerable<string> keys
        {
            get
            {
                if (indexer != null)
                {
                    return indexer.Keys;
                } else
                {
                    return null;
                }
            }
        }

        IEnumerator<KeyValuePair<string, R>> IEnumerable<KeyValuePair<string, R>>.GetEnumerator()
        {
            if (indexer != null)
            {
                return indexer.GetEnumerator();
            } else
            {
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (indexer != null)
            {
                return indexer.GetEnumerator();
            } else
            {
                
                return null;
            }
        }

        public override string ToString()
        {
            return PathUtil.GetFileNameWithoutExt(path);
        }
    }
}
