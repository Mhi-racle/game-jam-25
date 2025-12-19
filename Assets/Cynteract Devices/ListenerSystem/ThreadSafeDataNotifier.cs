using System;
using System.Collections.Generic;
#nullable enable
namespace Cynteract.InputDevices
{
    public class ThreadSafeDataNotifier
    {
        private readonly List<Action> listeners = new();
        private readonly object lockObject = new();

        public EventSubscription Listen(Action callback)
        {
            lock (lockObject)
            {
                if (!listeners.Contains(callback))
                    listeners.Add(callback);
            }

            return new EventSubscription(() =>
            {
                lock (lockObject)
                {
                    listeners.Remove(callback);
                }
            });
        }

        public void NotifyListeners()
        {
            lock (lockObject)
            {
                foreach (var listener in listeners)
                    listener?.Invoke();
            }
        }
    }
    public class ThreadSafeDataNotifier<T>
    {
        private readonly List<Action<T>> listeners = new();
        private readonly object lockObject = new();

        public EventSubscription Listen(Action<T> callback)
        {
            lock (lockObject)
            {
                listeners.Add(callback);
            }

            return new EventSubscription(() =>
            {
                lock (lockObject)
                {
                    listeners.Remove(callback);
                }
            });
        }

        public void NotifyListeners(T data)
        {
            lock (lockObject)
            {
                foreach (var listener in listeners)
                {
                    listener?.Invoke(data);
                }
            }
        }
    }
    public class ThreadSafeDataNotifier<T1, T2>
    {
        private readonly List<Action<T1, T2>> listeners = new();
        private readonly object lockObject = new();

        public EventSubscription Listen(Action<T1, T2> callback)
        {
            lock (lockObject)
            {
                if (!listeners.Contains(callback))
                    listeners.Add(callback);
            }

            return new EventSubscription(() =>
            {
                lock (lockObject)
                {
                    listeners.Remove(callback);
                }
            });
        }

        public void NotifyListeners(T1 arg1, T2 arg2)
        {
            lock (lockObject)
            {
                foreach (var listener in listeners)
                    listener?.Invoke(arg1, arg2);
            }
        }
    }

}

