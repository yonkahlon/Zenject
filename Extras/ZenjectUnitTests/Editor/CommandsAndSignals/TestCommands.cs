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
    public class TestCommands
    {
        DiContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DiContainer();
        }

        [Test]
        public void TestSingleMethod()
        {
            _container.Bind<Bar>().ToSingle();
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand>().HandleWithSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = _container.Resolve<Bar>();
            var foo = _container.Resolve<Foo>();

            Assert.That(!bar.WasTriggered);
            foo.Trigger();
            Assert.That(bar.WasTriggered);
        }

        [Test]
        public void TestTransientMethod()
        {
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand>().HandleWithTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = _container.Resolve<Foo>();

            Assert.IsEqual(Bar.Instances.Count, 0);
            foo.Trigger();
            Assert.IsEqual(Bar.Instances.Count, 1);

            var bar1 = Bar.Instances.Single();
            Assert.That(bar1.WasTriggered);

            bar1.WasTriggered = false;
            foo.Trigger();

            Assert.IsEqual(Bar.Instances.Count, 2);
            Assert.That(Bar.Instances.Last().WasTriggered);
            Assert.That(!bar1.WasTriggered);
        }

        [Test]
        public void TestSingleHandler()
        {
            _container.Bind<BarHandler>().ToSingle();
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand>().HandleWithSingle<BarHandler>().WhenInjectedInto<Foo>();

            var foo = _container.Resolve<Foo>();
            var handler = _container.Resolve<BarHandler>();

            Assert.That(!handler.WasTriggered);
            foo.Trigger();
            Assert.That(handler.WasTriggered);
        }

        [Test]
        public void TestTransientHandler()
        {
            _container.Bind<Foo>().ToSingle();
            _container.BindCommand<DoSomethingCommand>().HandleWithTransient<BarHandler>().WhenInjectedInto<Foo>();

            BarHandler.Instances.Clear();

            var foo = _container.Resolve<Foo>();

            Assert.IsEqual(BarHandler.Instances.Count, 0);
            foo.Trigger();
            Assert.IsEqual(BarHandler.Instances.Count, 1);

            var bar1 = BarHandler.Instances.Single();
            Assert.That(bar1.WasTriggered);

            bar1.WasTriggered = false;
            foo.Trigger();

            Assert.IsEqual(BarHandler.Instances.Count, 2);
            Assert.That(BarHandler.Instances.Last().WasTriggered);
            Assert.That(!bar1.WasTriggered);
        }

        public class DoSomethingCommand : Command
        {
        }

        public class Foo
        {
            readonly DoSomethingCommand _doSomethingCommand;

            public Foo(DoSomethingCommand doSomethingCommand)
            {
                _doSomethingCommand = doSomethingCommand;
            }

            public void Trigger()
            {
                _doSomethingCommand.Execute();
            }
        }

        public class Bar
        {
            public static List<Bar> Instances = new List<Bar>();

            public Bar()
            {
                Instances.Add(this);
            }

            public bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
            }
        }

        public class BarHandler : ICommandHandler
        {
            public static List<BarHandler> Instances = new List<BarHandler>();

            public BarHandler()
            {
                Instances.Add(this);
            }

            public bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
            }
        }
    }
}
