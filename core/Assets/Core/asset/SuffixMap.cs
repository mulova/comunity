#if FULL
using System.Collections.Generic;
using math.ex;
using UnityEngine;
using Object = UnityEngine.Object;


namespace core {
	/// <summary>
	/// 동일한 이름의 resource이면서 _01, _02 같은 suffix가 붙어있는 resource의 list를 관리한다.
	/// Groups resources by suffix removed names.
	/// A resource name is given by by .ToString().
	/// </summary>
	public class SuffixMap<T> {
		private Dictionary<string, List<T>> map = new Dictionary<string, List<T>>();
		private List<string> suffixes = new List<string>();
		private string separator;
		private RandomGenerator rand;
		
		public List<T> this[string key] {
			get { return GetSlot(key); }
		}
		
		public SuffixMap(params T[] arr) {
			foreach (T item in arr) {
				if (item != null) {
					Add (item);
				}
			}
		}
		
		/**
		 * underscore 이외의 구분자
		 */
		public void SetSeparator(string separator) {
			this.separator = separator;
		}
		
		/**
		 * 숫자 이외의 Suffix
		 */
		public void AddSuffix(params string[] suffix) {
			foreach (string s in suffix) {
				suffixes.Add(s);
			}
		}
		
		private string GetName(T item) {
			if (item is Object) {
				Object obj = item as Object;
				return obj.name;
			} else {
				return item.ToString();
			}
		}
		
		public void Add(params T[] items) {
			foreach (T item in items) {
				string name = GetName(item);
				Add(name, item);
				
				bool match = true;
				while (match) {
					match = false;
					if (separator != null) {
						string baseName = StringUtil.DetachSuffix(name, separator);
						if (name.Length != baseName.Length) {
							Add(baseName, item);
							name = baseName;
							match = true;
						}
					}
					
					if (!match) {
						string baseName = StringUtil.DetachSuffix(name);
						if (name.Length != baseName.Length) {
							Add(baseName, item);
							name = baseName;
							match = true;
						}
					}
					for (int i=0; i<suffixes.Count && !match; i++) {
						string baseName = StringUtil.DetachSpecifiedSuffix(name, suffixes[i]);
						if (name.Length != baseName.Length) {
							Add(baseName, item);
							name = baseName;
							match = true;
						}
					}
				}
			}
		}
		
		private void Add(string key, T item) {
			GetSlot(key).Add(item);
		}
		
		public void Clear() {
			map.Clear();
		}
		
		/**
		 * Animation name list와 suffix가 제거된 name list가 포함된다.
		 * 예를 들어 animation list가 { anim1_01, anim1_02, anim2 } 라면
		 * {anim1, anim2, anim1_01, anim1_02 } 을 반환한다.
		 */
		public string[] Keys {
			get {
				string[] names = new string[map.Keys.Count];
				map.Keys.CopyTo(names, 0);
				return names;
			}
		}
		
		public T[] Values {
			get {
				List<T> list = new List<T>();
				foreach (List<T> slot in map.Values) {
					list.AddRange(slot);
				}
				return list.ToArray();
			}
		}
		
		public bool ContainsKey(string key) {
			return map.ContainsKey(key);
		}
		
		public bool ContainsValue(T v) {
			List<T> slot = GetSlot(GetName(v));
			return slot.Contains(v);
		}
		
		/**
		 * key에 해당하는 모든 Item을 반환한다.
		 */
		public List<T> GetSlot(string key) {
			List<T> list = null;
			if (!map.TryGetValue(key, out list)) {
				list = new List<T>();
				map[key] = list;
			}
			return list;
		}
		
		/**
		 * Key에 해당하는 item중 임의로 하나를 반환한다.
		 */
		public T GetItem(string key) {
			List<T> list = GetSlot(key);
			if (list.Count == 0) {
				return default(T);
			} else if (list.Count == 1) {
				return list[0];
			}
			if (rand == null) {
				rand = new RandomGenerator(System.DateTime.Now.Ticks);
			}
			return list[rand.NextInt(key, 0, list.Count-1)];
		}
		
		/**
		 * key에 해당하는 clip중 임의의 하나의 clip이름을 반환한다.
		 * 없을 경우 null을 반환한다.
		 */
		public string GetItemName(string key) {
			T item = GetItem(key);
			if (item == null) {
				return null;
			}
			return GetName(item);
		}
		
		public void SetRandomGenerator(RandomGenerator rand) {
			this.rand = rand;
		}
	}
}
#endif