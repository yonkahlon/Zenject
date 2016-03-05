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
}
