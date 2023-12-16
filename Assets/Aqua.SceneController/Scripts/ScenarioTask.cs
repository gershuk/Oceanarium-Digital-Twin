using System;
using UniRx;

using Aqua.SocketSystem;
using System.Runtime.InteropServices;

namespace Aqua.SceneController
{
    [Serializable]
    public enum TaskState
    {
        NotCompleted = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3,
    }

    public enum ProcessingType
    {
        Active = 0,
        Freeze = 1,
    }

    public class ScenarioTask : IDisposable
    {
        private CompositeDisposable _disposables;

        protected string _failMessage;
        protected bool _isDisposed = false;

        private readonly MulticonnectionSocket<ProcessingType, ProcessingType> _processingTypeSocket = new(ProcessingType.Active);

        public IOutputSocket<ProcessingType> ProcessingTypeSocket => _processingTypeSocket;

        public virtual string FailMessage
        {
            get => _failMessage;
            protected set
            {
                _failMessage = value;
            }
        }

        protected readonly MulticonnectionSocket<string, string> _descriptionSocket;
        protected readonly MulticonnectionSocket<Guid, Guid> _guidSocket;
        protected readonly MulticonnectionSocket<string, string> _nameSocket;
        protected readonly MulticonnectionSocket<TaskState, TaskState> _stateSocket;

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
                if (ProcessingType == ProcessingType.Freeze)
                    return;

                if (!_stateSocket.TrySetValue(value))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public IOutputSocket<TaskState> StateSocket => _stateSocket;

        protected CompositeDisposable Disposables
        {
            get => _disposables;
            set => _disposables = value;
        }

        public ProcessingType ProcessingType
        {
            get => _processingTypeSocket.GetValue();
            set => _processingTypeSocket.TrySetValue(value);
        }

        public ScenarioTask (string name, string description, string failMessage = "Task failed", TaskState completed = default)
        {
            Disposables = new();
            _guidSocket = new(Guid.NewGuid());
            _nameSocket = new(name ?? throw new NullReferenceException(nameof(name)));
            _descriptionSocket = new(description ?? throw new NullReferenceException(nameof(description)));
            FailMessage = failMessage;
            _stateSocket = new(completed);
        }

        ~ScenarioTask () => Dispose(false);

        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                }

                Disposables.Dispose();
                _isDisposed = true;
            }
        }
    }
}
