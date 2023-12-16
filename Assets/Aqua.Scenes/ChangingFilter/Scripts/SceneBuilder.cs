#nullable enable

using UniRx;

using System;
using System.Collections;

using Aqua.FPSController;
using Aqua.Items;
using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;
using Aqua.UIBaseElements;

using UnityEngine;
using Aqua.BaseTasks;

namespace Aqua.Scenes.ChangingFilter
{
    public class SceneBuilder : ScenarioSceneBuilder
    {
        private int _taskIndex = 0;

        [SerializeField]
        private TaskListViewModel _taskListViewModel;

        [SerializeField]
        private ValveViewModel _inputWaterValve;

        [SerializeField]
        private ValveViewModel _outputWaterValve;

        [SerializeField]
        private ValveViewModel _filterValve;

        [SerializeField]
        private Item _cover;

        [SerializeField]
        private Item _dirtyFilter;

        [SerializeField]
        private Item _cleanFilter;

        [SerializeField]
        private ItemSlot _coverSlot;

        [SerializeField]
        private ItemSlot _filterSlot;

        [SerializeField]
        private ItemsCounter _binCounter;

        protected override void SubInit ()
        {
            base.SubInit();

            _inputWaterValve.ForceInit();
            _outputWaterValve.ForceInit();
            _filterValve.ForceInit();

            _cover.ForceInit();
            _dirtyFilter.ForceInit();
            _cleanFilter.ForceInit();

            _coverSlot.ForceInit();
            _filterSlot.ForceInit();

            _binCounter.ForceInit();

            if (_taskListViewModel == null)
                _taskListViewModel = FindFirstObjectByType<TaskListViewModel>();
            _taskListViewModel.ForceInit();
        }

        // ToDo : Reduce copypasting
        protected override IEnumerator BuildScene ()
        {
            _stateScoket.TrySetValue(BuilderState.Building);
            _buildingPercentSocket.TrySetValue(0);

            yield return null;

            _playerModel.ForceInit();
            _playerModel.State = PlayerControllerState.None;
            _buildingPercentSocket.TrySetValue(0.1f);

            yield return null;

            _tasks = new ScenarioTask[]
            {
                new CloseValveTask(_inputWaterValve,
                                   "Закройте вентиль на вход",
                                   "Закройте вентиль на вход",
                                   "Вентиль на вход не был закрыт"), //1

                new CloseValveTask(_outputWaterValve,
                                   "Закройте вентиль на выход",
                                   "Закройте вентиль на выход",
                                   "Вентиль на выход не был закрыт"), //2

                new OpenValveTask(_filterValve,
                                  "Откройте клапан фильтра",
                                  "Откройте клапан фильтра",
                                  "Клапан фильтра не был открыт"), //3

                new RemoveItemFromSlotTask(_coverSlot,
                                           _cover.NameSocket.GetValue(),
                                           "Снимите крышку фильтра",
                                           "Снимите крышку фильтра",
                                           "Крышка фильтра не была снята"), //4

                new RemoveItemFromSlotTask(_filterSlot,
                                           _dirtyFilter.NameSocket.GetValue(),
                                           "Уберите грязный фильтр",
                                           "Уберите грязный фильтр",
                                           "Грязный фильтр не был убран"), //5

                new AddItemToSlotTask(_filterSlot,
                                      _cleanFilter.NameSocket.GetValue(),
                                      "Вставьте чистый фильтр",
                                      "Вставьте чистый фильтр",
                                      "Чистый фильтр не был вставлен"), //6

                new AddItemToSlotTask(_coverSlot,
                                      _cover.NameSocket.GetValue(),
                                      "Поставьте крышку фильтра",
                                      "Поставьте крышку фильтра",
                                      "Крышка фильма не была установлена"), //7

                new CloseValveTask(_filterValve,
                                   "Закройте клапан фильтра",
                                   "Закройте клапан фильтра",
                                   "Клапан фильтра не был закрыт"), //8

                new OpenValveTask(_outputWaterValve,
                                  "Откройте вентиль на выход",
                                  "Откройте вентиль на выход",
                                  "Вентиль на выход не был открыт"), //9

                new OpenValveTask(_inputWaterValve,
                                  "Откройте вентиль на вход",
                                  "Откройте вентиль на вход",
                                  "Вентиль на вход не был открыт"), //10

                new ItemCounterTask(_binCounter,
                                    _dirtyFilter.NameSocket.GetValue(),
                                    1,
                                    "Выбросте грязный фильтр в ведро",
                                    "Выбросте грязный фильтр в ведро",
                                    "Грязный фильтр не был выброшен") //11
            };

            SetAllTaskNotCompleted();

            _taskIndex = -1;
            foreach (var task in _tasks)
            {
                task.StateSocket.ReadOnlyProperty.Subscribe(s=>CheckOrder(task));
            }

            SetFirstTasksHalfActive();

            _taskIndex = 0;

            _buildingPercentSocket.TrySetValue(0.2f);

            yield return null;

            for (var i = 0; i < _tasks.Length; i++)
            {
                var task = _tasks[i];
                _taskListViewModel.Model.Add(task);

                _buildingPercentSocket.TrySetValue(0.2f + 0.8f * i / _tasks.Length);

                yield return null;
            }

            _stateScoket.TrySetValue(BuilderState.BuildingEnded);

            _buildingPercentSocket.TrySetValue(1);
        }

        private void SetAllTaskNotCompleted ()
        {
            foreach (var task in _tasks)
            {
                task.State = TaskState.NotCompleted;
            }
        }

        private void SetFirstTasksHalfActive ()
        {
            for (var i = 0; i < 6; i++)
            {
                _tasks[i].ProcessingType = ProcessingType.Active;
            }

            for (var i = 6; i < 11; i++)
            {
                _tasks[i].ProcessingType = ProcessingType.Freeze;
            }
        }

        private void SetSecondTasksHalfActive ()
        {
            for (var i = 0; i < 6; i++)
            {
                _tasks[i].ProcessingType = ProcessingType.Freeze;
            }

            for (var i = 6; i < 11; i++)
            {
                _tasks[i].ProcessingType = ProcessingType.Active;
            }
        }

        private void CheckOrder(ScenarioTask scenarioTask)
        {
            if (_taskIndex == -1)
                return;

            if (Array.IndexOf(_tasks, scenarioTask) != _taskIndex)
            {
                _firstFailedTaskSocket.TrySetValue(_tasks[_taskIndex]);
                _playerModel.State = PlayerControllerState.Lose;
                return;
            }

            if (scenarioTask.State == TaskState.Completed)
                _taskIndex++;

            if (_taskIndex == 6)
            {
                _taskIndex = -1;
                SetSecondTasksHalfActive();
                _taskIndex = 6;
            }

            if (_taskIndex == 11)
                _playerModel.State = PlayerControllerState.Win;
        }
    }
}
