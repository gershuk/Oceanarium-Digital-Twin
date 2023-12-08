#nullable enable

using UnityEngine;

namespace Aqua.Items
{
    public class ProxyInteractableObject : MonoBehaviour,IInteractableObject
    {
        [SerializeField]
        protected GameObject _target;

        private IInteractableObject _interactableObject;

        protected IInteractableObject InteractableObject 
        { 
            get => _interactableObject; 
            set => _interactableObject = value; 
        }

        public void DoProcessingAction () => InteractableObject.DoProcessingAction();
        public void UndoProcessingAction () => InteractableObject.UndoProcessingAction();
        public void Use () => InteractableObject.Use();

        protected virtual void Awake ()
        {
            InteractableObject = _target.GetComponent<IInteractableObject>();

            if (InteractableObject == null )
            {
                Debug.LogWarning($"Target '{_target.name}' hasn't interactable component");
                return; 
            }
        }
    }
}
