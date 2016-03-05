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
            Container.Bind<Bar>().ToSingle();
            Container.Bind<Foo>().ToSingle();
            Container.BindCommand<DoSomethingCommand, string>()
                .ToSingle<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = Container.Resolve<Bar>();
            var foo = Container.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");
        }

        [Test]
        public void TestResolve()
        {
            Container.Bind<Bar>().ToSingle();
            Container.Bind<Foo>().ToSingle();
            Container.BindCommand<DoSomethingCommand, string>()
                .ToResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var bar = Container.Resolve<Bar>();
            var foo = Container.Resolve<Foo>();

            Assert.IsNull(bar.ReceivedValue);
            foo.Trigger("asdf");
            Assert.IsEqual(bar.ReceivedValue, "asdf");
        }

        [Test]
        public void TestOptionalResolve1()
        {
            Container.Bind<Foo>().ToSingle();
            Container.BindCommand<DoSomethingCommand, string>()
                .ToOptionalResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var foo = Container.Resolve<Foo>();
            foo.Trigger("asdf");
        }

        [Test]
        public void TestOptionalResolve2()
        {
            Container.Bind<Bar>().ToSingle();
            Container.Bind<Foo>().ToSingle();
            Container.BindCommand<DoSomethingCommand, string>()
                .ToOptionalResolve<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            var foo = Container.Resolve<Foo>();
            foo.Trigger("asdf");
            Assert.IsEqual(Container.Resolve<Bar>().ReceivedValue, "asdf");
        }

        [Test]
        public void TestTransient()
        {
            Container.Bind<Foo>().ToSingle();
            Container.BindCommand<DoSomethingCommand, string>().ToTransient<Bar>(x => x.Execute).WhenInjectedInto<Foo>();

            Bar.Instances.Clear();

            var foo = Container.Resolve<Foo>();

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
            Container.Bind<Foo>().ToSingle();

            Container.BindCommand<DoSomethingCommand, string>().ToNothing();

            var foo = Container.Resolve<Foo>();
            foo.Trigger("asdf");
        }

        [Test]
        public void TestMethod()
        {
            Container.Bind<Foo>().ToSingle();

            string receivedValue = null;
            Container.BindCommand<DoSomethingCommand, string>().ToMethod((v) => receivedValue = v);

            var foo = Container.Resolve<Foo>();

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

