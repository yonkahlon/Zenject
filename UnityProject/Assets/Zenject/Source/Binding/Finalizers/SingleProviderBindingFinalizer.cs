namespace Zenject
{
    public class SingleProviderBindingFinalizer : ProviderBindingFinalizer
    {
        readonly IProvider _provider;

        public SingleProviderBindingFinalizer(
            BindInfo bindInfo, IProvider provider)
            : base(bindInfo)
        {
            _provider = provider;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            RegisterProviderForAllContracts(container, _provider);
        }
    }
}

