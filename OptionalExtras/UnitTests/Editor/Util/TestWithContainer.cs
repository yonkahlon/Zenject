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

            _container.Validate();
            _container.Inject(this);
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
