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
    public class TestCommands : TestWithContainer
    {
        [Test]
        public void TestSingleMethod()
        {
            Bar.StaticWasTriggered = false;

            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var foo = Resolver.Resolve<Foo>();

            Assert.That(!Bar.StaticWasTriggered);
            foo.Trigger();
            Assert.That(Bar.StaticWasTriggered);
        }

        [Test]
        public void TestResolveMethod()
        {
            Binder.Bind<Bar>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = Resolver.Resolve<Bar>();
            var foo = Resolver.Resolve<Foo>();

            Assert.That(!bar.WasTriggered);
            foo.Trigger();
            Assert.That(bar.WasTriggered);
        }

        [Test]
        public void TestOptionalResolveMethod1()
        {
            Bar.StaticWasTriggered = false;

            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToOptionalResolve<Bar>(x => x.Execute);

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger();

            Assert.That(!Bar.StaticWasTriggered);
        }

        [Test]
        public void TestOptionalResolveMethod2()
        {
            Bar.StaticWasTriggered = false;

            Binder.Bind<Bar>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToOptionalResolve<Bar>(x => x.Execute);

            var foo = Resolver.Resolve<Foo>();

            Assert.That(!Bar.StaticWasTriggered);
            foo.Trigger();
            Assert.That(Bar.StaticWasTriggered);
        }

        [Test]
        public void TestTransientMethod()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = Resolver.Resolve<Foo>();

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
            Binder.Bind<BarHandler>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToSingle<BarHandler>().WhenInjectedInto<Foo>();

            var foo = Resolver.Resolve<Foo>();
            var handler = Resolver.Resolve<BarHandler>();

            Assert.That(!handler.WasTriggered);
            foo.Trigger();
            Assert.That(handler.WasTriggered);
        }

        [Test]
        public void TestResolveHandler()
        {
            Binder.Bind<BarHandler>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToResolve<BarHandler>();

            var foo = Resolver.Resolve<Foo>();
            var handler = Resolver.Resolve<BarHandler>();

            Assert.That(!handler.WasTriggered);
            foo.Trigger();
            Assert.That(handler.WasTriggered);
        }

        [Test]
        public void TestResolveOptionalHandler1()
        {
            Binder.Bind<BarHandler>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToOptionalResolve<BarHandler>();

            var foo = Resolver.Resolve<Foo>();
            var handler = Resolver.Resolve<BarHandler>();

            Assert.That(!handler.WasTriggered);
            foo.Trigger();
            Assert.That(handler.WasTriggered);
        }

        [Test]
        public void TestResolveOptionalHandler2()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToOptionalResolve<BarHandler>();

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger();
        }

        [Test]
        public void TestTransientHandler()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand>().ToTransient<BarHandler>().WhenInjectedInto<Foo>();

            BarHandler.Instances.Clear();

            var foo = Resolver.Resolve<Foo>();

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

        [Test]
        public void TestNothingHandler()
        {
            Binder.Bind<Foo>().ToSingle();

            Binder.BindCommand<DoSomethingCommand>().ToNothing();

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger();
        }

        [Test]
        public void TestWithMethodHandler()
        {
            Binder.Bind<Foo>().ToSingle();

            bool wasCalled = false;

            Binder.BindCommand<DoSomethingCommand>().ToMethod(() => wasCalled = true);

            var foo = Resolver.Resolve<Foo>();

            Assert.That(!wasCalled);
            foo.Trigger();
            Assert.That(wasCalled);
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

            public static bool StaticWasTriggered
            {
                get;
                set;
            }

            public bool WasTriggered
            {
                get;
                set;
            }

            public void Execute()
            {
                WasTriggered = true;
                StaticWasTriggered = true;
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
