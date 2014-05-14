using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ModestTree.Zenject
{
    public static class Instantiator
    {
        public static T Instantiate<T>(
            DiContainer container, params object[] constructorArgs)
        {
            return (T)Instantiate(container, typeof(T), constructorArgs);
        }

        public static object Instantiate(
            DiContainer container, Type concreteType, params object[] constructorArgs)
        {
            using (container.PushLookup(concreteType))
            {
                return InstantiateInternal(container, concreteType, constructorArgs);
            }
        }

        static object InstantiateInternal(
            DiContainer container, Type concreteType, params object[] constructorArgs)
        {
            ConstructorInfo method;
            var injectInfos = InjectablesFinder.GetConstructorInjectables(concreteType, out method);

            var paramValues = new List<object>();
            var extrasList = new List<object>(constructorArgs);

            Assert.That(!extrasList.Contains(null),
                "Null value given to factory constructor arguments. This is currently not allowed");

            foreach (var injectInfo in injectInfos)
            {
                var found = false;

                foreach (var extra in extrasList)
                {
                    if (extra.GetType().DerivesFrom(injectInfo.ContractType))
                    {
                        found = true;
                        paramValues.Add(extra);
                        extrasList.Remove(extra);
                        break;
                    }
                }

                if (!found)
                {
                    paramValues.Add(container.Resolve(injectInfo));
                }
            }

            object newObj;

            try
            {
                newObj = method.Invoke(paramValues.ToArray());
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    e, "Error occurred while instantiating object with type '" + concreteType.GetPrettyName() + "'");
            }

            FieldsInjecter.Inject(container, newObj, extrasList, true);

            return newObj;
        }

        static object ResolveFromType(
            DiContainer container, ResolveContext context, object injectable, InjectableInfo injectInfo)
        {
            if (container.HasBinding(injectInfo.ContractType, context))
            {
                return container.Resolve(injectInfo.ContractType, context);
            }

            // If it's a list it might map to a collection
            if (ReflectionUtil.IsGenericList(injectInfo.ContractType))
            {
                var subType = injectInfo.ContractType.GetGenericArguments().Single();
                return container.ResolveMany(subType, context, injectInfo.Optional);
            }

            if (!injectInfo.Optional)
            {
                throw new ZenjectResolveException(
                    "Unable to find field with type '{0}' when injecting dependencies into '{1}'. \nObject graph:\n {2}",
                    injectInfo.ContractType, injectable, container.GetCurrentObjectGraph());
            }

            return null;
        }
    }
}
