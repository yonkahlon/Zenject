using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    public class FieldsInjecter
    {
        public static void Inject(DiContainer container, object injectable)
        {
            Inject(container, injectable, new List<object>());
        }

        public static void Inject(DiContainer container, object injectable, List<object> additional)
        {
            Assert.That(injectable != null);

            var fields = InjectionInfoHelper.GetFieldDependencies(injectable.GetType());

            var parentDependencies = new List<Type>(container.LookupsInProgress);

            var additionalCopy = additional.ToList();

            foreach (var fieldInfo in fields)
            {
                var injectInfo = InjectionInfoHelper.GetInjectInfo(fieldInfo);
                Assert.That(injectInfo != null);

                bool foundAdditional = false;
                foreach (object obj in additionalCopy)
                {
                    if (fieldInfo.FieldType.IsAssignableFrom(obj.GetType()))
                    {
                        fieldInfo.SetValue(injectable, obj);
                        additionalCopy.Remove(obj);
                        foundAdditional = true;
                        break;
                    }
                }

                if (foundAdditional)
                {
                    continue;
                }

                var context = new ResolveContext()
                {
                    Target = injectable.GetType(),
                    FieldName = fieldInfo.Name,
                    Identifier = injectInfo.Identifier,
                    Parents = parentDependencies,
                    TargetInstance = injectable,
                };

                var valueObj = ResolveField(container, fieldInfo, context, injectInfo, injectable);

                fieldInfo.SetValue(injectable, valueObj);
            }

            if (!additionalCopy.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '" + injectable.GetType().GetPrettyName()
                    + "'. \nObject graph:\n" + container.GetCurrentObjectGraph());
            }
        }

        static object ResolveField(
            DiContainer container,
            FieldInfo fieldInfo, ResolveContext context,
            InjectInfo injectInfo, object injectable)
        {
            var desiredType = fieldInfo.FieldType;

            if (container.HasBinding(desiredType, context))
            {
                return container.Resolve(desiredType, context);
            }

            // Dependencies that are lists are only optional if declared as such using the inject attribute
            bool isOptional = (injectInfo == null ? false : injectInfo.Optional);

            // If it's a list it might map to a collection
            if (ReflectionUtil.IsGenericList(desiredType))
            {
                var subType = desiredType.GetGenericArguments().Single();

                return container.ResolveMany(subType, context, isOptional);
            }

            if (!isOptional)
            {
                throw new ZenjectResolveException(
                    "Unable to find field with type '" + fieldInfo.FieldType +
                    "' when injecting dependencies into '" + injectable +
                    "'. \nObject graph:\n" + container.GetCurrentObjectGraph());
            }

            return null;
        }
    }
}
