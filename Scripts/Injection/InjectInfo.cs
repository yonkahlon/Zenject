using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    internal class InjectInfo
    {
        public bool Optional;
        public object Identifier;
        public string Name;
        public Type ContractType;
        public Type ContainedType;
        // Null for constructor declared dependencies
        public Action<object, object> Setter;
    }

    // Helper to find data that needs to be injected
    internal static class InjectionInfoHelper
    {
        public static IEnumerable<MethodInfo> GetPostInjectMethods(Type type)
        {
            return type.MethodsWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(PostInjectAttribute));
        }

        public static IEnumerable<InjectInfo> GetAllDependencies(Type type)
        {
            return GetAllDependencies(type, true);
        }

        public static IEnumerable<InjectInfo> GetAllDependencies(Type type, bool strict)
        {
            return GetConstructorDependencies(type, strict)
                .Append(GetFieldAndPropertyDependencies(type));
        }

        public static IEnumerable<InjectInfo> GetFieldAndPropertyDependencies(Type type)
        {
            return GetFieldDependencies(type).Append(GetPropertyDependencies(type));
        }

        public static IEnumerable<InjectInfo> GetPropertyDependencies(Type type)
        {
            var propInfos = type.PropertiesWith(
                Fasterflect.Flags.InstanceAnyVisibility | Fasterflect.Flags.ExcludeBackingMembers,
                typeof(InjectAttribute), typeof(InjectOptionalAttribute));

            foreach (var propInfo in propInfos)
            {
                yield return CreateInjectInfoForMember(propInfo, type);
            }
        }

        public static IEnumerable<InjectInfo> GetFieldDependencies(Type type)
        {
            var fieldInfos = type.FieldsWith(
                Fasterflect.Flags.InstanceAnyVisibility, typeof(InjectAttribute), typeof(InjectOptionalAttribute));

            foreach (var fieldInfo in fieldInfos)
            {
                yield return CreateInjectInfoForMember(fieldInfo, type);
            }
        }

        public static IEnumerable<InjectInfo> GetConstructorDependencies(Type containedType)
        {
            return GetConstructorDependencies(containedType, true);
        }

        public static IEnumerable<InjectInfo> GetConstructorDependencies(
            Type containedType, bool strict)
        {
            ConstructorInfo method;
            return GetConstructorDependencies(containedType, out method, strict);
        }

        public static IEnumerable<InjectInfo> GetConstructorDependencies(Type containedType, out ConstructorInfo method)
        {
            return GetConstructorDependencies(containedType, out method, true);
        }

        public static IList<InjectInfo> GetConstructorDependencies(
            Type containedType, out ConstructorInfo method, bool strict)
        {
            var constructors = containedType.Constructors(Flags.Public | Flags.InstanceAnyVisibility);

            if (constructors.IsEmpty())
            {
                method = null;
                return new List<InjectInfo>();
            }

            if (constructors.HasMoreThan(1))
            {
                method = (from c in constructors where c.HasAnyAttribute(typeof(InjectAttribute)) select c).SingleOrDefault();

                if (!strict && method == null)
                {
                    return new List<InjectInfo>();
                }

                Assert.IsNotNull(method,
                    "More than one constructor found for type '{0}' when creating dependencies.  Use [Inject] attribute to specify which to use.", containedType);
            }
            else
            {
                method = constructors[0];
            }

            return method.Parameters().Select(paramInfo => CreateInjectInfoForConstructorParam(paramInfo, containedType)).ToList();
        }

        static InjectInfo CreateInjectInfoForConstructorParam(ParameterInfo paramInfo, Type containedType)
        {
            var injectAttr = paramInfo.Attribute<InjectAttribute>();

            return new InjectInfo()
            {
                Optional = paramInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                Name = paramInfo.Name,
                ContractType = paramInfo.ParameterType,
                ContainedType = containedType,
            };
        }

        static InjectInfo CreateInjectInfoForMember(MemberInfo memInfo, Type containedType)
        {
            var injectAttr = memInfo.Attribute<InjectAttribute>();

            var info = new InjectInfo()
            {
                Optional = memInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                Name = memInfo.Name,
                ContainedType = containedType,
            };

            if (memInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memInfo;
                info.Setter = ((object injectable, object value) => fieldInfo.SetValue(injectable, value));
                info.ContractType = fieldInfo.FieldType;
            }
            else
            {
                Assert.That(memInfo is PropertyInfo);
                var propInfo = (PropertyInfo)memInfo;
                info.Setter = ((object injectable, object value) => propInfo.SetValue(injectable, value, null));
                info.ContractType = propInfo.PropertyType;
            }

            return info;
        }
    }
}
