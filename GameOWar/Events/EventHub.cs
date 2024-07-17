using System;
using System.Collections.Generic;

public static class EventHub
{
    private static readonly Dictionary<string, List<Action<object>>> _eventHandlers = new();

    // Subscribe to an event with a handler
    public static void Subscribe<T>(Action<T> handler)
    {
        string eventType = typeof(T).FullName;

        if (!_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = new List<Action<object>>();
        }

        // Wrap the handler to match the expected delegate signature
        _eventHandlers[eventType].Add(e => handler((T)e));
    }

    // Unsubscribe from an event
    public static void Unsubscribe<T>(Action<T> handler)
    {
        string eventType = typeof(T).FullName;

        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType].Remove(e => handler((T)e));
        }
    }

    // Publish an event with data
    public static void Publish<T>(T eventData)
    {
        string eventType = typeof(T).FullName;

        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler(eventData);
            }
        }
    }
}
