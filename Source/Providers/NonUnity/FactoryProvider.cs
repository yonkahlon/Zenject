using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject;

namespace Zenject
{
    public abstract class FactoryProviderBase<TValue, TFactory> : IProvider
        where TFactory : IFactory
    {
        public FactoryProviderBase(DiContainer container)
        {
            Container = container;
        }

        protected DiContainer Container
        {
            get;
            private set;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(TValue);
        }

        public virtual IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            foreach (var error in Container.ValidateObjectGraph<TFactory>())
            {
                yield return error;
            }

            if (typeof(TFactory).DerivesFrom<IValidatable>())
            {
                var factory = Container.Instantiate<TFactory>();

                foreach (var error in ((IValidatable)factory).Validate())
                {
                    yield return error;
                }
            }
        }

        public abstract IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args);
    }

    // Zero parameters

    public class FactoryProvider<TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));

            yield return new List<object>() { Container.Instantiate<TFactory>().Create() };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEmpty(argTypes);
            return base.Validate(context, argTypes);
        }
    }

    // One parameters

    public class FactoryProvider<TParam1, TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TParam1, TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEqual(args.Count, 1);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.IsEqual(args[0].Type, typeof(TParam1));

            yield return new List<object>()
            {
                Container.Instantiate<TFactory>().Create((TParam1)args[0].Value)
            };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEqual(argTypes.Count, 1);
            Assert.IsEqual(argTypes[0], typeof(TParam1));

            return base.Validate(context, argTypes);
        }
    }

    // Two parameters

    public class FactoryProvider<TParam1, TParam2, TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TParam1, TParam2, TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEqual(args.Count, 2);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.IsEqual(args[0].Type, typeof(TParam1));
            Assert.IsEqual(args[1].Type, typeof(TParam2));

            yield return new List<object>()
            {
                Container.Instantiate<TFactory>().Create(
                    (TParam1)args[0].Value,
                    (TParam2)args[1].Value)
            };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEqual(argTypes.Count, 2);
            Assert.IsEqual(argTypes[0], typeof(TParam1));
            Assert.IsEqual(argTypes[1], typeof(TParam2));

            return base.Validate(context, argTypes);
        }
    }

    // Three parameters

    public class FactoryProvider<TParam1, TParam2, TParam3, TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TParam1, TParam2, TParam3, TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEqual(args.Count, 3);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.IsEqual(args[0].Type, typeof(TParam1));
            Assert.IsEqual(args[1].Type, typeof(TParam2));
            Assert.IsEqual(args[2].Type, typeof(TParam3));

            yield return new List<object>()
            {
                Container.Instantiate<TFactory>().Create(
                    (TParam1)args[0].Value,
                    (TParam2)args[1].Value,
                    (TParam3)args[2].Value)
            };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEqual(argTypes.Count, 3);
            Assert.IsEqual(argTypes[0], typeof(TParam1));
            Assert.IsEqual(argTypes[1], typeof(TParam2));
            Assert.IsEqual(argTypes[2], typeof(TParam3));

            return base.Validate(context, argTypes);
        }
    }

    // Four parameters

    public class FactoryProvider<TParam1, TParam2, TParam3, TParam4, TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEqual(args.Count, 4);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.IsEqual(args[0].Type, typeof(TParam1));
            Assert.IsEqual(args[1].Type, typeof(TParam2));
            Assert.IsEqual(args[2].Type, typeof(TParam3));
            Assert.IsEqual(args[3].Type, typeof(TParam4));

            yield return new List<object>()
            {
                Container.Instantiate<TFactory>().Create(
                    (TParam1)args[0].Value,
                    (TParam2)args[1].Value,
                    (TParam3)args[2].Value,
                    (TParam4)args[3].Value)
            };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEqual(argTypes.Count, 4);
            Assert.IsEqual(argTypes[0], typeof(TParam1));
            Assert.IsEqual(argTypes[1], typeof(TParam2));
            Assert.IsEqual(argTypes[2], typeof(TParam3));
            Assert.IsEqual(argTypes[3], typeof(TParam4));

            return base.Validate(context, argTypes);
        }
    }

    // Five parameters

    public class FactoryProvider<TParam1, TParam2, TParam3, TParam4, TParam5, TValue, TFactory> : FactoryProviderBase<TValue, TFactory>
        where TFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
    {
        public FactoryProvider(DiContainer container)
            : base(container)
        {
        }

        public override IEnumerator<List<object>> GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.IsEqual(args[0].Type, typeof(TParam1));
            Assert.IsEqual(args[1].Type, typeof(TParam2));
            Assert.IsEqual(args[2].Type, typeof(TParam3));
            Assert.IsEqual(args[3].Type, typeof(TParam4));
            Assert.IsEqual(args[4].Type, typeof(TParam5));

            yield return new List<object>()
            {
                Container.Instantiate<TFactory>().Create(
                    (TParam1)args[0].Value,
                    (TParam2)args[1].Value,
                    (TParam3)args[2].Value,
                    (TParam4)args[3].Value,
                    (TParam5)args[4].Value)
            };
        }

        public override IEnumerable<ZenjectException> Validate(
            InjectContext context, List<Type> argTypes)
        {
            Assert.IsEqual(argTypes.Count, 5);
            Assert.IsEqual(argTypes[0], typeof(TParam1));
            Assert.IsEqual(argTypes[1], typeof(TParam2));
            Assert.IsEqual(argTypes[2], typeof(TParam3));
            Assert.IsEqual(argTypes[3], typeof(TParam4));
            Assert.IsEqual(argTypes[4], typeof(TParam5));

            return base.Validate(context, argTypes);
        }
    }
}
