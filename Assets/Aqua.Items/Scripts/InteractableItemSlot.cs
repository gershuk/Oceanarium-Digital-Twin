#nullable enable

using UnityEngine;

namespace Aqua.Items
{
    public class InteractableItemSlot : ItemSlot, IInteractableObject
    {
        protected IInteractableObject? GetInteractableObjectFromInfo (IInfo? info) =>
            info is MonoBehaviour behaviour and not null
            && behaviour.gameObject.GetComponent<IInteractableObject>() is var interactableObject and not null
                ? interactableObject
                : null;

        public void DoProcessingAction () => GetInteractableObjectFromInfo(CurrentItem)?.DoProcessingAction();

        public void UndoProcessingAction () => GetInteractableObjectFromInfo(CurrentItem)?.UndoProcessingAction();

        public void Use () => GetInteractableObjectFromInfo(CurrentItem)?.Use();
    }
}