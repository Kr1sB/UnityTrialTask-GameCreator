using UnityEngine;

namespace GameCreator.UI
{
    public class UIView : MonoBehaviour
    {
        public bool isVisible =>
            gameObject.activeInHierarchy;

        public void Show(bool show=true)
        {
            if (show == isVisible)
                return;

            gameObject.SetActive(show);

            if(show)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
        }

        virtual protected void OnShow() { }

        public void Hide() =>
            Show(false);

        virtual protected void OnHide() { }
    }
}