//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
/**
Usage

Using the deque is simple. Just add and removes items from either end.
Or you can access items by index.
Deque<int> deque = new Deque<int>();
deque.AddToTail(1);
deque.AddToTail(2);
deque.AddToTail(3);
deque.AddToTail(4);
deque.AddToTail(5);
deque.AddToTail(6);
deque.AddToTail(7);
int index3 = deque[3]; // return the item at index 3 in the deque.
(i.e "4")
int last = deque.RemoveFromTail(); //remove and return the item at the
tail of list. (i.e "7")
int first = deque.RemoveFromHead(); //remove and return the item at
the head of the list (i.e. "1")
//deque will now contain 2,3,4,5,6



	 */
	/// <summary>
	/// A double-ended queue. Provides fast insertion and removal from the head or tail end,
	/// and fast indexed access.
	/// </summary>
	/// <typeparam name="T">The type of item to store in the deque.</typeparam>
	public class Deque<T> : ICollection<T>
	{
		//Constants
		private const int MIN_CAPACITY = 4;
		//Fields
		private int _capacity = MIN_CAPACITY;
		private int _head = 0;
		private int _tail = 0;
		private int _count;
		private T[] _data;
		/// <summary>
		/// Gets the number of items in the deque.
		/// </summary>
		public int Count {
			get { return _count; }
		}
		/// <summary>
		/// Gets or sets the capacity of the deque. If the count exceeds the
		/// capacity, the capacity will be automatically increased.
		/// </summary>
		public int Capacity {
			get { return _capacity; }
			set {
				int previousCapacity = _capacity;
				_capacity = Math.Max (value, Math.Max (_count,
					MIN_CAPACITY));
				T[] temp = new T[_capacity];
				if (_tail > _head) {
					Array.Copy (_data, _head, temp, 0, _tail + 1 -
						_head);
					_tail -= _head;
					_head = 0;
				} else {
					Array.Copy (_data, 0, temp, 0, _tail + 1);
					int length = previousCapacity - _head;
					Array.Copy (_data, _head, temp, _capacity - length,
						length);
					_head = _capacity - length;
				}
				_data = temp;
			}
		}
		/// <summary>
		/// Creates a new deque.
		/// </summary>
		public Deque ()
		{
			_data = new T[_capacity];
		}
		/// <summary>
		/// Creates a new deque.
		/// </summary>
		/// <param name="capacity">The initial capacity to give the deque.</param>
		public Deque (int capacity)
		{
			_capacity = capacity;
			_data = new T[_capacity];
		}
		/// <summary>
		/// Creates a new deque from a collection.
		/// </summary>
		/// <param name="collection">A collection of items of type T.</ param>
		public Deque (ICollection<T> collection)
		{
			Capacity = collection.Count;
			foreach (T item in collection) {
				this.Add (item);
			}
		}
		//Increments (and wraps if necessary) an index
		private int Increment (int index)
		{
			return (index + 1) % _capacity;
		}
		//Decrements (and wraps if necessary) an index
		private int Decrement (int index)
		{
			return (index + _capacity - 1) % _capacity;
		}
		/// <summary>
		/// Adds an item to the tail end of the deque.
		/// </summary>
		/// <param name="item">The item to be added.</param>
		public void AddToTail (T item)
		{
			_count++;
			if (_count > _capacity)
				Capacity *= 2;
			if (_count > 1)
				_tail = Increment (_tail);
			_data [_tail] = item;
		}
		/// <summary>
		/// Removes an item from the tail of the deque.
		/// </summary>
		/// <returns>An item of type T.</returns>
		public T RemoveFromTail ()
		{
			_count--;
			if (_count < 0) {
				throw new
					InvalidOperationException ("DequeEmptyException");
			}
			T item = _data [_tail];
			_data [_tail] = default(T);
			_tail = Decrement (_tail);
			return item;
		}
		/// <summary>
		/// Adds an item to the head of the deque.
		/// </summary>
		/// <param name="item">The item to be added.</param>
		public void AddToHead (T item)
		{
			_count++;
			if (_count > _capacity)
				Capacity *= 2;
			if (_count > 1)
				_head = Decrement (_head);
			_data [_head] = item;
		}
		/// <summary>
		/// Removes an item from the head of the deque.
		/// </summary>
		/// <returns>An item of type T.</returns>
		public T RemoveFromHead ()
		{
			_count--;
			if (_count < 0) {
				throw new
					InvalidOperationException ("DequeEmptyException");
			}
			T item = _data [_head];
			_data [_head] = default(T);
			_head = Increment (_head);
			return item;
		}
		/// <summary>
		/// Gets the item at the head of the deque.
		/// </summary>
		/// <returns>An item of type T.</returns>
		public T PeekHead ()
		{
			return _data [_head];
		}
		/// <summary>
		/// Gets the item at the tail of the deque.
		/// </summary>
		/// <returns>An item of type T.</returns>
		public T PeekTail ()
		{
			return _data [_tail];
		}
		/// <summary>
		/// Gets the item at the specified position.
		/// </summary>
		/// <param name="position">The position of the item to return.</param>
		/// <returns>An item of type T.</returns>
		public T this [int position] {
			get {
				if (position >= _count)
					throw new
						ArgumentOutOfRangeException ("position");
				return _data [(_head + position) % _capacity];
			}
		}
		/// <summary>
		/// Creates an array of the items in the deque.
		/// </summary>
		/// <returns>An array of type T.</returns>
		public T[] ToArray ()
		{
			T[] array = new T[_count];
			CopyTo (array, 0);
			return array;
		}
		/// <summary>
		/// Copies the deque to an array at the specified index.
		/// </summary>
		/// <param name="array">One dimensional array that is the destination of the copied elements.</param>
		/// <param name="arrayIndex">The zero-based index at which copying begins.</param>
		public void CopyTo (T[] array, int arrayIndex)
		{
			if (_count == 0)
				return;
			if (_head < _tail) {
				Array.Copy (_data, _head, array, arrayIndex, _tail + 1
					- _head);
			} else {
				int headLength = _capacity - _head;
				Array.Copy (_data, _head, array, arrayIndex,
					headLength);
				Array.Copy (_data, 0, array, arrayIndex + headLength,
					_tail + 1);
			}
		}
		/// <summary>
		/// Gets and removes an item at the specified index.
		/// </summary>
		/// <param name="index">The index at which to remove the item.</param>
		/// <returns>An item of type T.</returns>
		public T RemoveAt (int index)
		{
			_count--;
			if (index >= _count)
				throw new
					ArgumentOutOfRangeException ("index");
			int i = (_head + index) % _capacity;
			T item = _data [i];
			if (i < _head) {
				Array.Copy (_data, i + 1, _data, i, _tail - i);
				_data [_tail] = default(T);
				_tail = Decrement (_tail);
			} else {
				Array.Copy (_data, _head, _data, _head + 1, i - _head);
				_data [_head] = default(T);
				_head = Increment (_head);
			}
			return item;
		}
		/// <summary>
		/// Clears all items from the deque.
		/// </summary>
		public void Clear ()
		{
			Array.Clear (_data, 0, _capacity);
			_head = 0;
			_tail = 0;
			_count = 0;
		}
		/// <summary>
		/// Gets an enumerator for the deque.
		/// </summary>
		/// <returns>An IEnumerator of type T.</returns>
		public System.Collections.Generic.IEnumerator<T>
			GetEnumerator ()
		{
			int i=_head-1;
			while (i!=_tail) {
				i = (i+1)%_capacity;
				yield return _data[i];
			}
		}
		
		System.Collections.IEnumerator
			System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		public System.Collections.Generic.IEnumerator<T>
			GetDescendingEnumerator ()
		{
			int i=_tail+1;
			while (i!=_head) {
				i = (i+_capacity-1)%_capacity;
				yield return _data[i];
			}
		}
		
		/// <summary>
		/// Adds an item to the tail of the deque.
		/// </summary>
		/// <param name="item"></param>
		public void Add (T item)
		{
			AddToTail (item);
		}
		/// <summary>
		/// Checks to see if the deque contains the specified item.
		/// </summary>
		/// <param name="item">The item to search the deque for.</ param>
		/// <returns>A boolean, true if deque contains item.</returns>
		public bool Contains (T item)
		{
			for (int i = 0; i < this.Count; i++) {
				if (this [i].Equals (item)) {
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Gets whether or not the deque is readonly.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}
		
		/// <summary>
		/// Removes an item from the deque.
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <returns>Boolean true if the item was removed.</returns>
		public bool Remove (T item)
		{
			for (int i = 0; i < this.Count; i++) {
				if (this [i].Equals (item)) {
					RemoveAt (i);
					return true;
				}
			}
			return false;
		}
	}
	
}