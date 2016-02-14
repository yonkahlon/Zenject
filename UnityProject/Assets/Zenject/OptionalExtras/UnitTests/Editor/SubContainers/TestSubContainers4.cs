using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestSubContainers4 : TestWithContainer
    {
        readonly Dictionary<object, DiContainer> _subContainers = new Dictionary<object, DiContainer>();

        [Test]
        public void RunTest()
        {
            SetupContainer();

            var view1 = Resolver.Resolve<RotorView>();

            Assert.IsEqual(view1.Controller.Model, view1.Model);

            var view2 = Resolver.Resolve<RotorView>();

            Assert.IsEqual(view2.Controller.Model, view2.Model);

            Assert.IsNotEqual(view2.Model, view1.Model);
            Assert.IsNotEqual(view2, view1);
        }

        void SetupContainer()
        {
            Binder.Bind<RotorController>().ToMethod(SubContainerResolve<RotorController>)
                .WhenInjectedInto<RotorView>();

            Binder.Bind<RotorModel>().ToMethod(SubContainerResolve<RotorModel>)
                .WhenInjectedInto<RotorView>();

            Binder.Bind<RotorView>().ToTransient();
        }

        T SubContainerResolve<T>(InjectContext context)
        {
            Assert.IsNotNull(context.ObjectInstance);
            DiContainer subContainer;

            if (!_subContainers.TryGetValue(context.ObjectInstance, out subContainer))
            {
                subContainer = context.Container.CreateSubContainer();
                _subContainers.Add(context.ObjectInstance, subContainer);

                InstallViewBindings(subContainer);
            }

            return (T)subContainer.Resolver.Resolve(context);
        }

        void InstallViewBindings(DiContainer subContainer)
        {
            subContainer.Binder.Bind<RotorController>().ToSingle();
            subContainer.Binder.Bind<RotorModel>().ToSingle();
        }

        public class RotorController
        {
            [Inject]
            public RotorModel Model;
        }

        public class RotorView
        {
            [Inject]
            public RotorController Controller;

            [Inject]
            public RotorModel Model;
        }

        public class RotorModel
        {
        }
    }
}

