#nullable enable

using Aqua.Items;
using Aqua.SceneController;

using UniRx;

namespace Aqua.BaseTasks
{
    public class RemoveItemFromSlotTask : ScenarioTask
    {
        public string ItemName { get; }
        public ItemSlot ItemSlot { get; }

        public RemoveItemFromSlotTask (ItemSlot itemSlot,
                                string itemName,
                               string name = "Уберите предмет",
                               string description = "Уберите предмет",
                               string failMessage = "Предмет не был убран",
                               TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ItemSlot = itemSlot;
            ItemName = itemName;
            ItemSlot.ItemSocket.ReadOnlyProperty.Subscribe(v => State = v?.NameSocket.GetValue() switch
            {
                string s when s != ItemName => TaskState.Completed,
                null => TaskState.Completed,
                _ => TaskState.NotCompleted,
            }).AddTo(Disposables);
        }
    }
}