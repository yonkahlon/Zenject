using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    // Four Parameters
    public abstract class Signal<TDerived, TParam1, TParam2, TParam3, TParam4> : ISignal
        where TDerived : Signal<TDerived, TParam1, TParam2, TParam3, TParam4>
    {
        readonly List<Action<TParam1, TParam2, TParam3, TParam4>> _listeners = new List<Action<TParam1, TParam2, TParam3, TParam4>>();

        bool _hasDisposed;

        public void Listen(Action<TParam1, TParam2, TParam3, TParam4> listener)
        {
            Assert.That(!_listeners.Contains(listener),
                () => "Tried to add method '{0}' to signal '{1}' but it has already been added"
                .Fmt(listener.ToDebugString(), this.GetType().Name));
            _listeners.Add(listener);
        }

        public void Unlisten(Action<TParam1, TParam2, TParam3, TParam4> listener)
        {
            bool success = _listeners.Remove(listener);
            Assert.That(success,
                () => "Tried to remove method '{0}' from signal '{1}' without adding it first"
                .Fmt(listener.ToDebugString(), this.GetType().Name));
        }

        void ILateDisposable.LateDispose()
        {
            Assert.That(!_hasDisposed, "Tried to dispose signal '{0}' twice", this.GetType().Name);
            _hasDisposed = true;

            // If you don't want to verify that all event handlers have been removed feel free to comment out this assert or remove
            Assert.Warn(_listeners.IsEmpty(),
                () => "Found {0} methods still added to signal '{1}'.  Methods: {2}"
                .Fmt(_listeners.Count, this.GetType().Name, _listeners.Select(x => x.ToDebugString()).Join(", ")));
        }

        public static TDerived operator + (Signal<TDerived, TParam1, TParam2, TParam3, TParam4> signal, Action<TParam1, TParam2, TParam3, TParam4> listener)
        {
            signal.Listen(listener);
            return (TDerived)signal;
        }

        public static TDerived operator - (Signal<TDerived, TParam1, TParam2, TParam3, TParam4> signal, Action<TParam1, TParam2, TParam3, TParam4> listener)
        {
            signal.Unlisten(listener);
            return (TDerived)signal;
        }

        public void Fire(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4)
        {
            // Use ToArray in case they remove in the handler
            foreach (var listener in _listeners.ToArray())
            {
                listener(p1, p2, p3, p4);
            }
        }
    }
}
