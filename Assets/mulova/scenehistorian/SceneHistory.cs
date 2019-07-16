using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using comunity;
using Object = UnityEngine.Object;
using System.IO;

namespace scenehistorian
{
	[System.Serializable]
	public class SceneHistory
	{
		[SerializeField] private List<SceneHistoryItem> _items = new List<SceneHistoryItem>();
		[SerializeField] internal int maxSize = 100;
        [SerializeField] internal bool sort;
		[SerializeField] internal bool useCam;
		private List<SceneHistoryItem> _sorted = new List<SceneHistoryItem>();

		private class SceneNameSorter : IComparer<SceneHistoryItem>
		{
			public int Compare(SceneHistoryItem x,SceneHistoryItem y)
			{
				if (x != null)
				{
					if (y != null)
					{
						return x.ToString().CompareTo(y.ToString());
					} else
					{
						return -1;
					}
				} else
				{
					return 1;
				}
			}
		}

		public List<SceneHistoryItem> items
		{
			get
			{
				if (sort)
				{
					_sorted.Clear();
					_sorted.AddRange(_items);
					_sorted.Sort(new SceneNameSorter());
					return _sorted;
				} else
				{
					return _items;
				}
			}
		}


		public int Count
		{
			get 
			{
				return items.Count;
			}
		}

		public SceneHistoryItem this[int i]
		{
			get
			{
				return items[i];
			}
		}

		public void Add(SceneHistoryItem item)
		{
			items.Add(item);
			Resize();
		}

		public void RemoveAt(int i)
		{
			items.RemoveAt(i);
		}

		public void Insert(int i, SceneHistoryItem item)
		{
			items.Insert(i, item);
			Resize();
		}

		public void Clear()
		{
			items.Clear();
		}

		private void Resize()
		{
			int i = Count-1;
			while (Count > maxSize && i >= 0)
			{
				if (!items[i].starred)
				{
					items.RemoveAt(i);
				}
				i--;
			}
		}

		public void Add(Object obj)
		{
			Add(new SceneHistoryItem(obj));
		}

		public int IndexOf(Object obj)
		{
			for (int i=0; i<Count; ++i)
			{
				if (this[i].list.Count > 0 &&  this[i].list[0].reference == obj)
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(string path)
		{
			for (int i=0; i<Count; ++i)
			{
				if (this[i].list.Count > 0 &&  this[i].list[0].path == path)
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, Object obj)
		{
			Insert(index, new SceneHistoryItem(obj));
		}

		public void Remove(Object obj)
		{
			int index = items.FindIndex(o => o.firstRef == obj);
			if (index >= 0)
			{
				items.RemoveAt(index);
			}
		}

		public void Remove(string guid)
		{
			int index = items.FindIndex(o => o.first != null && o.first.id == guid);
			if (index >= 0)
			{
				items.RemoveAt(index);
			}
		}

		public bool Contains(Object obj)
		{
			return IndexOf(obj) >= 0;
		}

        public bool Contains(string path)
        {
            return IndexOf(path) >= 0;
        }

        public static SceneHistory Load(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					var text = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(text))
					{
						return JsonUtility.FromJson<SceneHistory>(text);
					}
				}
			} catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			return new SceneHistory();
		}

		public void Save(string path)
		{
			try
			{
				var json = JsonUtility.ToJson(this);
				File.WriteAllText(path, json);
			} catch(Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
		}
	}
}
