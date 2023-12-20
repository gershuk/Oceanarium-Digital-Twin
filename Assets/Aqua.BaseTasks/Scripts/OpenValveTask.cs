#nullable enable

using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;

using UniRx;

namespace Aqua.BaseTasks
{
    public class OpenValveTask : ScenarioTask
    {
        public ValveViewModel ValveViewModel { get; }

        public OpenValveTask (ValveViewModel valveViewModel,
                               string name = "Откройте вентиль",
                               string description = "Откройте вентиль",
                               string failMessage = "Внетиль не был открыт",
                               TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ValveViewModel = valveViewModel;
            ValveViewModel.Output.ReadOnlyProperty.Subscribe(v => State = v switch
            {
                1 => TaskState.Completed,
                > 0 => TaskState.InProgress,
                0 => TaskState.NotCompleted,
            }).AddTo(Disposables);
        }
    }
}