using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class SimpleWhaterSourceViewModel : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _volume = 1;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        private float _temp = 20;

        [SerializeField]
        private WaterData _initValue;

        private readonly Source<WaterData> _waterSource = new();

        public IOutputSocket<WaterData> OutputSocket => _waterSource.OutputSocket;

        private void Awake ()
        {
            _waterSource.Value = new WaterData(_volume, _ph, _temp);
        }
    }
}
