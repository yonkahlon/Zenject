using System;

namespace Zenject.SpaceFighter
{
    public class GameEvents
    {
        public Action<EnemyModel, Bullet> EnemyHit = delegate {};
        public Action<Bullet> PlayerHit = delegate {};
        public Action PlayerKilled = delegate {};
        public Action<EnemyModel> EnemyKilled = delegate {};
    }
}
