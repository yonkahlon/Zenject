using UnityEngine;

namespace ModestTree.Zenject
{
    // The class constraint is necessary to work property with Moq
    // Lazily create singleton providers and ensure that only one exists for every concrete type
    public class ReferenceBinder<TContract> : Binder<TContract> where TContract : class
    {
        public ReferenceBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container, singletonMap)
        {
        }

        public BindingConditionSetter AsTransient()
        {
            return Bind(new TransientProvider<TContract>(_container));
        }

        public BindingConditionSetter AsTransient<TConcrete>() where TConcrete : TContract
        {
            return Bind(new TransientProvider<TConcrete>(_container));
        }

        public BindingConditionSetter AsSingle()
        {
            //Assert.That(!typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use AsSingle for Monobehaviours (when binding type " + typeof(TContract).GetPrettyName() + "), you probably want AsSingleFromPrefab or AsSingleGameObject");

            return Bind(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter AsSingle<TConcrete>() where TConcrete : TContract
        {
            //Assert.That(!typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)),
            //    "Should not use AsSingle for Monobehaviours (when binding type " + typeof(TContract).GetPrettyName() + "), you probably want AsSingleFromPrefab or AsSingleGameObject");

            return Bind(_singletonMap.CreateProvider<TConcrete>());
        }

        public BindingConditionSetter AsSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            Assert.That(instance != null, "provided singleton instance is null");
            return Bind(new SingletonInstanceProvider(instance));
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter AsSingleFromPrefab(GameObject template)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter AsSingleFromPrefab<TConcrete>(GameObject template) where TConcrete : Component
        {
            Assert.IsNotNull(template, "Received null template while binding type '" + typeof(TConcrete).GetPrettyName() + "'");
            return Bind(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, template));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter AsTransientFromPrefab<TConcrete>(GameObject template) where TConcrete : Component
        {
            Assert.IsNotNull(template, "provided template instance is null");
            return Bind(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, template));
        }

        public BindingConditionSetter AsSingleGameObject()
        {
            return AsSingleGameObject(typeof(TContract).GetPrettyName());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter AsSingleGameObject(string name)
        {
            Assert.That(typeof(TContract).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TContract).GetPrettyName() + "'");
            return Bind(new GameObjectSingletonProvider<TContract>(_container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter AsSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour
        {
            Assert.That(typeof(TConcrete).IsSubclassOf(typeof(MonoBehaviour)), "Expected MonoBehaviour derived type when binding type '" + typeof(TConcrete).GetPrettyName() + "'");
            return Bind(new GameObjectSingletonProvider<TConcrete>(_container, name));
        }
    }
}


