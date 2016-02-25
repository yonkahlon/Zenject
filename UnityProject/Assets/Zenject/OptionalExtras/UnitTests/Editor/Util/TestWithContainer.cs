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

        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer(false);
            InstallBindings();

            AssertValidates();
            _container.Inject(this);
        }

        protected void AssertValidationFails()
        {
            Assert.That(Container.ValidateAll().HasMoreThan(0),
                "Expected validation to fail but it succeeded");
        }

        protected void AssertValidates()
        {
            _container.IsValidating = true;
            var error = Container.ValidateAll().FirstOrDefault();
            _container.IsValidating = false;

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
