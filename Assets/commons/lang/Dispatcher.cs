using System.Collections.Generic;
using System;

namespace commons
{
    public class Dispatcher
    {
        private readonly Dictionary<string, EventDispatcher> listeners = new Dictionary<string, EventDispatcher>();
        private bool oneshot;

        public Dispatcher(bool oneshot)
        {
            this.oneshot = oneshot;
        }

        public void Add(string key, Action callback)
        {
            EventDispatcher slot = GetSlot(key);
            slot.AddCallback(callback);
        }

        public void Remove(string key, Action callback)
        {
            EventDispatcher slot = GetSlot(key);
            slot.RemoveCallback(callback);
        }

        private EventDispatcher GetSlot(string key)
        {
            var slot = listeners.Get(key);
            if (slot == null)
            {
                slot = new EventDispatcher(oneshot);
                listeners[key] = slot;
            }
            return slot;
        }

        public void Broadcast(string key)
        {
            var slot = GetSlot(key);
            slot.Broadcast();
        }

        public void Clear()
        {
            listeners.Clear();
        }
    }

    public class Dispatcher<T>
    {
        private readonly Dictionary<string, EventDispatcher<T>> listeners = new Dictionary<string, EventDispatcher<T>>();
        private bool oneshot;

        public Dispatcher(bool oneshot)
        {
            this.oneshot = oneshot;
        }

        public void Add(string key, Action<T> callback)
        {
            EventDispatcher<T> slot = GetSlot(key);
            slot.AddCallback(callback);
        }

        public void Remove(string key, Action<T> callback)
        {
            EventDispatcher<T> slot = GetSlot(key);
            slot.RemoveCallback(callback);
        }

        private EventDispatcher<T> GetSlot(string key)
        {
            var slot = listeners.Get(key);
            if (slot == null)
            {
                slot = new EventDispatcher<T>(oneshot);
                listeners[key] = slot;
            }
            return slot;
        }

        public void Broadcast(string key, T arg)
        {
            var slot = GetSlot(key);
            slot.Broadcast(arg);
        }

        public void Clear()
        {
            listeners.Clear();
        }
    }
}