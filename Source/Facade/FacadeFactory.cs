using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public interface IFacadeFactory : IValidatable
    {
    }

    public abstract class FacadeFactoryBase<TFacade> : IFacadeFactory
    {
        [Inject]
        protected DiContainer _container = null;

        [InjectOptional]
        protected Type _installerType = null;

        public abstract IEnumerable<ZenjectResolveException> Validate();
    }

    // Zero Parameters

    public abstract class FacadeFactory<TFacade> : FacadeFactoryBase<TFacade>, IFactory<TFacade>
    {
        [InjectOptional]
        Action<DiContainer> _installMethod = null;

        public virtual TFacade Create()
        {
            return CreateSubContainer().Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer()
        {
            // Unlike other FacadeFactory's, these ones are in a util class for use
            // by FacadeMethodSingletonLazyCreator
            return FacadeUtil.CreateSubContainer(
                _container, _installMethod, _installerType);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            // Unlike other FacadeFactory's, these ones are in a util class for use
            // by FacadeMethodSingletonLazyCreator
            return FacadeUtil.Validate(
                typeof(TFacade), _container, _installMethod, _installerType);
        }
    }

    // One Parameter

    public abstract class FacadeFactory<TParam1, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TFacade>
    {
        [InjectOptional]
        Action<DiContainer, TParam1> _installMethod = null;

        public virtual TFacade Create(TParam1 param1)
        {
            return CreateSubContainer(param1).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(TParam1 param1)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1));

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

    // Two Parameters

    public abstract class FacadeFactory<TParam1, TParam2, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TParam2, TFacade>
    {
        [InjectOptional]
        Action<DiContainer, TParam1, TParam2> _installMethod = null;

        public virtual TFacade Create(TParam1 param1, TParam2 param2)
        {
            return CreateSubContainer(param1, param2).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(TParam1 param1, TParam2 param2)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1, param2);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1, param2));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2));

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

    // Three Parameters

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TParam2, TParam3, TFacade>
    {
        [InjectOptional]
        Action<DiContainer, TParam1, TParam2, TParam3> _installMethod = null;

        public virtual TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateSubContainer(param1, param2, param3).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1, param2, param3);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1, param2, param3));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3));

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

    // Four Parameters

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TParam2, TParam3, TParam4, TFacade>
    {
        [InjectOptional]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4> _installMethod = null;

        public virtual TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateSubContainer(param1, param2, param3, param4).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1, param2, param3, param4);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1, param2, param3, param4));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4));

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

    // Five Parameters

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TFacade>
    {
        [InjectOptional]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> _installMethod = null;

        public virtual TFacade Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreateSubContainer(param1, param2, param3, param4, param5).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1, param2, param3, param4, param5);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1, param2, param3, param4, param5));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5));

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

    // Six Parameters

    public abstract class FacadeFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>
        : FacadeFactoryBase<TFacade>, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TFacade>
    {
        [InjectOptional]
        ModestTree.Util.Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _installMethod = null;

        public virtual TFacade Create(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4,
            TParam5 param5, TParam6 param6)
        {
            return CreateSubContainer(param1, param2, param3, param4, param5, param6).Resolve<TFacade>();
        }

        protected DiContainer CreateSubContainer(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5,
            TParam6 param6)
        {
            Assert.That(_installMethod != null || _installerType != null);

            var subContainer = _container.CreateSubContainer();

            if (_installMethod != null)
            {
                Assert.IsNull(_installerType);

                _installMethod(subContainer, param1, param2, param3, param4, param5, param6);
            }

            if (_installerType != null)
            {
                Assert.IsNull(_installMethod);

                subContainer.InstallExplicit(
                    _installerType, InstantiateUtil.CreateTypeValueListExplicit(param1, param2, param3, param4, param5, param6));
            }

            return subContainer;
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            var subContainer = CreateSubContainer(
                default(TParam1), default(TParam2), default(TParam3), default(TParam4),
                default(TParam5), default(TParam6));

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
