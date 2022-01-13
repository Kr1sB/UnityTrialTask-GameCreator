using UnityEngine;
using GameCreator.Elements;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace GameCreator.Creator.UI
{
    public class ElementPalettePanel : MonoBehaviour
    {
        public delegate void ElementDragEvent(GameElement element, PointerEventData eventData);
        public event ElementDragEvent onStartDragElement;

        public delegate void PointerEvent(PointerEventData eventData);
        public event PointerEvent onDragElement;
        public event PointerEvent onEndDragElement;

        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private ElementPaletteButton buttonTemplate;

        private GameElementPalette palette;
        private List<ElementPaletteButton> buttons = new List<ElementPaletteButton>();

        public void Initialize(GameElementPalette palette)
        {
            this.palette = palette;

            buttonTemplate.gameObject.SetActive(false);
            BuildButtons();
        }

        private void BuildButtons()
        {
            for (int i = 0; i < palette.elements.Length; ++i)
                AddButton(palette.elements[i]);
        }

        private void AddButton(GameElement element)
        {
            int index = buttons.Count;

            ElementPaletteButton button = Instantiate(buttonTemplate);
            button.gameObject.SetActive(true);
            buttons.Add(button);

            RectTransform rect = (RectTransform)button.transform;
            rect.SetParent(buttonContainer);
            rect.SetAsLastSibling();

            RectTransform templateRect = (RectTransform)buttonTemplate.transform;
            rect.localScale = templateRect.localScale;
            rect.anchoredPosition3D = templateRect.anchoredPosition3D;

            button.Initialize(element);
            button.onStartDrag += eventData => OnButtonStartDrag(index, eventData);
            button.onDrag += OnButtonDrag;
            button.onEndDrag += OnButtonEndDrag;
        }


        private void OnButtonStartDrag(int index, PointerEventData eventData)
        {
            GameElement element = palette.elements[index];
            onStartDragElement?.Invoke(element, eventData);
        }


        private void OnButtonDrag(PointerEventData eventData)
        {
            onDragElement?.Invoke(eventData);
        }


        private void OnButtonEndDrag(PointerEventData eventData)
        {
            onEndDragElement?.Invoke(eventData);
        }
    }
}