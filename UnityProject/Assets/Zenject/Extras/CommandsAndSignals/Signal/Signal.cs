using System;

namespace Zenject.Commands
{
    public interface ISignal
    {
    }

    public class Signal : ISignal
    {
        public event Action Event = delegate {};

        public class TriggerBase
        {
            [Inject]
            Signal _signal = null;

            public void Fire()
            {
                _signal.Event();
            }
        }
    }

    public class Signal<TParam1> : ISignal
    {
        public event Action<TParam1> Event = delegate {};

        public class TriggerBase
        {
            [Inject]
            Signal<TParam1> _signal = null;

            public void Fire(TParam1 arg1)
            {
                _signal.Event(arg1);
            }
        }
    }
}
