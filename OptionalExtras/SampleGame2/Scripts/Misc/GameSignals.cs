using System;
using UnityEngine;
using Zenject;
using Zenject.Commands;

namespace ModestTree
{
    public class PlayerKilledSignal : Signal
    {
        public class Trigger : TriggerBase
        {
        }
    }
}
