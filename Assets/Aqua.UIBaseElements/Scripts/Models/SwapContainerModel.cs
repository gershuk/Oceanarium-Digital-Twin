#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class SwapContainerModel : MonoBehaviour
    {
        protected int? _activeIndex = default;

        [SerializeField]
        protected List<GameObject> _uiElements = new();

        public IReadOnlyList<GameObject> UIElements => _uiElements;

        protected void DisableAll ()
        {
            foreach (var uiElement in _uiElements)
            {
                uiElement.SetActive(false);
            }
        }

        protected bool TryDisableAcitveUI()
        {
            if (_activeIndex.HasValue)
                UIElements[_activeIndex.Value].SetActive(false);

            return _activeIndex.HasValue;
        }

        protected bool TrySetUIAcitve(int index)
        {
            if (UIElements.Count > index)
                UIElements[index].SetActive(true);

            _activeIndex = index;

            return UIElements.Count > index;
        }

        public bool TrySwap(int index)
        {
            TryDisableAcitveUI();
            return TrySetUIAcitve(index);
        }

        public void Swap(int index) 
        { 
            if (UIElements.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            TryDisableAcitveUI();
            TrySetUIAcitve(index);
        }

        protected virtual void Start ()
        {
            _uiElements ??= new List<GameObject>();
            TrySwap(0);
        }
    }
}
