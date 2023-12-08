#nullable enable

using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;

using UniRx;

namespace Aqua.Scenes.ChangingFilter
{
    public class CloseValveTask : ScenarioTask
    {
        public ValveViewModel ValveViewModel { get; }

        // ToDo : AddTo (this)
        public CloseValveTask (ValveViewModel valveViewModel,
                               string name = "�������� �������",
                               string description = "�������� �������",
                               string failMessage = "������� �� ��� ������",
                               TaskState completed = TaskState.NotCompleted) : base(name, description, failMessage, completed)
        {
            ValveViewModel = valveViewModel;
            ValveViewModel.Output.ReadOnlyProperty.Subscribe(v => State = v switch
            {
                0 => TaskState.Completed,
                _ => TaskState.NotCompleted,
            });
        }
    }
}
