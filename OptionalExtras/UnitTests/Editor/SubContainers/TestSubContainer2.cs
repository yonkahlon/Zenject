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
                _container.Rebind<IHaveMessage>().ToSingle<Bye>();
                _container.Inject(this);
            }
        }

        [Test]
        public void RebindingInSubContainer()
        {
            DiContainer parentContainer = new DiContainer();
            parentContainer.Bind<IHaveMessage>().ToSingle<Welcome>();

            Assert.AreEqual("Welcome", parentContainer.Resolve<IHaveMessage>().GetMessage());

            DiContainer childContainer = parentContainer.CreateSubContainer();
            childContainer.Rebind<IHaveMessage>().ToSingle<Bye>();

            Assert.AreEqual("Bye", childContainer.Resolve<IHaveMessage>().GetMessage());

            Assert.AreEqual("Welcome", parentContainer.Resolve<IHaveMessage>().GetMessage());
        }

        [Test]
        public void RebindingInSubContainer2()
        {
            DiContainer parentContainer = new DiContainer();
            parentContainer.Bind<IHaveMessage>().ToSingle<Welcome>();

            Assert.AreEqual("Welcome", parentContainer.Resolve<IHaveMessage>().GetMessage());

            DiContainer childContainer = parentContainer.CreateSubContainer();
            User user = new User();
            childContainer.Inject(user);

            Assert.AreEqual("Welcome", user.SayIt());
            user.Rebind();
            Assert.AreEqual("Bye", user.SayIt());

            parentContainer.Inject(user);
            Assert.AreEqual("Welcome", user.SayIt());
        }
    }

}
