#nullable enable

using System;
using System.Linq;

using Aqua.Items;
using Aqua.SceneController;

using UniRx;

namespace Aqua.BaseTasks
{
    public class ItemCounterTask : ScenarioTask
    {
        public ItemsCounter ItemsCounter { get; }

        public string ItemName { get; }

        public int RequiredCount { get; }

        public ItemCounterTask (ItemsCounter itemsCounter,
                                string itemName,
                                int requiredCount,
                                string name,
                                string description,
                                string failMessage = "Не найдено нужно количество предметов.",
                                TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ItemsCounter = itemsCounter;
            ItemName = itemName;
            RequiredCount = requiredCount;

            itemsCounter.Items.ObserveCountChanged()
                              .Subscribe(OnCountUpdate)
                              .AddTo(Disposables);
        }

        protected void OnCountUpdate (int count)
        {
            var itemsCounts = ItemsCounter.Items.Count(i => i.NameSocket.GetValue() == ItemName);
            State = itemsCounts switch
            {
                int c when c >= RequiredCount => TaskState.Completed,
                int c when c == 0 => TaskState.NotCompleted,
                int c when c < RequiredCount => TaskState.InProgress,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
