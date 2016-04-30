using UnityEngine;
using System.Collections;
using Zenject;

namespace ModestTree.Tests.Zenject.ToSubContainerPrefabResource
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
