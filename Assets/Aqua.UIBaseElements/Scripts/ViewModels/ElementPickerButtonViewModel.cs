#nullable enable

using System;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class ElementPickerButtonViewModel : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        private Image _image;
        private bool _isSelected;

        private RectTransform _rectTransform;

        [SerializeField]
        private Color _selectedStateColor;

        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        [SerializeField]
        private Color _unselectedStateColor;

        public Button Button
        {
            get => _button;
            protected set => _button = value;
        }

        public Action? ButtonAction { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                UpdateColor();
            }
        }

        public string Text
        {
            get => TextMeshPro.text;
            set => TextMeshPro.text = value;
        }

        public TextMeshProUGUI TextMeshPro
        {
            get => _textMeshPro;
            protected set => _textMeshPro = value;
        }

        protected void Awake ()
        {
            if (Button == null)
                Button = GetComponent<Button>();

            _rectTransform = Button.GetComponent<RectTransform>();
            _image = Button.GetComponent<Image>();

            if (TextMeshPro == null)
                TextMeshPro = GetComponent<TextMeshProUGUI>();

            IsSelected = false;
        }

        protected void UpdateColor () => _image.color = _isSelected ? _selectedStateColor : _unselectedStateColor;

        public void AddOnClickListener (UnityAction action) => Button.onClick.AddListener(action);

        public void InversState () => IsSelected = !IsSelected;

        public void Invoke ()
        {
            if (ButtonAction != null)
            {
                ButtonAction();
            }
            else
            {
                throw new NullReferenceException(nameof(ButtonAction));
            }
        }

        public void RemoveOnClickListener (UnityAction action) => Button.onClick?.RemoveListener(action);

        public void SetStateSelected () => IsSelected = true;

        public void SetStateUnselected () => IsSelected = false;

        public void SetWidth (float width) => _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}