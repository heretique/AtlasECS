using System;

public interface IEventAggregator
{
    void PublishEvent<TEventType>();
    void PublishEvent<TEventType>(TEventType eventToPublish);

    void SubscribeEvent(Object subscriber);

    void UnsubscribeEvent(Object subscriber);
}

