using System;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    ////////////////////////////// One parameter //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TParam1, TContract> : IIFactoryBinder<TParam1, TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;
        readonly ContainerTypes _containerType;

        public IFactoryBinder(
            DiContainer container, string identifier, ContainerTypes containerType)
        {
            _container = container;
            _identifier = identifier;
            _containerType = containerType;
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TParam1, TContract> method)
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((c) => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container)
                        .Instantiate<FactoryMethod<TParam1, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TContract>>(_containerType);
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            // We'd prefer to just call this but this fails due to what looks like a bug with Mono
            //return ToCustomFactory<TConcrete, Factory<TParam1, TConcrete>>();

            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<Factory<TParam1, TConcrete>>()));
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Resolve<IFactory<TParam1, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TContract>
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier).ToTransient<TFactory>(_containerType);
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>()));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(facadeInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer, TParam1> facadeInstaller)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TParam1, TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(facadeInstaller)));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory
            where TInstaller : Installer
        {
            return ToFacadeFactoryInstaller<TFactory>(typeof(TInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer
        {
            return ToFacadeFactoryInstaller<TConcrete, TFactory>(typeof(TInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TContract>, IFacadeFactory
        {
            if (!installerType.DerivesFromOrEqual<Installer>())
            {
                throw new ZenjectBindException(
                    "Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((c) => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(installerType));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TParam1, TConcrete>, IFacadeFactory
            where TConcrete : TContract
        {
            if (!installerType.DerivesFromOrEqual<Installer>())
            {
                throw new ZenjectBindException(
                    "Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TParam1, TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(installerType)));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            Assert.That(typeof(TContract).DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory< {0} >().ToPrefab".Fmt(typeof(TContract).Name()));
            }

            return _container.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((c) => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<MonoBehaviourFactory<TParam1, TContract>>(prefab));
        }
#endif
    }
}


