using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


namespace GameCreator.UI
{
    [RequireComponent(typeof(Button))]
    public class OptionGroupButton : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Image targetImage;
        [SerializeField] private TMP_Text label;

        public event System.Action onClick;
        public bool isSelected { get; private set; }

        public void ApplyData(OptionGroup.OptionData data)
        {
            if (label != null && data.text != null)
                label.text = data.text;

            if (targetImage != null && data.image != null)
                targetImage.sprite = data.image;
        }

        private void SetImageColor(Color color)
        {
            if (targetImage == null)
                return;

            targetImage.color = color;
        }

        private void SetLabelColor(Color color)
        {
            if (label == null)
                return;

            label.color = color;
        }

        public void Select(bool selected)
        {
            isSelected = selected;

            Color color = (isSelected)
                ? selectedColor
                : normalColor;

            SetLabelColor(color);
            SetImageColor(color);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}