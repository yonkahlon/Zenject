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
    public class TestSignalsSixParams : TestWithContainer
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

            Assert.IsNull(bar.ReceivedValue);
            foo.DoSomething("asdf", 5, 1.2f, "zxcv", 5, 123.0f);
            Assert.IsEqual(bar.ReceivedValue, "zxcv");

            bar.Dispose();
        }

        public class SomethingHappenedSignal : Signal<string, int, float, string, int, float>
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

            public void DoSomething(string value1, int value2, float value3, string value4, int value5, float value6)
            {
                _trigger.Fire(value1, value2, value3, value4, value5, value6);
            }
        }

        public class Bar
        {
            readonly SomethingHappenedSignal _signal;

            string _receivedValue;

            public Bar(SomethingHappenedSignal signal)
            {
                _signal = signal;
            }

            public string ReceivedValue
            {
                get
                {
                    return _receivedValue;
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

            void OnStarted(string value1, int value2, float value3, string value4, int value5, float value6)
            {
                _receivedValue = value4;
            }
        }
    }
}

