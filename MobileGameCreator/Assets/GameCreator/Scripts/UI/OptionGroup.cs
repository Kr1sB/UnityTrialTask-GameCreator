using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.UI
{
    public class OptionGroup : MonoBehaviour
    {
        [System.Serializable]
        public class OptionData
        {
            public string text;
            public Sprite image;
        }

        public delegate void OptionGroupEvent(int optionIndex);
        public event OptionGroupEvent onSelectionChanged;

        [SerializeField] private int defaultSelected;
        [SerializeField] private List<OptionData> defaultOptions;
        private List<OptionData> options = new List<OptionData>();
        private List<OptionGroupButton> buttons = new List<OptionGroupButton>();

        [SerializeField] private OptionGroupButton optionTemplate;

        private int selectedIndex = -1;

        private void Awake()
        {
            optionTemplate.gameObject.SetActive(false);

            foreach (OptionData data in defaultOptions)
                AddButton(data);

            SelectOption(defaultSelected);
        }

        public void AddOptions(List<string> options)
        {
            foreach (string label in options)
                AddOption(label);
        }

        public void AddOptions(List<Sprite> options)
        {
            foreach (Sprite image in options)
                AddOption(image);
        }

        public void AddOptions(List<OptionData> options)
        {
            foreach (OptionData data in options)
                AddOption(data);
        }

        public void AddOption(string text) =>
            AddOption(new OptionData() { text = text });

        public void AddOption(Sprite image) =>
            AddOption(new OptionData() { image = image });


        public void AddOption(OptionData data)
        {
            options.Add(data);
            AddButton(data);
        }

        private void AddButton(OptionData data)
        {
            int index = buttons.Count;

            OptionGroupButton button = Instantiate(optionTemplate);
            button.gameObject.SetActive(true);
            buttons.Add(button);

            RectTransform rect = (RectTransform)button.transform;
            rect.SetParent((RectTransform)transform);
            rect.SetAsLastSibling();

            RectTransform templateRect = (RectTransform)optionTemplate.transform;
            rect.localScale = templateRect.localScale;
            rect.anchoredPosition3D = templateRect.anchoredPosition3D;

            button.ApplyData(data);
            button.onClick += () => Select(index);
        }


        public void ClearOptions()
        {
            for (int i = 0; i < buttons.Count; ++i)
                Destroy(buttons[i].gameObject);

            options.Clear();
            buttons.Clear();
            selectedIndex = -1;
        }


        private void SelectOption(int index)
        {
            if (buttons.Count < 1)
                return;

            if (selectedIndex >= 0)
                buttons[selectedIndex].Select(false);

            selectedIndex = index;

            if (selectedIndex >= 0)
                buttons[selectedIndex].Select(true);
        }


        public void SelectWithoutNotify(int index)
        {
            index = Mathf.Clamp(index, 0, options.Count - 1);

            if (index == selectedIndex)
                return;

            SelectOption(index);
        }

        public void Select(int index)
        {
            SelectWithoutNotify(index);
            onSelectionChanged?.Invoke(selectedIndex);
        }
    }
}