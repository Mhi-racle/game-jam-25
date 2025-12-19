using System;
#nullable enable
namespace Cynteract.InputDevices
{
    public class EventSubscription

    {
        private readonly Action removeListener;

        public EventSubscription(Action removeListener)
        {
            this.removeListener = removeListener;
        }

        public void Remove()
        {
            removeListener?.Invoke();
        }
    }
}

