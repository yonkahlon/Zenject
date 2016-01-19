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
    public class TestSignalsSixParams
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();
        }

        [Test]
        public void RunTest()
        {
            _container.Bind<Foo>().ToSingle();
            _container.Bind<Bar>().ToSingle();

            _container.BindSignal<SomethingHappenedSignal, string, int, float, string, int, float>();
            _container.BindTrigger<SomethingHappenedSignal, string, int, float, string, int, float>()
                .WhenInjectedInto<Foo>();

            var foo = _container.Resolve<Foo>();
            var bar = _container.Resolve<Bar>();
            bar.Initialize();

            Assert.IsNull(bar.ReceivedValue);
            foo.DoSomething("asdf", 5, 1.2f, "zxcv", 5, 123.0f);
            Assert.IsEqual(bar.ReceivedValue, "zxcv");

            bar.Dispose();
        }

        public class SomethingHappenedSignal : Signal<SomethingHappenedSignal, string, int, float, string, int, float>
        {
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

