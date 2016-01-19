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
    public class TestSignalsOneParam
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

            Assert.Throw("TODO");
            //_container.BindSignal<SomethingHappenedSignal, string>();
            //_container.BindTrigger<SomethingHappenedSignal.Trigger, string>();

            var foo = _container.Resolve<Foo>();
            var bar = _container.Resolve<Bar>();
            bar.Initialize();

            Assert.IsNull(bar.ReceivedValue);
            foo.DoSomething("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");

            bar.Dispose();
        }

        public class SomethingHappenedSignal : Signal<SomethingHappenedSignal, string>
        {
        }

        public class Foo
        {
            readonly SomethingHappenedSignal.Trigger _trigger;

            public Foo(SomethingHappenedSignal.Trigger trigger)
            {
                _trigger = trigger;
            }

            public void DoSomething(string value)
            {
                _trigger.Fire(value);
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

            void OnStarted(string value)
            {
                _receivedValue = value;
            }
        }
    }
}

