using UnityEngine;
using System.Collections;
using Zenject;

namespace Zenject.Tests.ToSubContainerPrefab
{
    public class FooInstaller : MonoInstaller
    {
        [SerializeField]
        Bar _bar;

        public override void InstallBindings()
        {
            Container.BindInstance(_bar);
            Container.Bind<Gorp>("gorp").AsSingle();
        }
    }
}
