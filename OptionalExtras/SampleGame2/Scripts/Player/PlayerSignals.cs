using System;
using Zenject.Commands;

namespace ModestTree
{
    public static class PlayerSignals
    {
        public class Hit : Signal<Bullet>
        {
            public class Trigger : TriggerBase
            {
            }
        }
    }
}

