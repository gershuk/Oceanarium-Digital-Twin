using System;
using System.Linq;

using Aqua.SceneController;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class TaskListViewModel : MonoBehaviour
    {
        [SerializeField]
        private Image _backgroundImage;

        [SerializeField]
        private RectTransform _content;

        private bool _isInited = false;
        private TaskListModel _model;

        [SerializeField]
        private GameObject _taskViewPrefab;

        public TaskListModel Model
        {
            get => _model;
            set => SetAndSubscribeToModel(value);
        }

        private void AttachView (TaskToggleViewModel viewModel) => viewModel.GetComponent<RectTransform>().SetParent(_content);

        private void CreateAndAttachView (ScenarioTask task) => AttachView(CreateTaskView(task));

        private TaskToggleViewModel CreateTaskView (ScenarioTask task)
        {
            var taskViewModel = Instantiate(_taskViewPrefab).GetComponent<TaskToggleViewModel>();
            taskViewModel.LinkToModel(task);
            return taskViewModel;
        }

        private TaskToggleViewModel FindTaskViewModel (ScenarioTask task) =>
            _content.GetComponentsInChildren<TaskToggleViewModel>().Where(v => v.Task == task).FirstOrDefault();

        private void RemoveAndDestoryTaskView (TaskToggleViewModel taskToggleViewModels)
        {
            var rect = taskToggleViewModels.GetComponent<RectTransform>();
            if (rect.parent != _content)
                throw new WrongParentException(taskToggleViewModels.name, _content.name);

            Destroy(taskToggleViewModels.gameObject);
        }

        private void SetAndSubscribeToModel (TaskListModel model)
        {
            _model = model;
            Model.Tasks.ObserveAdd().Subscribe(e => CreateAndAttachView(e.Value)).AddTo(this);
            Model.Tasks.ObserveRemove().Subscribe(e => RemoveAndDestoryTaskView(FindTaskViewModel(e.Value))).AddTo(this);
            Model.Tasks.ObserveCountChanged().Subscribe(UpdateBackground).AddTo(this);

            UpdateBackground(Model.Tasks.Count);
        }

        private void UpdateBackground (int count) => _backgroundImage.enabled = count > 0;

        public void Awake () => ForceInit();

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_backgroundImage == null)
                _backgroundImage = GetComponent<Image>();

            Model = new TaskListModel();

            _isInited = true;
        }
    }

    public class WrongParentException : Exception
    {
        public WrongParentException (string parentName, string childName) : base($"{childName} is not child of {parentName}")
        {
        }
    }
}