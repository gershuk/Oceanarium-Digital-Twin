#nullable enable

using Aqua.Items;

using UniRx;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class FilterViewModel : MonoBehaviour
    {
        [SerializeField]
        private ItemSlot _coverSlot;

        [SerializeField]
        private ItemSlot _filterSlot;

        private bool _isInited = false;

        private void Awake () => ForceInit();

        private void UpdateFilterState (Item? item) => _filterSlot.gameObject.SetActive(item == null);

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _coverSlot.ForceInit();
            _filterSlot.ForceInit();

            _coverSlot.ItemSocket.ReadOnlyProperty.Subscribe(UpdateFilterState).AddTo(this);

            _isInited = true;
        }
    }
}