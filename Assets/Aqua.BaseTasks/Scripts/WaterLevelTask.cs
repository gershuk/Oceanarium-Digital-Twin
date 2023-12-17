using UniRx;

using Aqua.FlowSystem;
using Aqua.SceneController;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.BaseTasks
{
    public class WaterLevelTask : ScenarioTask
    {
        private CombiningSocket<Water, double, double> _combiningSocket;
        public WaterLevelTask (IOutputSocket<double> maxLevelSocket,
                              IOutputSocket<Water> waterSocket,
                              double maxLevel = 1,
                              double minLevel = 0.5,
                              string name = "Поддерживайте указанный уровень воды",
                              string description = "Поддерживайте указанный уровень воды",
                              string failMessage = "Несоблюдение уровня воды",
                              TaskState completed = TaskState.NotCompleted) : base(name + $"({minLevel * 100}%,{maxLevel * 100}%)",
                                                                                   description + $"({minLevel * 100}%,{maxLevel * 100}%)",
                                                                                   failMessage,
                                                                                   completed)
        {
            _combiningSocket = new CombiningSocket<Water, double, double>(combineFunction: static (w, l) => w.Volume / l);
            _combiningSocket.SubscribeTo(maxLevelSocket);
            _combiningSocket.SubscribeTo(waterSocket);
            _combiningSocket.ReadOnlyProperty.Subscribe(с => State = с switch
            {
                double coef when coef <= maxLevel && coef>=minLevel => TaskState.Completed,
                double coef when coef <= maxLevel+0.01 && coef>=minLevel-0.01 => TaskState.InProgress,
                double coef when coef > maxLevel + 0.01 || coef < minLevel - 0.01 => TaskState.Failed,
            }).AddTo(Disposables);
        }
    }
}
