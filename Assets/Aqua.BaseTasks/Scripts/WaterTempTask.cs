#nullable enable

using System;

using Aqua.FlowSystem;
using Aqua.SceneController;
using Aqua.SocketSystem;

using UniRx;

namespace Aqua.BaseTasks
{
    public class WaterTempTask : ScenarioTask
    {
        public WaterTempTask (IOutputSocket<Water> waterSocket,
                              double reqTemp = 30,
                              double lowTemp = 10,
                              string name = "Добейтесь указанной температуры",
                              string description = "Добейтесь указанной температуры",
                              string failMessage = "Температура не была достигнута или упала ниже критического значения",
                              TaskState completed = TaskState.NotCompleted) : base(name + $"({reqTemp})",
                                                                                   description + $"({reqTemp})",
                                                                                   failMessage,
                                                                                   completed) => waterSocket.ReadOnlyProperty.Subscribe(w => State = w.Temperature switch
                                                                                   {
                                                                                       double temp when Math.Abs(temp - reqTemp) < 0.2 => TaskState.Completed,
                                                                                       double temp when temp < reqTemp && temp > lowTemp => TaskState.InProgress,
                                                                                       double temp when temp <= lowTemp => TaskState.Failed,
                                                                                   }).AddTo(Disposables);
    }
}