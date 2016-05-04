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
    public class TestTriggers1 : TestWithContainer
    {
        [Test]
        public void Test1()
        {
            Container.BindSignal<FooSignal>();
            Container.BindTrigger<FooSignal.Trigger>();

            var trigger = Container.Resolve<FooSignal.Trigger>();

            bool received = false;
            trigger.Event += delegate { received = true; };

            // This is a compiler error
            //trigger.Event();

            Assert.That(!received);
            trigger.Fire();
            Assert.That(received);
        }

        public class FooSignal : Signal
        {
            public class Trigger : TriggerBase
            {
            }
        }
    }
}

