using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Commands;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSignals : TestWithContainer
    {
        [Test]
        public void RunTest()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.Bind<Bar>().ToSingle();

            Binder.BindSignal<SomethingHappenedSignal>();
            Binder.BindTrigger<SomethingHappenedSignal.Trigger>()
                .WhenInjectedInto<Foo>();

            var foo = Resolver.Resolve<Foo>();
            var bar = Resolver.Resolve<Bar>();
            bar.Initialize();

            Assert.That(!bar.ReceivedSignal);
            foo.DoSomething();
            Assert.That(bar.ReceivedSignal);

            bar.Dispose();
        }

        public class SomethingHappenedSignal : Signal
        {
            public class Trigger : TriggerBase
            {
            }
        }

        public class Foo
        {
            readonly SomethingHappenedSignal.Trigger _trigger;

            public Foo(SomethingHappenedSignal.Trigger trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething()
            {
                _trigger.Fire();
            }
        }

        public class Bar
        {
            readonly SomethingHappenedSignal _signal;
            bool _receivedSignal;

            public Bar(SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public bool ReceivedSignal
            {
                get
                {
                    return _receivedSignal;
                }
            }

            public void Initialize()
            {
                _signal.Event += OnStarted;
            }

            public void Dispose()
            {
                _signal.Event -= OnStarted;
            }

            void OnStarted()
            {
                _receivedSignal = true;
            }
        }
    }
}
