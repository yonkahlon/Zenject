using Zenject;

namespace ModestTree
{
    public class EnemyHealthWatcher : ITickable
    {
        readonly EnemyStateManager _stateManager;
        readonly EnemyModel _model;

        public EnemyHealthWatcher(
            EnemyModel model,
            EnemyStateManager stateManager)
        {
            _stateManager = stateManager;
            _model = model;
        }

        public void Tick()
        {
            if (_model.Health <= 0)
            {
                _stateManager.ChangeState(EnemyStates.Dead);
            }
        }
    }
}
