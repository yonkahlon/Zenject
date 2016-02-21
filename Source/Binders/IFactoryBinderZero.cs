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
        readonly IBinder _binder;
        readonly string _identifier;

        public IFactoryBinder(IBinder binder, string identifier)
        {
            _binder = binder;
            _identifier = identifier;
        }

        public BindingConditionSetter ToInstance(TContract instance)
        {
            return ToMethod((c) => instance);
        }

        public BindingConditionSetter ToMethod(
            Func<IInstantiator, TContract> method)
        {
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Instantiator.Instantiate<FactoryMethod<TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract,
                "Unable to create abstract type '{0}' in Factory", typeof(TContract).Name());
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToTransient<Factory<TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            // We'd prefer to just call this but this fails due to what looks like a bug with Mono
            //return ToCustomFactory<TConcrete, Factory<TConcrete>>();

            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Instantiator.Instantiate<Factory<TConcrete>>()));
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
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Resolver.Resolve<IFactory<TConcrete>>()));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TFactory>(Action<IBinder> facadeInstaller)
            where TFactory : IFactory<TContract>, IFacadeFactory
        {
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => c.Instantiator.Instantiate<TFactory>(facadeInstaller));
        }

        public BindingConditionSetter ToFacadeFactoryMethod<TConcrete, TFactory>(Action<IBinder> facadeInstaller)
            where TFactory : IFactory<TConcrete>, IFacadeFactory
            where TConcrete : TContract
        {
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Instantiator.Instantiate<TFactory>(facadeInstaller)));
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

            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => c.Instantiator.Instantiate<TFactory>(installerType));
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

            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Instantiator.Instantiate<TFactory>(installerType)));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return ToCustomFactory(typeof(TFactory));
        }

        public BindingConditionSetter ToCustomFactory(Type factoryType)
        {
            Assert.DerivesFrom<IFactory<TContract>>(factoryType);
            return _binder.Bind<IFactory<TContract>>(_identifier).ToTransient(factoryType);
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => new FactoryNested<TContract, TConcrete>(c.Instantiator.Instantiate<TFactory>()));
        }

        public BindingConditionSetter ToCustomFactory(Type concreteType, Type factoryType)
        {
            var genericIFactoryType = typeof(IFactory<>).MakeGenericType(concreteType);
            Assert.That(factoryType.DerivesFrom(genericIFactoryType));

            Assert.DerivesFrom<TContract>(concreteType);

            var genericFactoryNestedType = typeof(FactoryNested<,>).MakeGenericType(typeof(TContract), concreteType);

            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod(c => (IFactory<TContract>)c.Instantiator.Instantiate(genericFactoryNestedType, c.Instantiator.Instantiate(factoryType)));
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

            return _binder.Bind<IFactory<TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Instantiator.Instantiate<GameObjectFactory<TContract>>(prefab));
        }
#endif
    }
}



