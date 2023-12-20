using UnityEngine;

namespace Aqua.Items
{
    [RequireComponent(typeof(Collider))]
    public class DeadZone : MonoBehaviour
    {
        private Collider _collider;

        private void Awake ()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.transform.GetComponent<Item>()?.TryResetPosition() is null or false)
            {
                Destroy(other.gameObject);
            }
        }
    }
}