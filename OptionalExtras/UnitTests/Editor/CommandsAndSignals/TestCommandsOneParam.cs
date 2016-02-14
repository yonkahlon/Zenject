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
    public class TestCommandsOneParam : TestWithContainer
    {
        [Test]
        public void TestSingle()
        {
            Binder.Bind<Bar>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand, string>()
                .ToSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = Resolver.Resolve<Bar>();
            var foo = Resolver.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");
        }

        [Test]
        public void TestResolve()
        {
            Binder.Bind<Bar>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand, string>()
                .ToResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = Resolver.Resolve<Bar>();
            var foo = Resolver.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");
        }

        [Test]
        public void TestOptionalResolve1()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand, string>()
                .ToOptionalResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger("asdf");
        }

        [Test]
        public void TestOptionalResolve2()
        {
            Binder.Bind<Bar>().ToSingle();
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand, string>()
                .ToOptionalResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger("asdf");
            Assert.IsEqual(Resolver.Resolve<Bar>().ReceivedValue, "asdf");
        }

        [Test]
        public void TestTransient()
        {
            Binder.Bind<Foo>().ToSingle();
            Binder.BindCommand<DoSomethingCommand, string>().ToTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = Resolver.Resolve<Foo>();

            Assert.IsEqual(Bar.Instances.Count, 0);
            foo.Trigger("asdf");
            Assert.IsEqual(Bar.Instances.Count, 1);

            var bar1 = Bar.Instances.Single();
            Assert.IsEqual(bar1.ReceivedValue, "asdf");

            bar1.ReceivedValue = null;
            foo.Trigger("zcxv");

            Assert.IsEqual(Bar.Instances.Count, 2);
            Assert.IsEqual(Bar.Instances.Last().ReceivedValue, "zcxv");
            Assert.IsNull(bar1.ReceivedValue);
        }

        [Test]
        public void TestNothing()
        {
            Binder.Bind<Foo>().ToSingle();

            Binder.BindCommand<DoSomethingCommand, string>().ToNothing();

            var foo = Resolver.Resolve<Foo>();
            foo.Trigger("asdf");
        }

        [Test]
        public void TestMethod()
        {
            Binder.Bind<Foo>().ToSingle();

            string receivedValue = null;
            Binder.BindCommand<DoSomethingCommand, string>().ToMethod((v) => receivedValue = v);

            var foo = Resolver.Resolve<Foo>();

            Assert.IsNull(receivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(receivedValue, "asdf");
        }

        public class DoSomethingCommand : Command<string>
        {
        }

        public class Foo
        {
            readonly DoSomethingCommand _doSomethingCommand;

            public Foo(DoSomethingCommand doSomethingCommand)
            {
                _doSomethingCommand = doSomethingCommand;
            }

            public void Trigger(string value)
            {
                _doSomethingCommand.Execute(value);
            }
        }


        public class Bar
        {
            public static List<Bar> Instances = new List<Bar>();

            public Bar()
            {
                Instances.Add(this);
            }

            public string ReceivedValue
            {
                get;
                set;
            }

            public void Execute(string value)
            {
                ReceivedValue = value;
            }
        }
    }
}

