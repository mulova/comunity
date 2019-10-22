using System;
using System.Collections.Generic;
using System.Text.Ex;
using mulova.commons;

namespace mulova.comunity
{
    public class EventRegistry
    {
        private bool registered;

        private class PopupEventCallback
        {
            public string eventId;
            public Action callback0;
            public Action<object> callback1;
            
            public void Register()
            {
                if (callback0 != null)
                {
                    RegisterListener(eventId, callback0);
                } else
                {
                    RegisterListener(eventId, callback1);
                }
            }
            
            public void Deregister()
            {
                if (callback0 != null)
                {
                    DeregisterListener(eventId, callback0);
                } else
                {
                    DeregisterListener(eventId, callback1);
                }
            }
        }
        
        private List<PopupEventCallback> callbackList = new List<PopupEventCallback>();
        
        private static readonly Dispatcher dispatcher0 = new Dispatcher(false);
        private static readonly Dispatcher dispatcherOneShot0 = new Dispatcher(true);
        private static readonly Dispatcher<object> dispatcher1 = new Dispatcher<object>(false);
        private static readonly Dispatcher<object> dispatcherOneShot1 = new Dispatcher<object>(true);
        
        public static ILog log = LogManager.GetLogger(typeof(EventRegistry));
        
        public void RegisterEvents()
        {
            registered = true;
            foreach (PopupEventCallback c in callbackList)
            {
                c.Register();
            }
        }
        
        public void DeregisterEvents()
        {
            registered = false;
            foreach (PopupEventCallback c in callbackList)
            {
                c.Deregister();
            }
        }
        
        public void AddCallback(string id, Action callback)
        {
            if (registered)
            {
                throw new Exception("Too late. Call before RegisterEvents()");
            }
            callbackList.Add(new PopupEventCallback() { eventId = id, callback0 = callback });
        }
        
        public void AddCallback(string id, Action<object> callback)
        {
            if (registered)
            {
                throw new Exception("Too late. Call before RegisterEvents()");
            }
            callbackList.Add(new PopupEventCallback() { eventId = id, callback1 = callback });
        }
        
        public static void SendEvent(string eventId, object data)
        {
            if (!eventId.IsEmpty())
            {
                try
                {
                    dispatcher1.Broadcast(eventId, data);
                    dispatcherOneShot1.Broadcast(eventId, data);
                } catch (Exception ex)
                {
                    log.Error(ex);
                }
                try
                {
                    dispatcher0.Broadcast(eventId);
                    dispatcherOneShot0.Broadcast(eventId);
                } catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Debug("Generate Event {0}", eventId);
                SceneEvent.SendEvent(eventId, data);
            }
        }
        
        public static void Clear()
        {
            dispatcher0.Clear();
            dispatcher1.Clear();
            dispatcherOneShot0.Clear();
            dispatcherOneShot1.Clear();
        }
        
        public static void RegisterListener(string eventId, Action<object> callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcher1.Add(eventId, callback);
            }
        }
        
        public static void RegisterListener(string eventId, Action callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcher0.Add(eventId, callback);
            }
        }
        
        public static void DeregisterListener(string eventId, Action<object> callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcher1.Remove(eventId, callback);
            }
        }
        
        public static void DeregisterListener(string eventId, Action callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcher0.Remove(eventId, callback);
            }
        }
        
        public static void RegisterOneShotListener(string eventId, Action<object> callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcherOneShot1.Add(eventId, callback);
            }
        }
        
        public static void RegisterOneShotListener(string eventId, Action callback)
        {
            if (!eventId.IsEmpty())
            {
                dispatcherOneShot0.Add(eventId, callback);
            }
        }
    }
}


