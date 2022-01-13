using UnityEngine;

namespace GameCreator.Game.UI
{
    public class GameUI : MonoBehaviour
    {
        public Camera _camera;

        public void Show(bool show = true)
        {
            gameObject.SetActive(show);
        }

        public void Hide() =>
            Show(false);
    }
}