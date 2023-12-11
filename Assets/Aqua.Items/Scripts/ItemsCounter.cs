#nullable enable

using UnityEngine;

using UniRx;

namespace Aqua.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemsCounter : MonoBehaviour
    {
        protected bool _isInted = false;

        [SerializeField]
        private Collider _collider;

        private ReactiveCollection<Item> _items;

        public IReadOnlyReactiveCollection<Item> Items => _items;

        public void ForceInit ()
        {
            if (_isInted)
                return;

            if (_collider == null)
                _collider = GetComponent<Collider>();

            _collider.isTrigger = true;

            _items = new();

            _isInted = true;
        }

        private void Awake ()
        {
            ForceInit();
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<Item>() is var item and not null)
                AddItem(item);
        }

        private void OnTriggerExit (Collider other)
        {
            if (other.GetComponent<Item>() is var item and not null)
                RemoveItem(item);
        }

        private void RemoveItem (Item item)
        {
            _items.Remove(item);
        }

        private void AddItem (Item item)
        {
            var dispose = new CompositeDisposable();
            item.StateSocket.ReadOnlyProperty.Subscribe(s =>
            {
                switch (s)
                {
                    case ItemState.Free:
                        break;
                    case ItemState.Picked:
                        dispose.Dispose();
                        RemoveItem(item);
                        break;
                }
            }).AddTo(dispose);
            _items.Add(item);
        }
    }
}
