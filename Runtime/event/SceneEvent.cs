using UnityEngine;
using System;
using mulova.commons;
using System.Text.Ex;


namespace mulova.comunity
{
    public class SceneEvent : SingletonBehaviour<SceneEvent>
    {
        public static ILog eventLog = LogManager.GetLogger(nameof(SceneEvent));
        
        private readonly Dispatcher dispatcher0 = new Dispatcher(false);
        private readonly Dispatcher dispatcherOneShot0 = new Dispatcher(true);
        private readonly Dispatcher<object> dispatcher1 = new Dispatcher<object>(false);
        private readonly Dispatcher<object> dispatcherOneShot1 = new Dispatcher<object>(true);
        
        public static void SendEvent(string eventId, object data) {
            if (IsInitialized()) {
                inst.SendEventImpl (eventId, data);
            }
        }
        
        private void SendEventImpl(string eventId, object data) {
            if (!eventId.IsEmpty()) {
                try {
                    dispatcher1.Broadcast(eventId, data);
                    dispatcherOneShot1.Broadcast(eventId, data);
                } catch (Exception ex) {
                    eventLog.Error(ex);
                }
                try {
                    dispatcher0.Broadcast(eventId);
                    dispatcherOneShot0.Broadcast(eventId);
                } catch (Exception ex) {
                    eventLog.Error(ex);
                }
                eventLog.Debug("Generate Event {0}", eventId);
            }
        }
        
        public void Clear() {
            dispatcher0.Clear ();
            dispatcher1.Clear ();
            dispatcherOneShot0.Clear ();
            dispatcherOneShot1.Clear ();
        }
        
        public void RegisterListener(string eventId, Action<object> callback) {
            if (!eventId.IsEmpty()) {
                dispatcher1.Add(eventId, callback);
            }
        }
        
        public void RegisterListener(string eventId, Action callback) {
            if (!eventId.IsEmpty()) {
                dispatcher0.Add(eventId, callback);
            }
        }
        
        public void DeregisterListener(string eventId, Action<object> callback) {
            if (!eventId.IsEmpty()) {
                dispatcher1.Remove(eventId, callback);
            }
        }
        
        public void DeregisterListener(string eventId, Action callback) {
            if (!eventId.IsEmpty()) {
                dispatcher0.Remove(eventId, callback);
            }
        }
        
        public void RegisterOneShotListener(string eventId, Action<object> callback) {
            if (!eventId.IsEmpty()) {
                dispatcherOneShot1.Add(eventId, callback);
            }
        }
        
        public void RegisterOneShotListener(string eventId, Action callback) {
            if (!eventId.IsEmpty()) {
                dispatcherOneShot0.Add(eventId, callback);
            }
        }
    }
}

