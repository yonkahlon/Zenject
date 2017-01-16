using System;
using ModestTree;

namespace Zenject
{
    public class PooledFactoryBindingFinalizer<TContract> : ProviderBindingFinalizer
    {
        readonly PooledFactoryBindInfo _poolBindInfo;
        readonly FactoryBindInfo _factoryBindInfo;

        public PooledFactoryBindingFinalizer(
            BindInfo bindInfo, FactoryBindInfo factoryBindInfo, PooledFactoryBindInfo poolBindInfo)
            : base(bindInfo)
        {
            // Note that it doesn't derive from PooledFactory<TContract>
            // when used with To<>, so we can only check IDynamicPooledFactory
            Assert.That(factoryBindInfo.FactoryType.DerivesFrom<IDynamicPooledFactory>());

            _factoryBindInfo = factoryBindInfo;
            _poolBindInfo = poolBindInfo;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            var provider = _factoryBindInfo.ProviderFunc(container);

            RegisterProviderForAllContracts(
                container,
                new CachedProvider(
                    new TransientProvider(
                        _factoryBindInfo.FactoryType,
                        container,
                        InjectUtil.CreateArgListExplicit(
                            typeof(TContract), provider, _poolBindInfo.InitialSize, _poolBindInfo.ExpandMethod),
                        null,
                        BindInfo.ContextInfo)));
        }
    }
}

