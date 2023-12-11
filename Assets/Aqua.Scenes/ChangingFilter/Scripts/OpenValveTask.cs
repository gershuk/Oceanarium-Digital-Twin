#nullable enable

using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;

using UniRx;

namespace Aqua.Scenes.ChangingFilter
{
    public class OpenValveTask : ScenarioTask
    {
        public ValveViewModel ValveViewModel { get; }

        public OpenValveTask (ValveViewModel valveViewModel,
                               string name = "�������� �������",
                               string description = "�������� �������",
                               string failMessage = "������� �� ��� ������",
                               TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ValveViewModel = valveViewModel;
            ValveViewModel.Output.ReadOnlyProperty.Subscribe(v => State = v switch
            {
                1 => TaskState.Completed,
                _ => TaskState.NotCompleted,
            }).AddTo(Disposables);
        }
    }
}
