using System;
using Zenject.Commands;

namespace ModestTree
{
    public static class EnemySignals
    {
        // Triggered when enemy is hit by a bullet
        public class Hit : Signal<Bullet>
        {
            public class Trigger : TriggerBase
            {
            }
        }
    }
}
