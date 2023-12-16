#nullable enable

using Aqua.Items;
using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;

using UniRx;

namespace Aqua.BaseTasks
{
    public class AddItemToSlotTask : ScenarioTask
    {
        public ItemSlot ItemSlot { get; }

        public string ItemName { get; }

        public AddItemToSlotTask (ItemSlot itemSlot,
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
                string s when s == ItemName => TaskState.Completed,
                _ => TaskState.NotCompleted,
            }).AddTo(Disposables);
        }
    }
}
