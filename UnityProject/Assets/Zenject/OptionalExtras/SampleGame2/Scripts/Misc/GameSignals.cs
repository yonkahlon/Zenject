using System;
using UnityEngine;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    // Triggered when the player explodes
    public class PlayerKilledSignal : Signal
    {
        public class Trigger : TriggerBase
        {
        }
    }

    // Triggered when the enemy explodes
    public class EnemyKilledSignal : Signal
    {
        public class Trigger : TriggerBase
        {
        }
    }
}
