using System;
using System.Collections.Generic;
using UnityEngine;


public class EventAggregator : IEventAggregator
{

    private Dictionary<Type, List<WeakReference>> _eventSubscribers = new Dictionary<Type, List<WeakReference>>();
    private List<WeakReference> _subscribersToRemove = new List<WeakReference>(32);

    #region IEventAggregator Members

    public void PublishEvent<TEventType>()
    {
        PublishEvent<TEventType>(Activator.CreateInstance<TEventType>());
    }

    public void PublishEvent<TEventType>(TEventType eventToPublish)
    {
        Type subscriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TEventType));
        List<WeakReference> subscribers = GetSubscriberList(subscriberType);
        WeakReference weakSubscriber;
        for (var i = 0; i < subscribers.Count; ++i)
        {
            weakSubscriber = subscribers[i];
            if (weakSubscriber.IsAlive)
            {
                ISubscriber<TEventType> subscriber = (ISubscriber<TEventType>)weakSubscriber.Target;
                InvokeSubscriberEvent<TEventType>(eventToPublish, subscriber);
            }
            else
            {
                _subscribersToRemove.Add(weakSubscriber);
            }
        }

        // remove dead subscribers
        for (var i = 0; i < _subscribersToRemove.Count; ++i)
            subscribers.Remove(_subscribersToRemove[i]);

        _subscribersToRemove.Clear();
    }

    public void SubscribeEvent(object subscriber)
    {
        WeakReference weakReference = new WeakReference(subscriber);
        Type[] subscriberTypes = subscriber.GetType().GetInterfaces();
        Type subscriberType;
        for (int i = 0; i < subscriberTypes.Length; ++i)
        {
            subscriberType = subscriberTypes[i];
            if (subscriberType.IsGenericType && subscriberType.GetGenericTypeDefinition() == typeof(ISubscriber<>))
            {
                List<WeakReference> subscribers = GetSubscriberList(subscriberType);
                subscribers.Add(weakReference);
            }
        }
    }

    public void UnsubscribeEvent(object subscriber)
    {
        Type[] subscriberTypes = subscriber.GetType().GetInterfaces();
        Type subscriberType;
        for (int i = 0; i < subscriberTypes.Length; ++i)
        {
            subscriberType = subscriberTypes[i];
            if (subscriberType.IsGenericType && subscriberType.GetGenericTypeDefinition() == typeof(ISubscriber<>))
            {
                List<WeakReference> subscribers = GetSubscriberList(subscriberType);
                subscribers.Remove(subscribers.Find(s => s.Target == subscriber));
            }
        }
    }

    #endregion

    private void InvokeSubscriberEvent<TEventType>(TEventType eventToPublish, ISubscriber<TEventType> subscriber)
    {
        // TODO: MAKE THIS ASYNC
        subscriber.OnEventHandler(eventToPublish);
    }

    private List<WeakReference> GetSubscriberList(Type subscriberType)
    {
        List<WeakReference> subscribersList = null;
        bool found = this._eventSubscribers.TryGetValue(subscriberType, out subscribersList);

        if (!found)
        {
            //First time create the list.
            subscribersList = new List<WeakReference>();
            this._eventSubscribers.Add(subscriberType, subscribersList);
        }
        return subscribersList;
    }
}
