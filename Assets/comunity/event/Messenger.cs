// Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
//
// Inspired by and based on Rod Hyde's Messenger:
// http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger
//
// This is a C# messenger (notification center). It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other. The major improvement from Hyde's implementation is that
// there is more extensive error detection, preventing silent bugs.
//
// Usage example:
// Messenger<float>.AddListener("myEvent", MyEventHandler);
// ...
// Messenger<float>.Broadcast("myEvent", 1.0f);
using System;
using System.Collections.Generic;
using UnityEngine;
using mulova.commons;
using System.Collections.Generic.Ex;
using System.Ex;

namespace comunity
{
    public static class Messenger
    {
        private static readonly Loggerx log = LogManager.GetLogger(typeof(Messenger));
        public static Dictionary<string, List<Action>> eventTable = new Dictionary<string, List<Action>>();
        public static Dictionary<string, List<Action>> oneShotEventTable = new Dictionary<string, List<Action>>();
        
        private static List<Action> GetSlot(Dictionary<string, List<Action>> table, string eventType)
        {
            List<Action> slot = table.Get(eventType);
            if (slot == null)
            {
                slot = new List<Action>();
                table[eventType] = slot;
            }
            return slot;
        }
        
        private static Action[] GetSlotCopy(Dictionary<string, List<Action>> table, string eventType)
        {
            List<Action> slot = GetSlot(table, eventType);
            if (slot.IsEmpty())
            {
                return null;
            }
            return slot.ToArray();
        }
        
        public static void AddListener(string eventType, Action handler)
        {
            if (handler == null)
            {
                return;
            }
            List<Action> slot = GetSlot(eventTable, eventType);
            if (Platform.isEditor)
            {
                if (slot.Contains(handler))
                {
                    log.Error("Duplicate {0}", handler.Method.Name);
                }
            }
            slot.Add(handler);
        }
        
        public static void RemoveListener(string eventType, Action handler)
        {
            if (eventTable.ContainsKey(eventType))
            {
                List<Action> slot = GetSlot(eventTable, eventType);
                slot.Remove(handler);
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            } else
            {
                log.Warn("No listener for {0}", eventType);
            }
        }
        
        public static void Broadcast(string eventType)
        {
            Broadcast(eventTable, eventType);
            Broadcast(oneShotEventTable, eventType);
            oneShotEventTable.Remove(eventType);
        }
        
        public static void AddOneShotHandler(string eventType, Action handler)
        {
            if (handler == null)
            {
                return;
            }
            GetSlot(oneShotEventTable, eventType).Add(handler);
        }
        
        private static void Broadcast(Dictionary<string, List<Action>> table, string eventType)
        {
            Action[] slot = GetSlotCopy(table, eventType);
            if (slot != null)
            {
                foreach (Action a in slot)
                {
                    try
                    {
                        a.Call();
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }
    }


// One parameter
    public static class Messenger<T>
    {
        private static readonly Loggerx log = LogManager.GetLogger(typeof(Messenger));
        public static Dictionary<string, List<Action<T>>> eventTable = new Dictionary<string, List<Action<T>>>();
        public static Dictionary<string, List<Action<T>>> oneShotEventTable = new Dictionary<string, List<Action<T>>>();

        private static List<Action<T>> GetSlot(Dictionary<string, List<Action<T>>> table, string eventType)
        {
            List<Action<T>> slot = table.Get(eventType);
            if (slot == null)
            {
                slot = new List<Action<T>>();
                table[eventType] = slot;
            }
            return slot;
        }

        private static Action<T>[] GetSlotCopy(Dictionary<string, List<Action<T>>> table, string eventType)
        {
            List<Action<T>> slot = GetSlot(table, eventType);
            if (slot.IsEmpty())
            {
                return null;
            }
            return slot.ToArray();
        }

        public static void AddListener(string eventType, Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }
            List<Action<T>> slot = GetSlot(eventTable, eventType);
            if (Platform.isEditor)
            {
                if (slot.Contains(handler))
                {
                    log.Error("Duplicate {0}", handler.Method.Name);
                }
            }
            slot.Add(handler);
        }

        public static void RemoveListener(string eventType, Action<T> handler)
        {
            if (eventTable.ContainsKey(eventType))
            {
                GetSlot(eventTable, eventType).Remove(handler);
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            } else
            {
                log.Warn("No listener for {0}", eventType);
            }
        }

        public static void Broadcast(string eventType, T arg1)
        {
            Broadcast(eventTable, eventType, arg1);
            Broadcast(oneShotEventTable, eventType, arg1);
            oneShotEventTable.Remove(eventType);
        }

        public static void AddOneShotHandler(string eventType, Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }
            GetSlot(oneShotEventTable, eventType).Add(handler);
        }

        private static void Broadcast(Dictionary<string, List<Action<T>>> table, string eventType, T arg1)
        {
            Action<T>[] slot = GetSlotCopy(table, eventType);
            if (slot != null)
            {
                foreach (Action<T> a in slot)
                {
                    try
                    {
                        a.Call(arg1);
                    } catch (Exception ex)
                    {
                        log.Error("{0}", ex, eventType);
                    }
                }
            }
        }
    }


// Two parameters
    public static class Messenger<T, U>
    {
        private static readonly Loggerx log = LogManager.GetLogger(typeof(Messenger));
        public static Dictionary<string, List<Action<T,U>>> eventTable = new Dictionary<string, List<Action<T,U>>>();
        public static Dictionary<string, List<Action<T,U>>> oneShotEventTable = new Dictionary<string, List<Action<T,U>>>();

        private static List<Action<T,U>> GetSlot(Dictionary<string, List<Action<T,U>>> table, string eventType)
        {
            List<Action<T,U>> slot = table.Get(eventType);
            if (slot == null)
            {
                slot = new List<Action<T,U>>();
                table[eventType] = slot;
            }
            return slot;
        }

        private static Action<T,U>[] GetSlotCopy(Dictionary<string, List<Action<T,U>>> table, string eventType)
        {
            List<Action<T,U>> slot = GetSlot(table, eventType);
            if (slot.IsEmpty())
            {
                return null;
            }
            return slot.ToArray();
        }

        public static void AddListener(string eventType, Action<T, U> handler)
        {
            if (handler == null)
            {
                return;
            }
            List<Action<T, U>> slot = GetSlot(eventTable, eventType);
            if (Platform.isEditor)
            {
                if (slot.Contains(handler))
                {
                    log.Error("Duplicate {0}", handler.Method.Name);
                }
            }
            slot.Add(handler);
        }

        public static void RemoveListener(string eventType, Action<T, U> handler)
        {
            if (eventTable.ContainsKey(eventType))
            {
                GetSlot(eventTable, eventType).Remove(handler);
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            } else
            {
                log.Warn("No listener for {0}", eventType);
            }
        }

        public static void Broadcast(string eventType, T arg1, U arg2)
        {
            Broadcast(eventTable, eventType, arg1, arg2);
            Broadcast(oneShotEventTable, eventType, arg1, arg2);
            oneShotEventTable.Remove(eventType);
        }

        public static void AddOneShotHandler(string eventType, Action<T, U> handler)
        {
            if (handler == null)
            {
                return;
            }
            GetSlot(oneShotEventTable, eventType).Add(handler);
        }

        private static void Broadcast(Dictionary<string, List<Action<T,U>>> table, string eventType, T arg1, U arg2)
        {
            Action<T,U>[] slot = GetSlotCopy(table, eventType);
            if (slot != null)
            {
                foreach (Action<T,U> a in slot)
                {
                    try
                    {
                        a.Call(arg1, arg2);
                    } catch (Exception ex)
                    {
                        log.Error("{0}", ex, eventType);
                    }
                }
            }
        }
    }


// Three parameters
    public static class Messenger<T, U, V>
    {
        public static readonly Loggerx log = LogManager.GetLogger(typeof(Messenger));
        public static Dictionary<string, List<Action<T,U,V>>> eventTable = new Dictionary<string, List<Action<T,U,V>>>();
        public static Dictionary<string, List<Action<T,U,V>>> oneShotEventTable = new Dictionary<string, List<Action<T,U,V>>>();

        private static List<Action<T,U,V>> GetSlot(Dictionary<string, List<Action<T,U,V>>> table, string eventType)
        {
            List<Action<T,U,V>> slot = table.Get(eventType);
            if (slot == null)
            {
                slot = new List<Action<T,U,V>>();
                table[eventType] = slot;
            }
            return slot;
        }

        private static Action<T,U,V>[] GetSlotCopy(Dictionary<string, List<Action<T,U,V>>> table, string eventType)
        {
            List<Action<T,U,V>> slot = GetSlot(table, eventType);
            if (slot.IsEmpty())
            {
                return null;
            }
            return slot.ToArray();
        }

        public static void AddListener(string eventType, Action<T, U, V> handler)
        {
            if (handler == null)
            {
                return;
            }
            List<Action<T, U, V>> slot = GetSlot(eventTable, eventType);
            if (Platform.isEditor)
            {
                if (slot.Contains(handler))
                {
                    log.Error("Duplicate {0}", handler.Method.Name);
                }
            }
            slot.Add(handler);
        }

        public static void RemoveListener(string eventType, Action<T, U, V> handler)
        {
            if (eventTable.ContainsKey(eventType))
            {
                GetSlot(eventTable, eventType).Remove(handler);
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            } else
            {
                log.Warn("No listener for {0}", eventType);
            }
        }

        public static void Broadcast(string eventType, T arg1, U arg2, V arg3)
        {
            Broadcast(eventTable, eventType, arg1, arg2, arg3);
            Broadcast(oneShotEventTable, eventType, arg1, arg2, arg3);
            oneShotEventTable.Remove(eventType);
        }

        public static void AddOneShotHandler(string eventType, Action<T, U, V> handler)
        {
            if (handler == null)
            {
                return;
            }
            GetSlot(oneShotEventTable, eventType).Add(handler);
        }

        private static void Broadcast(Dictionary<string, List<Action<T,U,V>>> table, string eventType, T arg1, U arg2, V arg3)
        {
            Action<T,U,V>[] slot = GetSlotCopy(table, eventType);
            if (slot != null)
            {
                foreach (Action<T,U,V> a in slot)
                {
                    try
                    {
                        a.Call(arg1, arg2, arg3);
                    } catch (Exception ex)
                    {
                        log.Error("{0}", ex, eventType);
                    }
                }
            }
        }
    }
}