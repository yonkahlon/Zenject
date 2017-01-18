using System;

namespace Zenject.SpaceFighter
{
    public class GameEvents
    {
        public Action PlayerKilled = delegate {};
        public Action EnemyKilled = delegate {};
    }
}
