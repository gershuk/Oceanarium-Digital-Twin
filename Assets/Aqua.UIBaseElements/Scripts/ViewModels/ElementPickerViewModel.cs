#nullable enable

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class ElementPickerViewModel : MonoBehaviour
    {
        private readonly List<ElementPickerButtonViewModel> _elementPickerButtonViewModels = new();

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private GameObject _elementPrefab;

        [SerializeField]
        [Range(0f, 1f)]
        private float _elementSize;

        private ElementPickerButtonViewModel? _selectedButton = default;

        protected void UnselectAll ()
        {
            foreach (var element in _elementPickerButtonViewModels)
            {
                element.SetStateUnselected();
            }

            _selectedButton = null;
        }

        public void ActiveElement ()
        {
            if (_selectedButton != null)
            {
                _selectedButton.Invoke();
            }
            else
            {
                Debug.Log("Element not selected");
            }
        }

        public ElementPickerButtonViewModel AddElement (string text, Action action)
        {
            var pickerButton = Instantiate(_elementPrefab, _content).GetComponent<ElementPickerButtonViewModel>();
            pickerButton.Text = text;
            pickerButton.ButtonAction = action;

            pickerButton.transform.SetParent(_content, false);

            pickerButton.SetWidth(_content.rect.width * _elementSize);

            pickerButton.AddOnClickListener(() =>
            {
                UnselectAll();
                pickerButton.SetStateSelected();
                _selectedButton = pickerButton;
            });

            _elementPickerButtonViewModels.Add(pickerButton);

            return pickerButton;
        }

        //ToDo : Add RemoveOnClickListener
        //protected void OnDestroy ()
        //{
        //    foreach (var element in _elementPickerButtonViewModels)
        //    {
        //        element.RemoveOnClickListener(element.SetStateSelected);
        //    }
        //}
    }
}