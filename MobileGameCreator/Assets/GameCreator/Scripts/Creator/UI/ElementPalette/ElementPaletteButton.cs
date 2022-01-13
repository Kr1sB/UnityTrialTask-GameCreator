using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameCreator.Elements;

namespace GameCreator.Creator.UI
{
    public class ElementPaletteButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;

        public delegate void ButtonEvent(PointerEventData eventData);
        public ButtonEvent onStartDrag;
        public ButtonEvent onDrag;
        public ButtonEvent onEndDrag;

        public void Initialize(GameElement element)
        {
            GameElement.Metadata data = element.metadata;
            if (icon != null && data.icon != null)
                icon.sprite = data.icon;

            if (label != null && data.name != null)
                label.text = data.name;
        }

        public void OnBeginDrag(PointerEventData eventData) =>
            onStartDrag?.Invoke(eventData);

        public void OnDrag(PointerEventData eventData) =>
            onDrag?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData) =>
            onEndDrag?.Invoke(eventData);
    }
}