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
        readonly IBinder _binder;
        readonly string _identifier;

        public IFactoryBinder(IBinder container, string identifier)
        {
            _binder = container;
            _identifier = identifier;
        }

        public BindingConditionSetter ToMethod(
            Func<IInstantiator, TParam1, TContract> method)
        {
            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Instantiator.Instantiate<FactoryMethod<TParam1, TContract>>(method));
        }

        public BindingConditionSetter ToFactory()
        {
            Assert.That(!typeof(TContract).IsAbstract);
            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToTransient<Factory<TParam1, TContract>>();
        }

        public BindingConditionSetter ToFactory<TConcrete>()
            where TConcrete : TContract
        {
            // We'd prefer to just call this but this fails due to what looks like a bug with Mono
            //return ToCustomFactory<TConcrete, Factory<TParam1, TConcrete>>();

            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        c.Instantiator.Instantiate<Factory<TParam1, TConcrete>>()));
        }

        // Note that we assume here that IFactory<TConcrete> is bound somewhere else
        public BindingConditionSetter ToIFactory<TConcrete>()
            where TConcrete : TContract
        {
            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        c.Resolver.Resolve<IFactory<TParam1, TConcrete>>()));
        }

        public BindingConditionSetter ToCustomFactory<TFactory>()
            where TFactory : IFactory<TParam1, TContract>
        {
            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier).ToTransient<TFactory>();
        }

        public BindingConditionSetter ToCustomFactory<TConcrete, TFactory>()
            where TFactory : IFactory<TParam1, TConcrete>
            where TConcrete : TContract
        {
            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod(c =>
                    new FactoryNested<TParam1, TContract, TConcrete>(
                        c.Instantiator.Instantiate<TFactory>()));
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

            return _binder.Bind<IFactory<TParam1, TContract>>(_identifier)
                .ToMethod((ctx) => ctx.Instantiator.Instantiate<GameObjectFactory<TParam1, TContract>>(prefab));
        }
#endif
    }
}


