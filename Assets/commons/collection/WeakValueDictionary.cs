//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Generic.Ex;

namespace commons
{
	public class WeakValueDictionary<K, V> : IDictionary<K, V> where V : class {
		private AssetRefType refType;
		private readonly Dictionary<K, Reference<V>> dict;
		
		public WeakValueDictionary(IEqualityComparer<K> comp) {
			dict = new Dictionary<K, Reference<V>>(comp);
		}
		
		public WeakValueDictionary() {
			dict = new Dictionary<K, Reference<V>>();
		}
		
		
		#region IDictionary<T,U> Members
		
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
			return GetDictionary().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator() {
			return GetDictionary().GetEnumerator();
		}
		
		private Dictionary<K, V> GetDictionary()
		{
			Dictionary<K, V> dic = new Dictionary<K, V>();
			foreach (KeyValuePair<K, Reference<V>> p in dict)
			{
				if (p.Value.IsAlive)
				{
					dic[p.Key] = p.Value.Target;
				}
			}
			return dic;
		}
		
		public void Add(KeyValuePair<K, V> item) {
			Add(item.Key, item.Value);
		}
		
		public void Clear() {
			dict.Clear();
		}
		
		bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item) {
			return dict.Contains(WeakPairFrom(item));
		}
		
		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}
		
		public bool Remove(KeyValuePair<K, V> item) {
			return ((ICollection<KeyValuePair<K, Reference<V>>>) dict).Remove(WeakPairFrom(item));
		}
		
		public int Count {
			get { return dict.Count; }
		}
		
		public bool IsReadOnly {
			get { return false; }
		}
		
		public bool ContainsKey(K key) {
			return dict.ContainsKey(key);
		}
		
		public void CleanDeadReferences() {
			foreach( var key in dict.Keys){
				if (!dict[key].IsAlive)
					dict.Remove(key);
			}
		}
		
		public void Add(K key, V value) {
			Reference<V> r = new Reference<V>(value);
			r.SetRefType(refType);
			dict.Add(key, r);
		}
		
		public bool Remove(K key) {
			var result = dict.Remove(key);
			return result;
		}
		
		public bool TryGetValue(K key, out V value) {
			Reference<V> weakValue;
			if (dict.TryGetValue(key, out weakValue)) {
				if (weakValue != null && weakValue.IsAlive) {
					value = weakValue.Target;
				} else {
					value = null;
				}
				return true;
			}
			value = null;
			return false;
		}
		
		public V this[K key] {
			get {
				Reference<V> r = dict.Get(key);
				if (r == null) {
					return null;
				}
				return r.Target;
			}
			set {
				Remove(key);
				Add(key, value);
			}
		}
		
		public ICollection<K> Keys {
			get { return dict.Keys; }
		}
		
		public ICollection<V> Values {
			get { return GetDictionary().Values; }
		}
		
		#endregion
		
		public V Get(K key) {
			return Get(key, default(V));
		}
		
		public V Get(K key, V defaultVal) {
			V val = defaultVal;
			if (TryGetValue(key, out val)) {
				return val;
			}
			return defaultVal;
		}
		
		private static KeyValuePair<K, Reference<V>> WeakPairFrom(KeyValuePair<K, V> item) {
			return new KeyValuePair<K, Reference<V>>(item.Key, new Reference<V>(item.Value));
		}
		
		private IEnumerable<Reference<V>> GetStrongReferences() {
			return dict.Values.Where(x => x!=null&&x.IsStrong);
		}
		
		private IEnumerable<Reference<V>> GetWeakReferences() {
			foreach (KeyValuePair<K, Reference<V>> pair in dict)
			{
				//			x!=null&&x.IsWeak
				if (pair.Value != null && pair.Value.IsWeak)
				{
					yield return pair.Value;
				}
			}
		}
		
		public void SetRefType(AssetRefType refType) {
			this.refType = refType;
			dict.Values.ForEach((x) => x.SetRefType(refType));
		}
	}
}

	

public class Reference<T> where T : class {
	private readonly WeakReference weakReference;
	private T target;
	
	public Reference(T target) {
		weakReference = new WeakReference(target);
	}
	
	public bool IsStrong {
		get { return !IsWeak; }
	}
	
	public T Target {
		get {
			if (target != null) {
				return target;
			}
			return weakReference.Target as T;
		}
	}
	
	public bool IsAlive {
		get { return weakReference.IsAlive; }
	}
	
	public bool IsWeak { get { return target == null; } }
	
	public void SetRefType(AssetRefType refType) {
		if (refType == AssetRefType.Strong) {
			target = weakReference.Target as T;
		} else {
			target = null;
		}
	}
}

public enum AssetRefType {
	Strong, Weak
}