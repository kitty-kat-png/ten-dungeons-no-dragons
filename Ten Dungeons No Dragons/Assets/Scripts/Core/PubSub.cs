using System;
using System.Collections.Generic;
using Singleton;

namespace PubSub
{
    public class BaseEvent { };

    /// <summary>
    /// Large Scale Event Hub for mass event distribution
    /// </summary>
    public class PubSub : Singleton<PubSub>
    {
        private Dictionary<string, Action<BaseEvent>> delegates;
        private Dictionary<int, Dictionary<int, Action<BaseEvent>>> handlers;

        void Awake()
        {
            delegates = new Dictionary<string, Action<BaseEvent>>();
            handlers = new Dictionary<int, Dictionary<int, Action<BaseEvent>>>();
            SetInstance(this);
            DontDestroyOnLoad(this);
        }

        public void Subscribe<TEvent>(ISubscriber<TEvent> subscriber) where TEvent : BaseEvent
        {
            if (!handlers.ContainsKey(subscriber.GetHashCode()))
            {
                handlers.Add(subscriber.GetHashCode(), new Dictionary<int, Action<BaseEvent>>());
            }

            Action<BaseEvent> handler = (eventT) => subscriber.HandleEvent((TEvent)eventT);

            if (handlers[subscriber.GetHashCode()].TryAdd(typeof(TEvent).GetHashCode(), handler))
            {
                if (!delegates.ContainsKey(typeof(TEvent).Name))
                {
                    delegates.Add(typeof(TEvent).Name, handlers[subscriber.GetHashCode()][typeof(TEvent).GetHashCode()]);
                }
                else
                {
                    delegates[typeof(TEvent).Name] += handlers[subscriber.GetHashCode()][typeof(TEvent).GetHashCode()];
                }
            }
        }

        public void Unsubscribe<TEvent>(ISubscriber<TEvent> subscriber) where TEvent : BaseEvent
        {
            if (handlers.ContainsKey(subscriber.GetHashCode()))
            {
                if (handlers[subscriber.GetHashCode()].Remove(typeof(TEvent).GetHashCode(), out Action<BaseEvent> handler))
                {
                    if (delegates.ContainsKey(typeof(TEvent).Name))
                    {
                        delegates[typeof(TEvent).Name] -= handler;
                    }
                }
            }
        }

        public bool PostEvent<T>(T dispatchEvent) where T : BaseEvent
        {
            bool anyListeners = false;

            if (delegates.ContainsKey(typeof(T).Name))
            {
                anyListeners = delegates[typeof(T).Name] != null;
                delegates[typeof(T).Name]?.Invoke(dispatchEvent);
            }

            return anyListeners;
        }
    }

}