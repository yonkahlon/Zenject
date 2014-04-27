using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class should ONLY be used the following way:
    //
    //  using (new LookupInProgressAdder(container))
    //  {
    //      new PropertiesInjector()
    //      container.Resolve()
    //      ...  etc.
    //  }
    public class LookupInProgressAdder : IDisposable
    {
        DiContainer _container;
        Type _concreteType;

        public LookupInProgressAdder(DiContainer container, Type concreteType)
        {
            if (container.LookupsInProgress.Contains(concreteType))
            {
                Assert.That(false, () => "Circular dependency detected! \nObject graph:\n" + container.GetCurrentObjectGraph());
            }

            container.LookupsInProgress.Push(concreteType);

            _container = container;
            _concreteType = concreteType;
        }

        public void Dispose()
        {
            Assert.That(_container.LookupsInProgress.Peek() == _concreteType);
            _container.LookupsInProgress.Pop();
        }
    }
}

