using System.Collections;
using System.Collections.Generic;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTankViewModel : MonoBehaviour, ITickObject
    {
        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        private float _temp = 20;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _volume = 1;

        protected readonly SimpleWaterTank _waterTank;

        public IInputSocket<WaterData> InputSocket => _waterTank.InputSocket;

        public IOutputSocket<WaterData> DataSocket => _waterTank.DataSocket;

        public SimpleWaterTankViewModel ()
        {
            _waterTank = new SimpleWaterTank(new WaterData(_volume, _temp, _ph));
        }

        public void Init (float startTime)
        {
            _waterTank.Init(startTime);
        }
        public void Tick (int tickNumber, float startTime, float tickTime) => _waterTank.Tick(tickNumber, startTime, tickTime);
    }
}
