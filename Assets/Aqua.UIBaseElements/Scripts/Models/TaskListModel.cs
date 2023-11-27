#nullable enable

using System;

using Aqua.SocketSystem;

using UniRx;

namespace Aqua.UIBaseElements
{
    [Serializable]
    public enum TaskState
    {
        NotCompleted = 0,
        Completed = 1,
        Failed = 2,
    }

    public class ScenarioTask
    {
        private readonly MulticonnectionSocket<string, string> _descriptionSocket;
        private readonly MulticonnectionSocket<Guid, Guid> _guidSocket;
        private readonly MulticonnectionSocket<string, string> _nameSocket;
        private readonly MulticonnectionSocket<TaskState, TaskState> _stateSocket;

        public string Description
        {
            get => _descriptionSocket.GetValue();
            set
            {
                if (!_descriptionSocket.TrySetValue(value ?? throw new ArgumentNullException(nameof(value))))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket!;

        public Guid Guid
        {
            get => _guidSocket.GetValue();
            set
            {
                if (!_guidSocket.TrySetValue(value))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public IOutputSocket<Guid> GuidSocket => _guidSocket;

        public string Name
        {
            get => _nameSocket.GetValue();
            set
            {
                if (!_nameSocket.TrySetValue(value ?? throw new ArgumentNullException(nameof(value))))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public IOutputSocket<string> NameSocket => _nameSocket!;

        public TaskState State
        {
            get => _stateSocket.GetValue();
            set
            {
                if (!_stateSocket.TrySetValue(value))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public IOutputSocket<TaskState> StateSocket => _stateSocket;

        public ScenarioTask (string name, string description, TaskState completed = default)
        {
            _guidSocket = new(Guid.NewGuid());
            _nameSocket = new(name ?? throw new NullReferenceException(nameof(name)));
            _descriptionSocket = new(description ?? throw new NullReferenceException(nameof(description)));
            _stateSocket = new(completed);
        }
    }

    public class TaskListModel
    {
        private readonly ReactiveCollection<ScenarioTask> _tasks = new();

        public IReadOnlyReactiveCollection<ScenarioTask> Tasks => _tasks;

        public void Add (ScenarioTask task) => _tasks.Add(task);

        public void Remove (ScenarioTask task) => _tasks.Remove(task);
    }
}