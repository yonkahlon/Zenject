using UnityEngine;
using Zenject;

namespace ModestTree
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [SerializeField]
        float _leftPadding;

        [SerializeField]
        float _bottomPadding;

        [SerializeField]
        float _width;

        [SerializeField]
        float _height;

        PlayerModel _model;

        [PostInject]
        public void Construct(PlayerModel model)
        {
            _model = model;
        }

        public void OnGUI()
        {
            var bounds = new Rect(_leftPadding, Screen.height - _bottomPadding, _width, _height);
            GUI.Label(bounds, "Health: {0:0}".Fmt(_model.Health));
        }
    }
}

