#if FULL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;

/// <summary>
/// Weak entry for the table
/// Similar to the ConditionalWeakTable in .Net Framework 4.5
/// </summary>
class WeakEntry 
{
	/// <summary>
	/// The weak reference to the collectable object
	/// </summary>
	public WeakReference weakReference;
	/// <summary>
	/// The associated object with the reference
	/// </summary>
	public object associate;
}

/// <summary>
/// if key is GC'ed, value is disposed.
/// </summary>
public class WeakKeyDictionary<T> : IDisposable where T : class
{
	
	//When finished we can dispose of the notification
	public void Dispose()
	{
		GCNotification.GCDone -= Collected;
	}
	
	/// <summary>
	/// The references held by the table
	/// </summary>
	Dictionary<int, List<WeakEntry>> references = new Dictionary<int, List<WeakEntry>>();

	private Func<T> factory;
	
	/// <summary>
	/// Gets an associated object give an index of another object
	/// </summary>
	/// <param name='index'>
	/// The object to use as an index
	/// </param>
	public T this[object key]
	{
		get
		{
			//Get the hash code of the indexed object
			int hash = key.GetHashCode();
			//Try to get a reference to it
			List<WeakEntry> entries;
			if(!references.TryGetValue(hash, out entries))
			{
				//If we failed then create a new entry
				references[hash] = entries = new List<WeakEntry>();
			}
			//Try to get an associated object of the right type for this
			//indexer/make sure it is still alive
			WeakEntry item = entries.FirstOrDefault(e=>e.weakReference.IsAlive && e.weakReference.Target == key);
			//Check if we got one
			if(item == null) 
			{
				//If we didn't then create a new one
				if (factory != null) {
					entries.Add(item = new WeakEntry { weakReference = new WeakReference(key), associate = factory() });
				} else {
					return null;
				}
			}
			//Return the associated object
			return (T)item.associate;
		}
	}

	public T0 Get<T0>(object key) where T0 : T, new()
	{
		return Get<T0>(key, ()=>{ return new T0(); });
	}
	
	/// <summary>
	/// Get an associate given an indexing object
	/// </summary>
	/// <param name='key'>
	/// The object to find the associate for
	/// </param>
	/// <typeparam name='T2'>
	/// The type of associate to find
	/// </typeparam>
	public T2 Get<T2>(object key, Func<object> t2Factory) where T2 : T
	{
		//Get the hash code of the indexing object
		int hash = key.GetHashCode();
		List<WeakEntry> entries;
		//See if we have a reference already
		if(!references.TryGetValue(hash, out entries))
		{   
			//If not the create the reference list
			references[hash] = entries = new List<WeakEntry>();
		}
		//See if we have an object of the correct type and that the
		//reference is still alive
		WeakEntry item = entries.FirstOrDefault(e=>e.weakReference.IsAlive && e.weakReference.Target == key && e.associate is T2);
		if(item == null) 
		{
			//If not create one
			if (t2Factory != null) {
				entries.Add(item = new WeakEntry { weakReference = new WeakReference(key), associate = t2Factory() });
			} else {
				return default(T2);
			}
		}
		//Return the associate
		return (T2)item.associate;
	}

	public void Remove(object key) {
		//Get the hash code of the indexing object
		int hash = key.GetHashCode();
		List<WeakEntry> entries = references.Get(hash);
		//See if we have a reference already
		if(entries != null)
		{   
			entries.RemoveAll(e=>e.weakReference.IsAlive && e.associate == key);
		}
	}
	
	public WeakKeyDictionary(Func<T> factory) 
	{
		this.factory = factory;
		//Setup garbage collection notification
		GCNotification.GCDone += Collected;
	}

	public bool ContainsKey(object key) {
		return this[key] != null;
	}
	
	/// <summary>
	/// Called when the garbage has been collected
	/// </summary>
	/// <param name='generation'>
	/// The generation that was collected
	/// </param>
	void Collected(int generation)
	{
		//Remove the references which are no longer alive
		
		//Scan each reference list
		foreach(var p in references)
		{
			//Scan each item in the references and remove
			//items that are missing
			removeEntries.Clear();
			foreach(var r in p.Value.Where(r=>!r.weakReference.IsAlive))
				removeEntries.Add(r);
			foreach(var entry in removeEntries)
			{
				if(entry.associate is IDisposable)
					(entry.associate as IDisposable).Dispose();                      
				p.Value.Remove(entry);
			}
		}
	}
	
	List<WeakEntry> removeEntries = new List<WeakEntry>();
	
}

///// <summary>
///// Extension class to support getting weak tables easily
///// </summary>
//public static class Extension
//{
//	static Dictionary<Type, WeakKeyDictionary<object>> extensions = new Dictionary<Type, WeakKeyDictionary<object>>();
//
//	public static T Get<T>(this object reference, bool create = true) where T : class, new() {
//		if (create) {
//			return Get<T>(reference, ()=> { return new T(); });
//		} else {
//			return Get<T>(reference, null);
//		}
//	}
//	/// <summary>
//	/// Get an associate for a particular object
//	/// </summary>
//	/// <param name='reference'>
//	/// The object whose associate should be found
//	/// </param>
//	/// <param name='create'>
//	/// Whether the associate should be created (defaults true)
//	/// </param>
//	/// <typeparam name='T'>
//	/// The type of associate
//	/// </typeparam>
//	public static T Get<T>(this object reference, Func<object> factory) where T : class
//	{
//		WeakKeyDictionary<object> references;
//		//Try to get a weaktable for the reference object
//		if(!extensions.TryGetValue(reference.GetType(), out references))
//		{
//			//Verify that we should be creating it if missing
//			if(factory == null)
//				return null;
//			//Create a new table
//			extensions[reference.GetType()] = references = new WeakKeyDictionary<object>(factory);
//		}
//		//Get the associate from the table
//		return (T)references.Get<T>(reference, factory);
//	}
//	
//}
#endif