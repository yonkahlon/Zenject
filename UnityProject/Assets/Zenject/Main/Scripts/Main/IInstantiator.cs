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
        object Instantiate(
            Type concreteType, IEnumerable<TypeValuePair> extraArgMapParam);

        object InstantiateExplicit(
            Type concreteType, List<TypeValuePair> extraArgMap);
    }

    public static class InstantiatorExtensions
    {
        public static T Instantiate<T>(
            this IInstantiator instantiator, params object[] extraArgs)
        {
            return (T)instantiator.Instantiate(typeof(T), extraArgs);
        }

        public static object Instantiate(
            this IInstantiator instantiator, Type concreteType, params object[] extraArgs)
        {
            Assert.That(!extraArgs.Contains(null),
                "Null value given to factory constructor arguments when instantiating object with type '{0}'. In order to use null use InstantiateExplicit", concreteType);

            return instantiator.InstantiateExplicit(
                concreteType, InstantiateUtil.CreateTypeValueList(extraArgs));
        }

        // This is used instead of Instantiate to support specifying null values
        public static T InstantiateExplicit<T>(
            this IInstantiator instantiator, List<TypeValuePair> extraArgMap)
        {
            return (T)instantiator.InstantiateExplicit(typeof(T), extraArgMap);
        }
    }
}

