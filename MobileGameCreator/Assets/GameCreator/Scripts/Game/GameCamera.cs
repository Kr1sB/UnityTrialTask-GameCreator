using UnityEngine;

namespace GameCreator.Game
{
    public class GameCamera : MonoBehaviour
    {
        public void SetActive(bool active) =>
            gameObject.SetActive(active);
    }
}