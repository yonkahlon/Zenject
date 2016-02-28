using System;
using UnityEngine;

namespace ModestTree
{
    [Serializable]
    public class EnemyTunables
    {
        public float Accuracy;
        public float Speed;
        public float AttackDistance;
    }

    [Serializable]
    public class EnemyGlobalTunables
    {
        public int NumAttacking;
        public int DesiredNumEnemies;
    }
}
