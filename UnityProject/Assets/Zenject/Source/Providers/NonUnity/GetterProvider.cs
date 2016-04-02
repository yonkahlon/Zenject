using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject;

namespace Zenject
{
    public class GetterProvider<TObj, TResult> : IProvider
    {
        readonly DiContainer _container;
        readonly string _identifier;
        readonly Func<TObj, TResult> _method;

        public GetterProvider(
            string identifier, Func<TObj, TResult> method,
            DiContainer container)
        {
            _container = container;
            _identifier = identifier;
            _method = method;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(TResult);
        }

        InjectContext GetSubContext(InjectContext parent)
        {
            var subContext = parent.CreateSubContext(
                typeof(TObj), _identifier);

            Assert.IsEqual(subContext.Container, _container);

            subContext.Optional = false;

            return subContext;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TResult).DerivesFromOrEqual(context.MemberType));

            yield return new List<object>() { _method(
                _container.Resolve<TObj>(GetSubContext(context))) };
        }

        public IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEmpty(argTypes);

            return _container.ValidateResolve(GetSubContext(context));
        }
    }
}
