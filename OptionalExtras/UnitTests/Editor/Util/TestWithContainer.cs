using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree.Util;
using Zenject;
using NUnit.Framework;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    public class TestWithContainer
    {
        DiContainer _container;

        protected DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        protected IBinder Binder
        {
            get
            {
                return _container.Binder;
            }
        }

        protected IResolver Resolver
        {
            get
            {
                return _container.Resolver;
            }
        }

        protected IInstantiator Instantiator
        {
            get
            {
                return _container.Instantiator;
            }
        }

        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer(false);
            InstallBindings();

            AssertValidates();
            _container.Resolver.Inject(this);
        }

        protected void AssertValidationFails()
        {
            Assert.That(Resolver.ValidateAll().HasMoreThan(0),
                "Expected validation to fail but it succeeded");
        }

        protected void AssertValidates()
        {
            var error = Resolver.ValidateAll().FirstOrDefault();

            if (error != null)
            {
                throw error;
            }
        }

        public virtual void InstallBindings()
        {
        }

        [TearDown]
        public virtual void Destroy()
        {
            _container = null;
        }
    }
}
