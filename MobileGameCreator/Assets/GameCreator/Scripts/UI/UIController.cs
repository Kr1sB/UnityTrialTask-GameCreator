using UnityEngine;
using UnityEngine.UI;

namespace GameCreator
{
    public class UIController : MonoBehaviour
    {
        public CanvasScaler scaler;
        public CanvasGroup canvasGroup;

        private void Start()
        {
            App.instance.onInteractionLockChanged +=
                (locked) => EnableInteraction(!locked);
        }

        public void EnableInteraction(bool enable = true)
        {
            canvasGroup.interactable = enable;
        }

        public void DisableInteraction() =>
            EnableInteraction(false);
    }
}