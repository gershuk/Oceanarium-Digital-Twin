using System.Collections;

using Aqua.BaseTasks;
using Aqua.FPSController;
using Aqua.SceneController;
using Aqua.TanksSystem;
using Aqua.UIBaseElements;

using UniRx;

using UnityEngine;

namespace Aqua.Scenes.TankFlushing
{
    public class SceneBuilder : ScenarioSceneBuilder
    {
        [SerializeField]
        private SimpleWaterSourceViewModel _coldWaterSource;

        [SerializeField]
        private SimpleWaterTubeWithValveViewModel _coldWaterTube;

        [SerializeField]
        private SimpleWaterSourceViewModel _hotWaterSource;

        [SerializeField]
        private SimpleWaterTubeWithValveViewModel _hotWaterTube;

        [SerializeField]
        private SimpleWaterTankViewModel _simpleWaterTankViewModel;

        [SerializeField]
        private TaskListViewModel _taskListViewModel;

        [SerializeField]
        private TickSystem _tickSystem;

        protected int _taskIndex;

        private void SetAllTaskActive ()
        {
            foreach (var task in _tasks)
            {
                task.ProcessingType = ProcessingType.Active;
            }
        }

        private void SetAllTaskNotCompleted ()
        {
            foreach (var task in _tasks)
            {
                task.State = TaskState.NotCompleted;
            }
        }

        private void Update ()
        {
            if (_stateScoket.GetValue() == BuilderState.StartingEnded
                && _playerModel.State is not (PlayerControllerState.Lose or PlayerControllerState.Win))
            {
                _tickSystem.Tick();
            }
        }

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
                new WaterLevelTask(_simpleWaterTankViewModel.MaxVolumeSocket, _simpleWaterTankViewModel.StoredSubstanceSocket),
                new WaterTempTask(_simpleWaterTankViewModel.StoredSubstanceSocket)
            };

            SetAllTaskNotCompleted();

            _taskIndex = -1;

            SetAllTaskActive();

            _taskIndex = 0;

            _buildingPercentSocket.TrySetValue(0.2f);

            yield return null;

            _tickSystem.AddToEnd(_simpleWaterTankViewModel);
            _tickSystem.Init();

            _buildingPercentSocket.TrySetValue(0.3f);

            yield return null;

            for (var i = 0; i < _tasks.Length; i++)
            {
                var task = _tasks[i];
                _taskListViewModel.Model.Add(task);
                task.StateSocket.ReadOnlyProperty.Subscribe(s =>
                {
                    switch (s)
                    {
                        case TaskState.NotCompleted:
                            break;

                        case TaskState.InProgress:
                            break;

                        case TaskState.Completed:
                            foreach (var taks in _tasks)
                            {
                                if (taks.State != TaskState.Completed)
                                    return;
                            }
                            _playerModel.State = PlayerControllerState.Win;
                            break;

                        case TaskState.Failed:
                            _firstFailedTaskSocket.TrySetValue(task);
                            _playerModel.State = PlayerControllerState.Lose;
                            break;
                    }
                });

                _buildingPercentSocket.TrySetValue(0.3f + (0.7f * i / _tasks.Length));

                yield return null;
            }

            _stateScoket.TrySetValue(BuilderState.BuildingEnded);

            _buildingPercentSocket.TrySetValue(1);
        }

        protected override void SubInit ()
        {
            base.SubInit();

            _coldWaterSource.ForceInit();
            _hotWaterSource.ForceInit();

            _coldWaterTube.ForceInit();
            _hotWaterTube.ForceInit();

            _coldWaterTube.InputSocket.SubscribeTo(_coldWaterSource.OutputSocket);
            _hotWaterTube.InputSocket.SubscribeTo(_hotWaterSource.OutputSocket);

            _simpleWaterTankViewModel.ForceInit();
            _simpleWaterTankViewModel.InputColdWaterSocket.SubscribeTo(_coldWaterTube.OutputSocket);
            _simpleWaterTankViewModel.InputHotWaterSocket.SubscribeTo(_hotWaterTube.OutputSocket);

            if (_taskListViewModel == null)
                _taskListViewModel = FindFirstObjectByType<TaskListViewModel>();
            _taskListViewModel.ForceInit();
        }
    }
}