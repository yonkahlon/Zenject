using System;
namespace ModestTree.Zenject
{
    public class MethodProvider<T> : ProviderBase
    {
        public delegate T Method(DiContainer c);

        readonly DiContainer _container;
        readonly Method _method;

        public MethodProvider(Method method, DiContainer container)
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
                "Method provider returned null when looking up type '" + typeof(T).GetPrettyName() + "'. \nObject graph:\n" + _container.GetCurrentObjectGraph());
            return obj;
        }
    }
}
