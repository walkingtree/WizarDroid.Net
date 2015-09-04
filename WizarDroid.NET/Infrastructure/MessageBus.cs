using System;
using System.Collections.Generic;

namespace WizarDroid.NET.Infrastructure
{
    public class MessageBus
    {
        private static MessageBus instance = new MessageBus();

        private static Dictionary<ISubscriber, Type> Subscribers = new Dictionary<ISubscriber, Type>();

        public static MessageBus GetInstance()
        {
            return instance;
        }

        public void Publish<T>(T eventArgs) 
        {
            Type messageType = typeof(T);
            foreach (var subscriber in Subscribers)
            {
                if(subscriber.Value == messageType){
                    subscriber.Key.Receive(eventArgs);
                }
            }
        }


        public void Register(ISubscriber subscriber, Type eventType)
        {
            if (!Subscribers.ContainsKey(subscriber)) {
                Subscribers.Add(subscriber, eventType);
            }
        }

        public void UnRegister(ISubscriber subscriber)
        {
            if (Subscribers.ContainsKey(subscriber)) {
                Subscribers.Remove(subscriber);
            }
        }
    }

}