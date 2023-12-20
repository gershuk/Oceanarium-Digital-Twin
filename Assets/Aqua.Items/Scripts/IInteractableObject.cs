namespace Aqua.Items
{
    public interface IInteractableObject
    {
        public void DoProcessingAction ();

        public void UndoProcessingAction ();

        public void Use ();
    }
}