using NUnit.Framework;
using Zenject;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSubContainer2
    {
        public interface IHaveMessage
        {
            string GetMessage();
        }

        public class Welcome : IHaveMessage
        {
            public string GetMessage()
            {
                return "Welcome";
            }
        }

        public class Bye : IHaveMessage
        {
            public string GetMessage()
            {
                return "Bye";
            }
        }

        public class User
        {
            [Inject]
            IHaveMessage _iHaveMessage = null;

            [Inject]
            DiContainer _container = null;

            public string SayIt()
            {
                return _iHaveMessage.GetMessage();
            }

            public void Rebind()
            {
                _container.Binder.Rebind<IHaveMessage>().ToSingle<Bye>();
                _container.Resolver.Inject(this);
            }
        }

        [Test]
        public void RebindingInSubContainer()
        {
            DiContainer parentContainer = new DiContainer();
            parentContainer.Binder.Bind<IHaveMessage>().ToSingle<Welcome>();

            Assert.AreEqual("Welcome", parentContainer.Resolver.Resolve<IHaveMessage>().GetMessage());

            DiContainer childContainer = parentContainer.CreateSubContainer();
            childContainer.Binder.Rebind<IHaveMessage>().ToSingle<Bye>();

            Assert.AreEqual("Bye", childContainer.Resolver.Resolve<IHaveMessage>().GetMessage());

            Assert.AreEqual("Welcome", parentContainer.Resolver.Resolve<IHaveMessage>().GetMessage());
        }

        [Test]
        public void RebindingInSubContainer2()
        {
            DiContainer parentContainer = new DiContainer();
            parentContainer.Binder.Bind<IHaveMessage>().ToSingle<Welcome>();

            Assert.AreEqual("Welcome", parentContainer.Resolver.Resolve<IHaveMessage>().GetMessage());

            DiContainer childContainer = parentContainer.CreateSubContainer();
            User user = new User();
            childContainer.Resolver.Inject(user);

            Assert.AreEqual("Welcome", user.SayIt());
            user.Rebind();
            Assert.AreEqual("Bye", user.SayIt());

            parentContainer.Resolver.Inject(user);
            Assert.AreEqual("Welcome", user.SayIt());
        }
    }

}
