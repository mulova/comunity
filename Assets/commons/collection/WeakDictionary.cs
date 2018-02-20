//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons
{

	/// <summary>
	/// A generic dictionary, which allows both its keys and values 
	/// to be garbage collected if there are no other references
	/// to them than from the dictionary itself.
	/// </summary>
	/// 
	/// <remarks>
	/// If either the key or value of a particular entry in the dictionary
	/// has been collected, then both the key and value become effectively
	/// unreachable. However, left-over WeakReference objects for the key
	/// and value will physically remain in the dictionary until
	/// RemoveCollectedEntries is called. This will lead to a discrepancy
	/// between the Count property and the number of iterations required
	/// to visit all of the elements of the dictionary using its
	/// enumerator or those of the Keys and Values collections. Similarly,
	/// CopyTo will copy fewer than Count elements in this situation.
	/// </remarks>
	public sealed class WeakDictionary<TKey, TValue> : BaseDictionary<TKey, TValue>
		where TKey : class
		where TValue : class {

		private Dictionary<object, WeakReference<TValue>> dictionary;
		private WeakKeyComparer<TKey> comparer;

		public WeakDictionary()
			: this(0, null) { }

		public WeakDictionary(int capacity)
			: this(capacity, null) { }

		public WeakDictionary(IEqualityComparer<TKey> comparer)
			: this(0, comparer) { }

		public WeakDictionary(int capacity, IEqualityComparer<TKey> comparer) {
			this.comparer = new WeakKeyComparer<TKey>(comparer);
			this.dictionary = new Dictionary<object, WeakReference<TValue>>(capacity, this.comparer);
		}

		// WARNING: The count returned here may include entries for which
		// either the key or value objects have already been garbage
		// collected. Call RemoveCollectedEntries to weed out collected
		// entries and update the count accordingly.
		public override int Count {
			get { return this.dictionary.Count; }
		}

		public override void Add(TKey key, TValue value) {
			if (key == null) throw new ArgumentNullException("key"); 
			WeakReference<TKey> weakKey = new WeakKeyReference<TKey>(key, this.comparer);
			WeakReference<TValue> weakValue = WeakReference<TValue>.Create(value);
			this.dictionary.Add(weakKey, weakValue);
		}

		public override bool ContainsKey(TKey key) {
			return this.dictionary.ContainsKey(key);
		}

		public override bool Remove(TKey key) {
			return this.dictionary.Remove(key);
		}

		public override bool TryGetValue(TKey key, out TValue value) {
			WeakReference<TValue> weakValue;
			if (this.dictionary.TryGetValue(key, out weakValue)) {
				value = weakValue.Target;
				return weakValue.IsAlive;
			}
			value = null;
			return false;
		}

		protected override void SetValue(TKey key, TValue value) {
			WeakReference<TKey> weakKey = new WeakKeyReference<TKey>(key, this.comparer);
			this.dictionary[weakKey] = WeakReference<TValue>.Create(value);
		}

		public override void Clear() {
			this.dictionary.Clear();
		}

		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			foreach (KeyValuePair<object, WeakReference<TValue>> kvp in this.dictionary) {
				WeakReference<TKey> weakKey = (WeakReference<TKey>)(kvp.Key);
				WeakReference<TValue> weakValue = kvp.Value;
				TKey key = weakKey.Target;
				TValue value = weakValue.Target;
				if (weakKey.IsAlive && weakValue.IsAlive)
					yield return new KeyValuePair<TKey, TValue>(key, value);
			}
		}

		// Removes the left-over weak references for entries in the dictionary
		// whose key or value has already been reclaimed by the garbage
		// collector. This will reduce the dictionary's Count by the number
		// of dead key-value pairs that were eliminated.
		public void RemoveCollectedEntries() {
			List<object> toRemove = null;
			foreach (KeyValuePair<object, WeakReference<TValue>> pair in this.dictionary) {
				WeakReference<TKey> weakKey = (WeakReference<TKey>)(pair.Key);
				WeakReference<TValue> weakValue = pair.Value;

				if (!weakKey.IsAlive || !weakValue.IsAlive) {
					if (toRemove == null)
						toRemove = new List<object>();
					toRemove.Add(weakKey);
				}
			}

			if (toRemove != null) {
				foreach (object key in toRemove)
					this.dictionary.Remove(key);
			}
		}
	}


	/// <summary>
	/// Represents a dictionary mapping keys to values.
	/// </summary>
	/// 
	/// <remarks>
	/// Provides the plumbing for the portions of IDictionary<TKey,
	/// TValue> which can reasonably be implemented without any
	/// dependency on the underlying representation of the dictionary.
	/// </remarks>
	public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
		private const string PREFIX = "System.Collections.Generic.Mscorlib_";
		private const string SUFFIX = ",mscorlib,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089";

		private KeyCollection keys;
		private ValueCollection values;

		protected BaseDictionary() { }

		public abstract int Count { get; }
		public abstract void Clear();
		public abstract void Add(TKey key, TValue value);
		public abstract bool ContainsKey(TKey key);
		public abstract bool Remove(TKey key);
		public abstract bool TryGetValue(TKey key, out TValue value);
		public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
		protected abstract void SetValue(TKey key, TValue value);

		public bool IsReadOnly {
			get { return false; }
		}

		public ICollection<TKey> Keys {
			get {
				if (this.keys == null)
					this.keys = new KeyCollection(this);

				return this.keys;
			}
		}

		public ICollection<TValue> Values {
			get {
				if (this.values == null)
					this.values = new ValueCollection(this);

				return this.values;
			}
		}

		public TValue this[TKey key] {
			get {
				TValue value;
				if (!this.TryGetValue(key, out value))
					throw new KeyNotFoundException();

				return value;
			}
			set {
				SetValue(key, value);
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item) {
			this.Add(item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {
			TValue value;
			if (!this.TryGetValue(item.Key, out value))
				return false;

			return EqualityComparer<TValue>.Default.Equals(value, item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			Copy(this, array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) {
			if (!this.Contains(item))
				return false;

			return this.Remove(item.Key);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		private abstract class Collection<T> : ICollection<T> {
			protected readonly IDictionary<TKey, TValue> dictionary;

			protected Collection(IDictionary<TKey, TValue> dictionary) {
				this.dictionary = dictionary;
			}

			public int Count {
				get { return this.dictionary.Count; }
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public void CopyTo(T[] array, int arrayIndex) {
				Copy(this, array, arrayIndex);
			}

			public virtual bool Contains(T item) {
				foreach (T element in this)
					if (EqualityComparer<T>.Default.Equals(element, item))
						return true;
				return false;
			}

			public IEnumerator<T> GetEnumerator() {
				foreach (KeyValuePair<TKey, TValue> pair in this.dictionary)
					yield return GetItem(pair);
			}

			protected abstract T GetItem(KeyValuePair<TKey, TValue> pair);

			public bool Remove(T item) {
				throw new NotSupportedException("Collection is read-only.");
			}

			public void Add(T item) {
				throw new NotSupportedException("Collection is read-only.");
			}

			public void Clear() {
				throw new NotSupportedException("Collection is read-only.");
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return this.GetEnumerator();
			}
		}

		private class KeyCollection : Collection<TKey> {
			public KeyCollection(IDictionary<TKey, TValue> dictionary)
				: base(dictionary) { }

			protected override TKey GetItem(KeyValuePair<TKey, TValue> pair) {
				return pair.Key;
			}
			public override bool Contains(TKey item) {
				return this.dictionary.ContainsKey(item);
			}
		}

		private class ValueCollection : Collection<TValue> {
			public ValueCollection(IDictionary<TKey, TValue> dictionary)
				: base(dictionary) { }

			protected override TValue GetItem(KeyValuePair<TKey, TValue> pair) {
				return pair.Value;
			}
		}

		private static void Copy<T>(ICollection<T> source, T[] array, int arrayIndex) {
			if (array == null)
				throw new ArgumentNullException("array");

			if (arrayIndex < 0 || arrayIndex > array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");

			if ((array.Length - arrayIndex) < source.Count)
				throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");

			foreach (T item in source)
				array[arrayIndex++] = item;
		}
	}
}
