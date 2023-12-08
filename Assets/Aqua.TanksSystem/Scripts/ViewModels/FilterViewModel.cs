#nullable enable

using Aqua.Items;

using UnityEngine;

using UniRx;

namespace Aqua.TanksSystem
{
    public class FilterViewModel : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private ItemSlot _coverSlot;

        [SerializeField]
        private ItemSlot _filterSlot;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _coverSlot.ForceInit();
            _filterSlot.ForceInit();

            _coverSlot.ItemSocket.ReadOnlyProperty.Subscribe(UpdateFilterState).AddTo(this);

            _isInited = true;
        }

        private void UpdateFilterState (Item? item)
        {
            _filterSlot.gameObject.SetActive(item == null);
        }

        private void Awake ()
        {
            ForceInit();
        }
    }
}
