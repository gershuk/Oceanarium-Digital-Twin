#nullable enable

using Aqua.SceneController;

using UniRx;

namespace Aqua.UIBaseElements
{
    public class TaskListModel
    {
        private readonly ReactiveCollection<ScenarioTask> _tasks = new();

        public IReadOnlyReactiveCollection<ScenarioTask> Tasks => _tasks;

        public void Add (ScenarioTask task) => _tasks.Add(task);

        public void Remove (ScenarioTask task) => _tasks.Remove(task);
    }
}