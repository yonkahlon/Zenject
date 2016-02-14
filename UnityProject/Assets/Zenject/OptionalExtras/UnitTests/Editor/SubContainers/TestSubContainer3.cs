using NUnit.Framework;
using Zenject;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSubContainer3
    {
        public class Foo
        {
            public int Value;

            public Foo(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void Test1()
        {
            DiContainer parentContainer = new DiContainer();
            parentContainer.Binder.Bind<Foo>().ToTransient();

            // ToTransient should always use the DiContainer given by the inject context
            var subContainer = parentContainer.CreateSubContainer();
            subContainer.Binder.Bind<int>().ToInstance<int>(5);

            var foo = subContainer.Resolver.Resolve<Foo>();
            Assert.AreEqual(foo.Value, 5);
        }
    }
}

