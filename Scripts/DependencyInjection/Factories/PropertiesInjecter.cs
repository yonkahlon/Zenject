using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    public class PropertiesInjecter
    {
        DiContainer _container;
        List<object> _additional;

        public PropertiesInjecter(DiContainer container)
        {
            _container = container;
            _additional = new List<object>();
        }

        public PropertiesInjecter(DiContainer container, List<object> additional)
        {
            _container = container;
            _additional = additional;
        }

        public void Inject(object injectable)
        {
            Assert.That(injectable != null);

            var fields = ZenUtil.GetFieldDependencies(injectable.GetType());

            var parentDependencies = new List<Type>(_container.LookupsInProgress);

            foreach (var fieldInfo in fields)
            {
                var injectInfo = ZenUtil.GetInjectInfo(fieldInfo);
                Assert.That(injectInfo != null);

                bool foundAdditional = false;
                foreach (object obj in _additional)
                {
                    if (fieldInfo.FieldType.IsAssignableFrom(obj.GetType()))
                    {
                        fieldInfo.SetValue(injectable, obj);
                        _additional.Remove(obj);
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
                    Name = injectInfo.Name,
                    Parents = parentDependencies,
                    TargetInstance = injectable,
                };

                var valueObj = ResolveField(fieldInfo, context, injectInfo, injectable);

                fieldInfo.SetValue(injectable, valueObj);
            }
        }

        object ResolveField(
            FieldInfo fieldInfo, ResolveContext context,
            ZenUtil.InjectInfo injectInfo, object injectable)
        {
            var desiredType = fieldInfo.FieldType;

            if (_container.HasBinding(desiredType, context))
            {
                return _container.Resolve(desiredType, context);
            }

            // Dependencies that are lists are only optional if declared as such using the inject attribute
            bool isOptional = (injectInfo == null ? false : injectInfo.Optional);

            // If it's a list it might map to a collection
            if (ReflectionUtil.IsGenericList(desiredType))
            {
                var subType = desiredType.GetGenericArguments().Single();

                return _container.ResolveMany(subType, context, isOptional);
            }

            if (!isOptional)
            {
                throw new ZenjectResolveException(
                    "Unable to find field with type '" + fieldInfo.FieldType +
                    "' when injecting dependencies into '" + injectable +
                    "'. \nObject graph:\n" + _container.GetCurrentObjectGraph());
            }

            return null;
        }
    }
}
