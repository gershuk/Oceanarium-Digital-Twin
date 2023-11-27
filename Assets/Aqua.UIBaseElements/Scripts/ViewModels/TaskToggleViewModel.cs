using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public sealed class TaskToggleViewModel : MonoBehaviour
    {
        private readonly MulticonnectionSocket<string, string> _descriptionSocket = new();
        private readonly MulticonnectionSocket<Guid, Guid> _guidSocket = new();
        private readonly MulticonnectionSocket<string, string> _nameSocket = new();
        private readonly MulticonnectionSocket<TaskState, TaskState> _stateSocket = new();

        [SerializeField]
        private TaskToggleView _view;

        public ScenarioTask Task { get; private set; }

        private void Awake ()
        {
            if (_view == null)
                _view = GetComponent<TaskToggleView>();
            SubscribeViewToViewModel();
        }

        private void OnDestroy ()
        {
            if (Task != null)
                UnsubscribeViewModelToModel();
            UnsubscribeViewToViewModel();
        }

        private void SubscribeViewModelToModel ()
        {
            _stateSocket.SubscribeTo(Task.StateSocket);
            _nameSocket.SubscribeTo(Task.NameSocket);
            _descriptionSocket.SubscribeTo(Task.DescriptionSocket);
            _guidSocket.SubscribeTo(Task.GuidSocket);
        }

        private void SubscribeViewToViewModel ()
        {
            _view.StateSocket.SubscribeTo(_stateSocket);
            _view.NameSocket.SubscribeTo(_nameSocket);
            _view.DescriptionSocket.SubscribeTo(_descriptionSocket);
            _view.GuidSocket.SubscribeTo(_guidSocket);
        }

        private void UnsubscribeViewModelToModel ()
        {
            _stateSocket.UnsubscribeFrom(Task.StateSocket);
            _nameSocket.UnsubscribeFrom(Task.NameSocket);
            _descriptionSocket.UnsubscribeFrom(Task.DescriptionSocket);
            _guidSocket.UnsubscribeFrom(Task.GuidSocket);
        }

        private void UnsubscribeViewToViewModel ()
        {
            _view.StateSocket.UnsubscribeFrom(_stateSocket);
            _view.NameSocket.UnsubscribeFrom(_nameSocket);
            _view.DescriptionSocket.UnsubscribeFrom(_descriptionSocket);
            _view.GuidSocket.UnsubscribeFrom(_guidSocket);
        }

        public void LinkToModel (ScenarioTask task)
        {
            if (Task != null)
                throw new Exception($"Task already setted");
            Task = task;

            SubscribeViewModelToModel();
        }

        public void UnlinkFromModel ()
        {
            if (Task == null)
                throw new NullReferenceException(nameof(Task));
            UnsubscribeViewModelToModel();
            Task = null;
        }
    }
}