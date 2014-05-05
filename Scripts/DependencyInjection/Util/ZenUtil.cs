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
                else if (attr.GetType() == typeof(InjectAttribute))
                {
                    var injectAttr = (InjectAttribute)attr;
                    info.Identifier = injectAttr.Identifier;
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

        public static void InjectChildGameObjects(DiContainer container, Transform transform)
        {
            // Inject dependencies into child game objects
            foreach (var childTransform in transform.GetComponentsInChildren<Transform>())
            {
                InjectGameObject(container, childTransform.gameObject);
            }
        }

        public static void InjectGameObject(DiContainer container, GameObject gameObj)
        {
            var injecter = new PropertiesInjecter(container);

            foreach (var component in gameObj.GetComponents<MonoBehaviour>())
            {
                // null if monobehaviour link is broken
                if (component != null && component.enabled)
                {
                    using (container.PushLookup(component.GetType()))
                    {
                        injecter.Inject(component);
                    }
                }
            }
        }

        public static void LoadLevel(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevel(levelName);
        }

        public static void LoadLevelAdditive(string levelName, Action<DiContainer> extraBindings)
        {
            CompositionRoot.ExtraBindingsLookup = extraBindings;
            Application.LoadLevelAdditive(levelName);
        }

        public class InjectInfo
        {
            public bool Optional;
            public object Identifier;
        }
    }
}
