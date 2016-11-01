using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModestTree;

namespace Zenject
{
    // Zero Parameters
    public abstract class Signal<TDerived> : ISignal
        where TDerived : Signal<TDerived>
    {
        readonly List<Action> _listeners = new List<Action>();

        bool _hasDisposed;

        public void Listen(Action listener)
        {
            Assert.That(!_listeners.Contains(listener),
                () => "Tried to add method '{0}' to signal '{1}' but it has already been added"
                .Fmt(SignalInternalUtil.MethodToString(listener.Method), this.GetType().Name));
            _listeners.Add(listener);
        }

        public void Unlisten(Action listener)
        {
            bool success = _listeners.Remove(listener);

            Assert.That(success,
                () => "Tried to remove method '{0}' from signal '{1}' without adding it first"
                .Fmt(SignalInternalUtil.MethodToString(listener.Method), this.GetType().Name));
        }

        void IDisposable.Dispose()
        {
            Assert.That(!_hasDisposed, "Tried to dispose signal '{0}' twice", this.GetType().Name);
            _hasDisposed = true;

            // If you don't want to verify that all event handlers have been removed feel free to comment out this assert or remove
            Assert.That(_listeners.IsEmpty(),
                () => "Found {0} methods still added to signal '{1}'.  Methods: {2}"
                .Fmt(_listeners.Count, this.GetType().Name, _listeners.Select(x => SignalInternalUtil.MethodToString(x.Method)).Join(", ")));
        }

        public static TDerived operator + (Signal<TDerived> signal, Action listener)
        {
            signal.Listen(listener);
            return (TDerived)signal;
        }

        public static TDerived operator - (Signal<TDerived> signal, Action listener)
        {
            signal.Unlisten(listener);
            return (TDerived)signal;
        }

        public void Fire()
        {
            // Use ToArray in case they remove in the handler
            foreach (var listener in _listeners.ToArray())
            {
                listener();
            }
        }
    }
}
