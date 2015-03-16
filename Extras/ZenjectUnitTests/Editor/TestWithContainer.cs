using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;
using NUnit.Framework;

namespace ModestTree.Tests.Zenject
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
            _container = new DiContainer();

            InstallBindings();

            _container.InstallInstallers();
            FieldsInjecter.Inject(_container, this);
        }

        protected virtual void InstallBindings()
        {
        }

        [TearDown]
        public virtual void Destroy()
        {
            _container = null;
        }
    }
}
