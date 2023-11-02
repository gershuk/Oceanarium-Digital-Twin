using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class SimpleWhaterSourceViewModel : MonoBehaviour
    {
        private readonly Source<WaterData> _waterSource;

        [SerializeField]
        private WaterData _initValue;

        [SerializeField]
        private bool _isMulticonnection = false;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        private float _temp = 20;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _volume = 1;

        public IOutputSocket<WaterData> OutputSocket => _waterSource.OutputSocket;

        private SimpleWhaterSourceViewModel () => _waterSource = new Source<WaterData>(new WaterData(_volume, _ph, _temp), _isMulticonnection);
    }
}