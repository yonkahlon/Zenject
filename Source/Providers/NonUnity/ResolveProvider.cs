using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class ResolveProvider : IProvider
    {
        readonly string _identifier;
        readonly DiContainer _container;
        readonly Type _contractType;
        readonly bool _isOptional;

        bool _isValidating;

        public ResolveProvider(
            Type contractType, DiContainer container, string identifier, bool isOptional)
        {
            _contractType = contractType;
            _identifier = identifier;
            _container = container;
            _isOptional = isOptional;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _contractType;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(_contractType.DerivesFromOrEqual(context.MemberType));

            yield return _container.ResolveAll(GetSubContext(context)).Cast<object>().ToList();
        }

        InjectContext GetSubContext(InjectContext parent)
        {
            var subContext = parent.CreateSubContext(_contractType, _identifier);

            Assert.IsEqual(subContext.Container, _container);

            subContext.Optional = _isOptional;

            return subContext;
        }

        public IEnumerable<ZenjectException> Validate(InjectContext context, List<Type> argTypes)
        {
            Assert.IsEmpty(argTypes);
            if (_isValidating)
            {
                _isValidating = false;
                yield return new ZenjectException(
                    "Found circular dependency when validating type '{0}'", _contractType);
                yield break;
            }

            _isValidating = true;

            foreach (var err in _container.ValidateResolveAll(GetSubContext(context)))
            {
                yield return err;
            }

            _isValidating = false;
        }
    }
}
