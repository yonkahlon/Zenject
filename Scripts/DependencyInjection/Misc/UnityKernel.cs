using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Implement the task kernel using Unity's
    // Update() function
    public class UnityKernel : MonoBehaviour, IKernel
    {
        KernelCustom _impl = new KernelCustom();

        public void AddTask(ITickable task)
        {
            _impl.AddTask(task);
        }

        public void RemoveTask(ITickable task)
        {
            _impl.RemoveTask(task);
        }

        public void Update()
        {
            _impl.Update(int.MinValue, KernelCustom.UnityLateUpdateTickPriority);
        }

        public void LateUpdate()
        {
            _impl.Update(KernelCustom.UnityLateUpdateTickPriority, KernelCustom.UnityOnGuiTickPriority);
        }

        public void OnGUI()
        {
            _impl.Update(KernelCustom.UnityOnGuiTickPriority, int.MaxValue);
        }
    }
}
