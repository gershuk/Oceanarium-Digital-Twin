using System;
using System.Linq;

using Aqua.SceneController;

using UniRx;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class TaskListViewModel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _content;

        private TaskListModel _model;

        [SerializeField]
        private GameObject _taskViewPrefab;

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

        public void Awake ()
        {
            _model = new TaskListModel();
            _model.Tasks.ObserveAdd().Subscribe(e => CreateAndAttachView(e.Value)).AddTo(this);
            _model.Tasks.ObserveRemove().Subscribe(e => RemoveAndDestoryTaskView(FindTaskViewModel(e.Value))).AddTo(this);
        }
    }

    public class WrongParentException : Exception
    {
        public WrongParentException (string parentName, string childName) : base($"{childName} is not child of {parentName}")
        {
        }
    }
}