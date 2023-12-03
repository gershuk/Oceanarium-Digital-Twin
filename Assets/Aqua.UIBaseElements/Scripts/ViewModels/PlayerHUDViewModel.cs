using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public class PlayerHUDViewModel : MonoBehaviour
    {
        [SerializeField]
        private TaskListModel _taskListViewModel;

        [SerializeField]
        private ItemsPanelViewModel _itemsPanelViewModel;

        [SerializeField]
        private HUDAimViewModel _hudViewModel;
    }
}
