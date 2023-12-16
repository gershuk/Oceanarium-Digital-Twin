using System;

using Aqua.SceneController;
using Aqua.SocketSystem;

using TMPro;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class TaskToggleView : MonoBehaviour
    {
        private readonly MulticonnectionSocket<string, string> _descriptionSocket = new();

        private readonly MulticonnectionSocket<Guid, Guid> _guidSocket = new();

        private readonly MulticonnectionSocket<string, string> _nameSocket = new();

        private readonly MulticonnectionSocket<TaskState, TaskState> _stateSocket = new();

        [SerializeField]
        private Image _checkmark;

        [SerializeField]
        private Color _completedColor;

        [SerializeField]
        private Color _notCompletedColor;

        [SerializeField]
        private TMP_Text _text;

        public Image Checkmark => _checkmark;
        public IInputSocket<string> DescriptionSocket => _descriptionSocket!;
        public IInputSocket<Guid> GuidSocket => _guidSocket;
        public IInputSocket<string> NameSocket => _nameSocket!;
        public IInputSocket<TaskState> StateSocket => _stateSocket;
        public TMP_Text Text => _text;

        private void Start ()
        {
            _stateSocket.ReadOnlyProperty.Subscribe(UpdateView);
            _nameSocket.ReadOnlyProperty.Subscribe((name) => Text.text = name);
        }

        private void UpdateView (TaskState state)
        {
            switch (state)
            {
                case TaskState.NotCompleted:
                    Text.fontStyle &= ~FontStyles.Strikethrough;
                    Text.color = _notCompletedColor;
                    Checkmark.enabled = false;
                    break;

                case TaskState.Completed:
                    Text.fontStyle |= FontStyles.Strikethrough;
                    Text.color = _completedColor;
                    Checkmark.enabled = true;
                    break;

                case TaskState.InProgress:
                    break;

                case TaskState.Failed:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}