#nullable enable

using Aqua.Items;
using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;

using UniRx;

namespace Aqua.Scenes.ChangingFilter
{
    public class RemoveItemFromSlotTask : ScenarioTask
    {
        public ItemSlot ItemSlot { get; }

        public string ItemName { get; }

        public RemoveItemFromSlotTask (ItemSlot itemSlot,
                                string itemName,
                               string name = "������� �������",
                               string description = "������� �������",
                               string failMessage = "������� �� ��� �����",
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
