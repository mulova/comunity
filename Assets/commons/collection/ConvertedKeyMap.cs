using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;

namespace commons
{
	public class ConvertedKeyMap<K, V> : IDictionary<K, V>
	{
		private class Key<K2>  {
			public K2 src;
			public object conv;
			
			public Key(K2 src, object conv) {
				this.src = src;
				this.conv = conv;
			}
			
			public override bool Equals(object obj)
			{
				if (obj is Key<K>) {
					Key<K> that = obj as Key<K>;
					return this.conv.Equals(that.conv);
				}
				return false;
			}
			
			public override int GetHashCode()
			{
				return conv.GetHashCode();
			}
			
			public override string ToString()
			{
				return src.ToString();
			}
		}

		private Dictionary<Key<K>, V> map = new Dictionary<Key<K>, V>();
		private Func<K, object> converter;

		public ConvertedKeyMap(Func<K, object> converter) {
			this.converter = converter;
		}

		public void Add(K key, V value)
		{
			map.Add(KeyOf(key), value);
		}

		public bool ContainsKey(K key)
		{
			return map.ContainsKey(KeyOf(key));
		}

		public bool Remove(K key)
		{
			return map.Remove(KeyOf(key));
		}

		public bool TryGetValue(K key, out V value)
		{
			return map.TryGetValue(KeyOf(key), out value);
		}

		public V this[K key]
        {
            get
            {
                return map.Get(KeyOf(key));
            }
            set
            {
                map[KeyOf(key)] = value;
            }
        }

        public ICollection<K> Keys {
			get {
				List<K> keys = new List<K>(map.Count);
				foreach (Key<K> k in map.Keys) {
					keys.Add(k.src);
				}
				return keys;
			}
		}

		public ICollection<V> Values {
			get {
				return map.Values;
			}
		}

		public void Add(KeyValuePair<K, V> item)
		{
			map.Add(KeyOf(item.Key), item.Value);
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			return map.ContainsKey(KeyOf(item.Key));
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return map.Remove(KeyOf(item.Key));
		}

		public int Count {
			get {
				return map.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private Key<K> KeyOf(K key) {
			return new Key<K>(key, converter(key));
		}
	}
}

