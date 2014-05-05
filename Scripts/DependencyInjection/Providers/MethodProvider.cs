using System;
namespace ModestTree.Zenject
{
    public class MethodProvider<T> : ProviderBase
    {
        readonly DiContainer _container;
        readonly Func<DiContainer, T> _method;

        public MethodProvider(Func<DiContainer, T> method, DiContainer container)
        {
            _method = method;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            var obj = _method(_container);

            Assert.That(obj != null, () =>
                "Method provider returned null when looking up type '" +
                typeof(T).GetPrettyName() + "'. \nObject graph:\n" +
                _container.GetCurrentObjectGraph());

            return obj;
        }
    }
}
