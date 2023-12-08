using System;
using System.Net.Mail;

using Aqua.SocketSystem;

namespace Aqua.SceneController
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
        protected string _failMessage;

        public virtual string FailMessage
        {
            get => _failMessage;
            protected set
            {
                _failMessage = value;
            }
        }

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

        public ScenarioTask (string name, string description, string failMessage = "Task failed", TaskState completed = default)
        {
            _guidSocket = new(Guid.NewGuid());
            _nameSocket = new(name ?? throw new NullReferenceException(nameof(name)));
            _descriptionSocket = new(description ?? throw new NullReferenceException(nameof(description)));
            FailMessage = failMessage;
            _stateSocket = new(completed);
        }
    }
}
