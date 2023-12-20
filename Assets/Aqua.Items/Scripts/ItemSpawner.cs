using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.Items
{
    [Serializable]
    public class ItemSpawner : MonoBehaviour
    {
        private readonly List<Item> _spawnedItems = new();

        [SerializeField]
        private GameObject[] _itemPrefabs;

        [SerializeField]
        private Transform _spwanPosition;

        private Transform Anchor => _spwanPosition == null ? transform : _spwanPosition;

        private void SpawnItem (GameObject prefab)
        {
            var item = Instantiate(prefab, Anchor.position, Anchor.rotation).GetComponent<Item>();

            if (item == null)
            {
                Debug.LogError("Spawned object isn't item");
                return;
            }

            item.Spawner = this;
            _spawnedItems.Add(item);
        }

        public void ResetAllItemsPosition ()
        {
            foreach (var item in _spawnedItems)
                ResetSpawnedItemPosition(item);
        }

        public void ResetSpawnedItemPosition (Item item)
        {
            if (!_spawnedItems.Contains(item))
            {
                Debug.LogError($"Spawner '{gameObject.name}' doesn't contain iten '{item.gameObject.name}");
                return;
            }

            item.transform.position = Anchor.position;
            item.transform.rotation = Anchor.rotation;
        }

        public void SpawnAll ()
        {
            foreach (var prefab in _itemPrefabs)
            {
                SpawnItem(prefab);
            }
        }
    }
}