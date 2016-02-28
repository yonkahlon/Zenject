using System;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    ////////////////////////////// Zero parameters //////////////////////////////
    [System.Diagnostics.DebuggerStepThrough]
    public class IFactoryBinder<TContract> : IIFactoryBinder<TContract>
    {
        readonly DiContainer _container;
        readonly string _identifier;
        readonly ContainerTypes _containerType;

        public IFactoryBinder(
            DiContainer binder, string identifier, ContainerTypes containerType)
        {
            _container = binder;
            _identifier = identifier;
            _containerType = containerType;
        }

        public BindingConditionSetter ToInstance(TContract instance)
        {
            return ToMethod((c) => instance);
        }

        public BindingConditionSetter ToMethod(
            Func<DiContainer, TContract> method)
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((c) => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<FactoryMethod<TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract,
                "Unable to create abstract type '{0}' in Factory", typeof(TContract).Name());
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToTransient<Factory<TContract>>(_containerType);
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            // We'd prefer to just call this but this fails due to what looks like a bug with Mono
            //return ToCustomFactory<TConcrete, Factory<TConcrete>>();

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<Factory<TConcrete>>()));
        }

        public BindingConditionSetter ToFactory(Type concreteType)
        {
            Assert.DerivesFrom<TContract>(concreteType);

            var genericType = typeof(Factory<>).MakeGenericType(concreteType);
            return ToCustomFactory(concreteType, genericType);
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Resolve<IFactory<TConcrete>>()));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TContract>, IFacadeFactory
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(facadeInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<DiContainer> facadeInstaller)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(facadeInstaller)));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TFactory, TInstaller>()
            where TFactory : IFactory<TContract>, IFacadeFactory
            where TInstaller : Installer
        {
            return ToFacadeFactoryInstaller<TFactory>(typeof(TInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory, TInstaller>()
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
            where TInstaller : Installer
        {
            return ToFacadeFactoryInstaller<TConcrete, TFactory>(typeof(TInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TFactory>(Type installerType)
            where TFactory : IFactory<TContract>, IFacadeFactory
        {
            if (!installerType.DerivesFromOrEqual<Installer>())
            {
                throw new ZenjectBindException(
                    "Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(installerType));
        }

        public BindingConditionSetter ToFacadeFactoryInstaller<TConcrete, TFactory>(Type installerType)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
        {
            if (!installerType.DerivesFromOrEqual<Installer>())
            {
                throw new ZenjectBindException(
                    "Expected type '{0}' to derive from 'Installer'".Fmt(installerType.Name()));
            }

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>(installerType)));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return ToCustomFactory(typeof(TFactory));
        }

        public BindingConditionSetter ToCustomFactory(Type factoryType)
        {
            Assert.DerivesFrom<IFactory<TContract>>(factoryType);
            return _container.Bind<IFactory<TContract>>(_identifier).ToTransient(factoryType, _containerType);
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>((_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container).Instantiate<TFactory>()));
        }

        public BindingConditionSetter ToCustomFactory(Type concreteType, Type factoryType)
        {
            var genericIFactoryType = typeof(IFactory<>).MakeGenericType(concreteType);
            Assert.That(factoryType.DerivesFrom(genericIFactoryType));

            Assert.DerivesFrom<TContract>(concreteType);

            var genericFactoryNestedType = typeof(FactoryNested<,>).MakeGenericType(typeof(TContract), concreteType);

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c =>
                    {
                        var container = (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container);
                        return (IFactory<TContract>)container.Instantiate(
                            genericFactoryNestedType, container.Instantiate(factoryType));
                    });
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToPrefab(GameObject prefab)
        {
            var contractType = typeof(TContract);
            Assert.That(contractType.IsInterface || contractType.DerivesFrom<Component>());

            if (prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab provided to BindIFactory< {0} >().ToPrefab".Fmt(contractType.Name()));
            }

            return _container.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((c) => (_containerType == ContainerTypes.RuntimeContainer ? c.Container : _container)
                    .Instantiate<MonoBehaviourFactory<TContract>>(prefab));
        }
#endif
    }
}



