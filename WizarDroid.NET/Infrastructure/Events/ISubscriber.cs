using System;

namespace WizarDroid.NET.Infrastructure
{
    public interface ISubscriber
    {
        void Receive<T>(T eventArgs);
    }
}