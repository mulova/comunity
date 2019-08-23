using System.Collections;

public sealed class ObjectRegistry
{
	private static Queue atlasCreated = new Queue();
	private static Queue fontCreated = new Queue();

	public static void PushAtlas(object a)
	{
		Push(a, atlasCreated);
	}

	public static object PopAtlas()
	{
		return Pop(atlasCreated);
	}

	public static void PushFont(object a)
	{
		Push(a, fontCreated);
	}

	public static object PopFont()
	{
		return Pop(fontCreated);
	}

	private static void Push(object a, Queue q)
	{
		lock (q) {
			q.Enqueue(a);
		}
	}

	private static object Pop(Queue q)
	{
		if (q.Count > 0) {
			lock (q) {
				if (q.Count > 0) {
					return q.Dequeue();
				} else
				{
					return null;
				}
			}
		} else
		{
			return null;
		}
	}
}
