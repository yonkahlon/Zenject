using System.Collections.Generic;
using ModestTree.Util;
using Zenject;
using System.Linq;

namespace ModestTree
{
    public enum EnemyStates
    {
        None,
        Idle,
        RunAway,
        Attack,
        Follow,
        Dead,
    }

    public class EnemyStateFactory : IValidatable
    {
        readonly DiContainer _container;

        public EnemyStateFactory(DiContainer container)
        {
            _container = container;
        }

        public IEnemyState Create(EnemyStates state)
        {
            switch (state)
            {
                case EnemyStates.Idle:
                {
                    return _container.Instantiate<EnemyStateIdle>();
                }
                case EnemyStates.RunAway:
                {
                    return _container.Instantiate<EnemyStateRunAway>();
                }
                case EnemyStates.Attack:
                {
                    return _container.Instantiate<EnemyStateAttack>();
                }
                case EnemyStates.Follow:
                {
                    return _container.Instantiate<EnemyStateFollow>();
                }
            }

            throw Assert.CreateException();
        }

        // If we were using a factory then we wouldn't need to do this
        // However, since we are instantiating these classes directly
        // we need to (if we care enough about validating the state classes)
        public IEnumerable<ZenjectResolveException> Validate()
        {
            foreach (var error in _container.ValidateObjectGraph<EnemyStateIdle>()
                .Concat(_container.ValidateObjectGraph<EnemyStateRunAway>())
                .Concat(_container.ValidateObjectGraph<EnemyStateAttack>())
                .Concat(_container.ValidateObjectGraph<EnemyStateFollow>()))
            {
                yield return error;
            }
        }
    }
}
