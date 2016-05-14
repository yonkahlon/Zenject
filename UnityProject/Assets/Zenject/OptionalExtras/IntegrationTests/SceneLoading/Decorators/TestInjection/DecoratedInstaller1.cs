using UnityEngine;
using Zenject;

namespace Zenject.Tests.ToGameObject
{
    public class DecoratedInstaller1 : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Gorp>().AsSingle();
        }
    }
}
