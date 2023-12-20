#nullable enable

using Aqua.Items;
using Aqua.SceneController;

using UniRx;

namespace Aqua.BaseTasks
{
    public class AddItemToSlotTask : ScenarioTask
    {
        public string ItemName { get; }
        public ItemSlot ItemSlot { get; }

        public AddItemToSlotTask (ItemSlot itemSlot,
                                  string itemName,
                                  string name = "Уберите предмет",
                                  string description = "Уберите предмет",
                                  string failMessage = "Предмет не был убран",
                                  TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ItemSlot = itemSlot;
            ItemName = itemName;
            ItemSlot.ItemSocket.ReadOnlyProperty.Subscribe(v =>
            {
                var val = v != null ? v.NameSocket.GetValue() : null;
                State = val switch
                {
                    string s when s == ItemName => TaskState.Completed,
                    null => TaskState.NotCompleted,
                    _ => TaskState.NotCompleted,
                };
            }).AddTo(Disposables);
        }
    }
}