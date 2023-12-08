#nullable enable

using UnityEngine;

using UniRx;

namespace Aqua.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemsCounter : MonoBehaviour
    {
        [SerializeField]
        private Collider _collider;

        private ReactiveCollection<Item> _items = new();

        public IReadOnlyReactiveCollection<Item> Items => _items;

        private void Awake ()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            _collider.isTrigger = true;
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<Item>() is var item and not null)
                _items.Add(item);
        }

        private void OnTriggerExit (Collider other)
        {
            if (other.GetComponent<Item>() is var item and not null)
                _items.Remove(item);
        }
    }
}
