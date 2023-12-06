using UnityEngine;

namespace Aqua.Items
{
    public interface IInteractableObject
    {
        public void Use ();

        public void DoProcessingAction ();

        public void UndoProcessingAction ();
    }
}
