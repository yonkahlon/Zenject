using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ModestTree;

namespace Zenject
{
    internal static class TypeAnalyzer
    {
        static Dictionary<Type, ZenjectTypeInfo> _typeInfo = new Dictionary<Type, ZenjectTypeInfo>();

        public static ZenjectTypeInfo GetInfo(Type type)
        {
            ZenjectTypeInfo info;

            if (!_typeInfo.TryGetValue(type, out info))
            {
                info = CreateTypeInfo(type);
                _typeInfo.Add(type, info);
            }

            return info;
        }

        static ZenjectTypeInfo CreateTypeInfo(Type type)
        {
            var constructor = GetInjectConstructor(type);

            return new ZenjectTypeInfo(
                type,
                GetPostInjectMethods(type),
                constructor,
                GetFieldInjectables(type).ToList(),
                GetPropertyInjectables(type).ToList(),
                GetConstructorInjectables(type, constructor).ToList());
        }

        static IEnumerable<InjectableInfo> GetConstructorInjectables(Type parentType, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
            {
                return Enumerable.Empty<InjectableInfo>();
            }

            return constructorInfo.GetParameters().Select(
                paramInfo => CreateInjectableInfoForParam(parentType, paramInfo));
        }

        static InjectableInfo CreateInjectableInfoForParam(
            Type parentType, ParameterInfo paramInfo)
        {
            var identifier = paramInfo.AllAttributes<InjectAttribute>().Select(x => x.Identifier)
                .Concat(paramInfo.AllAttributes<InjectOptionalAttribute>().Select(x => x.Identifier)).FirstOrDefault();

            return new InjectableInfo(
                paramInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                identifier,
                paramInfo.Name,
                paramInfo.ParameterType,
                parentType,
                null);
        }

        static List<PostInjectableInfo> GetPostInjectMethods(Type type)
        {
            // Note that unlike with fields and properties we use GetCustomAttributes
            // This is so that we can ignore inherited attributes, which is necessary
            // otherwise a base class method marked with [Inject] would cause all overridden
            // derived methods to be added as well
            var methods = type.GetAllMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(PostInjectAttribute), false).Any()).ToList();

            var heirarchyList = type.Yield().Concat(type.GetParentTypes()).Reverse().ToList();

            // Order by base classes first
            // This is how constructors work so it makes more sense
            var values = methods.OrderBy(x => heirarchyList.IndexOf(x.DeclaringType));

            var postInjectInfos = new List<PostInjectableInfo>();

            foreach (var methodInfo in values)
            {
                var paramsInfo = methodInfo.GetParameters();

                postInjectInfos.Add(
                    new PostInjectableInfo(
                        methodInfo,
                        paramsInfo.Select(paramInfo =>
                            CreateInjectableInfoForParam(type, paramInfo)).ToList()));
            }

            return postInjectInfos;
        }

        static IEnumerable<InjectableInfo> GetPropertyInjectables(Type type)
        {
            var propInfos = type.GetAllProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.HasAttribute(typeof(InjectAttribute), typeof(InjectOptionalAttribute)));

            foreach (var propInfo in propInfos)
            {
                yield return CreateForMember(propInfo, type);
            }
        }

        static IEnumerable<InjectableInfo> GetFieldInjectables(Type type)
        {
            var fieldInfos = type.GetAllFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.HasAttribute(typeof(InjectAttribute), typeof(InjectOptionalAttribute)));

            foreach (var fieldInfo in fieldInfos)
            {
                yield return CreateForMember(fieldInfo, type);
            }
        }

        static InjectableInfo CreateForMember(MemberInfo memInfo, Type parentType)
        {
            var identifier = memInfo.AllAttributes<InjectAttribute>().Select(x => x.Identifier)
                .Concat(memInfo.AllAttributes<InjectOptionalAttribute>().Select(x => x.Identifier)).FirstOrDefault();

            Type memberType;
            Action<object, object> setter;

            if (memInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memInfo;
                setter = ((object injectable, object value) => fieldInfo.SetValue(injectable, value));
                memberType = fieldInfo.FieldType;
            }
            else
            {
                Assert.That(memInfo is PropertyInfo);
                var propInfo = (PropertyInfo)memInfo;
                setter = ((object injectable, object value) => propInfo.SetValue(injectable, value, null));
                memberType = propInfo.PropertyType;
            }

            return new InjectableInfo(
                memInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                identifier,
                memInfo.Name,
                memberType,
                parentType,
                setter);
        }

        static ConstructorInfo GetInjectConstructor(Type parentType)
        {
            var constructors = parentType.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.IsEmpty())
            {
                return null;
            }

            if (constructors.HasMoreThan(1))
            {
                // This will return null if there is more than one constructor and none are marked with the [Inject] attribute
                return (from c in constructors where c.HasAttribute<InjectAttribute>() select c).SingleOrDefault();
            }

            return constructors[0];
        }
    }
}
