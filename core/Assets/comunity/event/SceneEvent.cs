using UnityEngine;
using System;
using commons;

namespace core
{
    public class SceneEvent : SingletonBehaviour<SceneEvent>
    {
        public static Loggerx eventLog = LogManager.GetLogger(typeof(SceneEvent));
        
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
            if (eventId.IsNotEmpty()) {
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
                eventLog.Info("Generate Event {0}", eventId);
            }
        }
        
        public void Clear() {
            dispatcher0.Clear ();
            dispatcher1.Clear ();
            dispatcherOneShot0.Clear ();
            dispatcherOneShot1.Clear ();
        }
        
        public void RegisterListener(string eventId, Action<object> callback) {
            if (eventId.IsNotEmpty()) {
                dispatcher1.Add(eventId, callback);
            }
        }
        
        public void RegisterListener(string eventId, Action callback) {
            if (eventId.IsNotEmpty()) {
                dispatcher0.Add(eventId, callback);
            }
        }
        
        public void DeregisterListener(string eventId, Action<object> callback) {
            if (eventId.IsNotEmpty()) {
                dispatcher1.Remove(eventId, callback);
            }
        }
        
        public void DeregisterListener(string eventId, Action callback) {
            if (eventId.IsNotEmpty()) {
                dispatcher0.Remove(eventId, callback);
            }
        }
        
        public void RegisterOneShotListener(string eventId, Action<object> callback) {
            if (eventId.IsNotEmpty()) {
                dispatcherOneShot1.Add(eventId, callback);
            }
        }
        
        public void RegisterOneShotListener(string eventId, Action callback) {
            if (eventId.IsNotEmpty()) {
                dispatcherOneShot0.Add(eventId, callback);
            }
        }
    }
}

