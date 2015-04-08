using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModestTree;
using ModestTree.Util;
using ModestTree.Util.Debugging;

namespace Zenject
{
    public interface IInstantiator
    {
        object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap, InjectContext currentContext);
    }

    public static class InstantiatorExtensions
    {
        public static T Instantiate<T>(
            this DiContainer container, params object[] extraArgs)
        {
            return (T)container.Instantiate(typeof(T), extraArgs);
        }

        public static object Instantiate(
            this DiContainer container, Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return container.InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        public static T InstantiateExplicit<T>(
            this DiContainer container, List<TypeValuePair> extraArgMap)
        {
            return (T)container.InstantiateExplicit(typeof(T), extraArgMap);
        }

        public static object InstantiateExplicit(
            this DiContainer container, Type concreteType, List<TypeValuePair> extraArgMap)
        {
            return container.InstantiateExplicit(
                concreteType, extraArgMap, new InjectContext(container, concreteType, null));
        }
    }
}
