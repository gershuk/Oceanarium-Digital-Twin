#nullable enable

using System.Collections;

using Aqua.FPSController;
using Aqua.Items;
using Aqua.SceneController;
using Aqua.TanksSystem.ViewModels;
using Aqua.UIBaseElements;

using UnityEngine;

namespace Aqua.Scenes.ChangingFilter
{
    public class SceneBuilder : ScenarioSceneBuilder
    {
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
            _buildingPercentSocket.TrySetValue(0.2f);

            yield return null;

            _taskListViewModel.Model.Add(new CloseValveTask(_inputWaterValve, "Закройте вентиль на вход"));
            _taskListViewModel.Model.Add(new CloseValveTask(_outputWaterValve, "Закройте вентиль на выход"));
            _taskListViewModel.Model.Add(new OpenValveTask(_filterValve, "Откройте клапан фильтра"));

            _buildingPercentSocket.TrySetValue(0.6f);

            yield return null;

            _taskListViewModel.Model.Add(new RemoveItemFromSlotTask(_coverSlot, _cover.NameSocket.GetValue(), "Снимите крышку фильтра"));
            _taskListViewModel.Model.Add(new RemoveItemFromSlotTask(_filterSlot, _dirtyFilter.NameSocket.GetValue(), "Уберите грязный фильтр"));
            _taskListViewModel.Model.Add(new AddItemToSlotTask(_filterSlot, _cleanFilter.NameSocket.GetValue(), "Вставьте чистый фильтр"));
            _taskListViewModel.Model.Add(new AddItemToSlotTask(_coverSlot, _cover.NameSocket.GetValue(), "Закрутите крышку фильтра"));

            _buildingPercentSocket.TrySetValue(0.8f);

            yield return null;

            _stateScoket.TrySetValue(BuilderState.BuildingEnded);
            _buildingPercentSocket.TrySetValue(1);
        }
    }
}
