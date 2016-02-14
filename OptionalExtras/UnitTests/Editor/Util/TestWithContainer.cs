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
            _container = new DiContainer();
            InstallBindings();

            Validate();
            _container.Resolver.Inject(this);
        }

        void Validate()
        {
            var errors = Resolver.ValidateValidatables().Take(5).ToList();

            if (!errors.IsEmpty())
            {
                throw errors.First();

                // Print out 5 so you don't have to recompile and execute one by one
                //foreach (var err in errors)
                //{
                    //Log.ErrorException(err);
                //}

                //throw new Exception("Zenject Validation failed");
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
