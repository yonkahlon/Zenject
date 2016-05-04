using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSignals : TestWithContainer
    {
        [Test]
        public void RunTest()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Bar>().AsSingle();

            Container.BindSignal<SomethingHappenedSignal>();
            Container.BindTrigger<SomethingHappenedSignal.Trigger>()
                .WhenInjectedInto<Foo>();

            var foo = Container.Resolve<Foo>();
            var bar = Container.Resolve<Bar>();
            bar.Initialize();

            Assert.That(!bar.ReceivedSignal);
            foo.DoSomething();
            Assert.That(bar.ReceivedSignal);

            bar.Dispose();
        }

        [Test]
        public void RunTestSignalInterfaces()
        {
            Container.BindSignal<SomethingHappenedSignal>();
            Container.BindTrigger<SomethingHappenedSignal.Trigger>();

            Container.BindSignal<AnotherSignal>();
            Container.BindTrigger<AnotherSignal.Trigger>();

            Container.Bind<IFooSignal>()
                .To(typeof(AnotherSignal), typeof(SomethingHappenedSignal)).FromResolve();

            var fooSignals = Container.ResolveAll<IFooSignal>();

            Assert.IsEqual(fooSignals.Count, 2);
        }

        public interface IFooSignal
        {
        }

        public class SomethingHappenedSignal : Signal, IFooSignal
        {
            public class Trigger : TriggerBase
            {
            }
        }

        public class AnotherSignal : Signal, IFooSignal
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
