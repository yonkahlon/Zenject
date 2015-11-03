using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public abstract class FacadeFactory<TFacade> : IValidatable
        where TFacade : IDependencyRoot
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer> _containerInitializer = null;

        public TFacade Create()
        {
            var facade = CreateSubContainer().Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer()
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.AllowNullBindings = _container.AllowNullBindings;
            _containerInitializer(subContainer);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer();

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TFacade> : IValidatable
        where TFacade : IDependencyRoot
        where TParam1 : class
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1> _containerInitializer = null;

        public TFacade Create(TParam1 param1)
        {
            var facade = CreateSubContainer(param1).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.AllowNullBindings = _container.AllowNullBindings;
            _containerInitializer(subContainer, param1);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(null);

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TFacade> : IValidatable
        where TFacade : IDependencyRoot
        where TParam1 : class
        where TParam2 : class
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1, TParam2> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2)
        {
            var facade = CreateSubContainer(param1, param2).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.AllowNullBindings = _container.AllowNullBindings;
            _containerInitializer(subContainer, param1, param2);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(null, null);

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TFacade> : IValidatable
        where TFacade : IDependencyRoot
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<DiContainer, TParam1, TParam2, TParam3> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var facade = CreateSubContainer(param1, param2, param3).Resolve<TFacade>();
            facade.Initialize();
            return facade;
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            subContainer.AllowNullBindings = _container.AllowNullBindings;
            _containerInitializer(subContainer, param1, param2, param3);
            subContainer.Install<StandardInstaller<TFacade>>();
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(null, null, null);

            foreach (var error in subContainer.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}
