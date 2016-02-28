using System;
using Zenject.Commands;

namespace ModestTree
{
    public static class EnemySignals
    {
        public class Hit : Signal<Bullet>
        {
            public class Trigger : TriggerBase
            {
            }
        }
    }
}
