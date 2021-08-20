using System;
using System.Collections;
using System.Collections.Generic;

namespace TicTacToe.Common
{
    public sealed class EventService : IDisposable, IControllerEventService, IViewEventService
    {
        private static class EventServiceT<T>
        {
            internal static readonly Dictionary<EventService, EventHandler<T>> Callbacks = new Dictionary<EventService, EventHandler<T>>(1);
        }

        private readonly HashSet<IDictionary> _dependencies = new HashSet<IDictionary>();

        public void AddListener<T>(EventHandler<T> callback)
        {
            if (_dependencies.Add(EventServiceT<T>.Callbacks))
            {
                EventServiceT<T>.Callbacks[this] = callback;
            }
            else
            {
                EventServiceT<T>.Callbacks[this] += callback;
            }
        }

        public void Invoke<T>(object sender, T eventData)
        {
            if (EventServiceT<T>.Callbacks.TryGetValue(this, out EventHandler<T> eventHandler))
            {
                eventHandler?.Invoke(sender, eventData);
            }
        }

        public void RemoveAllListeners()
        {
            foreach (IDictionary dependency in _dependencies)
            {
                dependency.Remove(this);
            }

            _dependencies.Clear();
        }

        public void RemoveAllListeners<T>()
        {
            if (_dependencies.Remove(EventServiceT<T>.Callbacks))
            {
                EventServiceT<T>.Callbacks.Remove(this);
            }
        }

        public void RemoveListener<T>(EventHandler<T> callback)
        {
            if (EventServiceT<T>.Callbacks.TryGetValue(this, out EventHandler<T> eventHandler))
            {
                EventServiceT<T>.Callbacks[this] = eventHandler - callback;
            }
        }

        public void Dispose()
        {
            RemoveAllListeners();
        }

        void IControllerEventService.AddListener<T>(EventHandler<T> callback)
        {
            AddListener(callback);
        }

        void IControllerEventService.Invoke<T>(object sender, T eventData)
        {
            Invoke(sender, eventData);
        }

        void IControllerEventService.RemoveAllListeners<T>()
        {
            RemoveAllListeners<T>();
        }

        void IControllerEventService.RemoveListener<T>(EventHandler<T> callback)
        {
            RemoveListener(callback);
        }

        void IViewEventService.AddListener<T>(EventHandler<T> callback)
        {
            AddListener(callback);
        }

        void IViewEventService.Invoke<T>(object sender, T eventData)
        {
            Invoke(sender, eventData);
        }

        void IViewEventService.RemoveAllListeners<T>()
        {
            RemoveAllListeners<T>();
        }

        void IViewEventService.RemoveListener<T>(EventHandler<T> callback)
        {
            RemoveListener(callback);
        }
    }
}
