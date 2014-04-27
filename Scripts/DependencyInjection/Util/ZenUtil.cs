using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ZenUtil
    {
        public static InjectInfo GetInjectInfo(ParameterInfo param)
        {
            return GetInjectInfoInternal(param.GetCustomAttributes(typeof(InjectAttributeBase), true));
        }

        public static InjectInfo GetInjectInfo(MemberInfo member)
        {
            return GetInjectInfoInternal(member.GetCustomAttributes(typeof(InjectAttributeBase), true));
        }

        static InjectInfo GetInjectInfoInternal(object[] attributes)
        {
            if (!attributes.Any())
            {
                return null;
            }

            var info = new InjectInfo();

            foreach (var attr in attributes)
            {
                if (attr.GetType() == typeof(InjectOptionalAttribute))
                {
                    info.Optional = true;
                }
                else if (attr.GetType() == typeof(InjectNamedAttribute))
                {
                    var namedAttr = (InjectNamedAttribute)attr;
                    info.Name = namedAttr.Name;
                }
            }

            return info;
        }

        public static IEnumerable<FieldInfo> GetFieldDependencies(Type type)
        {
            return type.GetFieldsWithAttribute<InjectAttributeBase>();
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType)
        {
            ConstructorInfo method;
            return GetConstructorDependencies(concreteType, out method);
        }

        public static ParameterInfo[] GetConstructorDependencies(Type concreteType, out ConstructorInfo method)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            Assert.That(constructors.Length > 0,
                "Could not find constructor for type '" + concreteType + "' when creating dependencies");

            if (constructors.Length > 1)
            {
                method = (from c in constructors where c.GetCustomAttributes(typeof(InjectAttribute), true).Any() select c).SingleOrDefault();

                Assert.IsNotNull(method,
                    "More than one constructor found for type '{0}' when creating dependencies.  Use [Inject] attribute to specify which to use.", concreteType);
            }
            else
            {
                method = constructors[0];
            }

            return method.GetParameters();
        }

        public class InjectInfo
        {
            public bool Optional;
            public string Name;
        }
    }
}
