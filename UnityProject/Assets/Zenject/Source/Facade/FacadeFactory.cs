using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public interface IFacadeFactory : IValidatable
    {
    }

    public abstract class FacadeFactory<TFacade> : IFacadeFactory, IFactory<TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [InjectOptional]
        Type _installerType = null;

        [InjectOptional]
        Action<IBinder> _containerInitializer = null;

        public TFacade Create()
        {
            return FacadeUtil.CreateSubContainer(
                _container, _containerInitializer, _installerType).Resolver.Resolve<TFacade>();
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            return FacadeUtil.Validate(
                typeof(TFacade), _container, _containerInitializer, _installerType);
        }
    }

    public abstract class FacadeFactory<TParam1, TFacade>
        : IFacadeFactory, IFactory<TParam1, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<IBinder, TParam1> _containerInitializer = null;

        public TFacade Create(TParam1 param1)
        {
            return CreateSubContainer(param1).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(TParam1 param1)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<IBinder, TParam1, TParam2> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2)
        {
            return CreateSubContainer(param1, param2).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TParam3, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        Action<IBinder, TParam1, TParam2, TParam3> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateSubContainer(param1, param2, param3).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2, param3);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<IBinder, TParam1, TParam2, TParam3, TParam4> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateSubContainer(param1, param2, param3, param4).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2, param3, param4);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<IBinder, TParam1, TParam2, TParam3, TParam4, TParam5> _containerInitializer = null;

        public TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreateSubContainer(param1, param2, param3, param4, param5).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2, param3, param4, param5);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<IBinder, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _containerInitializer = null;

        public TFacade Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4,
            TParam5 param5, TParam6 param6)
        {
            return CreateSubContainer(param1, param2, param3, param4, param5, param6).Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5,
            TParam6 param6)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2, param3, param4, param5, param6);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(
                default(TParam1), default(TParam2), default(TParam3), default(TParam4),
                default(TParam5), default(TParam6));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TFacade>
        : IFacadeFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TFacade>
    {
        [Inject]
        DiContainer _container = null;

        [Inject]
        ModestTree.Util.Action<IBinder, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> _containerInitializer = null;

        public TFacade Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5,
            TParam6 param6, TParam7 param7)
        {
            return CreateSubContainer(param1, param2, param3, param4, param5, param6, param7)
                .Resolver.Resolve<TFacade>();
        }

        DiContainer CreateSubContainer(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5,
            TParam6 param6, TParam7 param7)
        {
            Assert.IsNotNull(_containerInitializer);
            var subContainer = _container.CreateSubContainer();
            _containerInitializer(subContainer, param1, param2, param3, param4, param5, param6, param7);
            return subContainer;
        }

        IEnumerable<ZenjectResolveException> IValidatable.Validate()
        {
            var subContainer = CreateSubContainer(
                default(TParam1), default(TParam2), default(TParam3), default(TParam4),
                default(TParam5), default(TParam6), default(TParam7));

            foreach (var error in subContainer.Resolver.ValidateResolve<TFacade>())
            {
                yield return error;
            }

            foreach (var error in subContainer.Resolver.ValidateValidatables())
            {
                yield return error;
            }
        }
    }
}
